import path from 'node:path';
import GlobalsPlugin from 'esbuild-plugin-globals';
import annotatePlugin from './plugins/esbuild-angularjs-annotate/index.js';
import inlineImportPlugin from 'esbuild-plugin-inline-import';
import stylesPlugin from './plugins/esbuild-styles-plugin/index.js';
import cleanupPlugin from './plugins/esbuild-cleanup-plugin/index.js';
import { postcssConfigFn } from '../../../postcss.config.js';
import browserslistToEsbuild from 'browserslist-to-esbuild';
import { projectsNames } from '../../shopVariables.js';

export const getConfig = (env) => {
    const { entryPoints, directoryWork, publicPath, project } = env;

    const loader = {
        '.html': 'file',
        '.svg': 'file',
        '.png': 'file',
        '.jpg': 'file',
        '.gif': 'file',
        '.cur': 'file',
    };

    const plugins = [
        GlobalsPlugin({
            jquery: 'jQuery',
            $: 'jQuery',
        }),
        annotatePlugin(),
        inlineImportPlugin({
            filter: /\?raw$/,
        }),
        stylesPlugin({
            postcssPlugins: postcssConfigFn({ options: { variables: env } }).plugins,
        }),
        cleanupPlugin(),
    ];

    const external = ['*.woff2', '*.woff', '*.ttf', '*.eot', '*.eot#iefix', '*.eot?#iefix'];
    return {
        bundle: true,
        metafile: true,
        ignoreAnnotations: true,
        platform: 'browser',
        target: browserslistToEsbuild(),
        format: 'iife',
        inject: [path.resolve(process.cwd(), 'node_scripts/bundle/esbuild/inject/tinycolor-inject.js')],
        entryPoints: entryPoints,
        absWorkingDir: directoryWork,
        outdir: path.join(directoryWork, `dist`, path.sep),
        assetNames: 'assets/[name].[hash]',
        chunkNames: 'chunks/[name].[hash]',
        entryNames: 'entries/[name].[hash]',
        loader,
        plugins,
        external,
        minifyIdentifiers: false,
        publicPath: env.project === projectsNames.templates ? publicPath + 'dist' : undefined,
    };
};
