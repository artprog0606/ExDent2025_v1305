import { existsSync, rmdir } from 'node:fs';
import { resolve } from 'node:path';
import { getDirectories, removeAllFiles } from './shopPath.js';

const removeList = new Set([
    'dist',
    'Areas/Mobile/dist',
    'Areas/Admin/dist',
    'Areas/Admin/Templates/AdminV3/dist',
    'Areas/Admin/Templates/Mobile/dist',
    'Areas/Landing/dist',
    'Areas/Partners/dist',
]);

const itemsRemoved = new Set();
const errorsList = [];
console.log('Clean:');
for (const item of removeList) {
    if (existsSync(item)) {
        try {
            await removeAllFiles(item);
            console.log(item);
        } catch (e) {
            let message;

            if (typeof e === 'string') {
                message = e;
            } else if (e.message != null) {
                message = e.message;
            } else {
                throw new Error('Unhandled error', e);
            }

            errorsList.push(message);
        }
    }
}
