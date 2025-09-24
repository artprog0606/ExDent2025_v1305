import { readFile, writeFile } from 'node:fs/promises';
import path from 'path';
import SVGSpriter from 'svg-sprite';
export default (options, filename) => {
    return {
        name: 'esbuild-svg-sprite-plugin',
        setup: (build) => {
            const spriter = new SVGSpriter(options);

            build.onLoad({ filter: /.svg$/ }, async (args) => {
                const svgContent = (await readFile(args.path)).toString();
                spriter.add(args.path, null, svgContent);

                return {
                    contents: svgContent,
                    loader: 'file',
                };
            });

            build.onEnd(async (args) => {
                const { result } = await spriter.compileAsync();
                for (const mode in result) {
                    for (const resource in result[mode]) {
                        await writeFile(path.resolve(options.dest, filename), result[mode][resource].contents);
                    }
                }
            });
        },
    };
};
