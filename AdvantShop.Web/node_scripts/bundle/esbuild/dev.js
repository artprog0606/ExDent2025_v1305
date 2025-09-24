import { getConfig as getBaseConfig } from './base.js';
import { projectsNames } from '../../shopVariables.js';
export const getConfig = (env) => {
    return Object.assign(getBaseConfig(env), {
        sourcemap: env.project === projectsNames.templates ? 'inline' : 'linked',
    });
};
