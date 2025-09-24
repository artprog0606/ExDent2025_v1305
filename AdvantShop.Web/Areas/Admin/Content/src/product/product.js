import offerStocksTemplate from './modal/offerStocks/offerStocks.html';

(function (ng) {
    'use strict';

    var ProductCtrl = function (
        $http,
        uiGridCustomConfig,
        toaster,
        SweetAlert,
        $window,
        productService,
        $document,
        $timeout,
        $translate,
        sidebarMenuService,
        $location,
        $uibModal,
    ) {
        var ctrl = this;
        ctrl.MinAmountLimit = 0;
        ctrl.MaxAmountLimit = 1_000_000;
        ctrl.$onInit = function () {
            ctrl.colors = null;
            ctrl.sizes = null;
            ctrl.isProcessGetTags = null;
            ctrl.activeMenu = null;
            ctrl.activeMunuIfNull = $window.location.hash.replace('#', '');
            sidebarMenuService.addCallback(function (menuState) {
                if (ctrl.transformer != null) {
                    ctrl.transformer.calc();
                }
            });
        };
        ctrl.goToPhotos = function () {
            var photosItem = angular.element(document.getElementById('photos'));
            $document.scrollTo(photosItem, 103, 1000);
        };
        ctrl.initProduct = function (productId, isMobile) {
            ctrl.productId = productId;
            ctrl.isMobile = isMobile;
            ctrl.getCategories();
            ctrl.getProductLastModified();

            // ctrl.enableSalesChannelCount = 0;
            // ctrl.allSalesChannelCount = 0;
            // ctrl.loadSalesChannels(productId);

            if (isMobile === true) {
                var hash = $window.location.hash || '#main';
                ctrl.setActiveData(hash, true);
                //var link = document.querySelector('.product-menu .chips-list__item[href$="' + hash + '"]');
                //link.scrollIntoView({ inline: 'center', behavior: 'smooth' });
            }
        };
        ctrl.setActiveElement = function (event, asTab) {
            event.preventDefault();
            //event.target.scrollIntoView({ inline: 'center', behavior: 'smooth'});

            ctrl.setActiveData(event.delegateTarget.hash, asTab);
        };
        ctrl.setActiveData = function (id, asTab) {
            ctrl.activeMunuIfNull = null;
            ctrl.activeMenu = document.querySelector(id);
            var idClear = id.replace('#', '');
            ctrl.activeMenuId = idClear;
            window.history.pushState({}, '', $window.location.pathname + id);
        };
        ctrl.scrollToActiveElement = function (action) {
            ctrl[action] = true;
            if (ctrl.isMobile === false) {
                if (ctrl.activeMenu != null) {
                    setTimeout(function () {
                        $document.scrollToElement(ctrl.activeMenu, 103, 500).catch(() => {
                            /*cancel animation*/
                            /*Если не указать catch, то выходит ошибка: Possibly unhandled rejection*/
                        });
                    }, 500);
                }
            }
        };
        ctrl.getProductLastModified = function () {
            productService.getProductLastModified(ctrl.productId).then(function (data) {
                if (data != null) {
                    ctrl.ModifiedDate = data.ModifiedDate;
                    ctrl.ModifiedBy = data.ModifiedBy;
                }
            });
        };

        //region categories
        ctrl.getCategories = function () {
            $http
                .get('product/getCategories', {
                    params: {
                        productId: ctrl.productId,
                    },
                })
                .then(function (response) {
                    ctrl.categories = response.data;
                });
        };
        ctrl.selectPseudoSelectItem = function (item) {
            ctrl.category = [item];
        };
        ctrl.setMainCategory = function () {
            if (ctrl.category == null || ctrl.category.length === 0 || ctrl.category[0].value == null) return;
            if (ctrl.category.length !== 1) {
                toaster.pop('error', '', $translate.instant('Admin.Js.Product.SelectOneCategory'));
                return;
            }
            var categoryId = ctrl.category[0].value;
            $http
                .post('product/setMainCategory', {
                    productId: ctrl.productId,
                    categoryId: categoryId,
                })
                .then(function (response) {
                    ctrl.getCategories();
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                });
        };
        ctrl.deleteCategory = function () {
            if (ctrl.category == null || ctrl.category.length === 0 || ctrl.category[0].value == null) return;
            var categoryIds = ctrl.category.map(function (x) {
                return x.value;
            });
            SweetAlert.confirm($translate.instant('Admin.Js.Product.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Product.Deleting'),
            }).then(function (result) {
                if (result.value === true) {
                    $http
                        .post('product/deleteCategory', {
                            productId: ctrl.productId,
                            categoryIds: categoryIds,
                        })
                        .then(function (response) {
                            if (response.data === true) {
                                ctrl.getCategories();
                                toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                            } else {
                                toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileDeleting'));
                            }
                        });
                }
            });
        };
        ctrl.addCategory = function (result) {
            var categoryId = result.categoryId;
            $http
                .post('product/addCategory', {
                    productId: ctrl.productId,
                    categoryId: categoryId,
                })
                .then(function (response) {
                    if (response.data === true) {
                        ctrl.getCategories();
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                    }
                });
        };
        ctrl.addCategories = function (result) {
            var categories = result.categories;
            $http
                .post('product/addCategories', {
                    productId: ctrl.productId,
                    categories: categories,
                })
                .then(function (response) {
                    if (response.data === true) {
                        ctrl.getCategories();
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                    }
                });
        };
        //end region

        /* brand */
        ctrl.changeBrand = function (result) {
            $http
                .post('product/changeBrand', {
                    productId: ctrl.productId,
                    brandId: result.brandId,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.brand = result.brandName;
                        ctrl.brandId = result.brandId;
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                    }
                });
        };
        ctrl.deleteBrand = function () {
            $http
                .post('product/deleteBrand', {
                    productId: ctrl.productId,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.brand = $translate.instant('Admin.Js.Product.NotSelected');
                        ctrl.brandId = 0;
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                    }
                });
        };
        /* end region */

        /* tags */

        ctrl.tagTransform = function (newTag) {
            return {
                value: newTag,
            };
        };
        ctrl.getTags = function () {
            ctrl.isProcessGetTags = true;
            $http
                .get('product/getTags', {
                    params: {
                        productId: ctrl.productId,
                    },
                })
                .then(function (response) {
                    ctrl.selectedTags = response.data.selectedTags;
                    return response.data;
                })
                .then(function (data) {
                    return $timeout(function () {
                        ctrl.form.$setPristine();
                        return data;
                    }, 0);
                })
                .then(function (data) {
                    return $timeout(function () {
                        ctrl.isProcessGetTags = false;
                        return data;
                    }, 500);
                });
        };
        ctrl.getTagsByPaging = function (itemsPerPage, q) {
            $http
                .get('product/getTagsByPaging', {
                    params: {
                        itemsPerPage: itemsPerPage,
                        q: q,
                    },
                })
                .then(function (response) {
                    ctrl.tags = response.data;
                    return response.data;
                });
        };
        /* end tags */

        ctrl.deleteProduct = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Product.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Product.Deleting'),
            }).then(function (result) {
                if (result.value) {
                    $http
                        .post('product/deleteProduct', {
                            productId: ctrl.productId,
                        })
                        .then(function (response) {
                            if (response.data.result === true) {
                                $window.location.assign('catalog');
                            } else {
                                toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileDeleting'));
                            }
                        });
                }
            });
        };

        /* offers */
        ctrl.initOffers = function (useOfferWeightAndDimensions, useOfferBarCode, priceRules, siteUrl, availableWarehouses) {
            ctrl.useOfferWeightAndDimensions = useOfferWeightAndDimensions;
            ctrl.useOfferBarCode = useOfferBarCode;
            ctrl.priceRules = priceRules;
            ctrl.availableWarehouses = availableWarehouses;
            var priceRulesEnabled = ctrl.priceRules != null && ctrl.priceRules.length > 0;
            var offersColumnDefs = [
                {
                    name: 'Main',
                    displayName: $translate.instant('Admin.Js.Product.Main'),
                    enableCellEdit: true,
                    enableSorting: false,
                    type: 'checkbox',
                    cellTemplate:
                        '<div class="ui-grid-cell-contents"><label class="ui-grid-custom-edit-field adv-checkbox-label" data-e2e="switchOnOffLabel"><input type="checkbox" class="adv-checkbox-input" ng-model="MODEL_COL_FIELD " data-e2e="switchOnOffSelect" /><span class="adv-checkbox-emul" data-e2e="switchOnOffInput"></span></label></div>',
                    width: 75,
                },
                {
                    name: 'ArtNo',
                    displayName: $translate.instant('Admin.Js.Product.VendorCode'),
                    enableCellEdit: true,
                    enableSorting: false,
                    width: 150,
                },
                {
                    name: 'SizeId',
                    displayName: $translate.instant('Admin.Js.Product.Size'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><div ng-bind="row.entity[\'Size\'] || \'––––\'"></div></div>',
                    enableSorting: false,
                    enableCellEdit: true,
                    type: 'select',
                    minWidth: 150,
                    uiGridCustomEdit: {
                        customViewValue: 'sizeViewValue',
                        onInit: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            uiGridEditCustom.sizeViewValue = rowEntity['Size'] || '––––';
                        },
                        onChange: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            for (var i = 0, len = ctrl.sizes.length; i < len; i++) {
                                if (ctrl.sizes[i].value === newValue) {
                                    rowEntity['Size'] = ctrl.sizes[i].label;
                                    break;
                                }
                            }
                            uiGridEditCustom.sizeViewValue = rowEntity['Size'] || '––––';
                        },
                        replaceNullable: false,
                        editDropdownOptionsFunction: function () {
                            return (
                                ctrl.sizes ||
                                productService
                                    .getSizes(ctrl.categories && ctrl.categories.length > 0 ? ctrl.categories[0].value : null)
                                    .then(function (result) {
                                        ctrl.sizes = [];
                                        ctrl.sizes.push({
                                            value: '',
                                            label: '––––',
                                        });
                                        ctrl.sizes = ctrl.sizes.concat(result);
                                        return ctrl.sizes;
                                    })
                            );
                        },
                    },
                },
                {
                    name: 'ColorId',
                    displayName: $translate.instant('Admin.Js.Product.Color'),
                    cellTemplate: '<div class="ui-grid-cell-contents"><div ng-bind="row.entity[\'Color\'] || \'––––\'"></div></div>',
                    enableSorting: false,
                    enableCellEdit: true,
                    type: 'select',
                    minWidth: 150,
                    uiGridCustomEdit: {
                        customViewValue: 'colorViewValue',
                        onInit: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            uiGridEditCustom.colorViewValue = rowEntity['Color'] || '––––';
                        },
                        onChange: function (rowEntity, colDef, newValue, uiGridEditCustom) {
                            for (var i = 0, len = ctrl.colors.length; i < len; i++) {
                                if (ctrl.colors[i].value === newValue) {
                                    rowEntity['Color'] = ctrl.colors[i].label;
                                    break;
                                }
                            }
                            uiGridEditCustom.colorViewValue = rowEntity['Color'] || '––––';
                        },
                        replaceNullable: false,
                        editDropdownOptionsFunction: function () {
                            return (
                                ctrl.colors ||
                                productService.getColors().then(function (result) {
                                    ctrl.colors = [];
                                    ctrl.colors.push({
                                        value: '',
                                        label: '––––',
                                    });
                                    ctrl.colors = ctrl.colors.concat(result);
                                    return ctrl.colors;
                                })
                            );
                        },
                    },
                },
                {
                    name: 'BasePrice',
                    displayName: $translate.instant('Admin.Js.Product.Price'),
                    enableCellEdit: true,
                    enableSorting: false,
                    width: 120,
                },
            ];
            if (priceRulesEnabled) {
                for (var i = 0; i < ctrl.priceRules.length; i++) {
                    offersColumnDefs.push({
                        name: 'OfferPriceRules[' + i + '].PriceByRule',
                        displayName: ctrl.priceRules[i],
                        enableCellEdit: true,
                        enableSorting: false,
                        width: 100,
                        uiGridCustomEdit: {
                            replaceNullable: false,
                        },
                    });
                }
            }
            offersColumnDefs = offersColumnDefs.concat([
                {
                    name: 'SupplyPrice',
                    displayName: $translate.instant('Admin.Js.Product.PurchasePrice'),
                    enableCellEdit: true,
                    enableSorting: false,
                    width: 120,
                },
                {
                    name: 'Amount',
                    displayName: $translate.instant('Admin.Js.Product.Amount'),
                    enableCellEdit: true,
                    enableSorting: false,
                    width: 100,
                    cellEditableCondition: function ($scope) {
                        return (
                            $scope.row.entity.UsedWarehouses === 1 ||
                            ($scope.row.entity.UsedWarehouses === 0 && $scope.row.entity.CountWarehouses === 1)
                        );
                    },
                },
            ]);
            if (ctrl.useOfferWeightAndDimensions) {
                offersColumnDefs = offersColumnDefs.concat([
                    {
                        name: 'Weight',
                        displayName: $translate.instant('Admin.Js.Product.Weight'),
                        enableCellEdit: true,
                        enableSorting: false,
                        width: 65,
                    },
                    {
                        name: 'Length',
                        displayName: $translate.instant('Admin.Js.Product.Edit.Length'),
                        enableCellEdit: true,
                        enableSorting: false,
                        width: 75,
                    },
                    {
                        name: 'Width',
                        displayName: $translate.instant('Admin.Js.Product.Edit.Width'),
                        enableCellEdit: true,
                        enableSorting: false,
                        width: 75,
                    },
                    {
                        name: 'Height',
                        displayName: $translate.instant('Admin.Js.Product.Edit.Height'),
                        enableCellEdit: true,
                        enableSorting: false,
                        width: 75,
                    },
                ]);
            }
            if (ctrl.useOfferBarCode) {
                offersColumnDefs = offersColumnDefs.concat([
                    {
                        name: 'BarCode',
                        displayName: $translate.instant('Admin.Js.Product.BarCode'),
                        enableCellEdit: true,
                        enableSorting: false,
                        cellClass: 'ui-grid-custom__cell-overflow',
                        width: 120,
                        uiGridCustomEdit: {
                            replaceNullable: false,
                        },
                    },
                ]);
            }
            offersColumnDefs = offersColumnDefs.concat([
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 105,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents ui-grid-cell-contents--with-icons"><div ng-class="{\'flex middle-xs between-xs\' : grid.appScope.$ctrl.isMobile}">' +
                        '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.showOfferStocks(row.entity); $event.preventDefault();" class="ui-grid-custom-service-icon fas fa-dolly" ng-if="grid.appScope.$ctrl.gridExtendCtrl.availableWarehouses && row.entity.CountWarehouses !== 1"></a>' +
                        '<a title="Ссылка на покупку товара" class="link-invert link-decoration-none ui-grid-custom-service-icon fas fa-shopping-basket" ng-click="grid.appScope.$ctrl.gridExtendCtrl.copyToClipboard(\'' +
                        siteUrl +
                        "/buy/' + row.entity.ArtNo + '/')\"></a>" +
                        '<ui-grid-custom-delete title="Удалить модификацию" url="product/deleteOffer" params="{\'offerId\': row.entity.OfferId }"></ui-grid-custom-delete>' +
                        '</div></div>',
                    /*  '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="product/deleteOffer" params="{\'offerId\': row.entity.OfferId }" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',*/
                },
            ]);
            uiGridCustomConfig.enableHorizontalScrollbar = 1;
            ctrl.gridOffersOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: offersColumnDefs,
                flatEntityAccess: !priceRulesEnabled,
            });
        };
        ctrl.offersShow = function () {
            if (ctrl.gridOffersShowed !== true) {
                ctrl.gridOffersShowed = true;
                ctrl.getOffersValidation();
            }
        };
        ctrl.getOffersValidation = function () {
            $http
                .get('product/getOffersValidation', {
                    params: {
                        productId: ctrl.productId,
                    },
                })
                .then(function (response) {
                    ctrl.offersValidation = response.data.result ? null : response.data.error;
                });
        };
        ctrl.gridOffersOnInit = function (grid) {
            ctrl.gridOffers = grid;
        };
        ctrl.gridOffersUpdate = function () {
            if (ctrl.gridOffers != null) {
                ctrl.gridOffers.fetchData();
            }
            ctrl.getOffersValidation();
            if (ctrl.productPhotos != null) {
                ctrl.productPhotos.load();
            }
        };
        ctrl.updateMainPhoto = function (mainPhoto) {
            ctrl.mainPhotoSrc = mainPhoto != null ? mainPhoto.ImageSrc : '../images/nophoto_middle.png';
        };
        ctrl.initProductPhotos = function (productPhotos) {
            ctrl.productPhotos = productPhotos;
        };
        ctrl.setDiscountType = function (type) {
            if (ctrl.discountType === type) {
                return;
            }
            ctrl.discountType = type;
            if (type === 0) {
                ctrl.DiscountPercent = ctrl.DiscountAmount;
                ctrl.DiscountAmount = 0;
            } else {
                ctrl.DiscountAmount = ctrl.DiscountPercent;
                ctrl.DiscountPercent = 0;
            }
        };
        ctrl.deleteLandingFunnel = function () {
            SweetAlert.confirm('Вы уверены, что хотите удалить воронку?', {
                title: $translate.instant('Admin.Js.Product.Deleting'),
            }).then(function (result) {
                if (result && !result.isDismissed) {
                    $http
                        .post('product/deleteLandingFunnel', {
                            productId: ctrl.productId,
                            landingSiteId: ctrl.landingFunnelId,
                        })
                        .then(function (response) {
                            if (response.data.result === true) {
                                ctrl.landingFunnelLink = null;
                                toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                            } else {
                                toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileDeleting'));
                            }
                        });
                }
            });
        };
        ctrl.onAddLandingFunnelLink = function () {
            ctrl.getLandingFunnelLink().then(function (data) {
                if (data.result === true && data.obj.url != null) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.LandingFunnelCreated'));
                }
            });
        };
        ctrl.getLandingFunnelLink = function () {
            return $http
                .get('product/getLandingFunnelLink', {
                    params: {
                        productId: ctrl.productId,
                    },
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.landingFunnelId = data.obj.id;
                        ctrl.landingFunnelLink = data.obj.url;
                    }
                    return response.data;
                });
        };
        ctrl.onAddExistingLandingFunnel = function (result) {
            $http
                .post('product/setLandingFunnel', {
                    productId: ctrl.productId,
                    landingSiteId: result.id,
                })
                .then(function (response) {
                    if (response.data.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.LandingFunnelCreated'));
                    }
                    ctrl.getLandingFunnelLink();
                });
        };
        ctrl.deleteLinkLandingFunnel = function () {
            SweetAlert.confirm('Вы уверены, что хотите отвязать воронку от продукта?', {
                title: '',
            }).then(function (result) {
                if (result && !result.isDismissed) {
                    $http
                        .post('product/unSetLandingFunnel', {
                            productId: ctrl.productId,
                            landingSiteId: ctrl.landingFunnelId,
                        })
                        .then(function (response) {
                            if (response.data.result === true) {
                                toaster.pop('success', '', 'Воронка отвязана от товара');
                            }
                            ctrl.getLandingFunnelLink();
                        });
                }
            });
        };
        ctrl.setSalesChannelExcludedList = function (channelsList) {
            ctrl.salesChannelExcludedList = channelsList;
        };
        ctrl.loadSalesChannels = function (productId) {
            $http
                .post('product/GetProductSalesChannels', {
                    id: productId,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        if (response.data.obj !== null) {
                            ctrl.allSalesChannelCount = response.data.obj.SalesChannelList.length;
                            ctrl.enableSalesChannelCount = 0;
                            for (var i = 0; i < response.data.obj.SalesChannelList.length; i++) {
                                if (response.data.obj.SalesChannelList[i].Enable) {
                                    ctrl.enableSalesChannelCount++;
                                }
                            }
                        }
                        //ctrl.landingFunnelLink = null;
                        //toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                    } else {
                        //toaster.pop('error', '', $translate.instant('Admin.Js.Product.ErrorWhileDeleting'));
                    }
                });
        };
        ctrl.addTransformerLeftMenu = function (transformer) {
            ctrl.transformer = transformer;
        };
        ctrl.showOfferStocks = function (offer) {
            var offerInfo = '';
            offerInfo += offer.ArtNo;
            offerInfo += (offer.Size ? ', ' : '') + offer.Size;
            offerInfo += (offer.Color ? (offer.Size ? '/' : ', ') : '') + offer.Color;
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalOfferStocksCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: offerStocksTemplate,
                    resolve: {
                        offerId: function () {
                            return offer.OfferId;
                        },
                        offerInfo: function () {
                            return offerInfo;
                        },
                    },
                    backdrop: 'static',
                })
                .result.then(
                    function (result) {
                        ctrl.gridOffersUpdate();
                        return result;
                    },
                    function (result) {
                        return 'cancel';
                    },
                );
        };
        ctrl.copyToClipboard = function (text) {
            let input = document.createElement('input');
            input.setAttribute('value', text);
            input.style.opacity = 0;
            document.body.appendChild(input);
            input.select();
            if (document.execCommand('copy')) {
                toaster.pop('success', '', 'Ссылка скопирована');
            } else {
                toaster.pop('error', '', 'Скопировать ссылку не удалось');
            }
        };

        ctrl.correctMultiplicity = function (value) {
            const temp = parseFloat(value);
            ctrl.Multiplicity = temp;
        };
    };
    ProductCtrl.$inject = [
        '$http',
        'uiGridCustomConfig',
        'toaster',
        'SweetAlert',
        '$window',
        'productService',
        '$document',
        '$timeout',
        '$translate',
        'sidebarMenuService',
        '$location',
        '$uibModal',
    ];
    ng.module('product', [
        'angular-inview',
        'uiGridCustom',
        'productPhotos',
        'productPhotos360',
        'productVideos',
        'productProperties',
        'relatedProducts',
        'productGifts',
        'productReviews',
        'lozadAdv',
        'ngCkeditor',
        'productsSelectvizr',
        'spinbox',
    ]).controller('ProductCtrl', ProductCtrl);
})(window.angular);
