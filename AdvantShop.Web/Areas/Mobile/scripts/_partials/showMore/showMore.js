(function (ng) {
    'use strict';

    var dependencyService = ng.injector(['dependency']).get('dependencyService');
    dependencyService.add(['showMore']);
    ng.module('showMore', []);
})(window.angular);
