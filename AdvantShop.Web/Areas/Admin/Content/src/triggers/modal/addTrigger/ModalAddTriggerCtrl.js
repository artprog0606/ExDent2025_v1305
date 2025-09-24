(function (ng) {
    'use strict';

    var ModalAddTriggerCtrl = function ($http, $uibModalInstance, triggersService, $window, toaster) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.params || {};
            ctrl.objectTypes = params.objectTypes;
            ctrl.orderSourceId = params.orderSourceId;
            ctrl.getFormData();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.addTrigger = function () {
            if (ctrl.btnSleep === true) {
                return;
            }

            ctrl.btnSleep = true;
            ctrl.serializeTriggerParams();

            var params = {
                categoryId: ctrl.categoryId,
                eventType: ctrl.eventType,
                eventObjId: ctrl.eventObject != null ? ctrl.eventObject.value : ctrl.eventObjId,
                eventObjValue: ctrl.eventObjValue,
                triggerParamsSerialized: ctrl.triggerParamsSerialized,
                orderSourceId: ctrl.orderSourceId,
            };

            $http.post('triggers/add', params).then(function (response) {
                var data = response.data;
                if (data.result == true) {
                    toaster.pop('success', '', 'Триггер успешно создан');
                    $window.location.assign('triggers/edit/' + data.obj.Id);
                } else {
                    if (data.errors != null) {
                        data.errors.forEach(function (err) {
                            toaster.pop('error', '', err);
                        });
                    } else {
                        toaster.pop('error', '', data.error);
                    }
                }
            });
        };

        ctrl.getFormData = function () {
            if (ctrl.eventType == null || ctrl.eventType == 0) {
                ctrl.eventType = 1;
            }

            return triggersService.getTriggerFormData(ctrl.eventType, ctrl.objectTypes).then(function (data) {
                ctrl.data = data;

                if (data.result === false) {
                    toaster.error('', (data.errors || [])[0] || 'Ошибка при загрузке данных');
                    ctrl.close();
                    return;
                }

                ctrl.preferredHours = data.PreferredHours;
                ctrl.sinceOptions = data.SinceOptions;
                ctrl.triggerParams = ctrl.triggerParams || {};
                ctrl.triggerParams.Since =
                    ctrl.triggerParams != null && ctrl.triggerParams.Since != null ? ctrl.triggerParams.Since : ctrl.sinceOptions[0].value;
                ctrl.triggerParams.Days = ctrl.triggerParams.Days || 0;
                ctrl.triggerParams.IgnoreYear = ctrl.triggerParams.IgnoreYear != null ? ctrl.triggerParams.IgnoreYear : true;

                ctrl.processType = data.ProcessType;

                ctrl.eventObjects = data.EventObjects;
                if (ctrl.eventObjects && ctrl.eventObjects.length > 0) {
                    ctrl.eventObject = ctrl.eventObjects[0];
                } else {
                    ctrl.eventObject = null;
                }

                ctrl.eventObjectsFetchUrl = data.EventObjectsFetchUrl;
                ctrl.eventObjectGroups = data.EventObjectGroups;
                if (ctrl.eventObjectGroups) {
                    ctrl.eventObjectGroup = ctrl.eventObjectGroups[0].value;
                } else {
                    ctrl.eventObjectGroup = null;
                }

                ctrl.categories = data.Categories;

                ctrl.categoryId = ctrl.categoryId || 0;

                ctrl.filter = { Comparers: [] };
                ctrl.showEventObject = (ctrl.eventObjects != null && ctrl.eventObjects.length) || ctrl.eventObjectsFetchUrl;
            });
        };

        ctrl.changeEventType = function () {
            ctrl.getFormData();
        };

        ctrl.getEventObjects = function (evenObjectGroup) {
            if (!ctrl.eventObjectsFetchUrl) return;

            return $http.get(ctrl.eventObjectsFetchUrl + evenObjectGroup).then(function (response) {
                ctrl.eventObjects = response.data;
                if (ctrl.eventObjects && ctrl.eventObjects.length > 0) {
                    ctrl.eventObject = ctrl.eventObjects[0];
                }
            });
        };

        ctrl.serializeTriggerParams = function () {
            ctrl.triggerParamsSerialized = ctrl.triggerParams != null ? JSON.stringify(ctrl.triggerParams) : null;
        };
    };

    ModalAddTriggerCtrl.$inject = ['$http', '$uibModalInstance', 'triggersService', '$window', 'toaster'];

    ng.module('uiModal').controller('ModalAddTriggerCtrl', ModalAddTriggerCtrl);
})(window.angular);
