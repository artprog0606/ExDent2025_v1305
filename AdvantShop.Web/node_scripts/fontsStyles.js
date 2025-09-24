import { CompileSync } from './stylesCompile.js';

/**
 * Генерирует стили для подключения шрифтов
 * @param {string} filePath Полный путь файла
 * @param {object} variables Объект с переменными, которые передаются в PostCSS
 * @param {object} options Настройки компиляции
 */
export function compileFontsStyles(filePath, variables, options = { watch: false }) {
    const compileSync = new CompileSync({ ...variables, cdnFonts: false }, { ...options, cssFilename: `[name].local` });
    const compileSyncCDN = new CompileSync({ ...variables, cdnFonts: true }, { ...options, cssFilename: `[name].cdn` });

    compileSync.render(filePath);
    compileSyncCDN.render(filePath);

    if (options != null && options.watch === true) {
        compileSync.watch(filePath);
        compileSyncCDN.watch(filePath);
    }
}
