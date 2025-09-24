(function (ng) {
    'use strict';

    var TriggerEditCtrl = function (toaster, triggersService, $filter, $http, $window, SweetAlert, $translate, isMobileService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.isMobile = isMobileService.getValue();

            ctrl.mode = ctrl.id == null || ctrl.id == '' ? 'add' : 'edit';

            ctrl.checkboxOptions = [
                { label: 'Да', value: 'true' },
                { label: 'Нет', value: 'false' },
            ];

            if (ctrl.mode == 'edit') {
                ctrl.getTrigger();
            } else {
                ctrl.getFormData();
            }

            if (ctrl.onInit != null) {
                ctrl.onInit({ triggerEditCtrl: ctrl });
            }
        };

        ctrl.getTrigger = function () {
            triggersService.getTrigger(ctrl.id).then(function (result) {
                ctrl.name = result.Name;
                ctrl.categoryId = result.CategoryId;
                ctrl.eventType = result.EventType;
                ctrl.eventTypeDescription = result.EventTypeDescription;
                ctrl.eventObjId = result.EventObjId;
                ctrl.eventObjValue = result.EventObjValue;
                ctrl.worksOnlyOnce = result.WorksOnlyOnce;
                ctrl.preferredHour = result.PreferredHour;
                ctrl.filter = result.Filter || { Comparers: [] };
                ctrl.actions = result.Actions;
                ctrl.triggerParams = result.TriggerParams;
                ctrl.coupon = result.Coupon;
                ctrl.getFormData();
            });
        };

        ctrl.getFormData = function () {
            if (ctrl.eventType == null || ctrl.eventType == 0) {
                ctrl.eventType = 1;
            }

            return triggersService.getTriggerFormData(ctrl.eventType).then(function (data) {
                ctrl.data = data;

                ctrl.preferredHours = data.PreferredHours;
                ctrl.sinceOptions = data.SinceOptions;
                ctrl.triggerParams = ctrl.triggerParams || {};
                ctrl.triggerParams.Since =
                    ctrl.triggerParams != null && ctrl.triggerParams.Since != null ? ctrl.triggerParams.Since : ctrl.sinceOptions[0].value;
                ctrl.triggerParams.Days = ctrl.triggerParams.Days || 0;
                ctrl.triggerParams.IgnoreYear = ctrl.triggerParams.IgnoreYear != null ? ctrl.triggerParams.IgnoreYear : true;

                ctrl.processType = data.ProcessType;
                ctrl.eventObjects = data.EventObjects;
                ctrl.eventObjectsFetchUrl = data.EventObjectsFetchUrl;
                ctrl.eventObjectGroups = data.EventObjectGroups;
                if (ctrl.eventObjectGroups) {
                    ctrl.eventObjectGroup = ctrl.eventObjectGroups[0].value;
                }

                ctrl.intervalTypes = data.IntervalTypes;
                ctrl.availableVariables = data.AvailableVariables;
                ctrl.availableVariablesKeys = data.AvailableVariables.map((x) => x.Key);
                ctrl.sendRequestParameters = data.SendRequestParameters;
                ctrl.defaultMailTemplate = data.DefaultMailTemplate || {};
                ctrl.IsSmsActive = data.IsSmsActive;
                ctrl.IsSmsAndSocialMediaActive = data.IsSmsAndSocialMediaActive;
                ctrl.IsWazzupActive = data.IsWazzupActive;
                ctrl.AllowSendNotification = data.AllowSendNotification;
                ctrl.PushNotificationErrorMessage = data.PushNotificationErrorMessage;
                ctrl.isLicense = data.IsLicense;
                ctrl.emailSettingsError = data.EmailSettingsError;
                ctrl.categories = data.Categories;
                ctrl.IsTriggerForOrder = data.IsTriggerForOrder;
                ctrl.SendToShippingServiceVariables = data.SendToShippingServiceVariables;
                ctrl.ShippingMethodsUseUnloadOrder = data.ShippingMethodsUseUnloadOrder;
                if (ctrl.mode == 'add' && !ctrl.categoryId) {
                    ctrl.categoryId = 0;
                }

                if (ctrl.mode == 'add') {
                    ctrl.filter = { Comparers: [] };
                    ctrl.showEventObject = (ctrl.eventObjects != null && ctrl.eventObjects.length) || ctrl.eventObjectsFetchUrl;
                }

                if (ctrl.actions == null || ctrl.actions.length == 0) {
                    ctrl.addAction();
                } else {
                    ctrl.actions.forEach(function (action) {
                        action.hideDelay = action.TimeDelay == null;
                    });
                }

                ctrl.getFilterComparers();

                ctrl.getEditActionFormData();
            });
        };

        ctrl.save = function () {
            ctrl.btnSleep = true;
            ctrl.serializeFilter();
            ctrl.serializeTriggerParams();

            var params = {
                id: ctrl.id,
                name: ctrl.name,
                categoryId: ctrl.categoryId,
                eventType: ctrl.eventType,
                eventObjId: ctrl.eventObject != null ? ctrl.eventObject.value : ctrl.eventObjId,
                eventObjValue: ctrl.eventObjValue,
                triggerParamsSerialized: ctrl.triggerParamsSerialized,
                worksOnlyOnce: ctrl.worksOnlyOnce,
                preferredHour: ctrl.preferredHour,
                filterSerialized: ctrl.filterSerialized,
                actions: ctrl.actions,
            };

            $http
                .post('triggers/save', params)
                .then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        toaster.pop('success', '', ctrl.mode == 'add' ? 'Триггер успешно создан' : 'Изменения успешно сохранены');

                        if (ctrl.mode == 'add') {
                            $window.location.assign('triggers/edit/' + data.obj.Id);
                        } else {
                            ctrl.id = data.obj.Id;
                            ctrl.getTrigger();
                        }
                    } else {
                        if (data.errors != null) {
                            data.errors.forEach(function (err) {
                                toaster.pop('error', '', err);
                            });
                        } else {
                            toaster.pop('error', '', data.error);
                        }
                    }
                })
                .finally(function () {
                    ctrl.btnSleep = false;
                });
        };

        ctrl.processFilterRule = function (prevRule, newRule) {
            if (newRule == null) {
                // remove
                SweetAlert.confirm($translate.instant('Admin.Js.TriggerEdit.SureWantToDelete'), {
                    title: $translate.instant('Admin.Js.TriggerEdit.Removal'),
                    confirmButtonText: $translate.instant('Admin.Js.TriggerEdit.OK'),
                    cancelButtonText: $translate.instant('Admin.Js.TriggerEdit.Cancel'),
                }).then(function (result) {
                    if (result && !result.isDismissed) {
                        ctrl.filter.Comparers = $filter('filter')(ctrl.filter.Comparers, function (comparer) {
                            return !ctrl.equalComparers(comparer, prevRule);
                        });
                        ctrl.getFilterComparers();
                        ctrl.serializeFilter();
                        toaster.pop('success', '', $translate.instant('Admin.Js.Design.SuccessfullyDeleted'));
                    } else if (result === false) {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Catalog.ErrorWhileDeleting'));
                    }
                });
            } else if (prevRule == null) {
                // add
                ctrl.filter.Comparers.push(newRule);
            } else {
                // update
                var ind;
                for (var i = 0, len = ctrl.filter.Comparers.length; i < len; i++) {
                    if (ctrl.equalComparers(ctrl.filter.Comparers[i], prevRule)) {
                        //angular.equals(ctrl.filter.Comparers[i], prevRule) === true) {
                        ind = i;
                        break;
                    }
                }

                if (ind !== -1) {
                    ctrl.filter.Comparers[ind] = newRule;
                }
            }
            ctrl.getFilterComparers();
            ctrl.serializeFilter();
        };

        ctrl.getFilterComparers = function () {
            if (ctrl.filter != null) {
                ctrl.filterComparers = [];

                for (var i = 0; i < ctrl.filter.Comparers.length; i++) {
                    var rule = ctrl.filter.Comparers[i];

                    var r = ctrl.filterComparers.filter(function (x) {
                        return (
                            x.FieldType == rule.FieldType &&
                            x.CompareType == rule.CompareType &&
                            (x.FieldComparer || {}).FieldObjId == (rule.FieldComparer || {}).FieldObjId
                        );
                    });

                    if (r == null || r.length == 0) {
                        var newRule = JSON.parse(JSON.stringify(rule));
                        newRule.FieldComparers = [];
                        newRule.FieldComparers.push(rule.FieldComparer);
                        newRule.FieldComparers[0].FieldValueObjectName = newRule.FieldValueObjectName;

                        ctrl.filterComparers.push(newRule);
                    } else {
                        var fieldComparerCopy = JSON.parse(JSON.stringify(rule.FieldComparer));
                        fieldComparerCopy.FieldValueObjectName = rule.FieldValueObjectName;

                        r[0].FieldComparers.push(fieldComparerCopy);
                    }
                }
            } else {
                ctrl.filterComparers = null;
            }
        };

        ctrl.getFilterComparerItem = function (comparer, fieldComparer) {
            var c = JSON.parse(JSON.stringify(comparer));
            c.FieldComparer = fieldComparer;
            delete c.FieldComparers;
            return c;
        };

        ctrl.serializeFilter = function () {
            ctrl.filterSerialized = ctrl.filter != null ? JSON.stringify(ctrl.filter) : null;
        };

        ctrl.serializeTriggerParams = function () {
            ctrl.triggerParamsSerialized = ctrl.triggerParams != null ? JSON.stringify(ctrl.triggerParams) : null;
        };

        ctrl.getEventObjects = function (evenObjectGroup) {
            if (!ctrl.eventObjectsFetchUrl) return;

            return $http.get(ctrl.eventObjectsFetchUrl + evenObjectGroup).then(function (response) {
                ctrl.eventObjects = response.data;
            });
        };

        ctrl.changeEventType = function () {
            if (ctrl.mode == 'add' && ctrl.actions != null && ctrl.actions.length > 0) {
                ctrl.actions[0].showCkeditor = false;
            }

            ctrl.getFormData().then(function () {
                ctrl.filter = { Comparers: [] };

                if (ctrl.mode == 'add') {
                    ctrl.actions[0].EmailSubject = '';
                    ctrl.actions[0].EmailBody = '';

                    ctrl.actions[0].showCkeditor = true;
                }
            });
        };

        ctrl.addAction = function (index) {
            ctrl.actions = ctrl.actions || [];

            var action = {
                IsNew: true,
                hideDelay: true,
                ActionType: 1,
                EmailSubject: '',
                EmailBody: '',
            };
            ctrl.actions.splice(index + 1, 0, action);
        };

        ctrl.removeAction = function (index) {
            SweetAlert.confirm($translate.instant('Admin.Js.TriggerEdit.SureWantToDelete'), {
                title: $translate.instant('Admin.Js.TriggerEdit.Removal'),
                confirmButtonText: $translate.instant('Admin.Js.TriggerEdit.OK'),
                cancelButtonText: $translate.instant('Admin.Js.TriggerEdit.Cancel'),
            }).then(function (result) {
                if (result && !result.isDismissed) {
                    ctrl.actions.splice(index, 1);
                    if (ctrl.mode === 'edit') {
                        ctrl.save();
                    }
                }
            });
        };

        ctrl.changeSortOrder = function (index, isUp) {
            ctrl.moveArray(ctrl.actions, index, isUp ? index - 1 : index + 1);
        };

        ctrl.moveArray = function (arr, old_index, new_index) {
            if (new_index >= arr.length) {
                var k = new_index - arr.length + 1;
                while (k--) {
                    arr.push(undefined);
                }
            }
            arr.splice(new_index, 0, arr.splice(old_index, 1)[0]);
            return arr;
        };

        ctrl.reloadCoupons = function (action) {
            $http.get('coupons/getCouponsByTriggerAction?triggerActionId=' + action.Id).then(function (response) {
                action.Coupons = response.data;
            });
        };

        ctrl.deleteCoupon = function (coupon, action) {
            SweetAlert.confirm($translate.instant('Admin.Js.TriggerEdit.SureWantToDelete'), {
                title: $translate.instant('Admin.Js.TriggerEdit.Removal'),
                confirmButtonText: $translate.instant('Admin.Js.TriggerEdit.OK'),
                cancelButtonText: $translate.instant('Admin.Js.TriggerEdit.Cancel'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('coupons/deleteCoupon', { couponId: coupon.CouponID }).then(function (response) {
                        ctrl.reloadCoupons(action);
                    });
                }
            });
        };

        ctrl.reloadTriggerCoupon = function () {
            $http.get('coupons/getCouponByTrigger?triggerId=' + ctrl.id).then(function (response) {
                ctrl.coupon = response.data;
            });
        };

        ctrl.deleteTriggerCoupon = function (coupon) {
            SweetAlert.confirm($translate.instant('Admin.Js.TriggerEdit.SureWantToDelete'), {
                title: $translate.instant('Admin.Js.TriggerEdit.Removal'),
                confirmButtonText: $translate.instant('Admin.Js.TriggerEdit.OK'),
                cancelButtonText: $translate.instant('Admin.Js.TriggerEdit.Cancel'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('coupons/deleteCoupon', { couponId: coupon.CouponID }).then(function (response) {
                        ctrl.reloadTriggerCoupon();
                    });
                }
            });
        };

        ctrl.getEditActionFormData = function () {
            return $http
                .get('triggers/getEditActionFormData', { params: { eventType: ctrl.eventType, rnd: Math.random() } })
                .then(function (response) {
                    var data = response.data;
                    if (data != null) {
                        ctrl.fields = data.fields;
                    }
                });
        };

        ctrl.trackEditField = function (item) {
            return { type: item.type, objId: item.objId };
        };

        ctrl.getFilterCompareType = function (index, type) {
            if (index > 0 && !ctrl.isMobile) return '';

            if (index > 0 && ctrl.isMobile && type != 1) return '= ';

            if (type == 1) {
                return ' не равно ';
            }

            return '= ';
        };

        ctrl.setName = function (id, value) {
            ctrl.name = value;

            triggersService
                .saveName(id, value)
                .then(function () {
                    toaster.pop('success', $translate.instant('Admin.Js.Triggers.SaveNameComplete'));
                })
                .catch(function () {
                    toaster.pop('error', $translate.instant('Admin.Js.Triggers.SaveNameError'));
                });
        };

        ctrl.equalComparers = function (x, y) {
            if (x == null && y == null) return true;

            if ((x == null && y != null) || (x != null && y == null)) return false;

            return (
                x.CompareType == y.CompareType &&
                x.FieldName == y.FieldName &&
                x.FieldType == y.FieldType &&
                ctrl.equalFieldComparer(x.FieldComparer, y.FieldComparer)
            );
        };

        ctrl.equalFieldComparer = function (x, y) {
            if (x == null && y == null) return true;

            if ((x == null && y != null) || (x != null && y == null)) return false;

            return (
                x.FieldObjId == y.FieldObjId &&
                x.Type == y.Type &&
                x.Value == y.Value &&
                x.ValueObjId == y.ValueObjId &&
                x.From == y.From &&
                x.To == y.To &&
                ctrl.equalArrayOfObject(x.Categories, y.Categories, 'Id') &&
                ctrl.equalArrayOfObject(x.Products, y.Products, 'Id')
            );
        };

        ctrl.equalArrayOfObject = function (x, y, propertyName) {
            if (x == null && y == null) return true;
            if ((x != null && y == null) || (x == null && y != null)) return false;

            return x.length === y.length && x.map((item) => item[propertyName]).join('_') === y.map((item) => item[propertyName]).join('_');
        };

        ctrl.copyTrigger = function () {
            SweetAlert.confirm('Сделать копию триггера?', { title: 'Копирование' }).then(function (result) {
                if (result && !result.isDismissed) {
                    $http.post('triggers/copyTrigger', { id: ctrl.id }).then(function (response) {
                        if (response.data.result == true) {
                            window.location.href = 'triggers/edit/' + response.data.obj;
                        }
                    });
                }
            });
        };

        ctrl.selectSmsTemplate = function (action, template) {
            action.SendSmsData.SmsTemplateId = template.Id;
            action.SendSmsData.SmsText = template.Text;
        };
    };

    TriggerEditCtrl.$inject = ['toaster', 'triggersService', '$filter', '$http', '$window', 'SweetAlert', '$translate', 'isMobileService'];

    ng.module('triggers').controller('TriggerEditCtrl', TriggerEditCtrl);
})(window.angular);
