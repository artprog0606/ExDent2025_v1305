import process from 'node:process';
import { writeFile, mkdir } from 'fs/promises';
import { existsSync } from 'fs';
import path from 'path';
import { chromium, devices } from 'playwright-core';
import http from 'http';
import https from 'https';
import postcss from 'postcss';
import postcssUrl from 'postcss-url';
import postcssDiscard from 'postcss-discard';
import { minify } from 'csso';

class Cratocss {
    static REGEXP_DOMAIN = /(http|https):\/\/(\S{1,})\.([\w\d]{2,})?\//;

    /**
     *
     * @param {Object} options
     * @param {string} options.baseURL BaseURL site from source critical css
     * @param {string} options.output Path for save file critical css
     * @param {string} options.filename Filename critical css. Default [name].critical.css
     * @param {string} options.device https://playwright.dev/docs/api/class-playwright#playwright-devices
     * @param {number} options.width
     * @param {number} options.height
     * @param {boolean} options.grabFontFace
     * @param {boolean} options.screenshots
     * @param {number} options.parallelStreamsCount
     * @param {Object} options.discardOptions https://github.com/bezoerb/postcss-discard
     * @param {String|RegExp|Function} options.discardOptions.atrule Match atrule like @font-face
     * @param {String|RegExp|Function} options.discardOptions.rule Match rule like .big-background-image {...}
     * @param {String|RegExp|Function} options.discardOptions.decl Match declarations
     * @param {String} options.discardOptions.css CSS String or path to file containing css
     * @param {Object} options.cssUrlOptions https://github.com/postcss/postcss-url
     */
    constructor(options) {
        if (options.baseURL == null) {
            throw Error(`Option "baseURL" is required`);
        }
        this.baseURLRaw = options.baseURL;
        this.baseURL = typeof options.baseURL === 'string' ? new URL(options.baseURL) : options.baseURL;
        this.output = options.output;
        this.filename = options.filename || `[name].critical.css`;
        this.device = options.device;
        this.width = options.width;
        this.height = options.height;
        this.grabFontFace = options.grabFontFace || false;
        this.screenshots = options.screenshots || false;
        this.parallelStreamsCount = options.parallelStreamsCount || 3;
        this.discardOptions = options.discardOptions || {
            //atrule: ['@font-face', /print/],
            decl: [/(.*)transition(.*)/, 'cursor', 'pointer-events', /(-webkit-)?tap-highlight-color/, /(.*)user-select/],
        };
        this.cssUrlOptions = options.cssUrlOptions;
        this.timeout = 60000;
        this.httpService = this.baseURL.protocol.startsWith('https://') ? https : http;
        this.cacheCSSRules = new Map();
        this.presetStyles = options.presetStyles;
    }

