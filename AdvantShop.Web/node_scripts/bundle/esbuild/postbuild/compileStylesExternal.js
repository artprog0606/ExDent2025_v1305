import { projectsNames } from '../../../shopVariables.js';
import path from 'node:path';
import { existsSync } from 'node:fs';
import { compileFontsStyles } from '../../../fontsStyles.js';
import { compileDesignStyles } from '../../../designStyles.js';

export const compileStylesExternal = (env, watch, cdnDesign) => {
    const { project } = env;
    if (
        (project === projectsNames.store || project === projectsNames.templates) &&
        env.directoryWork.includes(path.join('Areas', 'Mobile')) === false
    ) {
        const fontsDirectory = path.join(env.directoryWork, `fonts`);
        const fontsFilename = path.join(fontsDirectory, `fonts${project === projectsNames.store ? '.store' : ''}.css`);

        if (existsSync(fontsFilename)) {
            compileFontsStyles(
                fontsFilename,
                {
                    cdnDesign,
                    ...env,
                },
                { watch },
            );
        }

        if (project === projectsNames.store) {
            compileFontsStyles(path.join(fontsDirectory, `fonts.icons.css`), { ...env }, { watch });
        }

        compileDesignStyles(
            path.join(env.directoryWork, `design`),
            {
                cdnDesign,
                ...env,
            },
            { watch },
        );
    } else if (project === projectsNames.store) {
        const fontsDirectory = path.join(env.directoryWork, '..', '..', 'fonts');
        compileFontsStyles(path.join(fontsDirectory, `fonts.mobile.css`), { ...env }, { watch });
    }
};
