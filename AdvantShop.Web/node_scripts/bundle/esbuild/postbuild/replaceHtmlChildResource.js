import path from 'node:path';
import { readFile, writeFile } from 'node:fs/promises';

const attrs = ['oc-lazy-load', 'ng-include', 'template-url', 'template-path'];
const regexp = new RegExp(`(?<attr>${attrs.join('|')})="(?<src>.+?)"`, 'gmu');

const findResourceSimple = (outputData, src) => {
    let inputs;
    let result = [];
    for (const key in outputData) {
        if (key.endsWith('.html') === false) {
            continue;
        }
        inputs = outputData[key]?.inputs;
        if (!inputs) {
            continue;
        }

        for (const inputKey in inputs) {
            if (inputKey.includes(src)) {
                result = [key];
                break;
            }
        }
    }

    if (result.length > 1) {
        throw new Error(`replaceHtmlChildResource: Multiple files wrong for "${src}"`);
    }

    return result;
};

const findResourceOcLazyLoad = (outputData, src) => {
    const result = [];
    for (const key in outputData) {
        if (outputData[key]?.entryPoint && path.normalize(outputData[key].entryPoint).includes(src)) {
            result.push(key);

            if (outputData[key].cssBundle) {
                result.push(outputData[key].cssBundle);
            }
        }
    }
    return result;
};

const findResource = (outputData, src, isEntryPoint, altSearch) => {
    let result = isEntryPoint ? findResourceOcLazyLoad(outputData, src) : findResourceSimple(outputData, src);

    if (result.length === 0 && altSearch) {
        result = altSearch(src);
    }

    if (result.length === 0) {
        process.stdout.write(`replaceHtmlChildResource: Not found dist file for  ${isEntryPoint ? 'oc-lazy-load with' : ''} "${src}"\r\n`);
    }

    return result;
};

function stringifyOptions(assets) {
    if (typeof assets === 'string') {
        return assets;
    }

    return `[${  assets.map((x) => `'${x}'`).join(',')  }]`;
}

function normalizeUrl(url, absWorkingDir) {
    return url
        .replace(absWorkingDir, '')
        .replace(process.cwd(), '')
        .replace(/^..(?<temp1>[\/\\])areas(?<temp2>[\/\\])\w+(?<temp3>[\/\\])content/iu, 'Content')
        .replace(/^.?.?(?<temp1>[\/\\])?areas(?<temp2>[\/\\])\w+(?<temp3>[\/\\])/iu, '')
        .replace(/^(?<areas>[\/\\])/u, '')
        .replace(/^\.\//u, '');
}

export const replaceHtmlChildResource = async (
    outputDataItemKey,
    _outputDataItemValue,
    outputData,
    pagesEntries,
    absWorkingDir,
    publicPath,
    altSearch,
) => {
    if (outputDataItemKey.endsWith('.html') === false) {
        return;
    }
    const absImport = path.resolve(absWorkingDir, outputDataItemKey);
    const content = (await readFile(absImport)).toString();
    const contentNew = content.replaceAll(regexp, (...params) => {
        const groups = params.at(-1);
        let assets = [];
        if (groups.attr === 'oc-lazy-load') {
            // eslint-disable-next-line no-new-func
            const ocLazyLoadValue = new Function(`return ${  groups.src}`)();
            for (const item of ocLazyLoadValue) {
                if (typeof item === 'string') {
                    const url = pagesEntries[item] ?  normalizeUrl(pagesEntries[item], absWorkingDir) : item;
                    const resource = findResource(outputData, url, true, altSearch);
                    assets = assets.concat(resource);
                } else if (item.files) {
                    for (const itemInAttrFiles of item.files) {
                        // eslint-disable-next-line max-depth
                        if (typeof itemInAttrFiles === 'string' && pagesEntries[itemInAttrFiles]) {
                            const url = normalizeUrl(pagesEntries[itemInAttrFiles], absWorkingDir);
                            const resource = findResource(outputData, url, true, altSearch);
                            assets = assets.concat(resource);
                        }
                    }
                }
            }
        } else if (groups.src.endsWith('.html')) {
            const url = normalizeUrl(groups.src, absWorkingDir);
            // eslint-disable-next-line prefer-destructuring
            assets = findResource(outputData, url, false, altSearch)[0];
        }

        return `${groups.attr}="${
            assets.length > 0 ? stringifyOptions(Array.isArray(assets) ? assets.map((x) => typeof x === 'string' ? publicPath + x: `${x.publicPath}${x.path}`) : publicPath + assets) : groups.src
        }"`;
    });

    if (contentNew !== content) {
        await writeFile(absImport, contentNew);
    }
};
