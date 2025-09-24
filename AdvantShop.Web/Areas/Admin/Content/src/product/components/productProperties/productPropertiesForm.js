import productPropertiesFormTemplate from './productPropertiesForm.html';
(function (ng) {
    'use strict';

    var ProductPropertiesFormCtrl = function ($http, $filter, $q, $timeout, toaster, $translate, productPropertiesService) {
        var ctrl = this;
        ctrl.$onInit = function () {
            if (ctrl.resolve != null && ctrl.resolve.data != null) {
                ctrl.productId = ctrl.resolve.data.productId;
                ctrl.isMobileMode = ctrl.resolve.data.isMobileMode;
                ctrl.asModal = ctrl.resolve.data.asModal;
            }
            ctrl.propertiesPage = 0;
            ctrl.propertiesSize = 200;
            ctrl.propertiesTotalPageCount = 0;
            ctrl.propertyValuesPage = 0;
            ctrl.propertyValuesSize = 200;
            ctrl.propertyValuesTotalPageCount = 0;
            ctrl.propertiesList = [];
            ctrl.propertyValuesList = [];

            ctrl.firstCallProperties();
        };
        ctrl.addPropertyWithValue = function (FormCtrl) {
            var params = {
                ProductId: ctrl.productId,
            };
            if (ctrl.selectedPropertyId != null) {
                params.PropertyId = ctrl.selectedPropertyId;
            } else {
                params.PropertyName = ctrl.selectedProperty.Name;
            }
            if (ctrl.selectedPropertyId != null && ctrl.selectedPropertyValueId != null) {
                params.PropertyValueId = ctrl.selectedPropertyValueId;
            } else {
                params.PropertyValue = ctrl.selectedPropertyValue.Value;
            }
            productPropertiesService.addPropertyWithValue(params).then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSuccessfullySaved'));

                    //.then(ctrl.selectProperty);

                    if (ctrl.$selectProperty) {
                        ctrl.$selectProperty.search = '';
                    }
                    if (ctrl.$selectPropertyValue) {
                        ctrl.$selectPropertyValue.search = '';
                    }
                    ctrl.selectedProperty = null;
                    ctrl.selectedPropertyId = null;
                    ctrl.selectedPropertyValue = null;
                    ctrl.selectedPropertyValueId = null;
                    ctrl.propertiesQ = null;
                    FormCtrl.$setPristine();
                    if (ctrl.close != null) {
                        ctrl.close({
                            $value: data.result,
                        });
                    }
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileAddingProperty'));
                    if (ctrl.dismiss != null) {
                        ctrl.dismiss({
                            $value: data.result,
                        });
                    }
                }
            });
        };
        ctrl.selectProperty = function ($item, $model) {
            if ($item) {
                ctrl.choosedItem = $item;
                ctrl.selectedPropertyId = $item.PropertyId;
                ctrl.selectedPropertyValue = null;
                ctrl.selectedPropertyValueId = null;
                ctrl.propertiesQ = null;
                if (ctrl.$selectProperty) {
                    ctrl.$selectProperty.search = $model.Name;
                }
                if (ctrl.$selectPropertyValue) {
                    ctrl.$selectPropertyValue.search = '';
                }
                ctrl.propertyValuesList.length = 0;
                ctrl.findPropertyValue(ctrl.selectedPropertyId, null);
            }
        };
        ctrl.selectPropertyValue = function ($item, $model) {
            if (ctrl.$selectPropertyValue) {
                ctrl.$selectPropertyValue.search = $model != null ? $model.Value : '';
            }
            ctrl.propertyValuesQ = null;
            ctrl.selectedPropertyValueId = $model != null ? $model.PropertyValueId : null;
        };
        ctrl.findProperty = function (q, $select) {
            ctrl.$selectProperty = $select;
            ctrl.propertiesQ = q;
            ctrl.propertiesPage = 1;
            if (ctrl.$selectPropertyValue) {
                ctrl.$selectPropertyValue.search = '';
            }
            ctrl.selectedPropertyValue = null;
            ctrl.selectedPropertyValueId = null;
            ctrl.propertyValuesList.length = 0;
            productPropertiesService.getAllProperties(ctrl.propertiesPage, ctrl.propertiesSize, q, ctrl.productId).then(function (data) {
                var hasItems = data.DataItems.length > 0;
                var result = hasItems === true ? data.DataItems : [];
                var qItem = {
                    Name: q,
                };
                var itemFinded;
                if (q != null && q.length > 0) {
                    for (var i = 0, len = data.DataItems.length; i < len; i++) {
                        if (q === data.DataItems[i].Name) {
                            itemFinded = data.DataItems[i];
                        }
                    }
                }
                if (itemFinded != null) {
                    ctrl.selectedProperty = itemFinded;
                    ctrl.selectedPropertyId = itemFinded.PropertyId;
                } else if (q != null && q.length > 0) {
                    ctrl.selectedProperty = qItem;
                    ctrl.selectedPropertyId = null;
                    result.push(qItem);
                }
                return (ctrl.propertiesList = result);
            });
        };
        ctrl.findPropertyValue = function (propertyId, q, $select) {
            var defer = $q.defer();
            var promise;
            ctrl.$selectPropertyValue = $select;
            ctrl.propertyValuesQ = q;
            ctrl.propertyValuesPage = 1;
            if (propertyId != null) {
                promise = productPropertiesService
                    .getAllPropertyValues(propertyId, ctrl.propertyValuesPage, ctrl.propertyValuesSize, q)
                    .then(function (data) {
                        var hasItems = data.DataItems.length > 0;
                        var result = hasItems === true ? data.DataItems : [];
                        var qItem = {
                            Value: q,
                        };
                        var itemFinded;
                        if (q != null && q.length > 0) {
                            for (var i = 0, len = data.DataItems.length; i < len; i++) {
                                if (q === data.DataItems[i].Value) {
                                    itemFinded = data.DataItems[i];
                                }
                            }
                        }
                        if (itemFinded != null) {
                            ctrl.selectedPropertyValue = itemFinded;
                            ctrl.selectedPropertyValueId = itemFinded.PropertyValueId;
                        } else if (q != null && q.length > 0) {
                            ctrl.selectedPropertyValue = qItem;
                            ctrl.selectedPropertyValueId = null;
                            result.push(qItem);
                        }
                        return (ctrl.propertyValuesList = result);
                    });
            } else {
                ctrl.propertyValuesList =
                    q != null && q.length > 0
                        ? [
                              {
                                  Value: q,
                              },
                          ]
                        : [];
                promise = defer.promise;
                defer.resolve(ctrl.propertyValuesList);
            }
            return promise;
        };
        ctrl.getMore = function () {
            if (ctrl.propertiesPage > ctrl.propertiesTotalPageCount || ctrl.loadingProperties === true) {
                return $q.resolve();
            }
            ctrl.propertiesPage += 1;
            ctrl.loadingProperties = true;
            return productPropertiesService
                .getAllProperties(ctrl.propertiesPage, ctrl.propertiesSize, ctrl.propertiesQ, ctrl.productId)
                .then(function (data) {
                    ctrl.propertiesList = ctrl.propertiesPage === 1 ? data.DataItems : ctrl.propertiesList.concat(data.DataItems);
                    ctrl.propertiesTotalPageCount = data.TotalPageCount;
                    return data;
                })
                .finally(function () {
                    ctrl.loadingProperties = false;
                });
        };
        ctrl.getMorePropertiesValue = function () {
            if (ctrl.propertyValuesPage > ctrl.propertyValuesTotalPageCount || ctrl.loadingPropertyValues === true) {
                return $q.resolve();
            }
            if (ctrl.selectedProperty != null && ctrl.selectedProperty.PropertyId != null) {
                ctrl.propertyValuesPage += 1;
                ctrl.loadingPropertyValues = true;
                return productPropertiesService
                    .getAllPropertyValues(ctrl.selectedProperty.PropertyId, ctrl.propertyValuesPage, ctrl.propertyValuesSize, ctrl.propertyValuesQ)
                    .then(function (data) {
                        ctrl.propertyValuesList = ctrl.propertyValuesList.concat(data.DataItems);
                        ctrl.propertyValuesTotalPageCount = data.TotalPageCount;
                        return data;
                    })
                    .finally(function () {
                        ctrl.loadingPropertyValues = false;
                    });
            } else {
                return $q.resolve();
            }
        };
        ctrl.closeSelectProperty = function (isOpen) {
            //применяем к модели несуществующее свойство
            var propertyInList;
            if (isOpen == false) {
                if (ctrl.propertiesQ != null && ctrl.propertiesQ.length > 0) {
                    for (var i = 0, len = ctrl.propertiesList.length; i < len; i++) {
                        if (
                            ctrl.propertiesList[i].Name.toLowerCase() === ctrl.propertiesQ.toLowerCase() &&
                            (ctrl.selectedProperty != null ? ctrl.selectedProperty === ctrl.propertiesList[i] : true)
                        ) {
                            propertyInList = ctrl.propertiesList[i];
                            break;
                        }
                    }
                }
                if (ctrl.selectedProperty == null && ctrl.propertiesQ != null && propertyInList == null) {
                    ctrl.selectedProperty = {
                        Name: ctrl.propertiesQ,
                    };
                    ctrl.propertyValuesList.length = 0;
                }
                if (propertyInList != null) {
                    ctrl.selectedProperty = propertyInList;
                    ctrl.selectedPropertyId = propertyInList.PropertyId;
                }
                ctrl.propertiesPage = 0;
                ctrl.propertiesQ = null;
            }
        };
        ctrl.clickProperty = function () {
            if (ctrl.propertiesList != null && ctrl.propertiesList.length > 0) {
                let notSkip =
                    ctrl.propertiesList.length === 1 && ctrl.choosedItem != null && ctrl.propertiesList[0].PropertyId === ctrl.choosedItem.PropertyId;
                if (!notSkip) {
                    return;
                }
            }

            ctrl.firstCallProperties();
        };
        ctrl.firstCallProperties = function () {
            ctrl.propertiesPage = 0;
            ctrl.propertiesList = [];
            ctrl.propertiesTotalPageCount = 0;
            ctrl.getMore();
        };
        ctrl.firstCallPropertyValues = function () {
            ctrl.propertyValuesPage = 0;
            ctrl.propertyValuesList = [];
            ctrl.propertyValuesTotalPageCount = 0;
            ctrl.getMorePropertiesValue();
        };
    };
    ProductPropertiesFormCtrl.$inject = ['$http', '$filter', '$q', '$timeout', 'toaster', '$translate', 'productPropertiesService'];
    ng.module('productProperties')
        .controller('ProductPropertiesFormCtrl', ProductPropertiesFormCtrl)
        .component('productPropertiesForm', {
            templateUrl: productPropertiesFormTemplate,
            controller: 'ProductPropertiesFormCtrl',
            bindings: {
                close: '&',
                dismiss: '&',
                modalInstance: '<?',
                resolve: '<',
            },
        });
})(window.angular);
