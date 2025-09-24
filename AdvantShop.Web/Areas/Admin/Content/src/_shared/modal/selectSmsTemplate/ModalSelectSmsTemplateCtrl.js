import './selectSmsTemplate.html';

(function (ng) {
    'use strict';

    var ModalSelectSmsTemplateCtrl = function ($uibModalInstance, uiGridCustomConfig, $translate) {
        var ctrl = this;

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'Id',
                    displayName: 'Id',
                    width: 90,
                },
                {
                    name: 'Text',
                    displayName: $translate.instant('Admin.Js.SelectSmsTemplate.Text'),
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 100,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<a href="" class="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.choose(row.entity)">' +
                        $translate.instant('Admin.Js.Select') +
                        '</a> ' +
                        '</div></div>',
                },
            ],
        });

        if (ctrl.$resolve.multiSelect === false) {
            ng.extend(ctrl.gridOptions, {
                multiSelect: false,
                modifierKeysToMultiSelect: false,
                enableRowSelection: true,
                enableRowHeaderSelection: false,
            });
        }

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;
        };

        ctrl.choose = function (entity) {
            let result = { Id: entity.Id, Text: entity.Text };
            $uibModalInstance.close(result);
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalSelectSmsTemplateCtrl.$inject = ['$uibModalInstance', 'uiGridCustomConfig', '$translate'];

    ng.module('uiModal').controller('ModalSelectSmsTemplateCtrl', ModalSelectSmsTemplateCtrl);
})(window.angular);
