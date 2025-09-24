import path from 'node:path';
import { projectsNames } from './shopVariables.js';
import { ProjectPathData, getDirname } from './shopPath.js';

const __dirname = getDirname(import.meta.url);

const errorMessageOnlyTemplate = `Imposible get default bundles.json because not template`;

describe('ProjectPathData', () => {
    describe('desktop version', () => {
        test('should return store path data', () => {
            const displayName = 'Витрина магазина';
            const storePathData = new ProjectPathData(projectsNames.store, displayName, path.join(__dirname, '..', path.sep), './', '../');

            expect(storePathData.getName()).toBeUndefined();
            expect(storePathData.getProjectName()).toBe(projectsNames.store);
            expect(storePathData.getDisplayName()).toBe(displayName);
            expect(storePathData.getPhysicalPath()).toBe(path.join(__dirname, '..', path.sep));
            expect(storePathData.getVirtualPath()).toBe('./');
            expect(storePathData.getPublicPath()).toBe(storePathData.getVirtualPath());
            expect(storePathData.getRelativePathToRoot()).toBe('../');
            expect(storePathData.getBundlesFilePath()).toBe(path.resolve(storePathData.getPhysicalPath(), `dist`, `bundles.json`));
            expect(() => storePathData.getDefault()).toThrowError(errorMessageOnlyTemplate);
            expect(storePathData.isMobile()).toBeFalsy();
            expect(storePathData.isTemplate()).toBeFalsy();
        });

        test('should return template path data', () => {
            const displayName = 'Шаблоны';
            const physicalPath = path.join(__dirname, '..', 'Templates', path.sep, 'Yes2000', path.sep);
            const virtualPath = './Templates/Yes2000/';
            const templatesPathDataBase = new ProjectPathData(
                projectsNames.templates,
                displayName,
                path.join(__dirname, '..', 'Templates', path.sep),
                './',
                '../../../',
            );

            const templatesPathData = templatesPathDataBase.createPathData(physicalPath);

            expect(templatesPathData.getName()).toBe('Yes2000');
            expect(templatesPathData.getProjectName()).toBe(projectsNames.templates);
            expect(templatesPathData.getDisplayName()).toBe(displayName);
            expect(templatesPathData.getPhysicalPath()).toBe(physicalPath);
            expect(templatesPathData.getVirtualPath()).toBe(virtualPath);
            expect(templatesPathData.getPublicPath()).toBe(templatesPathData.getVirtualPath());
            expect(templatesPathData.getRelativePathToRoot()).toBe('../../../');
            expect(templatesPathData.getBundlesFilePath()).toBe(path.resolve(templatesPathData.getPhysicalPath(), `dist`, `bundles.json`));
            expect(templatesPathData.getDefault()).toEqual(templatesPathDataBase);
            expect(templatesPathData.isMobile()).toBeFalsy();
            expect(templatesPathData.isTemplate()).toBeTruthy();
        });

        test('should return admin path data', () => {
            const displayName = 'Панель администрирования';
            const virtualPath = '../Areas/Admin/';
            const physicalPath = path.join(__dirname, '..', 'Areas', 'Admin', path.sep);
            const adminPathData = new ProjectPathData(projectsNames.admin, displayName, physicalPath, virtualPath, '../../../');

            expect(adminPathData.getName()).toBeUndefined();
            expect(adminPathData.getProjectName()).toBe(projectsNames.admin);
            expect(adminPathData.getDisplayName()).toBe(displayName);
            expect(adminPathData.getPhysicalPath()).toBe(physicalPath);
            expect(adminPathData.getVirtualPath()).toBe(virtualPath);
            expect(adminPathData.getPublicPath()).toBe(adminPathData.getVirtualPath());
            expect(adminPathData.getRelativePathToRoot()).toBe('../../../');
            expect(adminPathData.getBundlesFilePath()).toBe(path.resolve(adminPathData.getPhysicalPath(), `dist`, `bundles.json`));
            expect(() => adminPathData.getDefault()).toThrowError(errorMessageOnlyTemplate);
            expect(adminPathData.isMobile()).toBeFalsy();
            expect(adminPathData.isTemplate()).toBeFalsy();
        });

        test('should return adminv3 path data', () => {
            const displayName = 'Панель администрирования';
            const virtualPath = '../Areas/Admin/';
            const physicalPath = path.join(__dirname, '..', 'Areas', 'Admin', path.sep);
            const adminPathData = new ProjectPathData(projectsNames.admin, displayName, physicalPath, virtualPath, '../../../');
            const physicalPathAdminV3 = path.join(__dirname, '..', '..', 'Areas', 'Admin', 'Templates', 'AdminV3', path.sep);
            const adminv3PathData = adminPathData.createPathData(physicalPathAdminV3);
            expect(adminv3PathData.getName()).toBe('AdminV3');
            expect(adminv3PathData.getProjectName()).toBe(projectsNames.admin);
            expect(adminv3PathData.getDisplayName()).toBe(displayName);
            expect(adminv3PathData.getPhysicalPath()).toBe(physicalPathAdminV3);
            expect(adminv3PathData.getVirtualPath()).toBe(virtualPath + 'Templates/AdminV3/');
            expect(adminv3PathData.getPublicPath()).toBe(adminv3PathData.getVirtualPath());
            expect(adminv3PathData.getRelativePathToRoot()).toBe('../../../../../');
            expect(adminv3PathData.getBundlesFilePath()).toBe(path.resolve(adminv3PathData.getPhysicalPath(), `dist`, `bundles.json`));
            expect(adminv3PathData.getDefault()).toEqual(adminPathData);
            expect(adminv3PathData.isMobile()).toBeFalsy();
            expect(adminv3PathData.isTemplate()).toBeTruthy();
        });

        test('should return partners path data', () => {
            const displayName = 'Партнёрский кабинет';
            const virtualPath = '../Areas/Partners/';
            const physicalPath = path.join(__dirname, '..', 'Areas', 'Partners', path.sep);
            const partnersPathData = new ProjectPathData(projectsNames.partners, displayName, physicalPath, virtualPath, '../../../');

            expect(partnersPathData.getName()).toBeUndefined();
            expect(partnersPathData.getProjectName()).toBe(projectsNames.partners);
            expect(partnersPathData.getDisplayName()).toBe(displayName);
            expect(partnersPathData.getPhysicalPath()).toBe(physicalPath);
            expect(partnersPathData.getVirtualPath()).toBe(virtualPath);
            expect(partnersPathData.getPublicPath()).toBe(partnersPathData.getVirtualPath());
            expect(partnersPathData.getRelativePathToRoot()).toBe('../../../');
            expect(partnersPathData.getBundlesFilePath()).toBe(path.resolve(partnersPathData.getPhysicalPath(), `dist`, `bundles.json`));
            expect(() => partnersPathData.getDefault()).toThrowError(errorMessageOnlyTemplate);
            expect(partnersPathData.isMobile()).toBeFalsy();
            expect(partnersPathData.isTemplate()).toBeFalsy();
        });

        test('should return funnels path data', () => {
            const displayName = 'Воронки';
            const virtualPath = 'Areas/Landing/';
            const physicalPath = path.join(__dirname, '..', 'Areas', 'Landing', path.sep);
            const funnelsPathData = new ProjectPathData(projectsNames.funnels, displayName, physicalPath, virtualPath, '../../../');

            expect(funnelsPathData.getName()).toBeUndefined();
            expect(funnelsPathData.getProjectName()).toBe(projectsNames.funnels);
            expect(funnelsPathData.getDisplayName()).toBe(displayName);
            expect(funnelsPathData.getPhysicalPath()).toBe(physicalPath);
            expect(funnelsPathData.getVirtualPath()).toBe(virtualPath);
            expect(funnelsPathData.getPublicPath()).toBe(funnelsPathData.getVirtualPath());
            expect(funnelsPathData.getRelativePathToRoot()).toBe('../../../');
            expect(funnelsPathData.getBundlesFilePath()).toBe(path.resolve(funnelsPathData.getPhysicalPath(), `dist`, `bundles.json`));
            expect(() => funnelsPathData.getDefault()).toThrowError(errorMessageOnlyTemplate);
            expect(funnelsPathData.isMobile()).toBeFalsy();
            expect(funnelsPathData.isTemplate()).toBeFalsy();
        });

        test('should return modules path data', () => {
            const displayName = 'Модули';
            const virtualPath = './';
            const physicalPath = path.join(__dirname, '..', 'Modules', path.sep);
            const modulesPathData = new ProjectPathData(projectsNames.modules, displayName, physicalPath, virtualPath, '../../../');

            expect(modulesPathData.getName()).toBeUndefined();
            expect(modulesPathData.getProjectName()).toBe(projectsNames.modules);
            expect(modulesPathData.getDisplayName()).toBe(displayName);
            expect(modulesPathData.getPhysicalPath()).toBe(physicalPath);
            expect(modulesPathData.getVirtualPath()).toBe(virtualPath);
            expect(modulesPathData.getPublicPath()).toBe(modulesPathData.getVirtualPath());
            expect(modulesPathData.getRelativePathToRoot()).toBe('../../../');
            expect(modulesPathData.getBundlesFilePath()).toBe(path.resolve(modulesPathData.getPhysicalPath(), `dist`, `bundles.json`));
            expect(() => modulesPathData.getDefault()).toThrowError(errorMessageOnlyTemplate);
            expect(modulesPathData.isMobile()).toBeFalsy();
            expect(modulesPathData.isTemplate()).toBeFalsy();
        });
    });

    describe('mobile version', () => {
        const getMobilePath = (objPath) => path.join(objPath, 'Areas', 'Mobile');

        test('should return store path data', () => {
            const displayName = 'Витрина магазина';
            const storePathData = new ProjectPathData(projectsNames.store, displayName, path.join(__dirname, '..', path.sep), './', '../');

            const storeMobilePathData = storePathData.createPathData(getMobilePath(path.join(__dirname, '..', path.sep)));
            expect(storeMobilePathData.getName()).toBeUndefined();
            expect(storeMobilePathData.getProjectName()).toBe(projectsNames.store);
            expect(storeMobilePathData.getDisplayName()).toBe(displayName);
            expect(storeMobilePathData.getPhysicalPath()).toBe(path.join(__dirname, '..', path.sep, 'Areas', 'Mobile', path.sep));
            expect(storeMobilePathData.getVirtualPath()).toBe('./Areas/Mobile/');
            expect(storeMobilePathData.getPublicPath()).toBe(storeMobilePathData.getVirtualPath());
            expect(storeMobilePathData.getRelativePathToRoot()).toBe('../../../');
            expect(storeMobilePathData.getBundlesFilePath()).toBe(path.resolve(storeMobilePathData.getPhysicalPath(), `dist`, `bundles.json`));
            expect(() => storeMobilePathData.getDefault()).toThrowError(errorMessageOnlyTemplate);
            expect(storeMobilePathData.isMobile()).toBeTruthy();
            expect(storeMobilePathData.isTemplate()).toBeFalsy();
        });

        test('should return template path data', () => {
            const displayName = 'Шаблоны';
            const physicalPath = path.join(__dirname, '..', 'Templates', path.sep, 'Yes2000', path.sep);
            const virtualPath = './Templates/Yes2000/';
            const templatesPathDataBase = new ProjectPathData(
                projectsNames.templates,
                displayName,
                path.join(__dirname, '..', 'Templates', path.sep),
                './',
                '../../../',
            );

            const templatesPathData = templatesPathDataBase.createPathData(physicalPath);
            const templatesMobilePathData = templatesPathData.createPathData(getMobilePath(templatesPathData.getPhysicalPath()));
            expect(templatesMobilePathData.getName()).toBe('Yes2000');
            expect(templatesMobilePathData.getProjectName()).toBe(projectsNames.templates);
            expect(templatesMobilePathData.getDisplayName()).toBe(displayName);
            expect(templatesMobilePathData.getPhysicalPath()).toBe(path.join(templatesPathData.getPhysicalPath(), 'Areas', 'Mobile', path.sep));
            expect(templatesMobilePathData.getVirtualPath()).toBe(virtualPath + 'Areas/Mobile/');
            expect(templatesMobilePathData.getPublicPath()).toBe(templatesMobilePathData.getVirtualPath());
            expect(templatesMobilePathData.getRelativePathToRoot()).toBe('../../../../../');
            expect(templatesMobilePathData.getBundlesFilePath()).toBe(
                path.resolve(templatesMobilePathData.getPhysicalPath(), `dist`, `bundles.json`),
            );
            expect(templatesMobilePathData.getDefault()).toEqual(templatesPathDataBase);
            expect(templatesMobilePathData.isMobile()).toBeTruthy();
            expect(templatesMobilePathData.isTemplate()).toBeTruthy();
        });

        test('should return admin path data', () => {
            const displayName = 'Панель администрирования';
            const virtualPath = '../Areas/Admin/';
            const physicalPath = path.join(__dirname, '..', 'Areas', 'Admin', path.sep);
            const adminPathData = new ProjectPathData(projectsNames.admin, displayName, physicalPath, virtualPath, '../../../');

            const physicalPathAdminV3 = path.join(__dirname, '..', '..', 'Areas', 'Admin', 'Templates', 'Mobile', path.sep);
            const adminvMobilePathData = adminPathData.createPathData(physicalPathAdminV3);
            expect(adminvMobilePathData.getName()).toBe('Mobile');
            expect(adminvMobilePathData.getProjectName()).toBe(projectsNames.admin);
            expect(adminvMobilePathData.getDisplayName()).toBe(displayName);
            expect(adminvMobilePathData.getPhysicalPath()).toBe(physicalPathAdminV3);
            expect(adminvMobilePathData.getVirtualPath()).toBe(virtualPath + 'Templates/Mobile/');
            expect(adminvMobilePathData.getPublicPath()).toBe(adminvMobilePathData.getVirtualPath());
            expect(adminvMobilePathData.getRelativePathToRoot()).toBe('../../../../../');
            expect(adminvMobilePathData.getBundlesFilePath()).toBe(path.resolve(adminvMobilePathData.getPhysicalPath(), `dist`, `bundles.json`));
            expect(adminvMobilePathData.getDefault()).toEqual(adminPathData);
            expect(adminvMobilePathData.isMobile()).toBeFalsy();
            expect(adminvMobilePathData.isTemplate()).toBeTruthy();
        });
    });
});
