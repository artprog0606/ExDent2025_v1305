import * as sass from 'sass';
import postcss from 'postcss';
import { readFile } from 'node:fs/promises';

export default (options) => {
    return {
        name: 'esbuild-styles-plugin',
        setup: async (build) => {
            build.onLoad({ filter: /.s?css$/ }, async (args) => {
                try {
                    const content = args.path.includes('.scss')
                        ? sass.compile(args.path, {
                              loadPaths: ['node_modules'],
                          }).css
                        : (await readFile(args.path)).toString();

                    const postcssResult = await postcss(options.postcssPlugins).process(content, {
                        from: args.path.replace('.scss', '.css'),
                    });
                    return {
                        contents: postcssResult.css,
                        loader: 'css',
                    };
                } catch (e) {
                    //console.error(e)
                    return { errors: [convertMessage(e)] };
                }
            });
        },
    };
};

let convertMessage = (error) => {
    const { message, start, end } = error;
    let location = undefined;
    // if (start && end) {
    //     let lineText = source.split(/\r\n|\r|\n/g)[start.line - 1]
    //     let lineEnd = start.line === end.line ? end.column : lineText.length
    //     location = {
    //         file: filename,
    //         line: start.line,
    //         column: start.column,
    //         length: lineEnd - start.column,
    //         lineText,
    //     }
    // }
    return { text: message, location, detail: error };
};
