import { projectsNames } from '../shopVariables.js';
import path from 'node:path';
import process from 'node:process';
import svgSpritePlugin from './esbuild/plugins/esbuild-svg-sprite-plugin/index.js';
import { projectsPathData } from '../shopPath.js';
import fg from 'fast-glob';
import { existsSync } from 'node:fs';
import { getEnvParamsForConfig } from './buildHelpers.js';

export const getPrepareData = (getConfig) => {
    const getConfigListForStore = async () => {
        const project = projectsNames.store;
        const mapConfigs = new Map();
        //Store
        const envParams = await getEnvParamsForConfig(project);
        //StoreMobile
        const envParamsMobile = await getEnvParamsForConfig(project, path.resolve(process.cwd(), 'Areas', 'Mobile'));

        mapConfigs.set('store', { config: getConfig(envParams), env: envParams });
        mapConfigs.set('storeMobile', { config: getConfig(envParamsMobile), env: envParamsMobile });

        return mapConfigs;
    };
    const getConfigListForAdmin = async () => {
        const project = projectsNames.admin;
        const mapConfigs = new Map();
        //Admin
        const envParams = await getEnvParamsForConfig(project);
        //AdminV3
        const envParamsAdminV3 = await getEnvParamsForConfig(project, path.resolve(process.cwd(), 'Areas', 'Admin', 'Templates', 'AdminV3'));
        //AdminMobile
        const envParamsAdminMobile = await getEnvParamsForConfig(project, path.resolve(process.cwd(), 'Areas', 'Admin', 'Templates', 'Mobile'));

        mapConfigs.set('admin', { config: getConfig(envParams), env: envParams });
        mapConfigs.set('adminV3', { config: getConfig(envParamsAdminV3), env: envParamsAdminV3 });

        const adminMobileConfig = getConfig(envParamsAdminMobile);
        const spritemapName = `spritemap.${new Date().getTime()}.svg`;

        adminMobileConfig.plugins.push(
            svgSpritePlugin(
                {
                    dest: adminMobileConfig.outdir,
                    shape: {
                        id: {
                            generator: (filename) => 'sprite-' + filename.replace('.svg', ''),
                        },
                    },
                    mode: {
                        symbol: {
                            bust: true,
                        },
                    },
                },
                spritemapName,
            ),
        );

        envParamsAdminMobile.spritemapName = spritemapName;

        mapConfigs.set('adminMobile', { config: adminMobileConfig, env: envParamsAdminMobile });

        return mapConfigs;
    };
    const getConfigListForFunnels = async () => {
        const project = projectsNames.funnels;
        const mapConfigs = new Map();

        const envParams = await getEnvParamsForConfig(project);

        mapConfigs.set('funnels', { config: getConfig(envParams), env: envParams });

        return mapConfigs;
    };
    const getConfigListForPartners = async () => {
        const project = projectsNames.partners;
        const mapConfigs = new Map();

        const envParams = await getEnvParamsForConfig(project);

        mapConfigs.set('partners', { config: getConfig(envParams), env: envParams });

        return mapConfigs;
    };
    const getConfigListForTemplates = async (templatesPath = '*') => {
        const mapConfigs = new Map();
        let _templatesPath = templatesPath;

        if (templatesPath === '*') {
            const templatesPathData = projectsPathData.get(projectsNames.templates);

            _templatesPath = await fg.async('*', {
                cwd: templatesPathData.getPhysicalPath(),
                onlyDirectories: true,
                absolute: true,
            });
        }

        let mapKey, envParams, envParamsMobile;
        for (const templatesPathItem of _templatesPath) {
            mapKey = 'template ' + templatesPathItem.split(path.sep).at(-1);
            envParams = await getEnvParamsForConfig(projectsNames.templates, templatesPathItem);
            mapConfigs.set(mapKey, {
                config: getConfig(envParams),
                env: envParams,
            });
            if (existsSync(path.join(templatesPathItem, 'Areas', 'Mobile')) === true) {
                envParamsMobile = await getEnvParamsForConfig(projectsNames.templates, path.join(templatesPathItem, 'Areas', 'Mobile'));
                mapConfigs.set(mapKey + ' mobile', {
                    config: getConfig(envParamsMobile),
                    env: envParamsMobile,
                });
            }
        }

        return mapConfigs;
    };
    const getConfigListForModules = async (modulesPath = '*') => {
        const mapConfigs = new Map();
        let _modulesPath = modulesPath;

        if (modulesPath === '*') {
            const modulesPathData = projectsPathData.get(projectsNames.modules);

            _modulesPath = await fg.async('*', {
                cwd: modulesPathData.getPhysicalPath(),
                onlyDirectories: true,
                absolute: true,
            });
        }

        let envParams;
        for (const modulesPathItem of _modulesPath) {
            if (existsSync(path.join(modulesPathItem, 'bundle_config')) === true) {
                envParams = await getEnvParamsForConfig(projectsNames.modules, modulesPathItem);
                mapConfigs.set('module ' + modulesPathItem.split(path.sep).at(-1), {
                    config: getConfig(envParams),
                    env: envParams,
                });
            }
        }

        return mapConfigs;
    };

    return {
        getConfigListForStore,
        getConfigListForAdmin,
        getConfigListForFunnels,
        getConfigListForPartners,
        getConfigListForTemplates,
        getConfigListForModules,
    };
};
