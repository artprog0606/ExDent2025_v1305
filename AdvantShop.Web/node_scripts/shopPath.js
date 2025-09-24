import path from 'path';
import fs from 'fs';
import { fileURLToPath } from 'url';
import { projectsNames, projectsAreas } from './shopVariables.js';

const __dirname = getDirname(import.meta.url);

export class ProjectPathData {
    #projectName;
    #displayName;
    #physicalPath;
    #virtualPath;
    #relativePathTooRoot;
    #objPath;
    #objName;

    #rootApp;
    constructor(projectName, displayName, physicalPath, virtualPath, relativePathTooRoot, objPath = null, rootApp = path.resolve(__dirname, '..')) {
        this.#projectName = projectName;
        this.#displayName = displayName;
        this.#physicalPath = physicalPath;
        this.#virtualPath = virtualPath;
        this.#relativePathTooRoot = relativePathTooRoot;
        this.#objPath = objPath;
        this.#rootApp = rootApp;

        if (objPath != null) {
            this.#parseObjPath();
        }
    }

    #parseObjPath() {
        const folderName = getFolderName(this.#objPath);

        if (this.#objPath.replace(this.#rootApp, ``).includes('Templates')) {
            this.#objName = folderName;
        }
    }

    getName() {
        return this.#objName;
    }

    createPathData(objPath) {
        return new ProjectPathData(this.#projectName, this.#displayName, this.#physicalPath, this.#virtualPath, this.#relativePathTooRoot, objPath);
    }

    getProjectName() {
        return this.#projectName;
    }

    getDisplayName() {
        return this.#displayName;
    }

    getPhysicalPath() {
        return path.join(this.#objPath || this.#physicalPath, path.sep);
    }

    getVirtualPath() {
        return (
            this.#virtualPath +
            (this.isTemplate() ? 'Templates/' : '') +
            (this.#objName != null ? this.#objName + '/' : '') +
            (this.isMobile() ? 'Areas/Mobile/' : '')
        );
    }

    getPublicPath() {
        return this.getVirtualPath();
    }

    getRelativePathToRoot() {
        let addingPath = '';
        if (this.#objPath != null) {
            if (this.#projectName === projectsNames.store) {
                addingPath = this.isMobile() ? '../../' : '';
            } else if (this.#projectName === projectsNames.templates) {
                addingPath = this.isMobile() ? '../../' : '';
            } else if (this.#projectName === projectsNames.admin) {
                addingPath = this.isTemplate() ? '../../' : '';
            }
        }
        return this.#relativePathTooRoot + addingPath;
    }

    getBundlesFilePath() {
        return path.resolve(this.getPhysicalPath(), `dist`, `bundles.json`);
    }

    getDefault() {
        if (this.isTemplate()) {
            const projectKeyMain = this.getProjectName();
            return projectsPathData.get(projectKeyMain);
        } else {
            throw new Error(`Imposible get default bundles.json because not template`);
        }
    }

    isMobile() {
        return (this.#objPath != null ? this.#objPath : this.#physicalPath).replace(this.#rootApp, ``).includes(path.join('Areas', 'Mobile'));
    }

    isTemplate() {
        return (this.#objPath != null ? this.#objPath : this.#physicalPath).replace(this.#rootApp, ``).includes('Templates');
    }
}

export const projectsPathData = new Map();

const storePathData = new ProjectPathData(projectsNames.store, 'Витрина магазина', path.join(__dirname, '..', path.sep), projectsAreas.store, '../');
projectsPathData.set(projectsNames.store, storePathData);

const adminPathData = new ProjectPathData(
    projectsNames.admin,
    'Панель администрирования',
    path.join(__dirname, '..', 'Areas', 'Admin', path.sep),
    projectsAreas.admin,
    '../../../',
);
projectsPathData.set(projectsNames.admin, adminPathData);

const funnelsPathData = new ProjectPathData(
    projectsNames.funnels,
    'Воронки',
    path.join(__dirname, '..', 'Areas', 'Landing', path.sep),
    projectsAreas.funnels,
    '../../../',
);
projectsPathData.set(projectsNames.funnels, funnelsPathData);

const partnersPathData = new ProjectPathData(
    projectsNames.partners,
    'Партнёрский кабинет',
    path.join(__dirname, '..', 'Areas', 'Partners', path.sep),
    projectsAreas.partners,
    '../../../',
);
projectsPathData.set(projectsNames.partners, partnersPathData);

const templatesPathData = new ProjectPathData(
    projectsNames.templates,
    'Шаблоны',
    path.join(__dirname, '..', 'Templates', path.sep),
    projectsAreas.templates,
    '../../../',
);
projectsPathData.set(projectsNames.templates, templatesPathData);

const modulesPathData = new ProjectPathData(
    projectsNames.modules,
    'Модули',
    path.join(__dirname, '..', 'Modules', path.sep),
    projectsAreas.modules,
    '../../../',
);

projectsPathData.set(projectsNames.modules, modulesPathData);

export function getDirectories(source) {
    return fs
        .readdirSync(path.resolve(source), { withFileTypes: true })
        .filter((dirent) => dirent.isDirectory())
        .map((dirent) => dirent.name);
}

export function removeAllFilesSync(directory) {
    const files = fs.readdirSync(directory);

    for (const file of files) {
        fs.unlinkSync(path.join(directory, file));
    }
}

export function removeAllFiles(directory) {
    return fs.promises.readdir(directory).then((files) => {
        let promises = files.map((filesItem) => fs.promises.unlink(path.join(directory, filesItem)));
        return Promise.all(promises);
    });
}

/**
 * Поиск файлов по расширению
 * @param {string} startPath Начальный каталог поиска
 * @param {RegExp|Function} filter Регулярное выражение поиска
 * @param {Function} callback Функция обратного вызова
 */
export function getFilesListByExt(startPath, filter) {
    let list = [];

    (function findItem(startPath, filter) {
        if (!fs.existsSync(startPath)) {
            console.log('no dir ', startPath);
            return;
        }

        var files = fs.readdirSync(startPath);
        for (var i = 0; i < files.length; i++) {
            var filename = path.join(startPath, files[i]);
            var stat = fs.lstatSync(filename);
            if (stat.isDirectory()) {
                findItem(filename, filter); //recurse
            } else {
                let result = false;
                if (typeof filter === 'function') {
                    result = filter(filename) === true;
                } else if (filter instanceof RegExp) {
                    result = filter.test(filename) === true;
                }

                if (result === true) {
                    list.push(filename);
                }
            }
        }
    })(startPath, filter);

    return list;
}

export function getDirname(url) {
    const __filename = fileURLToPath(url);
    return path.dirname(__filename);
}

export function getFolderName(filename) {
    return path.parse(filename.replace(path.join('Areas', 'Mobile'), '')).base;
}

export const getPathPages = async (objPath) => {
    const pathPages = path.resolve(objPath, 'bundle_config', '_pages.js');

    const original = await import(`file://` + pathPages);

    return Object.assign(Object.create(Object.getPrototypeOf(original.default)), original.default);
};
