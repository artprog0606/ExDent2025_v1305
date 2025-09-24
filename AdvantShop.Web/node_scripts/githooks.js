import { existsSync } from 'node:fs';
import { execSync } from 'node:child_process';
import path from 'node:path';

const dirGitHooks = path.join('..', '.githooks', 'install.mjs');
export const setGitHooks = () => {
    if (existsSync(dirGitHooks)) {
        console.log('Githooks folder found');
        try {
            const result = execSync(`node ${dirGitHooks}`, {
                stdio: ['inherit', 'inherit', 'inherit'],
            });
        } catch (e) {
            console.log('Githooks not install!');
            console.log(e.message != null ? e.message : typeof e === 'string' ? e : 'Error unknown format');
        }
    } else {
        console.log('Githooks folder not found');
    }
};
