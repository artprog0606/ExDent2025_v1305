(function (ng) {
    'use strict';

    /**
     * @ngdoc overview
     * @name autocompleter.module:autocompleterModule
     * @function
     *
     * @description
     * Test module
     */

    angular.module('autocompleter', []).constant('autocompleterConfig', {
        minLength: 3,
        requestUrl: undefined,
        field: undefined,
        templatePath: undefined,
        linkAll: undefined,
        maxHeightList: undefined,
        minHeightList: undefined,
    });
})(window.angular);
