import { manifestGenerate } from './esbuild/postbuild/manifest.js';
import { getPathPages, projectsPathData } from '../shopPath.js';
import { replaceHtmlChildResource } from './esbuild/postbuild/replaceHtmlChildResource.js';
import { cssRewriteUrl } from './esbuild/postbuild/cssRewriteUrl.js';
import { projectsNames } from '../shopVariables.js';
import { getJsonFileSync } from './buildHelpers.js';
import { existsSync } from 'node:fs';

export const afterBuild = async (name, config, env, bundleResult, mode) => {
    if (bundleResult.errors.length > 0) {
        return;
    }

    const { append: manifestAppend, generate: manifestGenerateFn } = manifestGenerate({
        filename: 'bundles.json',
        outdir: config.outdir,
        files: name === 'adminMobile' ? { 'spritemap.svg': env.spritemapName } : {},
        customData: {
            mode: mode === 'prod' ? 'production' : 'development',
        },
    });

    const { append: cssRewriteUrlAppend, replace: cssRewriteUrlReplace } = cssRewriteUrl(
        {
            from: `${env.publicPath}dist/`,
            to: '../',
        },
        env.directoryWork,
    );

    const pagesEntries = (await getPathPages(env.directoryWork)).getEntries();

    for (const outputsItemKey in bundleResult.metafile.outputs) {
        if (!Object.hasOwn(bundleResult.metafile.outputs, outputsItemKey)) {
            continue;
        }
        manifestAppend(outputsItemKey, bundleResult.metafile.outputs[outputsItemKey]);
        cssRewriteUrlAppend(outputsItemKey, bundleResult.metafile.outputs[outputsItemKey]);

        // eslint-disable-next-line no-await-in-loop
        await replaceHtmlChildResource(
            outputsItemKey,
            bundleResult.metafile.outputs[outputsItemKey],
            bundleResult.metafile.outputs,
            pagesEntries,
            config.absWorkingDir,
            env.publicPath,
            (url) => {
                let result = [];
                const projectEntity = projectsPathData.get(projectsNames.store);
                const filePathProjectDefaultBundle = projectEntity.getBundlesFilePath();

                if(!existsSync(filePathProjectDefaultBundle)){
                    return [];
                }

                const data = getJsonFileSync(`file://${filePathProjectDefaultBundle}`);

                const fileName = url.split('/').at(-1);

                if (data.files[fileName]) {
                    result.push({
                        path: data.files[fileName],
                        publicPath: './dist/',
                    });
                } else if (data.entrypoints[fileName]) {
                    const { assets } = data.entrypoints[fileName];
                    for (const key in assets) {
                        if (assets[key]) {
                            result = result.concat(assets[key].map(path => ({
                                path,
                                publicPath: './dist/',
                            })));
                        }
                    }
                }

                return result;
            },
        );
    }
    await cssRewriteUrlReplace();
    await manifestGenerateFn();
};
