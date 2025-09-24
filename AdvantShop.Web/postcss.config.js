import cdnURLs from './node_scripts/cdnURLs.js';
import { projectsNames } from './node_scripts/shopVariables.js';
import eol from './node_scripts/postcss-eol.js';
import urlrewrite from 'postcss-urlrewrite';
import autoprefixer from 'autoprefixer';
import postcssPresetEnv from 'postcss-preset-env';
const regexpDesign = /design[\\]([\w\d\\-]+)[\\]styles[\\]/;

function getFontBasePath(cdn, relativePath) {
    let result;

    if (cdn) {
        result = cdnURLs.fonts;
    } else {
        if (relativePath != null) {
            result = `../${relativePath}fonts/`;
        }
    }

    return result;
}

export const postcssConfigFn = (ctx) => {
    const variables = ctx?.options?.variables;
    let urlRewriteRules = [];

    if (variables != null) {
        const { directoryWork, projectPath, cdnDesign, cdnFonts, project } = variables;

        let fontsPath = getFontBasePath(cdnFonts, projectPath.getRelativePathToRoot());

        if (fontsPath != null) {
            urlRewriteRules.push({ from: /^\/fonts\//, to: fontsPath });
        }

        if (ctx.file?.includes('\\design\\') && cdnDesign) {
            const regexpResult = regexpDesign.exec(ctx.file);
            const folderName = project === projectsNames.templates ? projectPath.getName() : '_default';
            urlRewriteRules.push({
                from: /\.\.\//,
                to: cdnURLs.templates + folderName + '/design/' + regexpResult[1] + '/',
            });
        }
    }

    return {
        plugins: [
            postcssPresetEnv(),
            urlrewrite({
                properties: ['src', 'background', 'background-image'],
                rules: urlRewriteRules,
            }),
            autoprefixer(),
            eol(undefined, false),
        ],
    };
};
