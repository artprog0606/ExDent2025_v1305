import { config } from './urlHelper.config.js';
import { projectsAreas } from '../../../node_scripts/shopVariables.js';

const regexDomain = new RegExp('^[a-z0-9]{2,}://[a-z0-9.]+', 'i');

/**
 * Class is singleton
 */

export class urlHelper {
    constructor(tagBaseHref, urlHelperConfig) {
        if (urlHelper.instance) {
            return urlHelper.instance;
        }

        urlHelper.instance = this;

        this.urlHelperConfig = urlHelperConfig || config;
        this.tagBaseHref = tagBaseHref || document.documentElement.baseURI;
    }

    /**
     * get url param value by name.
     * @param {string} paramName  param name
     * @param {boolean} toLower  - convert to lower case
     * @returns {string|null}
     */
    getUrlParam(paramName, toLower) {
        paramName = toLower !== false ? paramName.toLowerCase() : paramName;
        const query = toLower !== false ? window.location.search.substring(1).toLowerCase() : window.location.search.substring(1);
        const lets = query.split('&');

        for (let i = 0; i < lets.length; i++) {
            const pair = lets[i].split('=');
            if (pair[0] == paramName) {
                return pair[1];
            }
        }
        return null;
    }

    /**
     * get url param value by name
     * @param {string} name
     * @returns {string|null}
     */
    getUrlParamByName(name) {
        const url = window.location.href.toLowerCase();
        name = name.toLowerCase().replace(/[\[\]]/g, '\\$&');
        const regex = new RegExp(`[?&]${name}(=([^&#]*)|&|#|$)`),
            results = regex.exec(url);
        if (!results) return null;
        if (!results[2]) return '';

        return decodeURIComponent(results[2].replace(/\+/g, ' '));
    }

    /**
     *
     * @param {Function} fn  - filter param name
     * @param {object} paramsAsObject  - object to validate
     * @returns {object}
     */
    getUrlParamDictionaryByNameFunc(fn, paramsAsObject) {
        if (!paramsAsObject) return;

        const result = [];
        const paramsName = Object.keys(paramsAsObject);

        for (let i = 0; i < paramsName.length; i++) {
            if (fn(paramsName[i]) === true) {
                result.push({
                    name: paramsName[i],
                    value: paramsAsObject[paramsName[i]],
                });
            }
        }

        return result;
    }

    /**
     * convert string url params in object
     * @param {string} string
     * @returns {object}
     */
    getUrlParamsAsObject(string) {
        const searchParams = new URLSearchParams(string);
        const obj = {};

        searchParams.forEach((value, key) => {
            obj[key] = value;
        });

        return obj;
    }

    getBaseHref() {
        return this.tagBaseHref;
    }

    /**
     * check that url is domain
     * @param {string} url
     * @returns {boolean}
     */
    hasDomain(url) {
        return regexDomain.test(url);
    }
    addSlash(url) {
        return url.at(-1) === '/' ? url : `${url}/`;
    }

    /** get absolute url
     * @param {string} url
     * @param {boolean} excludeAdmin
     * @returns {string} absolute url
     */
    getAbsUrl(url, excludeAdmin) {
        const base = this.getBaseHref();
        let basePrepare;

        if ((excludeAdmin != null && excludeAdmin === true) || this.urlHelperConfig.isAdmin === false) {
            basePrepare = base.replace(this.urlHelperConfig.adminPath, '');
        } else {
            basePrepare = base;
        }

        if (this.hasDomain(url) === false) {
            //убераем впереди слеш
            if (url.charAt(0) === '/') {
                url = url.substring(1);
            }

            url = this.addSlash(basePrepare) + url;
        }

        return url;
    }

    getUrl(url, excludeAdmin) {
        const base = this.getBaseHref();
        let basePrepare = base.replace(/.*\/\/[^\/]*/, '');

        if ((excludeAdmin != null && excludeAdmin === true) || this.urlHelperConfig.isAdmin === false) {
            basePrepare = basePrepare.replace(this.urlHelperConfig.adminPath, '');
        }

        if (this.hasDomain(url) === false) {
            if (url.charAt(0) === '/') {
                url = url.substring(1);
            }
            url = this.addSlash(basePrepare) + url;
        }
        return url;
    }

    /**
     * convert object params in string param.
     * Concat by "&"
     * @param {object} object
     * @returns {string}
     */
    paramsToString(object) {
        //let result = "";
        const result = [];
        for (const key in object) {
            if (Object.hasOwn(object, key)) {
                //result += key + "=" + object[key] + "&";
                result.push(`${key}=${object[key]}`);
            }
        }

        return result.join('&');
    }

    /**
     *  Update parameters in URL.
     *  @example
     *  If value null - remove parameter.
     *  If key not found in URL params - add new param in URL
     *  If value not null and key found - update old value on new value
     * @param {string} uri
     * @param {string} key
     * @param {string | null} value
     * @returns {string} new search params
     */
    updateQueryStringParameter(uri, key, value) {
        const _uri = new URL(uri);
        const searchParams = new URLSearchParams(_uri.search);

        if (value == null) {
            searchParams.delete(key);
        } else if (searchParams.has(key)) {
            searchParams.set(key, value);
        } else {
            searchParams.append(key, value);
        }

        _uri.search = searchParams.toString();

        return _uri.toString();
    }

    /**
     *
     * @param {string} key
     * @param {string} value
     * @param {boolean} replace
     */
    setLocationQueryParams(key, value, replace) {
        const url = this.updateQueryStringParameter(window.location.href, key, value);
        history[replace ? 'replaceState' : 'pushState']({ url }, '', url);
    }

    /**
     *
     * @param {string} string
     * @param {boolean} withHash
     * @returns {string}
     */
    getHashFromString(string, withHash) {
        const hash = string.split('#')[1];
        return withHash ? `#${hash}` : hash;
    }

    getPhysicalPath(area, isMobile = false) {
        const key = area + (isMobile ? 'Mobile' : '');

        if (projectsAreas.hasOwnProperty(key) === false) {
            throw new Error(`urlHelper: unknown area is "${area}"`);
        }
        return projectsAreas[key];
    }

    normalizeBundleFilesUrl(projectArea, url) {
        return url.replace(/^..\/(assets|chunks|entries)\//u, `${projectArea}dist/$1/`);
    }

    toAnchor(value) {
        return value.replaceAll(/https?|:|\/\//g, '').replaceAll(/\//g, '_');
    }

    getSubfolder() {
        const pathSplitted = this.getBaseHref()
            .replace(window.location.origin, '')
            .replace(/^\//u, '')
            .split('/')
            .filter((item) => item !== 'adminv3');
        return `/${pathSplitted.length > 0 ? pathSplitted.at(0) : ''}`;
    }

    transformUrlToKey(url) {
        return url
            .replaceAll(/\W|https?|www/gu, '_')
            .replaceAll('__', '')
            .replaceAll(/_*$/gu, '');
    }

    transformBaseUriToKey(excludeAdmin = true) {
        const baseUri = this.getBaseHref();

        return this.transformUrlToKey(excludeAdmin ? baseUri.replace(/(?<admin>adminv3|admin|adminv2)\/?$/iu, '') : baseUri);
    }
}
