(function (ng) {
    'use strict';

    var ModalAddLandingSiteCtrl = function ($uibModalInstance, $translate, $http, toaster, $q, $scope, SweetAlert) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve.data || { data: {} };

            ctrl.productId = params.productId || null;
            ctrl.type = ctrl.productId != null ? 'oneproduct' : '';
            ctrl.showStepZero = params.showStepZero || false;

            ctrl.template = params.template;
            if (ctrl.template == null) {
                ctrl.template = 'Default';
            }

            ctrl.additionalSalesProductId = params.additionalSalesProductId || null;
            if (ctrl.additionalSalesProductId != null) {
                ctrl.getProduct(ctrl.additionalSalesProductId).then(function (data) {
                    ctrl.name = 'Воронка допродаж к ' + data.Name;
                });
            }

            ctrl.postActions = [
                { label: $translate.instant('Admin.Js.Content.ModalAddLandingSite.OnlineStoreCategory'), value: 1 },
                { label: $translate.instant('Admin.Js.Content.ModalAddLandingSite.Funnel'), value: 2 },
                { label: $translate.instant('Admin.Js.Content.ModalAddLandingSite.YourURL'), value: 3 },
            ];
            ctrl.postAction = ctrl.postActions[0].value;

            ctrl.checkBoxDownsellUpsselModel = {
                showCrossSells: ctrl.showCrossSells,
            };

            ctrl.changeLpType(params.lpType);

            ctrl.trackEventCreateFunnelShow();
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.changeLpType = function (type) {
            ctrl.lpType = type;
            ctrl.step = 10;

            if (ctrl.lpType != null) {
                switch (ctrl.lpType) {
                    case 'Default':
                    case 'LeadMagnet':
                    case 'VideoLeadMagnet':
                    case 'Booking':
                    case 'Events':
                    case 'Conference':
                    case 'EventAction':
                    case 'Course':
                    case 'Consulting':
                    case 'ServicesOnline':
                    case 'QuizFunnel':
                        ctrl.step = 10;
                        break;

                    default:
                        ctrl.step = 1;
                        break;
                }
            }

            if (ctrl.template == 'Article' || ctrl.template == 'CompanySiteWithCatalog') {
                ctrl.step = 1;
            }

            if (ctrl.lpType === 'ProductCrossSellDownSell') {
                if (ctrl.showStepZero) {
                    ctrl.step = 0;
                } else {
                    ctrl.notRedirect = true;
                    ctrl.checkBoxDownsellUpsselModel.showCrossSells = true;

                    ctrl.getProduct(ctrl.productId).then(function (data) {
                        ctrl.productCrossSell = data;
                    });
                }
            }
        };

        ctrl.changeStep = function () {
            ctrl.step += 1;
        };

        ctrl.setStep = function (step, func) {
            if (step != null) {
                ctrl.step = step;
            }
            if (func != null) {
                func();
            }
        };

        ctrl.addProductModal = function (result) {
            if (result != null && result.ids != null && result.ids.length > 0) {
                ctrl.productId = result.ids;
                ctrl.product = [];
                ctrl.getProduct(result.ids[0]).then(function (data) {
                    ctrl.product.push(data);
                    ctrl.name = data.Name;
                });
            }
        };

        ctrl.addProduct = function (result, func) {
            if (result != null && result.ids != null && result.ids.length > 0) {
                ctrl.productId = result.ids[0];
                ctrl.product = [];
                ctrl.getProduct(ctrl.productId).then(function (data) {
                    ctrl.product.push(data);
                    ctrl.name = data.Name;

                    if (func != null) {
                        func();
                    }

                    //ctrl.setStep(step, func);
                });
            }
        };

        ctrl.addProductList = function (result) {
            ctrl.categoryIds = null;
            ctrl.categories = null;

            ctrl.productIds = result != null ? result.ids : null;

            if (result != null && result.ids != null && result.ids.length > 0) {
                var promises = [];
                for (var i = 0; i < ctrl.productIds.length; i++) {
                    var promise = ctrl.getProduct(ctrl.productIds[i]).then(function (data) {
                        return data;
                    });
                    promises.push(promise);
                }

                return $q.all(promises).then(function (data) {
                    return (ctrl.products = data);
                });
            } else {
                return $q.resolve();
            }
        };

        ctrl.addCategories = function (result) {
            ctrl.productIds = null;
            ctrl.products = null;

            ctrl.categoryIds = result != null ? result.categoryIds : null;

            if (result != null && result.categoryIds != null) {
                ctrl.getCategories(ctrl.categoryIds).then(function (data) {
                    ctrl.categories = data;
                });
            }
        };

        ctrl.addCategoriesWithoutSubCats = function (result, step) {
            ctrl.productIds = null;
            ctrl.products = null;
            ctrl.categoryIds = result != null ? result.categories : null;

            if (ctrl.categoryIds != null) {
                ctrl.getCategories(ctrl.categoryIds).then(function (data) {
                    ctrl.categories = data;
                });
            }
        };

        ctrl.removeCategory = function (id) {
            ctrl.categoryIds = ctrl.categoryIds.filter(function (x) {
                return x != id;
            });
            ctrl.categories = ctrl.categories.filter(function (x) {
                return x.CategoryId != id;
            });
        };

        ctrl.setMultyFunnelMode = function () {
            if (ctrl.multyFunnelMode == 'products') {
                ctrl.categoryIds = null;
                ctrl.categories = null;
            } else {
                ctrl.productIds = null;
                ctrl.products = null;
            }
        };

        ctrl.addOfferList = function (result) {
            if (result != null && result.ids != null && result.ids.length > 0) {
                ctrl.offerIds = result.ids;
                var promises = [];
                for (var i = 0; i < ctrl.offerIds.length; i++) {
                    var promise = ctrl.getProductByOffer(ctrl.offerIds[i]).then(function (data) {
                        return data;
                    });
                    promises.push(promise);
                }

                return $q.all(promises).then(function (data) {
                    return (ctrl.offers = data);
                    //ctrl.setStep(step, func);
                });
            } else {
                return $q.resolve();
            }
        };

        ctrl.addUpsellProductWithDelay = function (result, step, func) {
            if (result != null && result.ids != null) {
                if (result.ids.length > 0) {
                    ctrl.upsellProductId = result.ids[0];
                    ctrl.getProduct(ctrl.upsellProductId).then(function (data) {
                        ctrl.upsellProduct = [data];

                        ctrl.setStep(step, func);
                    });
                } else {
                    ctrl.upsellProductId = null;
                }
            }
        };

        ctrl.addUpsellProduct = function (result, step, func) {
            if (result != null && result.ids != null) {
                if (result.ids.length > 0) {
                    ctrl.upsellProductId = result.ids[0];
                    ctrl.getProduct(ctrl.upsellProductId).then(function (data) {
                        ctrl.upsellProduct = [data];

                        if (func != null) {
                            func();
                        }
                    });
                } else {
                    ctrl.upsellProductId = null;
                }
            }
        };

        ctrl.addUpsell2ProductWithDelay = function (result, step, func) {
            if (result != null && result.ids != null && result.ids.length > 0) {
                ctrl.upsell2ProductId = result.ids[0];
                ctrl.getProduct(ctrl.upsell2ProductId).then(function (data) {
                    ctrl.upsell2Product = [data];

                    ctrl.setStep(step, func);
                });
            }
        };

        ctrl.addDownsellProductWithDelay = function (result, step, func) {
            if (result != null && result.ids != null) {
                if (result.ids.length > 0) {
                    ctrl.downsellProductId = result.ids[0];
                    ctrl.getProduct(ctrl.downsellProductId).then(function (data) {
                        ctrl.downsellProduct = [data];

                        ctrl.setStep(step, func);
                    });
                } else {
                    ctrl.downsellProductId = null;
                }
            }
        };

        ctrl.addDownsellProduct = function (result, step, func) {
            if (result != null && result.ids != null) {
                if (result.ids.length > 0) {
                    ctrl.downsellProductId = result.ids[0];
                    ctrl.getProduct(ctrl.downsellProductId).then(function (data) {
                        ctrl.downsellProduct = [data];

                        if (func != null) {
                            func();
                        }
                    });
                } else {
                    ctrl.downsellProductId = null;
                }
            }
        };

        ctrl.onKeydownAddLandingSite = function (event, name, step) {
            if (event.keyCode === 13 && name != null) {
                ctrl.addLandingSite();
                ctrl.step = step;
            }
        };

        ctrl.changeCategory = function (result) {
            ctrl.postActionCategoryId = result.categoryId;
            ctrl.postActionCategoryName = result.categoryName;
        };

        ctrl.getFunnels = function () {
            return $http.get('funnels/getLandingSitesList').then(function (response) {
                ctrl.funnels = response.data;
            });
        };

        ctrl.getPostActionFunnels = function () {
            ctrl.getFunnels().then(function () {
                ctrl.postActionFunnelSiteId = ctrl.funnels != null && ctrl.funnels.length > 0 ? ctrl.funnels[0].value : null;
            });
        };

        ctrl.changePostAction = function () {
            ctrl.postActionCategoryId = null;
            ctrl.postActionFunnelSiteId = null;
            ctrl.postActionUrl = null;
        };

        ctrl.selectPostActionCategory = function (event, data) {
            ctrl.postActionCategoryId = data.node.id;
        };

        ctrl.validateAndGo = function (step) {
            if (ctrl.lpType == 'ProductCrossSellDownSell' && ctrl.downsellProductId == null && ctrl.upsellProductId == null) {
                toaster.pop('error', '', 'Выберите товар допродажи');
            } else if (ctrl.downsellProductId != null && ctrl.upsellProductId == null) {
                toaster.pop('error', '', 'Выберите допродажу 1 (Upsell)');
            } else {
                ctrl.step = step;
            }
        };

        ctrl.addLandingSite = function () {
            var dataPost = {
                name: ctrl.name,
                lpType: ctrl.lpType,
                template: ctrl.template,
                productId: ctrl.productId,
                additionalSalesProductId: ctrl.additionalSalesProductId,
                upsellProductIdFirst: ctrl.upsellProductId,
                upsellProductIdSecond: ctrl.upsell2ProductId,
                downSellProductId: ctrl.downsellProductId,
                productIds:
                    ctrl.products != null && ctrl.products.length > 0
                        ? ctrl.products.map(function (x) {
                              return x.ProductId;
                          })
                        : ctrl.productIds,
                categoryIds: ctrl.categoryIds,
                offerIds: ctrl.offerIds,
                postAction: {
                    postActionType: ctrl.postAction,
                    postActionCategoryId: ctrl.postActionCategoryId,
                    postActionFunnelSiteId: ctrl.postActionFunnelSiteId,
                    postActionUrl: ctrl.postActionUrl,
                },
            };

            $http.post('funnels/addLandingSiteIsAllowed', dataPost).then(function (response) {
                var data = response.data;

                if (data.result) {
                    $http.post('funnels/addLandingSite', dataPost).then(function (response) {
                        var data = response.data;

                        $uibModalInstance.close();

                        if (data.result) {
                            if (ctrl.notRedirect !== true) {
                                window.location.href = data.obj.AdminUrl;
                            }
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                        }
                    });
                } else {
                    var alert = null;
                    if (data.errors != null) {
                        alert = data.errors.reduce(function (prev, error) {
                            return prev + error;
                        }, '');
                    }

                    $uibModalInstance.close();

                    SweetAlert.alert(alert || 'Нельзя создать шаблон', {
                        title: 'Новая воронка',
                        html: alert || 'Нельзя создать шаблон',
                    });
                }
            });
        };

        ctrl.getProduct = function (id) {
            return $http.get('product/getProductInfoByProductId?id=' + id).then(function (response) {
                return response.data;
            });
        };

        ctrl.getProductByOffer = function (id) {
            return $http.get('product/getProductNameByOfferId?offerId=' + id).then(function (response) {
                return response.data;
            });
        };

        ctrl.getCategories = function (ids) {
            return $http.post('category/GetCategoriesByCategoryIds', { categoryIds: ids }).then(function (response) {
                return response.data;
            });
        };

        ctrl.trackEventCreateFunnelShow = function () {
            return $http.post('funnels/TrackCreateFunnelShow').then(function (response) {
                return response.data;
            });
        };

        ctrl.trackEventCreateEmptyFunnelStep1 = function () {
            return $http.post('funnels/TrackCreateEmptyFunnelStep1').then(function (response) {
                return response.data;
            });
        };

        ctrl.trackEventCreateFreeShippingFunnelStep0 = function () {
            return $http.post('funnels/CreateFreeShippingFunnel_Step0').then(function (response) {
                return response.data;
            });
        };

        ctrl.trackEventCreateFreeShippingFunnelStep1 = function () {
            return $http.post('funnels/CreateFreeShippingFunnel_Step1').then(function (response) {
                return response.data;
            });
        };

        ctrl.trackEventCreateFreeShippingFunnelStep2 = function () {
            return $http.post('funnels/CreateFreeShippingFunnel_Step2').then(function (response) {
                return response.data;
            });
        };
        ctrl.trackEventCreateFreeShippingFunnelStep3 = function () {
            return $http.post('funnels/CreateFreeShippingFunnel_Step3').then(function (response) {
                return response.data;
            });
        };
        ctrl.trackEventCreateFreeShippingFunnelStep4 = function () {
            return $http.post('funnels/CreateFreeShippingFunnel_Step4').then(function (response) {
                return response.data;
            });
        };
        ctrl.trackEventCreateFreeShippingFunnelStep5 = function () {
            return $http.post('funnels/CreateFreeShippingFunnel_Step5').then(function (response) {
                return response.data;
            });
        };

        ctrl.trackEventCreateFreeShippingFunnelStepFinal = function () {
            return $http.post('funnels/CreateFreeShippingFunnel_StepFinal').then(function (response) {
                return response.data;
            });
        };

        ctrl.onChangeSwitcher = function (state) {
            ctrl.checkBoxDownsellUpsselModel.showCrossSells = state;
        };
    };

    ModalAddLandingSiteCtrl.$inject = ['$uibModalInstance', '$translate', '$http', 'toaster', '$q', '$scope', 'SweetAlert'];

    ng.module('uiModal').controller('ModalAddLandingSiteCtrl', ModalAddLandingSiteCtrl);
})(window.angular);
