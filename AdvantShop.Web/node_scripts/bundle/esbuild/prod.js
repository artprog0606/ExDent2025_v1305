import { getConfig as getBaseConfig } from './base.js';

export const getConfig = (env) => {
    return Object.assign(getBaseConfig(env), {
        minify: true,
    });
};
