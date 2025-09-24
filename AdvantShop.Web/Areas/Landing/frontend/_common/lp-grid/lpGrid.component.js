import lpGridTemplate from './lp-grid.html';
import lpGridColumnsTemplate from './lp-grid-columns.html';
(function (ng) {
    'use strict';

    ng.module('lpGrid').component('lpGrid', {
        templateUrl: lpGridTemplate,
        controller: 'LpGridCtrl',
        transclude: {
            add: '?lpGridAdd',
        },
        require: {
            parentForm: '^?form',
            lpGridParent: '^?lpGrid',
        },
        bindings: {
            source: '=',
            lpId: '<?',
            blockId: '<?',
            onDelete: '&',
            onOrderChanged: '&',
            onChange: '&',
            showHeader: '<?',
            editMode: '@',
            //values: runtime or demand
            selection: '<?',
            //values: true or false
            selectionStartIndex: '<?',
            //индекс выбранной по-умолчанию строки
            addButtonText: '<?',
            ngRequired: '<?',
            onBeforeAddNewElement: '&',
        },
    });
    ng.module('lpGrid').component('lpGridColumn', {
        require: {
            lpGrid: '^lpGrid',
        },
        transclude: true,
        controller: 'LpGridColumnCtrl',
        bindings: {
            classes: '<?',
            field: '@',
            type: '@',
            title: '@',
            placeholder: '<?',
            previewField: '@',
            previewWidth: '<',
            previewHeight: '<',
            selectData: '<?',
            selectLabel: '@',
            selectValue: '@',
            editable: '<?',
            settingsPicture: '<?',
            noPhoto: '<?',
            validRequired: '<?',
            onChange: '&',
            onClick: '&',
            visible: '<?',
            main: '@',
        },
    });
    ng.module('lpGrid').directive('lpAddGroupGrid', function () {
        return {
            require: {
                lpGrid: '^lpGrid',
            },
            controller: 'LpAddGroupGridCtrl',
            scope: true,
            bindToController: true,
            link: function (scope, element, attr, ctrls) {
                if (element[0].tagName !== 'SCRIPT') {
                    console.error('need use tag script with lpAddGroupGrid directive');
                }
            },
        };
    });
    ng.module('lpGrid').component('lpGridGroup', {
        controller: 'LpGridGroupCtrl',
        bindings: {
            row: '<',
            template: '<',
        },
    });
    ng.module('lpGrid').component('lpGridColumnTemplate', {
        controller: 'LpGridColumnTemplateCtrl',
        //require: {
        //    lpGrid: '^lpGrid'
        //},
        bindings: {
            col: '<',
            row: '<',
            rowIndex: '<',
        },
    });
    ng.module('lpGrid').component('lpGridCkeditorModal', {
        controller: 'LpGridCkeditorModalCtrl',
        bindings: {
            header: '<?',
            value: '<',
            onApply: '&',
        },
    });
    ng.module('lpGrid').component('lpGridColumns', {
        controller: 'LpGridColumnsCtrl',
        templateUrl: lpGridColumnsTemplate,
        bindings: {
            row: '<',
            parentCtrl: '<',
            rowIndex: '<',
            col: '<',
        },
    });
    ng.module('lpGrid').component('lpGridAdd', {});
    ng.module('lpGrid').directive('lpGridModel', function () {
        return {
            controller: 'LpGridModel',
            controllerAs: 'lpGridModel',
            bindings: true,
            scope: true,
        };
    });
})(window.angular);
