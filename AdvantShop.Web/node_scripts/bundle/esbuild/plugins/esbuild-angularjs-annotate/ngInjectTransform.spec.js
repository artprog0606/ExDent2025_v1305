import { ngInjectTransform } from './ngInjectTransform.js';

const minify = (content) => content.replaceAll(/\s/gu, '');

describe('ngInjectTransform', () => {
    test('should return correct script when @ngInject with method controller in object', () => {
        const script = `const colorsViewerItemBeforeComponent = () => {
            return {
                controllerAs: 'colorsViewerItemBefore',
                /* @ngInject */
                controller ($scope) {
                    console.log('test')
                }
            };
        };`;

        const expected = `const colorsViewerItemBeforeComponent = () => {
                              return {
                                controllerAs: 'colorsViewerItemBefore',
                                controller: ["$scope", function ($scope) {console.log('test');}]
                              };
                            };`;

        const result = ngInjectTransform(script);

        expect(minify(result)).toEqual(minify(expected));
    });

    test('should return correct script when @ngInject with object', () => {
        const script = `const colorsViewerItemBeforeComponent = () => {
            return {
                controllerAs: 'colorsViewerItemBefore',
                /* @ngInject */
                controller: function ($scope) {},
            };
        };`;

        const expected = `const colorsViewerItemBeforeComponent = () => {
                              return {
                                controllerAs: 'colorsViewerItemBefore',
                                controller: ["$scope", function ($scope) {}]
                              };
                            };`;

        const result = ngInjectTransform(script);

        expect(minify(result)).toEqual(minify(expected));
    });

    test('should return correct script when without @ngInject for object', () => {
        const script = `/*@ngInject*/
                            function maskDirective($filter, $parse, $timeout, $q, maskControlService) {
                                return {
                                    controller: [
                                        '$scope',
                                        '$element',
                                        '$attrs',
                                        function (scope, element, attrs) {
                                            const ctrl = this;
                                            ctrl.$onInit = function () {
                                                const presetList = {
                                                    phone: {
                                                        //other comment 1
                                                        //other comment 2
                                                        //other comment 3
                                                        dispatch: function (appended, dynamicMasked) {}
                                                    }
                                                };
                                            };
                                        },
                                    ],
                                };
                            }`;

        const expected = `function maskDirective($filter, $parse, $timeout, $q, maskControlService) {
                                return {
                                    controller: [
                                        '$scope',
                                        '$element',
                                        '$attrs',
                                        function (scope, element, attrs) {
                                            const ctrl = this;
                                            ctrl.$onInit = function () {
                                                const presetList = {
                                                    phone: {
                                                        //other comment 1
                                                        //other comment 2
                                                        //other comment 3
                                                        dispatch: function (appended, dynamicMasked) {}
                                                    }
                                                };
                                            };
                                        }
                                    ]
                                };
                            }
                            maskDirective.$inject = ["$filter", "$parse", "$timeout", "$q", "maskControlService"];`;

        const result = ngInjectTransform(script);

        expect(minify(result)).toEqual(minify(expected));
    });

    test('should return correct script when @ngInject use export variable is function', () => {
        const script = ` /* @ngInject */
                                 export const scrollToBlockService = function ($window, scrollToBlockConfig) {};`;

        const expected = `export const scrollToBlockService = function ($window, scrollToBlockConfig) {};
                                 scrollToBlockService.$inject = ["$window", "scrollToBlockConfig"];`;

        const result = ngInjectTransform(script);

        expect(minify(result)).toEqual(minify(expected));
    });
    test('should return correct script when @ngInject use constructor in TS', () => {
        const script = `export class CompareService implements ICompareService {
                                    /* @ngInject */
                                    constructor(private readonly $http: IHttpService) {}
                                }`;

        const expected = `export class CompareService implements ICompareService {
                                    constructor(private readonly $http: IHttpService) {}
                                }CompareService.$inject=["$http"];`;

        const result = ngInjectTransform(script, true);

        expect(minify(result)).toEqual(minify(expected));
    });
    test('should return correct script when @ngInject use function in TS', () => {
        const script = `/* @ngInject */
                export function GeoModeChangeAddressTriggerDirective(
                                    $parse: IParseService,
                                    geoModeService: IGeoModeService
                                ): IDirective<IScope, JQLite, IAttributes, IGeoModeCtrl> {
                                    return {
                                    };
                                }`;

        const expected = `export function GeoModeChangeAddressTriggerDirective(
                                    $parse: IParseService,
                                    geoModeService: IGeoModeService
                                ): IDirective<IScope, JQLite, IAttributes, IGeoModeCtrl> {
                                    return {
                                    };
                                }GeoModeChangeAddressTriggerDirective.$inject=["$parse","geoModeService"];`;

        const result = ngInjectTransform(script, true);

        expect(minify(result)).toEqual(minify(expected));
    });
});
