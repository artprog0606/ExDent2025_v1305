import iconMoveTemplate from './templates/iconMove.html';
(function (ng) {
    'use strict';

    ng.module('iconMove', []).component('iconMove', {
        templateUrl: iconMoveTemplate,
    });
})(window.angular);
