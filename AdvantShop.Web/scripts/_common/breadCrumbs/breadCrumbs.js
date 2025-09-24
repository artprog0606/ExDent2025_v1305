(function (ng) {
    'use strict';

    var dependencyService = ng.injector(['dependency']).get('dependencyService');
    dependencyService.add(['breadCrumbs']);

    ng.module('breadCrumbs', []);
})(window.angular);
