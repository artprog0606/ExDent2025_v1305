import path from 'path';
import { getDirectories } from './shopPath.js';
import { CompileSync } from './stylesCompile.js';
const FILE_NAME = `styles`;

export function compileDesignStyles(filePath, variables, options = { watch: false }) {
    const backgroundsRoot = path.join(filePath, `backgrounds`);
    const backgrounds = getDirectories(backgroundsRoot).map((x) => path.join(backgroundsRoot, x));

    const colorsRoot = path.join(filePath, `colors`);
    const colors = getDirectories(colorsRoot).map((x) => path.join(colorsRoot, x));

    const themesRoot = path.join(filePath, `themes`);
    const themes = getDirectories(themesRoot).map((x) => path.join(themesRoot, x));

    const compileSync = new CompileSync({ ...variables, cdnDesign: false }, { ...options, cssFilename: `[name].local`, ignore: [/local|cdn\.css$/] });
    const compileSyncCDN = new CompileSync({ ...variables, cdnDesign: true }, { ...options, cssFilename: `[name].cdn`, ignore: [/local|cdn\.css$/] });
    const compileSyncOnlyColors = new CompileSync(variables, { ...options, ignore: [/styles\.css$/] });

    doType(backgrounds, (pathStyle) => {
        compileSync.render(pathStyle, FILE_NAME);
        compileSyncCDN.render(pathStyle, FILE_NAME);
    });
    doType(colors, (pathStyle) => {
        compileSyncOnlyColors.render(pathStyle, FILE_NAME);
    });
    doType(themes, (pathStyle) => {
        compileSync.render(pathStyle, FILE_NAME);
        compileSyncCDN.render(pathStyle, FILE_NAME);
    });

    if (options != null && options.watch === true) {
        compileSync.watch([themesRoot, backgroundsRoot]);
        compileSyncCDN.watch([themesRoot, backgroundsRoot]);
        compileSyncOnlyColors.watch(colorsRoot);
    }
}

function doType(listNames, compilator) {
    listNames.forEach((designItemPath) => {
        compilator(path.join(designItemPath, `styles`));
    });
}
