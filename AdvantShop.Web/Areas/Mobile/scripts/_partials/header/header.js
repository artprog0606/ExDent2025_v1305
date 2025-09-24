(function (ng) {
    'use strict';

    ng.injector(['dependency']).get('dependencyService').add('header');

    ng.module('header', []);
})(window.angular);
