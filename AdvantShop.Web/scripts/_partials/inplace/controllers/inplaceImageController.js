var i = 0;

/* @ngInject */
function InplaceImageCtrl($compile, $element, $scope, $timeout, $window, domService, inplaceService, Upload, toaster) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.tagImage = $element[0];

        ctrl.isActive = false;
        ctrl.isHoverButtons = false;

        i += 1;

        ctrl.i = i;
    };

    ctrl.setPositionButtons = function (buttons) {
        buttons.css({
            top: $element[0].offsetTop + $element[0].height,
            left: $element[0].offsetLeft + $element[0].width - buttons[0].offsetWidth,
        });

        ctrl.buttonsAligned = true;
    };

    ctrl.onLoadButtons = function (buttonsCtrl, buttonsElement) {
        ctrl.buttons = buttonsCtrl;
        ctrl.buttonsRendered = true;
    };

    ctrl.active = function () {
        ctrl.isActive = true;
        ctrl.showButtons = true;
    };

    ctrl.fileDrop = function (files, event, rejectedFiles) {
        return ctrl.fileChange(files, event, ctrl.inplaceParams.id != null && ctrl.inplaceParams.id !== 0 ? 'update' : 'add');
    };

    ctrl.fileChange = function (files, event, command) {
        if (command == null) {
            throw Error("Parameter 'command' required for inplace image");
        }

        //in productview
        if (ctrl.productViewItem != null) {
            if (ctrl.productViewItem.picture.PhotoId != null) {
                angular.extend(ctrl.inplaceParams, { id: ctrl.productViewItem.picture.PhotoId });
            }

            if (ctrl.productViewItem.colorSelected != null) {
                angular.extend(ctrl.inplaceParams, { colorId: ctrl.productViewItem.colorSelected.ColorId });
            }
        } else if (ctrl.product != null) {
            //inplaceService.startProgress();

            if (ctrl.product.picture.PhotoId != null && ctrl.inplaceParams.field !== 'Review') {
                angular.extend(ctrl.inplaceParams, { id: ctrl.product.picture.PhotoId });
            }

            if (ctrl.product.colorSelected != null) {
                angular.extend(ctrl.inplaceParams, { colorId: ctrl.product.colorSelected.ColorId });
            }
        }

        ctrl.inplaceParams.command = command;

        return Upload.upload({
            url: ctrl.inplaceUrl,
            data: {
                data: JSON.stringify(angular.extend(ctrl.inplaceParams, { rnd: Math.random() })),
                file: files,
            },
        }).then(function (response) {
            var data = response.data;

            if (data.errors != null) {
                data.errors.forEach(function (err) {
                    toaster.pop('error', '', err);
                });
            } else {
                switch (command) {
                    case 'add':
                        ctrl.addedImage(response.data, ctrl.inplaceParams.field);
                        break;
                    case 'update':
                        ctrl.updatedImage(response.data, ctrl.inplaceParams.field);
                        break;
                    case 'delete':
                        ctrl.deletedImage(response.data, ctrl.inplaceParams.field);
                        break;
                }
            }
        });
        //    .finally(function () {
        //    inplaceService.stopProgress();
        //});
    };

    ctrl.addedImage = function (result, field) {
        var img = ctrl.tagImage,
            carousel,
            clone,
            cloneImg,
            cloneImgParams,
            cloneImgButtons,
            imgButtonsEmpty;

        if (result != null && result.length > 0 && field != null) {
            switch (field) {
                case 'Logo':
                case 'Brand':
                case 'News':
                case 'CategorySmall':
                case 'CategoryBig':
                case 'Review':
                    img.src = result[0].filename;
                    break;
                case 'Product':
                    for (var i = 0, len = result.length; i < len; i++) {
                        img.src = result[i].filename;
                        cloneImgParams = new Function('return ' + img.getAttribute('data-inplace-params'))();
                        cloneImgParams.id = result[i].id;
                        img.setAttribute('data-inplace-params', JSON.stringify(cloneImgParams).replace(/"/g, "'"));

                        if (i === 0) {
                            if (ctrl.productViewItem != null) {
                                ctrl.productViewItem.picture.PhotoId = cloneImgParams.id;
                                ctrl.productViewItem.clearPhotos();
                            } else if (ctrl.product != null) {
                                $window.location.reload(true);
                            }
                        }
                    }

                    break;
                case 'Carousel':
                    carousel = ctrl.carousel;

                    clone = carousel.carouselNative.getActiveItem().cloneNode(true);
                    cloneImg = clone.querySelector('[data-inplace-image]');

                    clone.querySelector('.inplace-buttons').parentNode.removeChild(clone.querySelector('.inplace-buttons'));

                    //#region edit inplace params
                    cloneImgParams = new Function('return ' + cloneImg.getAttribute('data-inplace-params'))();
                    cloneImgParams.id = result[0].id;
                    cloneImg.id = 'inplaceImage_' + result[0].id;
                    cloneImg.setAttribute('data-inplace-params', JSON.stringify(cloneImgParams).replace(/"/g, "'"));
                    //#endregion

                    //#region edit inplace image buttons
                    cloneImgButtons = new Function('return ' + cloneImg.getAttribute('data-inplace-image-buttons-visible'))();
                    cloneImgButtons.add = true;
                    cloneImgButtons.update = true;
                    cloneImgButtons.delete = true;
                    cloneImgButtons.permanentVisible = false;
                    cloneImg.setAttribute('data-inplace-image-buttons-visible', JSON.stringify(cloneImgButtons).replace(/"/g, "'"));
                    //#endregion

                    cloneImg.src = result[0].filename;

                    //remove special slide for inplace
                    if (carousel.carouselNative.items.length === 1) {
                        imgButtonsEmpty = new Function(
                            'return ' +
                                carousel.carouselNative
                                    .getActiveItem()
                                    .querySelector('[data-inplace-image]')
                                    .getAttribute('data-inplace-image-buttons-visible'),
                        )();

                        if (imgButtonsEmpty.add === true && imgButtonsEmpty.update === false && imgButtonsEmpty.delete === false) {
                            carousel.carouselNative.removeItem(carousel.carouselNative.getActiveItem(), false);
                        }
                    }
                    var indexNewSlide = carousel.carouselNative.options.indexActive + 1;
                    carousel.carouselNative.addItem(clone, indexNewSlide);

                    if (!cloneImg.complete || typeof cloneImg.naturalWidth !== 'undefined' || cloneImg.naturalWidth === 0) {
                        cloneImg.onload = function () {
                            carousel.carouselNative.update();
                            carousel.carouselNative.goto(carousel.carouselNative.items.length > 0 ? indexNewSlide : 0);
                        };
                    } else {
                        carousel.carouselNative.update();
                        carousel.carouselNative.goto(carousel.carouselNative.items.length > 0 ? indexNewSlide : 0);
                    }

                    $compile(cloneImg)($scope);
                    break;
                default:
                    throw Error('Unknow type for inplace image: ' + field);
            }
        }
    };

    ctrl.updatedImage = function (result, field) {
        var img = ctrl.tagImage,
            cloneImgParams;

        if (result != null && result.length > 0 && field != null) {
            switch (field) {
                case 'Logo':
                case 'Brand':
                case 'News':
                case 'CategorySmall':
                case 'CategoryBig':
                    img.src = result[0].filename;
                    break;
                case 'Carousel':
                    img.src = result[0].filename;
                    ctrl.carousel.carouselNative.update();
                    break;
                case 'Product':
                    img.src = result[0].filename;
                    cloneImgParams = new Function('return ' + img.getAttribute('data-inplace-params'))();
                    cloneImgParams.id = result[0].id;
                    img.setAttribute('data-inplace-params', JSON.stringify(cloneImgParams).replace(/"/g, "'"));

                    if (ctrl.productViewItem != null) {
                        ctrl.productViewItem.picture.PhotoId = cloneImgParams.id;
                        ctrl.productViewItem.clearPhotos();
                    } else if (ctrl.product != null) {
                        $window.location.reload();
                    }

                    break;
                case 'Review':
                    img.src = result[0].filename;

                    //cloneImgParams = (new Function('return ' + img.getAttribute('data-inplace-params')))();
                    //cloneImgParams.id = result[0].id;
                    //img.setAttribute('data-inplace-params', JSON.stringify(cloneImgParams).replace(/"/g, '\''));
                    ctrl.inplaceParams.id = result[0].id;
                    break;
                default:
                    throw Error('Unknow type for inplace image: ' + field);
            }
        }
    };

    ctrl.deletedImage = function (result, field) {
        var img = ctrl.tagImage,
            carousel,
            itemIndex,
            clone,
            cloneImg,
            cloneImgParams,
            cloneImgButtons;

        if (result != null && result.length > 0 && field != null) {
            switch (field) {
                case 'Logo':
                case 'Brand':
                case 'News':
                case 'CategorySmall':
                case 'CategoryBig':
                    img.src = result[0].filename;
                    break;
                case 'Product':
                    img.src = result[0].filename;
                    cloneImgParams = new Function('return ' + img.getAttribute('data-inplace-params'))();
                    cloneImgParams.id = result[0].id;
                    img.setAttribute('data-inplace-params', JSON.stringify(cloneImgParams).replace(/"/g, "'"));

                    if (ctrl.productViewItem != null) {
                        ctrl.productViewItem.picture.PhotoId = cloneImgParams.id;
                        ctrl.productViewItem.clearPhotos();
                    } else if (ctrl.product != null) {
                        $window.location.reload();
                    }

                    break;
                case 'Review':
                    img.src = result[0].filename;

                    //cloneImgParams = (new Function('return ' + img.getAttribute('data-inplace-params')))();
                    //cloneImgParams.id = result[0].id;
                    //img.setAttribute('data-inplace-params', JSON.stringify(cloneImgParams).replace(/"/g, '\''));
                    ctrl.inplaceParams.id = result[0].id;
                    break;
                case 'Carousel':
                    carousel = ctrl.carousel;

                    if (carousel != null) {
                        //clone element which will deleted
                        if (carousel.carouselNative.items.length === 1) {
                            clone = carousel.carouselNative.getActiveItem().cloneNode(true);
                            cloneImg = clone.querySelector('[data-inplace-image]');

                            clone.querySelector('.inplace-buttons').parentNode.removeChild(clone.querySelector('.inplace-buttons'));

                            //#region edit inplace params
                            cloneImgParams = new Function('return ' + cloneImg.getAttribute('data-inplace-params'))();
                            cloneImgParams.id = 0;
                            cloneImg.setAttribute('data-inplace-params', JSON.stringify(cloneImgParams).replace(/"/g, "'"));
                            //#endregion

                            //#region edit inplace image buttons
                            cloneImgButtons = new Function('return ' + cloneImg.getAttribute('data-inplace-image-buttons-visible'))();
                            cloneImgButtons.update = false;
                            cloneImgButtons.delete = false;
                            cloneImgButtons.permanentVisible = true;
                            cloneImg.setAttribute('data-inplace-image-buttons-visible', JSON.stringify(cloneImgButtons).replace(/"/g, "'"));
                            //#endregion

                            cloneImg.src = result[0].filename;
                        }

                        itemIndex = domService.closest(img, '.js-carousel-item').carouselItemData.index;
                        carousel.carouselNative.removeItem(carousel.carouselNative.items[itemIndex], false);

                        if (clone != null) {
                            carousel.carouselNative.addItem(clone);
                            $compile(cloneImg)($scope);
                        }

                        if (cloneImg != null && (!cloneImg.complete || typeof cloneImg.naturalWidth !== 'undefined' || cloneImg.naturalWidth === 0)) {
                            cloneImg.onload = function () {
                                carousel.carouselNative.update();
                                carousel.carouselNative.goto(carousel.carouselNative.items.length - 1);
                            };
                        } else {
                            carousel.carouselNative.update();
                            carousel.carouselNative.goto(carousel.carouselNative.items.length - 1);
                        }
                    }
                    break;
                default:
                    throw Error('Unknow type for inplace image: ' + field);
            }
        }
    };
}

export default InplaceImageCtrl;
