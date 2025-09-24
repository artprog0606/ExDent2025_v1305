import './AddEditTriggerFilterRule.html';

(function (ng) {
    'use strict';

    var ModalAddEditTriggerFilterRuleCtrl = function ($http, $filter, $uibModalInstance, SweetAlert, toaster, $q) {
        var ctrl = this;
        ctrl.formInited = false;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            if (params.event != null) {
                ctrl.eventType = params.event.type;
            }
            ctrl.mode = params.rule != null ? 'edit' : 'add';
            ctrl.isLicense = params.isLicense != null && params.isLicense;

            ctrl.compareTypes = [
                { name: 'равно', value: 0 },
                { name: 'не равно', value: 1 },
            ];
            ctrl.checkboxOptions = [
                { label: 'Да', value: true },
                { label: 'Нет', value: false },
            ];

            ctrl.getFormData().then(function () {
                if (ctrl.mode == 'add') {
                    ctrl.resetComparers();
                } else {
                    ctrl.field = $filter('filter')(ctrl.fields, function (item) {
                        return (
                            item.type == params.rule.FieldType &&
                            (params.rule.FieldComparer == null ||
                                (params.rule.FieldComparer != null &&
                                    (params.rule.FieldComparer.FieldObjId == null || item.objId == params.rule.FieldComparer.FieldObjId)))
                        );
                    })[0];

                    ctrl.field.FieldTypeStr = params.rule.FieldTypeStr;

                    if (params.rule.FieldComparer != null) {
                        $q.when(ctrl.setParamValues(ctrl.field.type)).then(function () {
                            ctrl.fieldComparer = {
                                Type: params.rule.FieldComparer.Type,
                            };
                            ctrl.field.CompareType = params.rule.CompareType;

                            switch (ctrl.fieldComparer.Type) {
                                case 1:
                                case 4:
                                    ctrl.fieldComparer.Value = params.rule.FieldComparer.Value;
                                    break;
                                case 2:
                                    ctrl.fieldComparer.From = params.rule.FieldComparer.From;
                                    ctrl.fieldComparer.To = params.rule.FieldComparer.To;
                                    ctrl.fieldComparer.DateFrom = params.rule.FieldComparer.DateFromString;
                                    ctrl.fieldComparer.DateTo = params.rule.FieldComparer.DateToString;
                                    break;
                                case 3:
                                    ctrl.fieldComparer.Flag = params.rule.FieldComparer.Flag;
                                    break;
                                case 5:
                                    ctrl.fieldComparer.Products = params.rule.FieldComparer.Products;
                                    break;
                                case 6:
                                    ctrl.fieldComparer.Categories = params.rule.FieldComparer.Categories;
                                    ctrl.categoryIds = ctrl.fieldComparer.Categories.map(function (x) {
                                        return x.Id;
                                    });
                                    break;
                                case 7:
                                    ctrl.fieldComparer.Value =
                                        params.rule.FieldComparer.Value != null ? params.rule.FieldComparer.Value.toString() : null;
                                    break;
                                case 8:
                                case 9:
                                case 10:
                                    ctrl.fieldComparer.From = params.rule.FieldComparer.From;
                                    ctrl.fieldComparer.To = params.rule.FieldComparer.To;
                                    break;
                                case 11:
                                case 12:
                                    ctrl.fieldComparer.SalesFunnelId = params.rule.FieldComparer.SalesFunnelId;
                                    ctrl.changeSalesFunnel(ctrl.fieldComparer.SalesFunnelId).then(function () {
                                        ctrl.fieldComparer.DealStatusId = params.rule.FieldComparer.DealStatusId;
                                    });
                                    break;
                            }
                        });
                    }
                    ctrl.addEditRuleForm.$setPristine();
                }
                ctrl.formInited = true;
            });
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.apply = function () {
            var result = {
                FieldType: ctrl.field.type,
                FieldTypeStr: ctrl.field.FieldTypeStr || ctrl.field.typeStr,
                FieldName: ctrl.field.name,
                FieldComparer: {
                    Type: ctrl.fieldComparer.Type,
                    FieldObjId: ctrl.field.objId,
                },
                CompareType: ctrl.field.CompareType,
            };
            switch (ctrl.fieldComparer.Type) {
                case 1: // equal
                case 4: // contains
                case 7: // customersegment
                    result.FieldComparer.Value = ctrl.fieldComparer.Value != null ? ctrl.fieldComparer.Value : '';
                    if (!ctrl.paramValuesEmpty()) {
                        var selected = $filter('filter')(ctrl.compareValues, function (item) {
                            return item.value === ctrl.fieldComparer.Value;
                        })[0];
                        result.FieldValueObjectName = selected != null ? selected.label : null;
                        if (ctrl.field.objId == null) {
                            result.FieldComparer.ValueObjId = ctrl.fieldComparer.Value;
                        }
                    }
                    break;
                case 2: // range
                    if (ctrl.field.fieldType == 'date' || ctrl.field.fieldType == 'datetime' || ctrl.field.fieldType == 'time') {
                        result.FieldComparer.DateFrom = result.FieldComparer.DateFromString = ctrl.fieldComparer.DateFrom;
                        result.FieldComparer.DateTo = result.FieldComparer.DateToString = ctrl.fieldComparer.DateTo;
                    } else {
                        result.FieldComparer.From = ctrl.fieldComparer.From;
                        result.FieldComparer.To = ctrl.fieldComparer.To;
                    }
                    break;
                case 3: // flag
                    result.FieldComparer.Flag = ctrl.fieldComparer.Flag;
                    break;
                case 5: // product chooser
                    result.FieldComparer.Products = ctrl.fieldComparer.Products;
                    break;
                case 6: // category chooser
                    result.FieldComparer.Categories = ctrl.fieldComparer.Categories;
                    break;
                case 8: // orderspaidsum
                case 9: // orderscount
                case 10: // orderspaidcount
                    result.FieldComparer.From = ctrl.fieldComparer.From;
                    result.FieldComparer.To = ctrl.fieldComparer.To;
                    break;
                case 11: // openleadsalesfunnels
                case 12: // dealstatus
                    result.FieldComparer.SalesFunnelId = ctrl.fieldComparer.SalesFunnelId;
                    result.FieldComparer.DealStatusId = ctrl.fieldComparer.DealStatusId;

                    result.FieldComparer.SalesFunnelName = ctrl.salesFunnels.filter(function (x) {
                        return x.value == ctrl.fieldComparer.SalesFunnelId;
                    })[0].label;

                    var statuses = ctrl.dealStatuses.filter(function (x) {
                        return x.value == ctrl.fieldComparer.DealStatusId;
                    });
                    result.FieldComparer.DealStatusName = statuses != null && statuses.length > 0 ? statuses[0].label : 'Любой';
                    break;
            }
            $uibModalInstance.close(result);
        };

        ctrl.resetComparers = function () {
            ctrl.fieldComparer = {
                Type: ctrl.eventType == 'MessageReply' ? 0 : 1,
            };
            ctrl.compareValues = null;
        };

        ctrl.paramValuesEmpty = function () {
            return ctrl.compareValues == null || ctrl.compareValues.length == 0;
        };

        ctrl.setParamValues = function (field) {
            ctrl.resetComparers();

            var promise = null;

            if (field != null) {
                ctrl.field.CompareType = 0;

                if (field.fieldType == 'checkbox') {
                    ctrl.fieldComparer.Type = 3;
                    ctrl.fieldComparer.Flag = true;
                } else if (
                    ((ctrl.eventType == 'LeadCreated' || ctrl.eventType == 'LeadStatusChanged') && field.type == 14) || // описание лида
                    ((ctrl.eventType == 'TaskCreated' || ctrl.eventType == 'TaskStatusChanged') && (field.type == 5 || field.type == 6))
                ) {
                    // название и описание задачи

                    ctrl.fieldComparer.Type = 4;
                } else if (ctrl.field.fieldType == 'productchooser') {
                    ctrl.fieldComparer.Type = 5;
                } else if (ctrl.field.fieldType == 'categorychooser') {
                    ctrl.fieldComparer.Type = 6;
                } else if (ctrl.field.fieldType == 'datetime' || ctrl.field.fieldType == 'time') {
                    ctrl.fieldComparer.Type = 2;
                } else if (ctrl.field.typeStr == 'orderspaidsum') {
                    ctrl.fieldComparer.Type = 8;
                } else if (ctrl.field.typeStr == 'orderscount') {
                    ctrl.fieldComparer.Type = 9;
                } else if (ctrl.field.typeStr == 'orderspaidcount') {
                    ctrl.fieldComparer.Type = 10;
                } else if (ctrl.field.typeStr == 'openleadsalesfunnels') {
                    ctrl.fieldComparer.Type = 11;

                    promise = ctrl.getParamValues().then(function (data) {
                        ctrl.salesFunnels = data != null ? data.values : null;
                        return data;
                    });
                } else if (ctrl.field.typeStr == 'dealstatus') {
                    ctrl.fieldComparer.Type = 12;

                    promise = ctrl.getParamValues().then(function (data) {
                        ctrl.salesFunnels = data != null ? data.values : null;
                        return data;
                    });
                } else {
                    if (ctrl.field.typeStr == 'customersegment') {
                        ctrl.fieldComparer.Type = 7;
                    }

                    promise = ctrl.getParamValues().then(function (data) {
                        ctrl.compareValues = data != null ? data.values : null;
                        return data;
                    });
                }
            }

            return promise;
        };

        ctrl.getFormData = function () {
            return $http
                .get('triggers/getFilterRuleFormData', { params: { eventType: ctrl.eventType, rnd: Math.random() } })
                .then(function (response) {
                    var data = response.data;
                    if (data != null) {
                        ctrl.fields = data.fields;
                    }
                });
        };

        ctrl.getParamValues = function () {
            return $http
                .get('triggers/getFilterRuleParamValues', {
                    params: {
                        eventType: ctrl.eventType,
                        fieldType: ctrl.field.type,
                        fieldObjId: ctrl.field.objId,
                    },
                })
                .then(function (response) {
                    return response.data;
                });
        };

        ctrl.changeSalesFunnel = function (salesFunnelId) {
            return $http.get('salesFunnels/getDealStatuses', { params: { salesFunnelId: salesFunnelId } }).then(function (response) {
                ctrl.dealStatuses = response.data;
            });
        };

        ctrl.chooseProducts = function (result) {
            if (result == null || result.ids == null) return;

            ctrl.fieldComparer.Products = ctrl.fieldComparer.Products || [];

            result.ids.forEach(function (id) {
                var p = ctrl.fieldComparer.Products.filter(function (x) {
                    return x.Id == id;
                });
                if (p.length != 0) return;

                $http.get('product/getProductInfoByProductId', { params: { id: id } }).then(function (response) {
                    var data = response.data;
                    if (data != null) {
                        ctrl.fieldComparer.Products.push({ Id: data.ProductId, Name: data.Name });
                    }
                });
            });
            ctrl.addEditRuleForm.modified = true;
        };

        ctrl.removeProduct = function (index) {
            ctrl.fieldComparer.Products.splice(index, 1);
            ctrl.addEditRuleForm.modified = true;
        };

        ctrl.chooseCategories = function (result) {
            if (result == null || result.categoryIds == null) return;

            ctrl.fieldComparer.Categories = ctrl.fieldComparer.Categories || [];

            $http.post('category/getCategoriesByCategoryIds', { categoryIds: result.categoryIds }).then(function (response) {
                response.data.forEach(function (category) {
                    var c = ctrl.fieldComparer.Categories.filter(function (x) {
                        return x.Id == category.CategoryId;
                    });
                    if (c == null || c.length == 0) {
                        ctrl.fieldComparer.Categories.push({ Id: category.CategoryId, Name: category.Name });
                    }
                });

                ctrl.categoryIds = ctrl.fieldComparer.Categories.map(function (x) {
                    return x.Id;
                });
            });
            ctrl.addEditRuleForm.modified = true;
        };

        ctrl.removeCategory = function (index) {
            ctrl.fieldComparer.Categories.splice(index, 1);
            ctrl.addEditRuleForm.modified = true;
        };
    };

    ModalAddEditTriggerFilterRuleCtrl.$inject = ['$http', '$filter', '$uibModalInstance', 'SweetAlert', 'toaster', '$q'];

    ng.module('uiModal').controller('ModalAddEditTriggerFilterRuleCtrl', ModalAddEditTriggerFilterRuleCtrl);
})(window.angular);
