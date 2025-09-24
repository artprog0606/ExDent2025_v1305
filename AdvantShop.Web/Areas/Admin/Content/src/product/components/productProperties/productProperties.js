import productPropertiesTemplate from './productProperties.html';
(function (ng) {
    'use strict';

    var ProductPropertiesCtrl = function ($http, $filter, $q, $timeout, toaster, $translate, productPropertiesService) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.getCurrentProperties();
            //.then(ctrl.selectProperty);

            ctrl.propertyValuesPage = 0;
            ctrl.propertyValuesSize = 200;
            ctrl.propertyValuesList = [];
            ctrl.pagingForExistPropeties = {};
        };
        ctrl.propertyValueTransform = function (newTag) {
            return {
                Value: newTag,
                isTag: true,
            };
        };
        ctrl.addPropertyValue = function (property, item, model) {
            var params = {
                ProductId: ctrl.productId,
                PropertyId: property.PropertyId,
                PropertyValueId: item.PropertyValueId,
                Value: item.Value,
                IsNew: isNaN(parseFloat(item.PropertyValueId)) || parseFloat(item.PropertyValueId) < 1 ? true : false,
            };
            if (
                property.SelectedPropertyValues.filter(function (child) {
                    return child.Value.toLowerCase() === model.Value.toLowerCase();
                }).length > 1
            ) {
                return;
            }
            productPropertiesService.addPropertyValue(params).then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSuccessfullySaved'));
                    item.PropertyValueId = data.propertyValueId;
                    model.PropertyValueId = data.propertyValueId;
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileAddingProperty'));
                }
            });
        };
        ctrl.removePropertyValue = function (propertyId, item, model, groupId) {
            var params = {
                ProductId: ctrl.productId,
                PropertyValueId: item.PropertyValueId,
            };
            ctrl.pagingForExistPropeties[propertyId].page = 0;
            productPropertiesService.removePropertyValue(params).then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSuccessfullySaved'));
                    //.then(ctrl.selectProperty);
                }
            });
        };
        ctrl.selectPropertyValue = function ($item, $model) {
            if (ctrl.$selectPropertyValue) {
                ctrl.$selectPropertyValue.search = $model != null ? $model.Value : '';
            }
            ctrl.propertyValuesQ = null;
            ctrl.selectedPropertyValueId = $model != null ? $model.PropertyValueId : null;
        };
        ctrl.updatePropertyValue = function (propertyId, propertyValue) {
            if (propertyValue.Value) {
                let params = {
                    ProductId: ctrl.productId,
                    PropertyValueId: propertyValue.PropertyValueId,
                };
                productPropertiesService.removePropertyValue(params).then(function (data) {
                    if (data.result === true) {
                        let params = {
                            ProductId: ctrl.productId,
                            PropertyId: propertyId,
                            Value: propertyValue.Value,
                            PropertyValueId: propertyValue.PropertyValueId,
                            IsNew: true,
                        };
                        productPropertiesService.addPropertyValue(params).then(function (data) {
                            if (data.result === true) {
                                toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSuccessfullySaved'));
                                propertyValue.PropertyValueId = data.propertyValueId;
                            } else {
                                toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileAddingProperty'));
                            }
                        });
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileAddingProperty'));
                    }
                });
            } else {
                let params = {
                    ProductId: ctrl.productId,
                    PropertyValueId: propertyValue.PropertyValueId,
                };
                productPropertiesService.removePropertyValue(params).then(function (data) {
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSuccessfullySaved'));
                    }
                });
            }
        };
        ctrl.getCurrentProperties = function () {
            return productPropertiesService.getCurrentProperties(ctrl.productId).then(function (data) {
                if (data != null) {
                    ctrl.categoryName = data.CategoryName;
                    ctrl.groups = data.Groups;
                }
            });
        };
        ctrl.getPropertyValuesByProperty = function (property, q) {
            return productPropertiesService
                .getAllPropertyValues(property.PropertyId, ctrl.propertyValuesPage, ctrl.propertyValuesSize, q)
                .then(function (data) {
                    return (property.PropertyValues =
                        data.DataItems.length > 0
                            ? data.DataItems
                            : [
                                  {
                                      Value: q,
                                  },
                              ]);
                });
        };
        ctrl.getMoreValuesForExistProperty = function (item, size, q) {
            var propertyId = item.PropertyId;
            ctrl.pagingForExistPropeties[propertyId] = ctrl.pagingForExistPropeties[propertyId] || {};
            var currentPage = ctrl.pagingForExistPropeties[propertyId].page || 0;
            var totalPageCount = ctrl.pagingForExistPropeties[propertyId].totalPageCount;
            var newPage;
            if (currentPage >= totalPageCount || ctrl.loadingValuesForExistProperty === true) {
                return $q.resolve();
            }
            if (propertyId != null) {
                newPage = currentPage + 1;
                ctrl.loadingValuesForExistProperty = true;
                return productPropertiesService
                    .getAllPropertyValues(propertyId, newPage, size, q)
                    .then(function (data) {
                        if (data.DataItems != null && data.DataItems.length > 0) {
                            item.PropertyValues = (item.PropertyValues != null ? item.PropertyValues.concat(data.DataItems) : data.DataItems).filter(
                                function (iteration) {
                                    return (
                                        item.SelectedPropertyValues == null ||
                                        item.SelectedPropertyValues.length === 0 ||
                                        !item.SelectedPropertyValues.some(function (child) {
                                            return child.Value.toLowerCase() === iteration.Value.toLowerCase();
                                        })
                                    );
                                },
                            );
                            ctrl.pagingForExistPropeties[propertyId].page = newPage;
                            ctrl.pagingForExistPropeties[propertyId].totalPageCount = data.TotalPageCount;
                        }
                        return data;
                    })
                    .finally(function () {
                        ctrl.loadingValuesForExistProperty = false;
                    });
            } else {
                return $q.resolve();
            }
        };
        ctrl.closeSelectPropertyValue = function (isOpen) {
            var propertyValueInList;
            if (isOpen == false) {
                if (ctrl.propertyValuesQ != null && ctrl.propertyValuesQ.length > 0) {
                    for (var i = 0, len = ctrl.propertyValuesList.length; i < len; i++) {
                        if (
                            ctrl.propertyValuesList[i].Value.toLowerCase() === ctrl.propertyValuesQ.toLowerCase() &&
                            (ctrl.selectedPropertyValue != null ? ctrl.selectedPropertyValue === ctrl.propertyValuesList[i] : true)
                        ) {
                            propertyValueInList = ctrl.propertyValuesList[i];
                            break;
                        }
                    }
                }
                //применяем к модели несуществующее свойство
                if (ctrl.selectedPropertyValue == null && ctrl.propertyValuesQ != null && propertyValueInList == null) {
                    ctrl.selectedPropertyValue = {
                        Value: ctrl.propertyValuesQ,
                    };
                }
                if (propertyValueInList != null) {
                    ctrl.selectedPropertyValue = propertyValueInList;
                    ctrl.selectedPropertyValueId = propertyValueInList.PropertyValueId;
                }
                ctrl.propertyValuesPage = 0;
                ctrl.propertyValuesQ = null;
            }
        };
        ctrl.closeSelectPropertyExit = function (isOpen, property) {
            if (isOpen === false) {
                ctrl.pagingForExistPropeties[property.PropertyId].page = 0;
                property.PropertyValues.length = 0;
            }
        };
        ctrl.firstCallValuesForExistProperty = function (item, size) {
            ctrl.getMoreValuesForExistProperty(item, size);
        };
        ctrl.trackByPropertyValue = function (propertyValue) {
            return JSON.stringify(propertyValue);
        };
    };
    ProductPropertiesCtrl.$inject = ['$http', '$filter', '$q', '$timeout', 'toaster', '$translate', 'productPropertiesService'];
    ng.module('productProperties', ['ui.select'])
        .controller('ProductPropertiesCtrl', ProductPropertiesCtrl)
        .component('productProperties', {
            templateUrl: productPropertiesTemplate,
            controller: 'ProductPropertiesCtrl',
            bindings: {
                productId: '@',
                isMobileMode: '<?',
                dropDownList: '@',
            },
        });
})(window.angular);
