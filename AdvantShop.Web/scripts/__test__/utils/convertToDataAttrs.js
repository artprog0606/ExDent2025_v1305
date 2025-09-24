import { kebabCase } from './cases.js';

export const convertToDataAttrs = (obj) => {
    return Object.keys(obj).map((key) => {
        return 'data-' + kebabCase(key) + '=' + `"${typeof obj[key] === 'string' ? "'" + obj[key] + "'" : obj[key]}"`;
    });
};
