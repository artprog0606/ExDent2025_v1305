import { projectsNames } from '../shopVariables.js';
import { getOptions } from './options.js';
import { setGitHooks } from '../githooks.js';
import { runWatch } from './esbuild/index.js';

import { getPrepareData } from './prepareData.js';

import { afterBuild } from './afterBuild.js';

import { installPackagesFromCustomPackageJson } from './buildHelpers.js';
import child_process from 'child_process';
import { compileStylesExternal } from './esbuild/postbuild/compileStylesExternal.js';
import * as esbuild from 'esbuild';
import esbuildWatchCallbackPlugin from './esbuild/plugins/esbuild-watch-callback-plugin/index.js';

setGitHooks();

let { mode, watch, templates, modules, profile, cdnDesign, project, tsc } = await getOptions();

const { getConfig } = mode === 'dev' ? await import('./esbuild/dev.js') : await import('./esbuild/prod.js');

const {
    getConfigListForTemplates,
    getConfigListForStore,
    getConfigListForPartners,
    getConfigListForModules,
    getConfigListForFunnels,
    getConfigListForAdmin,
} = getPrepareData(getConfig);

let configList = new Map();

if (project === '*' || project === 'allWithoutModulesAndTemplates') {
    configList = new Map(
        [...(await getConfigListForStore())]
            .concat([...(await getConfigListForAdmin())])
            .concat([...(await getConfigListForFunnels())])
            .concat([...(await getConfigListForPartners())]),
    );

    if (project !== 'allWithoutModulesAndTemplates') {
        configList = new Map([...configList].concat([...(await getConfigListForTemplates())]).concat([...(await getConfigListForModules())]));
    }
} else if (modules != null || templates != null) {
    let objForBuild = modules || templates;
    objForBuild = objForBuild === '*' || objForBuild[0] === '*' ? '*' : objForBuild;
    configList = project === projectsNames.templates ? await getConfigListForTemplates(objForBuild) : await getConfigListForModules(objForBuild);
} else {
    switch (project) {
        case projectsNames.store:
            configList = await getConfigListForStore();
            break;
        case projectsNames.admin:
            configList = await getConfigListForAdmin();
            break;
        case projectsNames.funnels:
            configList = await getConfigListForFunnels();
            break;
        case projectsNames.partners:
            configList = await getConfigListForPartners();
            break;
        default:
            throw new Error(`Unknown project "${project}" for bundle`);
    }
}

if (tsc === true) {
    const spawnBuffer = child_process.spawn('tsc', watch ? ['-w'] : undefined, {
        shell: true,
        stdio: 'inherit',
    });
}

let watchData = new Map();

for (const [name, value] of configList) {
    if (value.env.project !== projectsNames.store) {
        installPackagesFromCustomPackageJson(value.env.directoryWork);
    }

    compileStylesExternal(value.env, watch, cdnDesign);

    if (watch) {
        value.config.plugins.push(
            esbuildWatchCallbackPlugin(name, async (bundleResult) => {
                await afterBuild(name, value.config, value.env, bundleResult, mode);
            }),
        );
        const ctx = await esbuild.context(value.config);
        watchData.set(name, ctx);
    } else {
        console.log(`[${new Date().toLocaleTimeString()}] Build start: ${name}`);
        let bundleResult = await esbuild.build(value.config);
        await afterBuild(name, value.config, value.env, bundleResult, mode);
        console.log(`[${new Date().toLocaleTimeString()}] Build complete: ${name}`);
    }
}

if (watch) {
    await runWatch(watchData);
} else {
    process.exit(0);
}
