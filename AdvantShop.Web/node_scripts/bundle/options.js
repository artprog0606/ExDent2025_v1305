import process from 'node:process';
import yargs from 'yargs';
import { projectsPathData, getDirname } from '../shopPath.js';
import { projectsNames } from '../shopVariables.js';
import inquirer from 'inquirer';
import inquirerFuzzyPathPrompt from 'inquirer-fuzzy-path';
import path from 'path';

const __dirname = getDirname(import.meta.url);

export const getOptions = async () => {
    const argv = yargs(process.argv)
        .scriptName('bundle')
        .usage('$0 <cmd> [args]')
        .option('mode', {
            alias: 'm',
            description: 'Bundle mode: development or production',
            type: 'string',
            choices: ['dev', 'prod'],
            default: 'prod',
        })
        .option('watch', {
            alias: 'w',
            description: 'Enable watch mode',
            type: 'boolean',
            default: false,
        })
        .option('templates', {
            alias: 'l',
            description: 'Templates list name for build',
            type: 'array',
        })
        .option('modules', {
            alias: 'c',
            description: 'Modules list name for build',
            type: 'array',
        })
        .option('profile', {
            alias: 'p',
            description: 'Generate json-stats file',
            type: 'boolean',
            default: false,
        })
        .option('cdnDesign', {
            alias: 'd',
            description: 'Rewrite url in css on cdn for design',
            type: 'boolean',
            default: false,
        })
        .option('silent', {
            alias: 's',
            description: 'Silent build without interactive',
            type: 'boolean',
            default: false,
        })
        .option('tsc', {
            description: 'Run typescript compiler',
            type: 'boolean',
            default: true,
        })
        .help()
        .alias('help', 'h').argv;

    const templatesPathData = projectsPathData.get(projectsNames.templates);
    const modulesPathData = projectsPathData.get(projectsNames.modules);
    let answers = {};

    if (argv.silent === false) {
        inquirer.registerPrompt('fuzzypath', inquirerFuzzyPathPrompt);
        answers = await inquirer.prompt([
            {
                type: 'list',
                name: 'project',
                message: 'Что будем собирать?',
                default: 0,
                pageSize: 100,
                waitUserInput: false,
                choices: [
                    { value: '*', name: 'Всё' },
                    { value: 'allWithoutModulesAndTemplates', name: 'Всё, кроме модулей и шаблонов' },
                    ...Array.from(projectsPathData.values()).map((x) => {
                        return {
                            value: x.getProjectName(),
                            name: x.getDisplayName(),
                        };
                    }),
                ],
            },
            {
                type: 'fuzzypath',
                name: 'templates',
                message: 'Какие шаблоны?',
                default: '*',
                when: ({ project }) => project === projectsNames.templates,
                itemType: 'directory',
                suggestOnly: true,
                depthLimit: 0,
                rootPath: templatesPathData.getPhysicalPath(),
                excludeFilter: (nodePath) => /Templates\\$/.test(nodePath),
            },
            {
                type: 'fuzzypath',
                name: 'modules',
                message: 'Какие модули?',
                default: '*',
                when: ({ project }) => project === projectsNames.modules,
                itemType: 'directory',
                suggestOnly: true,
                depthLimit: 0,
                rootPath: modulesPathData.getPhysicalPath(),
                excludeFilter: (nodePath) => /Modules\\$/.test(nodePath),
            },
        ]);
    }

    let { templates, modules, ...options } = Object.assign({}, argv, answers);

    templates = templates != null ? (Array.isArray(templates) || templates === '*' ? templates : [templates]) : null;
    modules = modules != null ? (Array.isArray(modules) || modules === '*' ? modules : [modules]) : null;

    if (Array.isArray(templates)) {
        templates = templates.map((x) => (/^[\w\d]+$/.test(x) ? path.resolve(__dirname, '..', '..', 'Templates', x) : x));
    }

    if (Array.isArray(modules)) {
        modules = modules.map((x) => (/^[\w\d]+$/.test(x) ? path.resolve(__dirname, '..', '..', 'Modules', x) : x));
    }

    return { templates, modules, ...options };
};
