import switchOnOffDefaultTemplate from './templates/default.html';
import './styles/switch-on-off.scss';
import './themes/blue.scss';

(function (ng) {
    'use strict';

    var idIncrement = 0;

    ng.module('switchOnOff', []).directive('switchOnOff', [
        '$ocLazyLoad',
        'urlHelper',
        function ($ocLazyLoad, urlHelper) {
            return {
                controller: function () {
                    var ctrl = this;

                    ctrl.change = function () {
                        ctrl.onClick({ state: ctrl.checked, name: ctrl.id });
                        ctrl.onChange({ checked: ctrl.checked });
                    };
                },
                bindToController: true,
                templateUrl: switchOnOffDefaultTemplate,
                controllerAs: '$ctrl',
                scope: {
                    checked: '<?',
                    onChange: '&',
                    readonly: '<?',
                    id: '@',
                    onClick: '&',
                    theme: '<?',
                    label: '@',
                    disabled: '<?',
                },
                link: function (scope, element, attrs, ctrl) {
                    ctrl.disabled = ctrl.disabled || false;

                    if (ctrl.id == null || ctrl.id.length === 0) {
                        ctrl.id = 'switchOnOff_' + idIncrement;
                        idIncrement = idIncrement + 1;
                    } else {
                        element.attr('data-id', ctrl.id);
                        element.removeAttr('id');
                    }

                    ctrl.labelFor = ctrl.readonly !== true ? `checkbox-${ctrl.id}` : '';
                    ctrl.theme = attrs.theme != null ? attrs.theme : 'default';
                },
            };
        },
    ]);
})(window.angular);
