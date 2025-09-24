import fg from 'fast-glob';
import path from 'path';
import { unlink } from 'fs/promises';

export default (pattern = '**/*') => {
    return {
        name: 'esbuild-cleanup-plugin',
        setup(build) {
            const options = build.initialOptions;
            if (!options.outdir) {
                console.log('[esbuild cleanup] Not outdir configured - skipping the cleanup');
                return;
            }
            if (!options.metafile) {
                console.log('[esbuild cleanup] Metafile is not enabled - skipping the cleanup');
                return;
            }

            build.onEnd(async (result) => {
                if (result.errors?.length > 0) {
                    return;
                }

                const files = await fg.async(fg.convertPathToPattern(path.join(options.outdir, pattern)), {
                    cwd: options.outdir,
                    ignore: ['**/bundles.json'].concat(Object.keys(result.metafile.outputs).map((x) => '**/' + x)),
                });

                for (let filesItem of files) {
                    await unlink(filesItem);
                }
            });
        },
    };
};