    /**
     * Generate file name critical css
     * @param {string} name
     * @returns {string} file name
     */
    #getFilenameCriticalCSS(name) {
        return this.filename.replace(`[name]`, name);
    }

    /**
     * Return list visible elements
     * @param {*} page Page object (playwright)
     * @returns
     */
    async getElementsVisible(page) {
        //let result = await page.evaluateHandle(() => {
        //    return new Promise((resolve) => {
        //        const itemsInViewport = new Set();

        //        function getObserver() {
        //            const threshold = 0;
        //            return new IntersectionObserver((entries) => {
        //                entries.forEach(entry => (threshold === 1 ? entry.intersectionRatio === 1 : entry.intersectionRatio > threshold) ? itemsInViewport.add(entry.target) : null);
        //                observer.disconnect();
        //                resolve(itemsInViewport);
        //            }
        //            )
        //        };

        //        const observer = getObserver();

        //        const walker = document.createTreeWalker(document, NodeFilter.SHOW_ELEMENT, () => NodeFilter.FILTER_ACCEPT, true);

        //        while (walker.nextNode()) {
        //            observer.observe(walker.currentNode);
        //        }

        //        requestAnimationFrame(() => { });
        //    });
        //});

        let result = await page.evaluateHandle(() => {
            return new Promise((resolve) => {
                const result = [];
                const elements = document.querySelectorAll(`html, body, body *:not(link):not(style):not(script)`);

                let boundingBox;

                for (let el of elements) {
                    boundingBox = el.getBoundingClientRect();
                    if (boundingBox.height === 0 || boundingBox.width === 0) {
                        if (el.style.getPropertyValue('display') === 'none') {
                            el.style.setProperty('display', 'initial', 'important');
                        }
                        if (['hidden', 'collapse'].some((item) => el.style.getPropertyValue('visibility') === item)) {
                            el.style.setProperty('visibility', 'visible', 'important');
                        }
                        boundingBox = el.getBoundingClientRect();
                    }

                    if (boundingBox.y <= window.innerHeight) {
                        result.push(el);
                    }
                }

                resolve(result);
            });
        });

        return result;
    }

    /**
     * @param {*} page Page object (playwright)
     * @returns
     */
    async getCSSData(page) {
        const result = await page.evaluate(
            function ({ cacheFilesNameCSS, grabFontFace }) {
                const siteHostname = window.location.hostname;
                const styleSheetsList = document.styleSheets;
                let data = null;
                //правильный порядок link
                const linksHref = [];
                let rules;
                let href;
                for (let index in styleSheetsList) {
                    href = styleSheetsList[index].href;
                    if (href != null && siteHostname === new URL(href).hostname) {
                        linksHref.push(href);
                        if (cacheFilesNameCSS.includes(href) === false) {
                            rules = Array.from(styleSheetsList[index].cssRules || styleSheetsList[index].rules);
                            data = data || {};
                            data[href] = rules.reduce((prev, current) => {
                                if (current instanceof CSSMediaRule) {
                                    prev.push({
                                        conditionText: current.conditionText,
                                        rulesMedia: Array.from(current.cssRules).map((subItem) => {
                                            return { cssText: subItem.cssText, selectorText: subItem.selectorText };
                                        }),
                                    });
                                } else if (current.selectorText != null) {
                                    prev.push({ cssText: current.cssText, selectorText: current.selectorText });
                                } else if (grabFontFace === true && current.constructor.name === 'CSSFontFaceRule') {
                                    prev.push({ cssText: current.cssText, selectorText: 'html' });
                                }

                                return prev;
                            }, []);
                        }
                    }
                }
                return { data, linksHref };
            },
            { cacheFilesNameCSS: Array.from(this.cacheCSSRules.keys()), grabFontFace: this.grabFontFace },
        );

        if (result.data != null) {
            let postcssPlugins = [];
            let postcssObj;
            const listPromises = [];
            let _cssUrlOptionsNew;

            for (const href of result.linksHref) {
                if (result.data[href] != null) {
                    if (this.discardOptions || this.cssUrlOptions) {
                        if (this.discardOptions) {
                            postcssPlugins.push(postcssDiscard(this.discardOptions));
                        }

                        if (this.cssUrlOptions) {
                            _cssUrlOptionsNew = { ...this.cssUrlOptions };
                        } else {
                            _cssUrlOptionsNew = {
                                //TODO: перенести логику в criticalcssProcess
                                //Проблема: не получается передать параметр "href"
                                url: (asset, dir, options, decl, warn, result) => {
                                    let newUrl = ``;

                                    if (asset.pathname != null) {
                                        let rootPath = href.toLowerCase().replace(this.baseURLRaw.toLowerCase(), '');
                                        let indexStart = rootPath.lastIndexOf('/');
                                        let start = rootPath.substring(0, indexStart);
                                        newUrl = path.join(start, asset.url).replace(/\\/g, '/');
                                    } else {
                                        newUrl = asset.url;
                                    }

                                    return newUrl;
                                },
                            };
                        }

                        postcssPlugins.push(postcssUrl(_cssUrlOptionsNew));

                        postcssObj = postcss(postcssPlugins);

                        for (const item of result.data[href]) {
                            if (item.rulesMedia != null) {
                                listPromises.push(
                                    Promise.all(
                                        item.rulesMedia.map(
                                            (x) =>
                                                new Promise((resolve, reject) => {
                                                    postcssObj
                                                        .process(x.cssText, { from: undefined })
                                                        .then(({ css }) =>
                                                            resolve({
                                                                selectorText: x.selectorText,
                                                                cssText: css,
                                                            }),
                                                        )
                                                        .catch((err) => {
                                                            console.error(err);
                                                            reject(err);
                                                        });
                                                }),
                                        ),
                                    )
                                        .then((data) => {
                                            return { conditionText: item.conditionText, rulesMedia: data };
                                        })
                                        .catch((err) => {
                                            console.error(err);
                                            return Promise.reject(err);
                                        }),
                                );
                            } else {
                                listPromises.push(
                                    new Promise((resolve, reject) => {
                                        postcssObj
                                            .process(item.cssText, { from: undefined })
                                            .then(({ css }) => {
                                                resolve({ selectorText: item.selectorText, cssText: css });
                                            })
                                            .catch((err) => {
                                                console.error(err);
                                                reject(err);
                                            });
                                    }),
                                );
                            }
                        }

                        result.data[href] = await Promise.all(listPromises);

                        this.cacheCSSRules.set(href, result.data[href]);
                    }

                    postcssPlugins.length = 0;
                    listPromises.length = 0;
                }
            }
        }

        return {
            linksHref: result.linksHref,
            rules: result.linksHref.map((linksHrefItem) => this.cacheCSSRules.get(linksHrefItem)).flat(),
        };
    }

    /**
     * Main method for generate CSS files critical css
     * @param {Map<String, String>|Map<String, String[]>|Map<String, Object>} data
     */
    async generate(data) {
        const browser = await chromium.launch();
        let contextOptions = {};
        let deviceData;
        let viewport = null;

        if ((this.width != null && this.height == null) || (this.width == null && this.height != null)) {
            throw new Error('Cratocss: options "width" and "height" both there must be  null or not null');
        } else if (this.width != null && this.height != null) {
            viewport = { width: this.width, height: this.height };
        }

        if (this.device != null) {
            deviceData = devices[this.device];

            if (viewport == null) {
                viewport = { ...deviceData.viewport };
            }
        }

        contextOptions = { ...deviceData };

        if (viewport != null) {
            contextOptions.viewport = viewport;
        }

        const context = await browser.newContext(contextOptions);
        context.setDefaultTimeout(this.timeout);

        const dataUrlsWithContext = await this.preparePages(data, async () => {
            const contextCustom = await browser.newContext(contextOptions);
            contextCustom.setDefaultTimeout(this.timeout);
            return contextCustom;
        });

        while (dataUrlsWithContext.size > 0) {
            await Promise.all(
                this.runInParallel(
                    Math.min(dataUrlsWithContext.size, this.parallelStreamsCount),
                    this.#runItem.bind(this, dataUrlsWithContext, context),
                ),
            );
        }
        await context.close();
        await browser.close();
    }

    async preparePages(data, factoryContext) {
        let _urls;
        let validUrlList;
        let options;
        let context;
        const result = new Map();

        for (const [name, urlList] of data) {
            if (Array.isArray(urlList)) {
                _urls = [...urlList];
            } else if (typeof urlList === 'string') {
                _urls = [urlList];
            } else if (typeof urlList === 'object') {
                if (urlList.cookies != null) {
                    context = await factoryContext();
                    await context.addCookies(urlList.cookies);
                }
                _urls = Array.isArray(urlList.url) ? urlList.url : [urlList.url];
                options = Object.assign({}, urlList);
                delete options.url;
            }

            validUrlList = await this.#checkPageWorking(_urls, options);

            if (validUrlList.length > 0) {
                result.set(name, {
                    url: validUrlList,
                    context,
                });
            } else {
                throw new Error(`Not finded working page in ${name}`);
            }

            context = undefined;
        }

        return result;
    }

    runInParallel(count, fn) {
        let list = [];
        for (let i = 0; i < count; i++) {
            list.push(fn());
        }
        return list;
    }

    async #runItem(data, contextDefault) {
        if (data == null || data.size === 0) {
            return '';
        }

        const [name, { url, context }] = data.entries().next().value;
        let contextCurrent = context || contextDefault;

        data.delete(name);

        await this.processItem(contextCurrent, name, url);

        if (context) {
            await context.close();
        }
    }

    async #processItemGrab(page, name) {
        const { rules } = await this.getCSSData(page);

        const elementsVisible = await this.getElementsVisible(page);

        return await page.evaluate(
            ({ elementsVisible, rules }) => {
                const pseudoElementRegExp = /(:?:before|:?:after)/;
                const cssBodyRegExp = /{.+\s*}$/;
                const symbolsInEndRegExp = /(>|~|>)\s*$/;
                let errors = '';

                function processCSS(element, selectorText, cssText, callback) {
                    const selectorParsed = parseSelector(selectorText);
                    if (selectorParsed.size > 0) {
                        for (let [key, value] of selectorParsed) {
                            if (key?.length > 0 && element.matches(key)) {
                                callback(renderCssText(cssText, key, value));
                            }
                        }
                    }
                }

                function parseSelector(selectorText) {
                    const result = new Map();

                    if (selectorText == null) {
                        return result;
                    } else {
                        const itemsList = selectorText.split(/,(?![^(]*\))/).map((x) => x.trim());
                        for (let item of itemsList) {
                            if (pseudoElementRegExp.test(item)) {
                                const selectorParsed = item.match(pseudoElementRegExp);
                                let selectorClean = item.replace(selectorParsed[1], '');

                                //браузеры вырезают символ "*" из selectorText
                                if (symbolsInEndRegExp.test(selectorClean)) {
                                    selectorClean += ' *';
                                }

                                result.set(selectorParsed.length > 1 ? selectorClean : item, selectorParsed.length > 1 ? selectorParsed[1] : null);
                            } else {
                                result.set(item, null);
                            }
                        }
                        return result;
                    }
                }

                function renderCssText(cssText, selector, pseudoElement) {
                    if (cssText.match(cssBodyRegExp) == null) {
                        if (cssText.length > 0) {
                            errors = errors + `\n selector: ${selector}, cssText: ${cssText} !\n`;
                        }
                        return '';
                    }
                    return `${pseudoElement != null ? selector + pseudoElement : selector}${cssText.match(cssBodyRegExp)[0]}`;
                }

                const result = new Set();
                for (let ruleItem of rules) {
                    if (ruleItem == null) {
                        continue;
                    }
                    for (let element of elementsVisible) {
                        if (ruleItem.selectorText == null && window.matchMedia(ruleItem.conditionText).matches === true) {
                            const mediaRules = new Set();

                            for (let { selectorText, cssText } of ruleItem.rulesMedia) {
                                processCSS(element, selectorText, cssText, (css) => mediaRules.add(css));
                            }

                            if (mediaRules.size > 0) {
                                result.add(`@media ${ruleItem.conditionText}{${Array.from(mediaRules).join('')}}`);
                            }
                        } else {
                            processCSS(element, ruleItem.selectorText, ruleItem.cssText, (css) => result.add(css));
                        }
                    }
                }
                return {
                    css: Array.from(result).join(''),
                    errors,
                };
            },
            { elementsVisible, rules },
        );
    }

    /**
     *
     * @param {Object} context
     * @param {string} urlList
     * @return {Page} Page object (playwright)
     */
    async #checkPageWorking(urls, options) {
        let response;
        let result = [];
        let headers = new Headers();

        if (options?.cookies != null) {
            let cookieString = '';
            for (const cookieItem of options.cookies) {
                const { name, value, ...props } = cookieItem;
                cookieString = `${name}=${value}`;
                for (const key in props) {
                    cookieString += `; ${key}=${props[key]}`;
                }
                headers.append('Cookie', cookieString);
            }
        }

        for (const item of urls) {
            response = await fetch(item, {
                method: 'HEAD',
                headers,
            });
            if (response.ok !== true && options?.ignoreStatusCodeError !== true) {
                console.error(`Response returned ${response.status} "${response.statusText}" by url ${item}`);
            } else {
                result.push(item);
            }
        }

        return result;
    }

    /**
     *
     * @param {*} page Page object (playwright)
     * @param {string} name
     * @param {string|string[]|object} urlList
     * @param {object} options
     */
    async processItem(context, name, urlList) {
        try {
            const result = ['', ''];
            const page = await context.newPage();

            for (const url of urlList) {
                await page.goto(url);

                await page.waitForLoadState();

                if (this.screenshots) {
                    const url = await page.url();
                    await page.screenshot({ path: `${this.output}/${url.replaceAll(/\W/g, '_')}.png` });
                }

                const { css, errors } = await this.#processItemGrab(page, name);

                result[0] += css;

                if (errors) {
                    result[1] += errors + '\n\r';
                }
            }

            await page.close();

            if (result[1].length > 0) {
                console.error(result[1]);
            }

            const resultMinified = minify((this.presetStyles ?? '') + result[0]).css;

            if (!existsSync(this.output)) {
                await mkdir(this.output, { recursive: true });
            }

            await writeFile(path.resolve(this.output, this.#getFilenameCriticalCSS(name)), resultMinified);
        } catch (err) {
            process.stderr.write(`Page: ${name}\n\r${err.message}\n\r${err.stack}`);
            process.exit(1);
        }
    }
}

export { Cratocss };
