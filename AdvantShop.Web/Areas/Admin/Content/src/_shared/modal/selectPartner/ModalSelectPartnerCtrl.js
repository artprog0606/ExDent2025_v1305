(function (ng) {
    'use strict';

    var ModalSelectPartnerCtrl = function ($uibModalInstance, $http, uiGridConstants, uiGridCustomConfig, $translate) {
        var ctrl = this;

        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            multiSelect: false,
            modifierKeysToMultiSelect: false,
            enableRowSelection: true,
            enableRowHeaderSelection: false,
            columnDefs: [
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><div>' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.selectPartner(row.entity.Id)">' +
                        $translate.instant('Admin.Js.SelectCustomer.Select') +
                        '</a>' +
                        '</div></div>',
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Partners.FullName'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Partners.FullName'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'Email',
                    displayName: 'Email',
                    filter: {
                        placeholder: 'Email',
                        type: uiGridConstants.filter.INPUT,
                        name: 'Email',
                    },
                },
            ],
        });

        ctrl.selectPartner = function (partnerId) {
            $uibModalInstance.close({ partnerId: partnerId });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };
    };

    ModalSelectPartnerCtrl.$inject = ['$uibModalInstance', '$http', 'uiGridConstants', 'uiGridCustomConfig', '$translate'];

    ng.module('uiModal').controller('ModalSelectPartnerCtrl', ModalSelectPartnerCtrl);
})(window.angular);
