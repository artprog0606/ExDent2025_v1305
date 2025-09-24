import path from 'node:path';
import { getDirname } from './shopPath.js';

export default class PagesStorage {
    constructor(dirname = '') {
        this.storage = {};
        this.dirname = dirname;
    }

    /**
     *
     * @param {string} entryName Имя бандла
     * @param {string} fileName Файл точки входа бандла
     */
    addItem(entryName, fileName) {
        if (this.storage[entryName] != null) {
            throw new Error(`Is exist ${entryName} in PagesStorage`);
        }

        return (this.storage[entryName] = { fileName: path.normalize(path.resolve(this.dirname, fileName)) });
    }

    /**
     *
     * @param {string} entryName Имя бандла
     * @param {string} fileName Файл точки входа бандла
     */
    replaceItem(entryName, fileName) {
        if (this.storage[entryName] == null) {
            throw new Error(`Not found ${entryName} in PagesStorage`);
        }

        this.storage[entryName].fileName = fileName;

        return this.storage[entryName];
    }

    /**
     *
     * @param {string} entryName Имя бандла
     */
    removeItem(entryName) {
        if (this.storage[entryName] == null) {
            throw new Error(`Not found ${entryName} in PagesStorage`);
        }

        delete this.storage[entryName];

        return this.storage;
    }

    /**
     * Возвращает словарь entries для webpack
     * */
    getEntries() {
        const result = {};
        Object.keys(this.storage).forEach((key) => (result[key] = this.storage[key].fileName));
        return result;
    }

    getList() {
        const result = [];
        Object.keys(this.storage).forEach((key) =>
            result.push({
                out: key,
                in: this.storage[key].fileName,
            }),
        );
        return result;
    }

    assign(pages) {
        Object.assign(this.storage, pages.storage);
    }
}
