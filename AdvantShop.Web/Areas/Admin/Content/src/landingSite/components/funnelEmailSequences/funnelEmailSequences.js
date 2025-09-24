(function (ng) {
    'use strict';

    var FunnelEmailSequencesCtrl = function ($http, $uibModal, $window, SweetAlert, $translate, toaster) {
        var ctrl = this;

        ctrl.init = function (model) {
            ctrl.fetch(model.dateFrom, model.dateTo);
            ctrl.triggersActive = model.triggersActive;
            ctrl.canAddSalesChannel = model.canAddSalesChannel;
            ctrl.triggerObjectTypes = model.triggerObjectTypes;
        };

        ctrl.fetch = function (dateFrom, dateTo) {
            if (dateFrom) {
                ctrl.emailsDateFrom = dateFrom;
            }
            if (dateTo) {
                ctrl.emailsDateTo = dateTo;
            }
            if (!ctrl.emailsDateFrom || !ctrl.emailsDateTo) {
                return;
            }

            $http
                .get('funnels/getSiteTriggerEmails', {
                    params: {
                        orderSourceId: ctrl.orderSourceId,
                        dateFrom: ctrl.emailsDateFrom,
                        dateTo: ctrl.emailsDateTo,
                    },
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        ctrl.triggerEmails = data.obj;
                    }
                });
        };

        ctrl.addTrigger = function () {
            if (ctrl.triggersActive) {
                ctrl.addTriggerModal();
                return;
            }
            if (!ctrl.canAddSalesChannel) {
                SweetAlert.alert(null, { title: '', text: $translate.instant('Admin.Js.Funnel.TriggerMarketing') });
                return;
            }

            SweetAlert.confirm(null, {
                title: '',
                text: $translate.instant('Admin.Js.Funnel.TriggerMarketing'),
                showCancelButton: true,
                type: 'warning',
                confirmButtonText: $translate.instant('Admin.Js.Funnel.Connect'),
                cancelButtonText: $translate.instant('Admin.Js.Funnel.Cancel'),
            }).then(function (result) {
                if (result === true || result.value) {
                    ctrl.addTriggersChannelModal();
                }
            });
        };

        ctrl.addTriggerModal = function () {
            $uibModal
                .open({
                    animation: false,
                    bindToController: true,
                    controller: 'ModalAddTriggerCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: '../areas/admin/content/src/triggers/modal/addTrigger/addTrigger.html',
                    resolve: { params: { orderSourceId: ctrl.orderSourceId, objectTypes: ctrl.triggerObjectTypes } },
                })
                .result.then(
                    function (result) {
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };

        ctrl.deleteTrigger = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Funnel.SureWantToDelete'), {
                title: $translate.instant('Admin.Js.GridCustomComponent.Deleting'),
                cancelButtonText: $translate.instant('Admin.Js.Funnel.DeleteCancel'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('triggers/deleteTrigger', { id: id }).then(function (response) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Funnel.SuccessfullySaved'));
                        ctrl.fetch(ctrl.emailsDateFrom, ctrl.emailsDateTo);
                    });
                }
            });
        };

        ctrl.addTriggersChannelModal = function () {
            $uibModal
                .open({
                    animation: false,
                    bindToController: true,
                    controller: 'ModalSalesChannelsCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: '../areas/admin/content/src/_shared/modal/salesChannels/salesChannels.html',
                    resolve: { data: { selectedChannelTypeStr: 'triggers', closeOnComplete: true } },
                    size: 'sidebar-unit-modal-trigger',
                    backdrop: 'static',
                    windowClass: 'simple-modal modal-sales-channels',
                })
                .result.then(
                    function (result) {
                        if (result === true) {
                            ctrl.triggersActive = true;
                            ctrl.addTriggerModal();
                        }
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };

        ctrl.setMailSettings = function () {
            SweetAlert.confirm('', {
                title: '',
                html:
                    '<div>Для сбора и отображения статистики открываемости писем необходимо подключить почтовый сервис ADVANTSHOP.</div>' +
                    '<a href="https://www.advantshop.net/help/pages/email-google-yandex#200" target="_blank">Подробнее...</a>',
                type: 'info',
                confirmButtonText: 'Подключить',
            }).then(function (result) {
                if (result === true || result.value) {
                    $window.location.assign('settingsmail?notifyTab=emailsettings');
                }
            });
        };
    };

    FunnelEmailSequencesCtrl.$inject = ['$http', '$uibModal', '$window', 'SweetAlert', '$translate', 'toaster'];

    ng.module('landingSite')
        .controller('FunnelEmailSequencesCtrl', FunnelEmailSequencesCtrl)
        .component('funnelEmailSequences', {
            templateUrl: 'funnels/_emailSequences',
            controller: 'FunnelEmailSequencesCtrl',
            transclude: true,
            bindings: {
                orderSourceId: '<',
            },
        });
})(window.angular);
