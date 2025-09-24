import process from 'node:process';
import yargs from 'yargs';
import child_process from 'child_process';
import path from 'path';
import fs from 'fs';
import { getDirname } from './shopPath.js';
const __dirname = getDirname(import.meta.url);

const argv = yargs(process.argv)
    .scriptName('critical-css')
    .usage('$0 <cmd> [args]')
    .option('name', {
        alias: 'n',
        description: 'Template Name',
        type: 'string',
        required: true,
    })
    .help()
    .alias('help', 'h').argv;

const sourcePath = path.resolve(__dirname, path.join(`..`, `..`, `templates`, `origin`));
const resultPath = path.resolve(__dirname, path.join(`..`, `templates`, argv.name));

fs.cpSync(sourcePath, resultPath, {
    force: false,
    errorOnExist: true,
    recursive: true,
});

console.log(`Template "${argv.name}" is created`);
