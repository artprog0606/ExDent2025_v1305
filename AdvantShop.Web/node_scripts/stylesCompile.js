import * as sass from 'sass';
import fs from 'fs';
import path from 'path';
import postcss from 'postcss';
import { postcssConfigFn } from '../postcss.config.js';

export class CompileSync {
    constructor(variables, options = { cssFilename: null, ignore: [] }) {
        this.variables = { ...variables };
        this.options = { ...options };
    }

    #loopIgnore(ignoreItem, filename) {
        if (ignoreItem instanceof RegExp) {
            return ignoreItem.test(filename);
        } else if (typeof ignoreItem === 'string') {
            return filename.includes(ignoreItem);
        } else if (typeof ignoreItem === 'function') {
            return ignoreItem(filename);
        }
    }

    isIgnore(filename) {
        return this.options.ignore != null && this.options.ignore.some((ignoreItem) => this.#loopIgnore(ignoreItem, filename));
    }

    /**
     * Проверка указынный путь это каталог или файл
     * @param {string} objPath Путь, который проверяется
     * @returns {boolean} Возвращает true, если указанный путь ссылается на файл
     */
    checkIsFile(objPath) {
        return fs.statSync(path.resolve(objPath)).isFile();
    }

    /**
     * Разбор и получение данных о переданном пути
     * @param {string} objPath Путь до директории, где располагаются файлы или полный путь к файлу
     * @param {string} name Название файла без расширения
     */
    getPathData(objPath, name) {
        const isFile = this.checkIsFile(objPath);
        let pathRoot;
        let pathSCSS;
        let pathCSS;
        let nameSource;

        if (isFile) {
            const pathParsed = path.parse(objPath);

            if (pathParsed.ext === '.scss') {
                pathSCSS = objPath;
            } else {
                pathCSS = objPath;
            }
            pathRoot = pathParsed.dir;
            nameSource = pathParsed.name;
        } else {
            if (name == null) {
                throw new Error('Parameter "name" is required');
            }

            pathSCSS = path.join(objPath, `${name}.scss`);
            pathCSS = path.join(objPath, `${name}.css`);
            pathRoot = objPath;
            nameSource = name;
        }

        return {
            pathSCSS,
            pathCSS,
            pathRoot,
            nameSource,
        };
    }

    /**
     * Возвращает новое название CSS файла
     * @param {string} fullPath Полный путь до файла
     * @returns {string | null}
     */
    getCssNameNew(fullPath) {
        let result;

        if (this.options.cssFilename == null) {
            result = null;
        } else {
            const { name } = path.parse(fullPath);
            result = this.options.cssFilename.replace('[name]', name);
        }

        return result;
    }

    /**
     * Запускает наблюдение за директорией или файлом с автоматической пересборкой
     * @param {string|string[]} filepath Путь до директории, где располагаются файлы или полный путь к файлу
     */
    watch(filepath) {
        (Array.isArray(filepath) ? filepath : [filepath]).forEach((pathItem) => {
            let timer;
            fs.watch(pathItem, { recursive: true }, (eventType, filename) => {
                if (filename != null) {
                    const { name, ext, dir } = path.parse(filename);
                    if (
                        ext === '.scss' ||
                        (ext === '.css' &&
                            fs.existsSync(path.resolve(pathItem, dir, `${name}.scss`)) === false &&
                            name !== this.options.cssFilename &&
                            this.isIgnore(filename) === false)
                    ) {
                        if (timer != null) {
                            clearTimeout(timer);
                        }
                        timer = setTimeout(() => {
                            this.render(path.resolve(pathItem, filename));
                            console.log(`Compile style: ${filename}`);
                        }, 100);
                    }
                }
            });
        });
    }

    /**
     *
     * @param {string} objPath Путь до директории, где располагаются файлы или полный путь к файлу
     * @param {string} name Название файла без расширения
     */
    render(objPath, name) {
        let { pathRoot, pathSCSS, pathCSS, nameSource } = this.getPathData(objPath, name);
        let cssNameNew;
        let cssContent;

        if (fs.existsSync(pathSCSS)) {
            if (this.isIgnore(pathSCSS) === true) {
                return;
            }

            cssNameNew = this.getCssNameNew(pathSCSS);
            let result = sass.compile(pathSCSS, { linefeed: 'crlf' });
            cssContent = result.css;
        } else if (fs.existsSync(pathCSS)) {
            if (this.isIgnore(pathCSS) === true) {
                return;
            }

            cssNameNew = this.getCssNameNew(pathCSS);
            cssContent = fs.readFileSync(pathCSS);

            if (cssNameNew == null) {
                throw new Error('Need set options.cssFilename when source file type css');
            } else if (cssNameNew === name) {
                throw new Error('CSS names are the same');
            }
        } else {
            throw new Error(`Wrong data objPath "${objPath}" and  name "${name}"`);
        }

        const pathCSSFinish = path.join(pathRoot, `${cssNameNew || name || nameSource}.css`);
        const { plugins } = postcssConfigFn({ options: { variables: this.variables }, file: pathCSSFinish });

        let postcssApi = postcss(plugins).process(cssContent);
        fs.writeFileSync(pathCSSFinish, postcssApi.css);
    }
}
