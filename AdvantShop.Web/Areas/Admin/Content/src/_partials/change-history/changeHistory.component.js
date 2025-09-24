import changeHistoryTemplate from './templates/changeHistory.tpl.html';
(function (ng) {
    'use strict';

    ng.module('changeHistory').component('changeHistory', {
        templateUrl: changeHistoryTemplate,
        controller: 'ChangeHistoryCtrl',
        bindings: {
            objId: '<',
            objType: '<',
            type: '@',
            hide: '=',
        },
    });
})(window.angular);
