import process from 'node:process';
import yargs from 'yargs';
import fs from 'fs';
import { execSync } from 'child_process';
import { getDirectories } from './shopPath.js';
const argv = yargs(process.argv)
    .scriptName('critical-css')
    .usage('$0 <cmd> [args]')
    .option('baseUrl', {
        alias: 'b',
        description: 'Base URI to site',
        type: 'string',
    })
    .option('templates-list', {
        alias: 'l',
        description: 'Templates list name for build',
        type: 'array',
    })
    .option('templates-all', {
        alias: 'a',
        description: 'Build all templates',
        type: 'boolean',
        default: false,
    })
    .option('screenshots', {
        alias: 's',
        description: 'Enable generate screenshot page',
        type: 'boolean',
        default: false,
    })
    .option('grab-font-face', {
        alias: 'g',
        description: 'Enable grab font-face css text',
        type: 'boolean',
        default: false,
    })
    .help()
    .alias('help', 'h').argv;
const ignoreListTemplates = new Set(['.git']);

let __baseUrl = argv.baseUrl || 'http://localhost:8825/';

const { templatesList, templatesAll, screenshots, grabFontFace } = argv;

if (__baseUrl[__baseUrl.length - 1] != '/') {
    __baseUrl += '/';
}

let templatesNameList;
if (templatesAll) {
    templatesNameList = getDirectories('Templates').reduce((prev, current) => {
        if (ignoreListTemplates.has(current) === false) {
            prev.push(current);
        }
        return prev;
    }, []);
} else if (templatesList) {
    if (templatesList[0] === '*') {
        templatesNameList = getDirectories('Templates').filter((x) => !x.startsWith('.'));
    } else {
        templatesList
            .filter((current) => ignoreListTemplates.has(current) === false)
            .forEach((templateName) => fs.accessSync(`Templates/${templateName}/`));
        templatesNameList = templatesList;
    }
} else {
    templatesNameList = ['_default'];
}

const errors = [];

console.log(`Running generate critical css for ${templatesNameList.length} templates`);

for (let templateName of templatesNameList) {
    try {
        execSync(
            `node node_scripts/criticalcssProcess.js -n ${templateName} -b ${__baseUrl} ${screenshots ? '-s' : ''} ${grabFontFace ? '-g' : ''}`,
            { stdio: ['ignore', 'inherit', 'pipe'] },
        );
    } catch (err) {
        errors.push(`Error in template: ${templateName}\n\r${err.stack}`);
    }
}

if (errors.length > 0) {
    process.exitCode = 1;
    console.error(`errors.length: ${errors.length}`);
    console.error(errors.join('\n\r'));
} else {
    console.log(`Success generate  critical css`);
}
