import postcss from 'postcss';
import { EOL } from 'os';

const replaceEOL = (value, eol) => {
    return value.replace(/\r?\n/gu, eol);
};
const getCurrentValue = (raws, current, eol) => {
    if (typeof raws[current] === 'boolean') {
        return {
            [current]: raws[current],
        };
    }

    if (typeof raws[current] === 'object') {
        return {
            [current]: replaceEOL(raws[current].raw, eol),
        };
    }

    return {
        [current]: replaceEOL(raws[current], eol),
    };
};

const replaceToEOL = (raws = {}, eol) =>
    Object.keys(raws).reduce((previous, current) => Object.assign(previous, getCurrentValue(raws, current, eol)), {});

const plugin = (eol = EOL, append = true) => {
    return {
        postcssPlugin: 'postcss-eol',
        Once(root, { result }) {
            root.raws = replaceToEOL(root.raws, eol);
            if (append && !new RegExp(eol + '', 'u').test(root.raws.after)) {
                root.raws.after = eol;
            }
            root.walk((el) => {
                el.raws = replaceToEOL(el.raws, eol);
                if (el?.selector != null) {
                    el.selector = replaceEOL(el.selector, eol);
                }
            });
        },
    };
};

plugin.postcss = true;

export default plugin;
