import { readFile, writeFile } from 'node:fs/promises';
import path from 'node:path';

export const cssRewriteUrl = ({ from, to }, root) => {
    const storage = new Set();

    const append = (fileName, output) => {
        if (fileName.endsWith('.css')) {
            storage.add(fileName);
        }
    };

    const replace = async () => {
        let cssContent, filePathAbs;
        for (const cssFileName of storage) {
            filePathAbs = path.join(root, cssFileName);
            cssContent = (await readFile(filePathAbs)).toString();
            if (cssContent.includes('url(')) {
                await writeFile(filePathAbs, cssContent.replaceAll(from, to));
            }
        }
        storage.clear();
    };

    return {
        append,
        replace,
    };
};
