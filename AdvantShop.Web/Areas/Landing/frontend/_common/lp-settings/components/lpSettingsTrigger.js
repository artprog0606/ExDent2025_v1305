(function (ng) {
    'use strict';

    ng.module('lpSettings').directive('lpSettingsTrigger', function () {
        return {
            controller: 'LpSettingsTriggerCtrl',
            controllerAs: 'lpSettingsTrigger',
            bindToController: true,
            scope: true,
            link: function (scope, element, attrs, ctrl) {
                ctrl.lpId = attrs.lpId;
            },
        };
    });
})(window.angular);
