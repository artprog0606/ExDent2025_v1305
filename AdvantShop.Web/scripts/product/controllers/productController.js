import { PubSub } from '../../_common/PubSub/PubSub.js';

/*@ngInject*/
var ProductCtrl = function ($q, $scope, $sce, $timeout, productService, modalService, toaster, $translate, $window, urlHelper, customOptionsService) {
    var ctrl = this;
    var callbackListColorsAndSizes = [];
    const deferPostLoad = $q.defer();
    let lastValueChangesSizeColor = undefined;

    ctrl.$onInit = function () {
        ctrl.productView = 'photo';

        ctrl.Price = {};

        ctrl.picture = {};

        ctrl.dirty = false;

        ctrl.offerSelected = {};

        ctrl.carouselHidden = true;

        productService.addToStorage(ctrl);

        ctrl.isOpenPreviewModal = false;
    };

    ctrl.$onDestroy = function () {
        modalService.destroy('modalProductVideo');
    };

    ctrl.productTabsModeInMobile = function (isAccordion, isMobile) {
        if (isAccordion && isMobile) {
            document.querySelectorAll('.accordion-css__state')[0].checked = true;
        }
    };

    ctrl.getPrice = function () {
        return productService
            .getPrice(
                ctrl.offerSelected.OfferId,
                ctrl.customOptions != null ? ctrl.customOptions.xml : null,
                ctrl.lpBlockId,
                ctrl.offerSelected.AmountBuy,
            )
            .then(function (price) {
                ctrl.Price = price;
                ctrl.Price.PriceString = $sce.trustAsHtml(ctrl.Price.PriceString);
                ctrl.Price.Bonuses = $sce.trustAsHtml(ctrl.Price.Bonuses);

                ctrl.offerSelected.AllowBuyOutOfStockProducts = price.AllowBuyOutOfStockProducts;
                ctrl.offerSelected.IsAvailableForPurchase = price.IsAvailableForPurchase;
                ctrl.offerSelected.IsAvailableForPurchaseOnBuyOneClick = price.IsAvailableForPurchaseOnBuyOneClick;

                return ctrl.Price;
            });
    };

    ctrl.getFirstPaymentPrice = function (price, discount, discountAmount) {
        if (price == null || discount == null || discountAmount == null) {
            return null;
        }
        return productService.getFirstPaymentPrice(price, discount, discountAmount).then(function (firstPaymentPrice) {
            ctrl.FirstPaymentPrice = $sce.trustAsHtml(firstPaymentPrice);
            ctrl.visibilityFirstPaymentButton = firstPaymentPrice != null && firstPaymentPrice.length > 0;
        });
    };

    ctrl.refreshPrice = function () {
        var defer = $q.defer();
        if (ctrl.offerSelected.AmountBuy == null) return;
        ctrl.getPrice()
            .then(function (price) {
                return ctrl.getFirstPaymentPrice(
                    price.PriceOldNumber != null && price.PriceOldNumber != 0 ? price.PriceOldNumber : price.PriceNumber,
                    ctrl.discount,
                    ctrl.discountAmount,
                );
            })
            .then(function () {
                if (ctrl.priceAmountList != null) {
                    ctrl.priceAmountList.update().then(function (data) {
                        defer.resolve(data);
                        return data;
                    });
                } else {
                    defer.resolve();
                }
            })
            .then(function () {
                if (ctrl.shippingVariants != null) {
                    ctrl.shippingVariants.update().then(function (data) {
                        defer.resolve(data);
                        return data;
                    });
                } else {
                    defer.resolve();
                }
                productService.processCallback('refreshPrice');
            });

        return defer.promise;
    };

    ctrl.prepareOffers = function (data) {
        for (var i = 0, len = data.Offers.length; i < len; i++) {
            if (data.Offers[i].Available != null && angular.isString(data.Offers[i].Available) === true) {
                data.Offers[i].Available = $sce.trustAsHtml(data.Offers[i].Available);
            }
        }

        return data;
    };

    ctrl.loadData = function (productId, colorId, sizeId, hiddenPrice, filterPhotosEnable, preventChangeLocation) {
        ctrl.productId = productId;
        ctrl.hiddenPrice = hiddenPrice;
        ctrl.filterPhotosEnable = filterPhotosEnable != null ? filterPhotosEnable : true;
        ctrl.preventChangeLocation = preventChangeLocation === true;

        return productService.getOffers(productId, colorId, sizeId).then(function (data) {
            if (data == null) {
                if (colorId == null) {
                    ctrl.carouselHidden = false;
                }
                return null;
            }

            ctrl.data = ctrl.prepareOffers(data);

            ctrl.offerSelected = productService.findOfferSelected(data.Offers, data.StartOfferIdSelected);

            ctrl.dirty = true;

            ctrl.getColorsViewer()
                .then(function () {
                    if (ctrl.colorsViewer != null && ctrl.offerSelected.Color != null) {
                        ctrl.setColorSelected(ctrl.colorsViewer, ctrl.offerSelected.Color != null ? ctrl.offerSelected.Color.ColorId : null);
                    }

                    return ctrl.data;
                })
                .then(ctrl.getSizesViewer)
                .then(function () {
                    if (ctrl.sizesViewer != null && ctrl.offerSelected.Size != null) {
                        ctrl.setSizeSelected(ctrl.sizesViewer, ctrl.offerSelected.Size.SizeId);
                    }
                    return ctrl.data;
                })
                .then(ctrl.getCarousel)
                .then(function () {
                    if (ctrl.filterPhotosEnable === true && ctrl.carousel != null) {
                        ctrl.filterPhotos(ctrl.offerSelected.Color != null ? ctrl.offerSelected.Color.ColorId : null, ctrl.carousel);
                    }
                    ctrl.carouselHidden = false;
                })
                .finally(() => {
                    ctrl.isPostLoad = true;
                    deferPostLoad.resolve();
                });

            return ctrl.data;
        });
    };

    ctrl.refreshSelectedOffer = function () {
        if (ctrl.offerSelected == null) {
            return false;
        }

        productService
            .getOffers(ctrl.offerSelected.ProductId, ctrl.offerSelected.Color?.ColorId, ctrl.offerSelected.Size?.SizeId)
            .then(function (data) {
                if (data == null || data.Offers == null) {
                    return null;
                }

                ctrl.data = ctrl.prepareOffers(data);
                ctrl.offerSelected = productService.findOfferSelected(ctrl.data.Offers, ctrl.offerSelected.OfferId);

                return ctrl.data;
            });
    };

    ctrl.validate = function () {
        if (ctrl.customOptions == null) {
            return true;
        }

        const { invalidOptions, isValidOptions } = customOptionsService.isValidOptions(ctrl.customOptions.items);
        const form = ctrl.customOptions.customOptionsForm;

        if (form.$invalid || !isValidOptions) {
            form.$setSubmitted();
            form.$setDirty();

            if (invalidOptions.size > 0) {
                invalidOptions.forEach((option) => {
                    let errorText = '';
                    const { MinQuantity, MaxQuantity, InputType, Title } = option;

                    if (MinQuantity != null && MaxQuantity != null && MaxQuantity === MinQuantity) {
                        errorText = `: Выберите ${MaxQuantity} варианта`;
                    } else {
                        const minText = MinQuantity != null ? `значение должно быть от ${MinQuantity}` : '';
                        const maxText = MaxQuantity != null && MaxQuantity !== MinQuantity ? ` до ${MaxQuantity}` : '';
                        errorText = `${minText}${minText && maxText ? ' до ' : ''}${maxText}`;
                    }

                    const errorMsg = InputType === 6 ? `Не выбрано поле ${Title} ${errorText}` : `Неверно заполнено поле ${Title} ${errorText}`;

                    toaster.pop('error', errorMsg);
                });
            } else {
                toaster.pop('error', $translate.instant('Js.Product.InvalidCustomOptions'));
            }

            return false;
        }

        return true;
    };

    //#region compare and wishlist

    ctrl.compareInit = function (compare) {
        ctrl.compare = compare;
    };

    ctrl.wishlistControlInit = function (wishlistControl) {
        ctrl.wishlistControl = wishlistControl;
    };

    //#endregion

    //#region customOptions

    ctrl.customOptionsInitFn = function (customOptions) {
        ctrl.customOptions = customOptions;
    };

    ctrl.customOptionsChange = function (data) {
        if (!ctrl.hiddenPrice) {
            ctrl.refreshPrice().then(() => {
                ctrl.disabledBuyButton = false;
            });
        }
        PubSub.publish('product.customOptions.change', {
            productId: ctrl.productId,
            offerId: ctrl.offerSelected.OfferId,
            items: ctrl.customOptions.items,
        });
    };

    ctrl.beforeCustomOptionsChange = function () {
        if (!ctrl.hiddenPrice) {
            ctrl.disabledBuyButton = true;
        }
    };

    //#endregion

    //#region colors

    ctrl.initColors = function (colorsViewer) {
        ctrl.colorsViewer = colorsViewer;

        if (ctrl.colorsViewerDefer != null) {
            ctrl.colorsViewerDefer.resolve();
            delete ctrl.colorsViewerDefer;
        }
    };

    ctrl.getColorsViewer = function () {
        var defer = $q.defer();

        if (ctrl.colorsExist === true && ctrl.colorsViewer == null) {
            ctrl.colorsViewerDefer = defer;
        } else {
            defer.resolve(ctrl.colorsViewer);
        }

        return defer.promise;
    };

    ctrl.changeColor = function (color) {
        ctrl.colorSelected = color;

        if (ctrl.sizesViewer != null) {
            ctrl.sizeSelected = ctrl.getSizeAvalable(ctrl.data.Offers, ctrl.colorSelected.ColorId, ctrl.sizesViewer.sizes, ctrl.data.AllowPreOrder);
            if (ctrl.preventChangeLocation !== true) {
                urlHelper.setLocationQueryParams(
                    'size',
                    ctrl.sizeSelected != null && ctrl.sizeSelected.SizeId != null ? ctrl.sizeSelected.SizeId : null,
                    true,
                );
            }
        }

        ctrl.offerSelected = productService.getOffer(
            ctrl.data.Offers,
            ctrl.colorSelected.ColorId,
            ctrl.sizeSelected != null && ctrl.sizeSelected.isDisabled === false ? ctrl.sizeSelected.SizeId : null,
            ctrl.data.AllowPreOrder,
        );

        if (!ctrl.hiddenPrice) {
            ctrl.refreshPrice();
        }

        if (ctrl.compare != null) {
            ctrl.compare.checkStatus(ctrl.offerSelected.OfferId);
        }

        if (ctrl.wishlistControl != null) {
            ctrl.wishlistControl.checkStatus(ctrl.offerSelected.OfferId);
        }

        ctrl.setPreviewByColorId(ctrl.colorSelected.ColorId, ctrl.filterPhotosEnable, ctrl.carousel);

        if (ctrl.preventChangeLocation !== true) {
            urlHelper.setLocationQueryParams('color', ctrl.colorSelected.ColorId, true);
        }

        ctrl.processChangeSizeAndColorCallback(ctrl.colorSelected, 'color', { offer: ctrl.offerSelected });

        if (ctrl.sizesViewer != null) {
            ctrl.processChangeSizeAndColorCallback(ctrl.sizeSelected, 'size', { offer: ctrl.offerSelected });
        }
    };

    ctrl.setColorSelected = function (colorsViewer, colorId) {
        for (var i = colorsViewer.colors.length - 1; i >= 0; i--) {
            if (colorsViewer.colors[i].ColorId === colorId) {
                ctrl.colorSelected = colorsViewer.colors[i];
                break;
            }
        }

        ctrl.processChangeSizeAndColorCallback(ctrl.colorSelected, 'color', { offer: ctrl.offerSelected });
    };

    //#endregion

    //#region sizes

    ctrl.initSizes = function (sizesViewer) {
        ctrl.sizesViewer = sizesViewer;

        if (ctrl.sizesViewerDefer != null) {
            ctrl.sizesViewer.sizes = JSON.parse(JSON.stringify(ctrl.sizesViewer.sizes));
            ctrl.sizesViewerDefer.resolve();
            delete ctrl.sizesViewerDefer;
        }
    };

    ctrl.getSizesViewer = function () {
        var defer = $q.defer();

        if (ctrl.sizesExist === true && ctrl.sizesViewer == null) {
            ctrl.sizesViewerDefer = defer;
        } else {
            defer.resolve(ctrl.sizesViewer);
        }

        return defer.promise;
    };

    ctrl.changeSize = function (size) {
        ctrl.sizeSelected = size;

        ctrl.offerSelected = productService.getOffer(
            ctrl.data.Offers,
            ctrl.colorSelected != null ? ctrl.colorSelected.ColorId : 0,
            ctrl.sizeSelected.isDisabled ? null : ctrl.sizeSelected.SizeId,
            ctrl.data.AllowPreOrder,
        );

        if (!ctrl.hiddenPrice) {
            ctrl.refreshPrice();
        }

        if (ctrl.compare != null) {
            ctrl.compare.checkStatus(ctrl.offerSelected.OfferId);
        }

        if (ctrl.wishlistControl != null) {
            ctrl.wishlistControl.checkStatus(ctrl.offerSelected.OfferId);
        }
        if (ctrl.preventChangeLocation !== true) {
            urlHelper.setLocationQueryParams('size', ctrl.sizeSelected.SizeId, true);
        }

        ctrl.processChangeSizeAndColorCallback(ctrl.sizeSelected, 'size', { offer: ctrl.offerSelected });
    };

    ctrl.setSizeSelected = function (sizesViewer, sizeId) {
        for (var i = sizesViewer.sizes.length - 1; i >= 0; i--) {
            if (sizesViewer.sizes[i].SizeId === sizeId) {
                ctrl.sizeSelected = sizesViewer.sizes[i];
                break;
            }
        }

        ctrl.sizeSelected = ctrl.getSizeAvalable(
            ctrl.data.Offers,
            ctrl.colorSelected != null ? ctrl.colorSelected.ColorId : 0,
            ctrl.sizesViewer.sizes,
            ctrl.data.AllowPreOrder,
            true,
        );

        ctrl.processChangeSizeAndColorCallback(ctrl.sizeSelected, 'size', { offer: ctrl.offerSelected });
    };

    ctrl.getSizeAvalable = function (offers, colorId, sizes, allowPreorder, notChangeSelectdSize) {
        var offerItem, sizeSelected, loopCheckStart;

        sizes.forEach(function (item) {
            item.isDisabled = true;
        });

        for (var i = offers.length - 1; i >= 0; i--) {
            offerItem = offers[i];

            if (colorId == null || offerItem.Color == null) {
                loopCheckStart = true;
            } else {
                loopCheckStart = offerItem.Color != null && offerItem.Color.ColorId === colorId;
            }
            if (loopCheckStart === true) {
                for (var s = sizes.length - 1; s >= 0; s--) {
                    if (offerItem.Size.SizeId == sizes[s].SizeId && (allowPreorder === true || offerItem.Amount > 0)) {
                        sizes[s].isDisabled = false;
                        break;
                    }
                }
            }
        }

        if (!notChangeSelectdSize && (ctrl.sizeSelected == null || ctrl.sizeSelected.isDisabled === true)) {
            for (var j = 0, l = sizes.length; j < l; j++) {
                if (sizes[j].isDisabled == null || sizes[j].isDisabled == false) {
                    sizeSelected = sizes[j];
                    break;
                }
            }
        } else {
            sizeSelected = ctrl.sizeSelected;
        }

        return sizeSelected;
    };

    //#endregion

    //#region carousels

    ctrl.addCarousel = function (carousel) {
        ctrl.carousel = carousel;

        if (ctrl.carouselDefer != null) {
            if (ctrl.carousel.options.asNavFor != null) {
                ctrl.carousel.whenAsNavForReady(ctrl.carousel.options.asNavFor, function () {
                    ctrl.carouselDefer.resolve();
                    delete ctrl.carouselDefer;
                });
            } else {
                ctrl.carouselDefer.resolve();
                delete ctrl.carouselDefer;
            }
        }
    };

    ctrl.getCarousel = function () {
        var defer = $q.defer();

        if (ctrl.carouselExist === true && ctrl.carousel == null) {
            ctrl.carouselDefer = defer;
        } else {
            defer.resolve();
        }

        return defer.promise;
    };

    ctrl.carouselItemSelect = function (carousel, item, index) {
        ctrl.setPreview(item.parameters);

        ctrl.updateModalPreview(item.parameters.originalPath);

        if (carousel != null && ctrl.carousel != null && carousel != ctrl.carousel) {
            ctrl.carousel.setItemSelect(index);
        } else if (ctrl.carouselPreview != null && carousel != ctrl.carouselPreview) {
            ctrl.carouselPreview.setItemSelect(index);
        }
    };

    //#endregion

    //#region modal preview

    ctrl.carouselPreviewNext = function () {
        var items = ctrl.carouselPreview.getItems(),
            itemSelected,
            itemSelectedNew,
            newIndex;

        itemSelected = ctrl.carouselPreview.getSelectedItem() || (items != null ? items[0] : null);

        if (ctrl.carouselPreview.getSelectedItem() === items[items.length - 1]) {
            ctrl.carouselPreview.goto(0, false);
            newIndex = 0;
        } else {
            ctrl.carouselPreview.next();
            newIndex = itemSelected.carouselItemData.index + 1;
        }

        if (itemSelected != null) {
            itemSelectedNew = items[newIndex];

            if (itemSelectedNew) {
                ctrl.carouselPreview.setItemSelect(itemSelectedNew);
                ctrl.setPreview(itemSelectedNew.carouselItemData.parameters);
                ctrl.updateModalPreview(itemSelectedNew.carouselItemData.parameters.originalPath);
            }
        }
    };

    ctrl.carouselPreviewPrev = function () {
        var items = ctrl.carouselPreview.getItems(),
            itemSelected,
            itemSelectedNew,
            newIndex;

        itemSelected = ctrl.carouselPreview.getSelectedItem() || (items != null ? items[0] : null);

        if (ctrl.carouselPreview.getSelectedItem() === items[0]) {
            ctrl.carouselPreview.goto(items.length - 1, false);
            newIndex = items.length - 1;
        } else {
            ctrl.carouselPreview.prev();
            newIndex = itemSelected.carouselItemData.index - 1;
        }

        if (itemSelected != null) {
            itemSelectedNew = items[newIndex];

            if (itemSelectedNew) {
                ctrl.carouselPreview.setItemSelect(itemSelectedNew);
                ctrl.setPreview(itemSelectedNew.carouselItemData.parameters);
                ctrl.updateModalPreview(itemSelectedNew.carouselItemData.parameters.originalPath);
            }
        }
    };

    ctrl.addModalPictureCarousel = function (carouselPreview) {
        ctrl.carouselPreview = carouselPreview;
        ctrl.carouselPreviewUpdate();
    };

    ctrl.carouselPreviewUpdate = function () {
        if (ctrl.carouselPreview != null) {
            ctrl.getDialog().then(function (modal) {
                if (modal.modalScope.isOpen === true) {
                    ctrl.filterPreviewCarouselItems();
                    ctrl.carouselPreview.update();
                }
            });
        }
    };

    ctrl.updateModalPreview = function (imgSrc) {
        productService.getPhoto(imgSrc).then(function (img) {
            $timeout(function () {
                ctrl.maxHeightModalPreview = ctrl.getMaxHeightModalPreview();
                ctrl.modalPreviewHeight = img.naturalHeight > ctrl.maxHeightModalPreview ? ctrl.maxHeightModalPreview : img.naturalHeight;
            }, 0);
        });
    };

    ctrl.modalPreviewCallbackOpen = function () {
        ctrl.setPreviewByColorId(
            ctrl.offerSelected.Color != null ? ctrl.offerSelected.Color.ColorId : null,
            ctrl.filterPhotosEnable,
            ctrl.carouselPreview,
        );

        $timeout(function () {
            ctrl.carouselPreviewUpdate();
            ctrl.isOpenPreviewModal = true;
        }, 100);
    };

    ctrl.modalPreviewCallbackClose = function () {
        if (!('ontouchstart' in window)) {
            $window.removeEventListener(`keydown`, ctrl.onKeydownBackForward);
        }
        ctrl.isOpenPreviewModal = false;
    };

    ctrl.modalPreviewOpen = function (event, picture) {
        event.preventDefault();
        event.stopPropagation();

        if (ctrl.isPostLoad !== true) {
            return;
        }

        ctrl.modalPreviewState = 'load';

        ctrl.dialogOpen().then(function (modal) {
            let modalElement = modal.modalElement[0];
            modalElement.classList.add('product-preview-modal-wrap');
            let htmlMain = document.querySelector('html');
            htmlMain.classList.add('overflow-hidden');
            modal.modalScope.callbackClose = function () {
                htmlMain.classList.remove('overflow-hidden');
            };
            productService.getPhoto(picture == null ? ctrl.picture.originalPath : picture.originalPath).then(function (img) {
                $timeout(function () {
                    ctrl.maxHeightModalPreview = ctrl.getMaxHeightModalPreview();
                    ctrl.modalPreviewHeight = img.naturalHeight > ctrl.maxHeightModalPreview ? ctrl.maxHeightModalPreview : img.naturalHeight;
                    //ctrl.carouselPreviewUpdate();
                    ctrl.modalPreviewState = 'complete';
                    if (ctrl.carouselPreview != null) {
                        ctrl.filterPreviewCarouselItems();
                    }
                }, 0);
            });
        });
    };

    ctrl.getMaxHeightModalPreview = function () {
        var result = 0,
            height,
            modalElement,
            modaPreview = document.getElementById('modalPreview_' + ctrl.productId);

        if (modaPreview != null) {
            modalElement = modaPreview.querySelector('.modal-content');
        }
        if (modalElement != null) {
            height = parseFloat(getComputedStyle(modalElement).height);
            result = isNaN(height) === false ? height : 0;
        }

        return result;
    };

    ctrl.dialogOpen = function () {
        return deferPostLoad.promise
            .then(() => ctrl.getDialog())
            .then(function (modal) {
                if (!('ontouchstart' in window)) {
                    $window.addEventListener(`keydown`, ctrl.onKeydownBackForward);
                }
                modal.modalScope.open();
                return modal;
            });
    };

    ctrl.getDialog = function () {
        return modalService.getModal('modalPreview_' + ctrl.productId);
    };

    ctrl.resizeModalPreview = function () {
        $scope.$apply(function () {
            ctrl.updateModalPreview(ctrl.picture.originalPath);
            ctrl.carouselPreviewUpdate();
        });
    };

    //#endregion

    //#region productViewChange

    ctrl.showVideo = function (visible) {
        ctrl.visibleVideo = visible;

        if (visible === false) {
            ctrl.videosInModalReceived = false;
            ctrl.carouselVideosInModalInit = false;
        }
    };

    ctrl.onReceiveVideosInModal = function () {
        ctrl.videosInModalReceived = true;
    };

    ctrl.onInitCarouselVideosInModal = function () {
        ctrl.carouselVideosInModalInit = true;
    };

    ctrl.showRotate = function (visible) {
        ctrl.visibleRotate = visible;
    };

    //#endregion

    //#region shippingVariants
    ctrl.addShippingVariants = function (shippingVariants) {
        ctrl.shippingVariants = shippingVariants;
    };
    //#endregion

    //#region priceAmountList
    ctrl.addPriceAmountList = function (priceAmountList) {
        ctrl.priceAmountList = priceAmountList;
    };
    //#endregion

    //#region spinbox amount
    ctrl.updateAmount = function () {
        ctrl.refreshPrice();
    };
    //#endregion

    ctrl.filterPhotosFunction = function (item, index) {
        return (
            item != null &&
            (item.carouselItemData.parameters.colorId == null ||
                ctrl.offerSelected.Color == null ||
                item.carouselItemData.parameters.colorId == ctrl.offerSelected.Color.ColorId)
        );
    };

    ctrl.setPreviewByColorId = function (colorId, filterEnabled, carousel) {
        var findArray;

        if (ctrl.carousel) {
            if (filterEnabled === true) {
                ctrl.filterPhotos(colorId, carousel, ctrl.picture.PhotoId);
            } else {
                findArray = ctrl.carousel.items.filter(ctrl.filterPhotosFunction);

                if (findArray != null && findArray.length > 0) {
                    ctrl.setPreview(findArray[0].carouselItemData.parameters);
                }
            }
        }
    };

    ctrl.filterPhotos = function (colorId, carousel, selectedPhotoId) {
        var selectedItem, items, oldItem;

        if (carousel) {
            oldItem = carousel.getActiveItem();
            items = carousel.filterItems(ctrl.filterPhotosFunction, colorId);

            /*if (selectedPhotoId != null) {
                items = carousel.getItems();

                for (var i = 0, len = items.length; i < len; i++) {
                    if (items[i].carouselItemData.parameters.colorId === colorId) {
                        carousel.setItemSelect(items[i]);
                        findedSelected = true;
                        break;
                    }
                }
            }*/

            if (items == null || items.length === 0) {
                carousel.addItem(oldItem);
            }

            selectedItem = carousel.getActiveItem();

            if (selectedItem != null) {
                carousel.setItemSelect(selectedItem);
                ctrl.setPreview(selectedItem.carouselItemData.parameters);
            }
        }
    };

    ctrl.setView = function (viewName) {
        ctrl.productView = viewName;

        ctrl.stopVideo();
    };

    ctrl.setPreview = function (picture) {
        ctrl.picture = picture;
    };

    ctrl.getUrl = function (url) {
        var result = url,
            params = [];

        if (ctrl.colorsViewer != null && ctrl.colorsViewer.colorSelected != null) {
            params.push('color=' + ctrl.colorsViewer.colorSelected.ColorId);
        }

        if (ctrl.sizesViewer != null && ctrl.sizesViewer.sizeSelected != null) {
            params.push('size=' + ctrl.sizesViewer.sizeSelected.SizeId);
        }

        if (params.length > 0) {
            result = result + '?' + params.join('&');
        }

        return result;
    };

    ctrl.getCommentsCount = function () {
        productService.getReviewsCount(ctrl.productId).then(function (result) {
            if (result != null) {
                ctrl.reviewsCount = result.reviewsCount;
            }
        });
    };

    ctrl.addChangeSizeAndColorCallback = function (callback) {
        if (callback == null) {
            throw new Error('Parameter "callback is required"');
        }

        callbackListColorsAndSizes.push(callback);

        if (lastValueChangesSizeColor !== undefined) {
            callback(lastValueChangesSizeColor);
        }
    };

    ctrl.processChangeSizeAndColorCallback = function (value, type, additionalData) {
        lastValueChangesSizeColor = value;
        if (callbackListColorsAndSizes.length > 0) {
            callbackListColorsAndSizes.forEach((fn) => fn(value, type, additionalData));
        }
    };

    ctrl.onKeydownBackForward = (e) => {
        if (e.code === `ArrowRight`) {
            ctrl.carouselPreviewNext();
        }
        if (e.code === `ArrowLeft`) {
            ctrl.carouselPreviewPrev();
        }
    };

    ctrl.handleChangeInplaceArtNo = function (value, $scope) {
        ctrl.offerSelected.ArtNo = value;
    };

    ctrl.filterPreviewCarouselItems = function () {
        if (ctrl.carouselPreview != null) {
            if (ctrl.filterPhotosEnable === true && ctrl.picture != null) {
                ctrl.filterPhotos(
                    ctrl.offerSelected.Color != null ? ctrl.offerSelected.Color.ColorId : null,
                    ctrl.carouselPreview,
                    ctrl.picture.PhotoId,
                );
            }
            var items = ctrl.carouselPreview.getItems();
            for (var i = 0; i < items.length; i++) {
                if (items[i].carouselItemData.parameters.PhotoId == ctrl.picture.PhotoId) {
                    ctrl.carouselPreview.setItemSelect(items[i]);
                    ctrl.setPreview(items[i].carouselItemData.parameters);
                    ctrl.updateModalPreview(items[i].carouselItemData.parameters.originalPath);
                    break;
                }
            }
        }
    };

    //модальное окно с магазинами
    const modalId = 'mapShops';
    ctrl.openShopsMap = function (offerId, isMobile, yaMapsKey) {
        modalService.renderModal(
            modalId,
            $translate.instant('Js.Product.ShopsMap.Header'),
            `<product-availability-map data-offer-id="${offerId}" data-mobile-mode="${isMobile}" data-api-key-map="${yaMapsKey}"></product-availability-map>`,
            null,
            { destroyOnClose: true, modalClass: 'warehouses-list-modal' },
        );
        modalService.getModal(modalId).then(function (modal) {
            modal.modalScope.open();
        });
    };
};

export default ProductCtrl;
