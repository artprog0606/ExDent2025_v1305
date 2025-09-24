import inplaceModuleName from '../../../scripts/_partials/inplace/inplace.max.module.js';

angular.module(inplaceModuleName).config(
    /* @ngInject */ function (inplaceConfig) {
        inplaceConfig.basePath = '..';
    },
);
