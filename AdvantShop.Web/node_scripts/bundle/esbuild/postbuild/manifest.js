import { writeFile } from 'node:fs/promises';
import path from 'node:path';

const regexp = /.*dist\//;
export const manifestGenerate = (options) => {
    const bundles = Object.assign(
        {
            files: options?.files ?? {},
            entrypoints: {},
        },
        options?.customData,
    );

    const append = (outputDataItemKey, outputDataItemValue) => {
        if (outputDataItemValue.entryPoint != null) {
            let tempName = getFileNameOriginalWithoutExt(outputDataItemKey);
            if (bundles.entrypoints[tempName] == null) {
                bundles.entrypoints[tempName] = {
                    assets: {
                        js: [removeDistPath(outputDataItemKey)],
                        css: outputDataItemValue.cssBundle ? [removeDistPath(outputDataItemValue.cssBundle)] : null,
                    },
                };
            }
        } else {
            let inputs = Object.keys(outputDataItemValue.inputs);
            if (inputs.length === 0 || inputs.length > 1) {
                return;
            }
            bundles.files[getFileName(inputs[0])] = removeDistPath(outputDataItemKey);
        }
    };

    const generate = async () => {
        await writeFile(path.resolve(options.outdir, options.filename), JSON.stringify(bundles, null, 2));
    };

    return {
        append,
        generate,
    };
};

const getFileName = (filePath) => filePath.split('/').at(-1);
//const getFileNameWithoutExt = (filePath) => getFileName(filePath).replace(/\.\w+$/, '');
const removeDistPath = (filepath) => filepath.replace(regexp, '');

const getFileNameOriginalWithoutExt = (filePath) => getFileName(filePath).replace(/\.\w+\.\w+$/, '');
