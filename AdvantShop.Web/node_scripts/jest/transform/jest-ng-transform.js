import { ngInjectTransform } from '../../bundle/esbuild/plugins/esbuild-angularjs-annotate/ngInjectTransform.js';

export default {
    process(content, filename, { transformerConfig }) {
        return { code: content.includes('@ngInject') ? ngInjectTransform(content, filename.endsWith('.ts')) : content };
    },
};
