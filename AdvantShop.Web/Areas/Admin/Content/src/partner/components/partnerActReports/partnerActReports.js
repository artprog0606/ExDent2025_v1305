import partnerActReportsTemplate from './partnerActReports.html';
(function (ng) {
    'use strict';

    var PartnerActReportsCtrl = function ($http, uiGridCustomConfig, $translate, toaster) {
        var ctrl = this;
        ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
            columnDefs: [
                {
                    name: 'FileName',
                    displayName: 'Файл',
                    enableSorting: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"> ' +
                        '<div class="m-l-xs"><a href="partners/actReport/{{row.entity.Id}}" target="_blank">Скачать</a></div> ' +
                        '</div>',
                },
                {
                    name: 'PeriodFromFormatted',
                    displayName: 'Период начислений, от',
                },
                {
                    name: 'PeriodToFormatted',
                    displayName: 'Период начислений, до',
                },
                {
                    name: 'DateCreatedFormatted',
                    displayName: 'Создан',
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 35,
                    enableSorting: false,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div class="js-grid-not-clicked">' +
                        '<ui-grid-custom-delete url="partners/deleteActReport" params="{id: row.entity.Id }"></ui-grid-custom-delete>' +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="partners/deleteActReport" params="{id: row.entity.Id }" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ],
        });
        ctrl.$onInit = function () {
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    partnerActReportsCtrl: ctrl,
                });
            }
        };
        ctrl.generateActReport = function (sendMail) {
            $http
                .post('partners/generateActReport', {
                    partnerId: ctrl.partnerId,
                    sendMail: sendMail,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.success('', 'Акт-отчет сформирован' + (sendMail ? ' и отправлен партнеру' : ''));
                        ctrl.gridPartnerActReports.fetchData();
                    } else {
                        toaster.error('', (data.errors || [])[0] || 'Не удалось сформировать акт-отчет');
                    }
                });
        };
    };
    PartnerActReportsCtrl.$inject = ['$http', 'uiGridCustomConfig', '$translate', 'toaster'];
    ng.module('partnerActReports', ['uiGridCustom'])
        .controller('PartnerActReportsCtrl', PartnerActReportsCtrl)
        .component('partnerActReports', {
            templateUrl: partnerActReportsTemplate,
            controller: PartnerActReportsCtrl,
            bindings: {
                partnerId: '<?',
                onInit: '&',
            },
        });
})(window.angular);
