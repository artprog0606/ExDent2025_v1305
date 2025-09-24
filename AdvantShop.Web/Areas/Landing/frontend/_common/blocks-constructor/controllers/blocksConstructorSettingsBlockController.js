(function (ng) {
    'use strict';

    /* @ngInject */
    var BlocksConstructorSettingsBlockCtrl = function (
        $window,
        blocksConstructorService,
        blocksConstructorPaddingTop,
        blocksConstructorPaddingBottom,
        toaster,
        modalService,
        lpSettingsService,
        SweetAlert,
        inplaceRichConfig,
        $timeout,
        $http,
        uiGridCustomConfig,
        $translate,
        $filter,
        domService,
    ) {
        var ctrl = this;
        //var BACKGROUND_FIXED = 'background-attachment-fixed',
        //    BACKGROUND_SCROLL = 'background-attachment-scroll';

        ctrl.$onInit = function () {
            ctrl.isMobile = window.matchMedia('(max-width: 768px)').matches;

            ctrl.ckeditorOptionsSlim = ng.extend({}, inplaceRichConfig, $window._LandingCKeditorConfig, { height: 50 });

            ctrl.positionTypes = [
                {
                    label: 'По-умолчанию',
                    value: null,
                },
                {
                    label: 'Фиксировать при прокрутке страницы',
                    value: 'fixedOnScroll',
                },
                {
                    label: 'Наложение на соседний блок',
                    value: 'runDown',
                },
            ];

            ctrl.pictureMaxSize = {
                xSmallWidth: 100,
                xSmallHeight: 100,

                smallWidth: 480,
                smallHeight: 480,

                middleWidth: 1024,
                middleHeight: 768,

                largeWidth: 1600,
                largeHeight: 1200,

                wallWidth: 1920,
                wallHeight: 1080,
            };

            ctrl.settingsMaster = ng.copy(ctrl.modalData.settings);

            if (ctrl.modalData.settings.background_settings) {
                ctrl.backgroundSettings =
                    ctrl.modalData.settings.background_settings.background_fixed == true
                        ? 'background_fixed'
                        : ctrl.modalData.settings.background_settings.parallax == true
                          ? 'parallax'
                          : '';
            } else {
                //для старых версий, у которых настройки находятся не в объекте
                ctrl.modalData.settings.background_settings = {
                    background_fixed: false,
                    parallax: false,
                };
            }

            ctrl.listPadding = {
                top: blocksConstructorPaddingTop,
                bottom: blocksConstructorPaddingBottom,
            };

            if (ctrl.modalData.settings.style != null) {
                ctrl.customColor = ctrl.modalData.settings.style['color'];
            }

            ctrl.textColorPickerOptions = {
                required: true,
                format: 'rgb',
                case: 'lower',
                preserveInputFormat: true,
                swatch: true,
                swatchBootstrap: false,
                swatchOnly: false,
                inputClass: 'blocks-constructor-input',
            };

            ctrl.textColorPickerEventApi = {
                onChange: function (api, color, $event) {
                    ctrl.changeTextColorPicker($event, color);
                },
            };

            ctrl.rangeSlider = {
                min: 0,
                max: 1,
                step: 0.1,
                change: function (event, modelMin, modelMax) {
                    ctrl.darkenBackgroundImage(modelMax);
                },
                changeItems: function (event, modelMin, modelMax) {
                    ctrl.darkenBackgroundItems(modelMax);
                },
            };

            ctrl.rangeSliderProductDarken = {
                min: 0,
                max: 1,
                step: 0.1,
                change: function (event, modelMin, modelMax) {
                    ctrl.darkenBackgroundProduct(modelMax);
                },
            };

            ctrl.rangeSliderPadding = {
                min: 0,
                max: 375,
                step: 5,
                changeTop: function (event, modelMin, modelMax) {
                    ctrl.changePadding('top', modelMax);
                },
                changeBottom: function (event, modelMin, modelMax) {
                    ctrl.changePadding('bottom', modelMax);
                },
            };

            var findValue;

            ctrl.positionTypes.forEach(function (item) {
                if (ctrl.modalData.settings[item.value] === true) {
                    findValue = item.value;
                }
            });

            ctrl.headerSettingsPosition = findValue || null;

            ctrl.rangeMenuItemsSpacing = {
                step: 1,
                max: 50,
                min: 0,
                change: function (event, modelMin, modelMax, subblock) {
                    subblock.Settings.style['padding-left'] = modelMax + 'px';
                    subblock.Settings.style['padding-right'] = modelMax + 'px';
                },
            };

            ctrl.modalData.settings.darken = ctrl.modalData.settings.darken !== undefined ? ctrl.modalData.settings.darken || 0 : undefined;

            ctrl.colorPickerOptions = {
                swatchBootstrap: false,
                format: 'rgb',
                alpha: true,
                case: 'lower',
                swatchOnly: false,
                allowEmpty: true,
                required: false,
                preserveInputFormat: false,
                restrictToFormat: false,
                inputClass: 'blocks-constructor-input',
            };

            ctrl.colorPickerEventApi = {
                onChange: function (colorPicker, value, event) {
                    return $timeout(function () {
                        colorPicker.getScope().AngularColorPickerController.setNgModel(value);
                        return colorPicker;
                    });
                },
            };

            ctrl.hiddenClasses = {
                mobile: ['hidden-device-mobile'],
                desktop: ['hidden-device-desktop'],
            };

            ctrl.colorSchemeOld = ctrl.modalData.settings.color_scheme;
        };

        ctrl.changeColorScheme = function (colorScheme) {
            var index = ctrl.modalData.settings.classes.indexOf(ctrl.colorSchemeOld);

            if (index !== -1) {
                ctrl.modalData.settings.classes.splice(index, 1);
            }

            ctrl.modalData.settings.background_color = null;
            ctrl.modalData.settings.style['background-color'] = null;

            ctrl.modalData.settings.color = null;
            ctrl.modalData.settings.style['color'] = null;

            ctrl.modalData.settings.classes.push(colorScheme);

            if (colorScheme === 'color-scheme--custom' && ctrl.modalData.settings.color_scheme_custom == null) {
                return lpSettingsService.get(ctrl.modalData.landingpageId).then(function (data) {
                    return (ctrl.modalData.settings.color_scheme_custom = data.ColorSchemes[0]);
                });
            }

            ctrl.colorSchemeOld = colorScheme;
        };

        ctrl.changeTextColorPicker = function ($event, color) {
            ctrl.modalData.settings.style['color'] = color;
            ctrl.modalData.settings.color = color;
        };

        ctrl.darkenBackgroundImage = function (value) {
            if ((ctrl.modalData.settings['background_image'] != null && value > 0) || ctrl.modalData.settings['darken'] != null) {
                ctrl.modalData.settings.style['box-shadow'] = '200vh 200vw rgba(0,0,0, {alfa}) inset'.replace(/{alfa}/g, value);
            } else {
                delete ctrl.modalData.settings.style['box-shadow'];
            }
        };

        ctrl.darkenBackgroundItems = function (value) {
            if (value > 0 || ctrl.modalData.settings['darkenItems'] != null) {
                ctrl.modalData.settings.itemsStyle['box-shadow'] = '200vh 200vw rgba(0,0,0, {alfa}) inset'.replace(/{alfa}/g, value);
            } else {
                delete ctrl.modalData.settings.itemsStyle['box-shadow'];
            }
        };
        ctrl.darkenBackgroundProduct = function (value) {
            if (ctrl.modalData.settings['background_product_darken'] != null) {
                ctrl.modalData.settings.product_darken_style['box-shadow'] = '200vh 200vw rgba(0,0,0, {alfa}) inset'.replace(/{alfa}/g, value);
            } else {
                delete ctrl.modalData.settings.product_darken_style['box-shadow'];
            }
        };
        ctrl.changeBackgroundSettings = function (value) {
            switch (value) {
                case 'parallax':
                    ctrl.enableParallax();
                    break;

                case 'background_fixed':
                    ctrl.modalData.settings.background_settings.background_fixed = true;
                    ctrl.fixedBackground();
                    break;

                default:
                    ctrl.modalData.settings.background_settings.parallax = false;
                    ctrl.modalData.settings.background_settings.background_fixed = false;
                    //ctrl.modalData.settings.style['background-attachment'] = 'scroll';
                    //ctrl.modalData.settings.style['background-position'] = 'center center';
                    //ctrl.modalData.settings.style['background-size'] = 'cover';
                    //ctrl.clearBGClasses();
                    //ctrl.modalData.settings.classes.push(BACKGROUND_SCROLL);

                    ctrl.disableParallax();
                    break;
            }
        };

        ctrl.onBackgroundLazyLoadChange = function (result) {
            ctrl.modalData.settings.background_settings.background_lazy_load_enabled = result;
        };

        ctrl.fixedBackground = function () {
            //ctrl.modalData.settings.style['background-attachment'] = ctrl.modalData.settings.background_settings.background_fixed ? 'fixed' : 'scroll';
            //ctrl.modalData.settings.style['background-size'] = 'cover';
            //ctrl.modalData.settings.style['background-position'] = 'center center';
            //ctrl.clearBGClasses();
            //ctrl.modalData.settings.classes.push(ctrl.modalData.settings.background_settings.background_fixed ? BACKGROUND_FIXED : BACKGROUND_SCROLL);

            if (ctrl.modalData.settings.background_settings.parallax) {
                ctrl.disableParallax();
            }
        };

        //ctrl.clearBGClasses = function () {
        //    ctrl.modalData.settings.classes = ctrl.modalData.settings.classes.filter(function (cssClass) {
        //        return cssClass !== BACKGROUND_FIXED && cssClass !== BACKGROUND_SCROLL;
        //    });
        //};

        ctrl.enableParallax = function () {
            ctrl.modalData.settings.background_settings.parallax = true;
            ctrl.modalData.settings.background_settings.background_fixed = false;
            //ctrl.modalData.settings.style['background-attachment'] = 'scroll';
            ctrl.clearBGClasses();
            //ctrl.modalData.settings.classes.push(BACKGROUND_SCROLL);
        };

        ctrl.applyStyleParallax = function (styles) {
            ctrl.modalData.settings.style['background-position'] = 'center ' + styles + 'px';
        };

        ctrl.disableParallax = function () {
            ctrl.modalData.settings.background_settings.parallax = false;
            //$('#block_' + ctrl.modalData.blockId).enllax('destroy');
        };

        ctrl.changePadding = function (type, value) {
            if (ctrl.modalData.settings.padding_inline === true) {
                ctrl.modalData.settings.style['padding-' + type] = value + 'px';
            } else {
                var keysPaddings = Object.keys(ctrl.listPadding[type]);

                ctrl.modalData.settings.classes = ctrl.modalData.settings.classes.filter(function (className) {
                    return !keysPaddings.some(function (paddingItem) {
                        return className === ctrl.listPadding[type][paddingItem];
                    });
                });

                ctrl.modalData.settings['padding_' + type] = value;
                ctrl.modalData.settings.classes.push(ctrl.listPadding[type][value]);
            }
        };

        ctrl.selectBlock = function (item, blockSelected) {
            item.href = '#' + blockSelected.id;
        };

        //#region add category

        ctrl.addCategory = function () {
            var modalId = 'modalAddCategory_' + ctrl.blockId;

            ctrl.modalAddCategoryData = {};

            ctrl.modalAddCategoryData.data = {
                modalData: {
                    settings: ctrl.modalData.settings,
                    apply: function (modalData) {
                        var callback = ctrl.modalAddCategoryData.data.modalData.blocksConstructorAddCategoryCtrl.apply;
                        if (callback) {
                            callback(ctrl.modalAddCategoryData.data.modalData).then(function (categories) {
                                if (categories != null) {
                                    ctrl.modalData.settings.categories.length = 0;
                                    ctrl.modalData.settings.categories = ctrl.modalData.settings.categories.concat(categories);
                                }
                                modalService.close(modalId);
                            });
                        }
                    },
                    onInit: function (blocksConstructorAddCategory) {
                        ctrl.modalAddCategoryData.data.modalData.blocksConstructorAddCategoryCtrl = blocksConstructorAddCategory;
                    },
                    templateUrlByType: 'areas/landing/frontend/_common/blocks-constructor/templates/subblock/categories.html',
                },
            };

            modalService.renderModal(
                modalId,
                'Выбор категории',
                '<blocks-constructor-modal-add-subblock on-init="modalData.onInit(blocksConstructorSubblock)" data-modal-data="modalData" />',
                '<div class="blocks-constructor-modal-categories-buttons">' +
                    '<input type="button" class="blocks-constructor-btn-confirm" data-ng-click="modalData.apply(modalData)" value="Добавить" />' +
                    '<input type="button" class="blocks-constructor-btn-cancel" data-modal-close="" data-modal-close-callback="$ctrl.onCancel()" value="Отмена" />' +
                    '</div>',
                {
                    modalClass: 'blocks-constructor-modal modal-container-fluid-p-n',
                    modalOverlayClass: 'blocks-constructor-modal-floating-wrap',
                    isFloating: false,
                    backgroundEnable: true,
                    destroyOnClose: true,
                },
                ctrl.modalAddCategoryData.data,
            );

            modalService.getModal(modalId).then(function (modal) {
                modal.modalScope.open();

                ctrl.modalAddCategoryData.backup = ng.copy(ctrl.modalAddCategoryData.data);
            });
        };

        //#endregion end add category

        ctrl.getIndexSubblockByName = function (name) {
            return blocksConstructorService.getIndexSubblockByName(ctrl.modalData.data.Subblocks, name);
        };

        ctrl.changeHiddenOnDevice = function (deviceType, isHidden) {
            var classesSelected = ctrl.hiddenClasses[deviceType];

            if (isHidden === true) {
                ctrl.modalData.settings.classes = ctrl.modalData.settings.classes.concat(classesSelected);
            } else {
                ctrl.hiddenClasses[deviceType].forEach(function (classItem) {
                    var index = ctrl.modalData.settings.classes.indexOf(classItem);

                    if (index !== -1) {
                        ctrl.modalData.settings.classes.splice(index, 1);
                    }
                });
            }
        };

        //#region background

        ctrl.onUpdateFileBackground = function (result) {
            ctrl.modalData.settings.background_image = result.picture;

            if (result.picture == null) {
                delete ctrl.modalData.settings.style['background-image'];
                ctrl.modalData.settings.darken = 0;
                ctrl.darkenBackgroundImage(0);
            } else {
                ctrl.modalData.settings.style['background-image'] = "url('" + result.picture + "')";
            }

            return result;
        };

        ctrl.onApplyFileBackground = function () {
            blocksConstructorService.saveBlockSettings(ctrl.modalData.blockId, ctrl.modalData.data);
        };

        ctrl.onUpdateBackground = function (value, type) {
            if (type === 'color') {
                ctrl.modalData.settings.background_color = value;
                ctrl.modalData.settings.style['background-color'] = value;
            }
            //else if (type === 'gradient') {
            //    ctrl.modalData.settings.background_image = value;
            //    ctrl.modalData.settings.style['background-image'] = value;
            //}
        };

        ctrl.pictureLoaderSave = function (pictureLoader, saveFn, base64String) {
            ctrl.modalData.pictureLoaderSaveFn = saveFn;

            return ctrl.onUpdateFileBackground({ picture: base64String });
        };
        //#endregion

        ctrl.onUpdateColor = function (value) {
            ctrl.modalData.settings.color = value;
            ctrl.modalData.settings.style['color'] = value;
        };

        ctrl.convertToHtmlBlock = function () {
            SweetAlert.alert('Вы уверены что хотите конвертировать данный блок в HTML? Изменения нельзя будет вернуть обратно.', {
                title: '',
                showCancelButton: true,
            })
                .then(function (result) {
                    if (result === true || result.value === true) {
                        blocksConstructorService.convertToHtmlBlock(ctrl.modalData.blockId).then(function () {
                            $window.location.reload(true);
                        });
                    }
                })
                .catch(function (result) {
                    //не удалять Possibly unhandled rejection
                });
        };

        ctrl.copyBlock = function () {
            blocksConstructorService.copyBlock(ctrl.modalData.blockId).then(function () {
                $window.location.reload(true);
            });
        };

        ctrl.tryUpdatelBlock = function () {
            SweetAlert.alert('Вы уверены что хотите обновить блок?', { title: '', showCancelButton: true })
                .then(function (result) {
                    if (result === true || result.value === true) {
                        blocksConstructorService.tryUpdatelBlock(ctrl.modalData.blockId).then(function () {
                            $window.location.reload(true);
                        });
                    }
                })
                .catch(function (result) {
                    //не удалять Possibly unhandled rejection
                });
        };

        ctrl.showModalColorSchemeCustom = function (colorSchemeCustom) {
            var master = ng.copy(colorSchemeCustom);

            var modalId = 'modalSettingsBlock_colorSchemeCustom';

            modalService.renderModal(
                modalId,
                'Пользовательская цветовая схема',
                '<color-scheme-settings settings="colorSchemeCustom" on-update-background="onUpdateBackground(cssString, type)" on-update-color="onUpdateColor(cssString)"></color-scheme-settings>',
                '<input type="button" class="blocks-constructor-btn-confirm" data-ng-click="onApply()" data-e2e="SaveCustomColorScheme" value="Сохранить" /><input type="button" class="blocks-constructor-btn-cancel blocks-constructor-btn-mar" data-modal-close="" data-modal-close-callback="onCancel()" data-e2e="CancelCustomColorScheme" value="Отмена" />',
                {
                    modalClass: 'blocks-constructor-modal',
                    modalOverlayClass: 'blocks-constructor-modal-floating-wrap blocks-constructor-modal--settings',
                    isFloating: true,
                    backgroundEnable: false,
                    destroyOnClose: true,
                },
                {
                    colorSchemeCustom: colorSchemeCustom,
                    onUpdateBackground: ctrl.onUpdateBackground,
                    onUpdateColor: ctrl.onUpdateColor,
                    onApply: function () {
                        blocksConstructorService.saveBlockSettings(ctrl.modalData.blockId, ctrl.modalData.data).then(function () {
                            $window.location.reload(true);
                        });
                    },
                    onCancel: function () {
                        colorSchemeCustom = ng.extend(colorSchemeCustom, master);
                        ctrl.onUpdateBackground(colorSchemeCustom.BackgroundColor, 'color');
                        ctrl.onUpdateColor(colorSchemeCustom.TextColor);
                    },
                },
            );

            modalService.getModal(modalId).then(function (modal) {
                modal.modalScope.open();
            });
        };

        /*
        ctrl.getProductNameByOfferId = function (offerId, setPrice) {
            if (offerId == null || offerId.length == 0 || offerId == 0) {
                return;
            }

            $http.get('adminv3/product/getProductNameByOfferId', { params: { offerId: offerId } }).then(function (response) {
                var data = response.data;
                if (data != null) {
                    ctrl.productId = data.ProductId;
                    ctrl.productName = "[" + data.ArtNo + "] " + data.Name;
                    ctrl.productEnabled = data.Enabled;
                }
            });
        }
        */

        ctrl.selectOffer = function (formOptions) {
            var parentData = {
                modalData: {
                    selectvizrGridOptions: ng.extend({}, uiGridCustomConfig, {
                        columnDefs: [
                            {
                                name: 'ArtNo',
                                displayName: $translate.instant('Admin.Js.OfferSelect.VendorCode'),
                                width: 100,
                                enableSorting: false,
                            },
                            {
                                name: 'Name',
                                displayName: $translate.instant('Admin.Js.OffersSelect.Name'),
                                enableSorting: false,
                            },
                            {
                                name: 'ColorName',
                                displayName: $translate.instant('Admin.Js.OffersSelect.Color'),
                                width: 100,
                                enableSorting: false,
                            },
                            {
                                name: 'SizeName',
                                displayName: $translate.instant('Admin.Js.OffersSelect.Size'),
                                width: 100,
                                enableSorting: false,
                            },
                            {
                                name: 'PriceFormatted',
                                displayName: $translate.instant('Admin.Js.OffersSelect.Price'),
                                width: 120,
                                enableSorting: false,
                            },
                        ],

                        showTreeExpandNoChildren: false,
                        uiGridCustom: {
                            rowClick: function ($event, row, grid) {
                                if (
                                    row.treeNode.children &&
                                    row.treeNode.children.length > 0 &&
                                    domService.closest($event.target, '.ui-grid-tree-base-row-header-buttons') == null
                                ) {
                                    grid.gridApi.treeBase.toggleRowTreeState(row);
                                }
                            },
                            rowClasses: function (row) {
                                var classes = [];

                                if (row.treeNode.children == null || row.treeNode.children.length === 0) {
                                    classes.push('ui-grid-custom-prevent-pointer');
                                }

                                if (!row.entity.Enabled) {
                                    classes.push('ui-grid-custom-disabled-item-row');
                                }

                                return classes.join(' ');

                                //return row.treeNode.children == null || row.treeNode.children.length === 0 ? 'ui-grid-custom-prevent-pointer' : '';
                            },
                        },
                    }),
                    gridOnFetch: function (grid) {
                        if (grid != null && grid.gridOptions != null && grid.gridOptions.data != null && grid.gridOptions.data.length > 0) {
                            for (var i = 0, len = grid.gridOptions.data.length; i < len; i++) {
                                if (grid.gridOptions.data[i].Main === true) {
                                    grid.gridOptions.data[i].$$treeLevel = 0;
                                }
                            }
                        }
                    },
                    onChange: function (categoryId, ids, selectMode) {
                        ctrl.data = {
                            categoryId: categoryId,
                            ids: ids,
                            selectMode: selectMode,
                        };
                    },
                    select: async function () {
                        if (ctrl.data.selectMode == 'all') {
                            $http
                                .get('adminv3/catalog/getCatalogOfferIds', { params: ctrl.data })
                                .then(function (response) {
                                    if (response.data != null) {
                                        ctrl.data.selectMode = 'none';
                                        ctrl.data.ids = response.data.ids.filter(function (item) {
                                            return ctrl.data.ids.indexOf(item) === -1;
                                        });
                                    }
                                    formOptions.OfferId = null;
                                    formOptions.OfferItems = ctrl.getOfferItems(formOptions.OfferItems, ctrl.data.ids);
                                    return ctrl.getOfferItemsPrepared(formOptions);
                                })
                                .then((data) => {
                                    modalService.close('modalSelectOffer');
                                    return ctrl.data;
                                });
                        } else {
                            formOptions.OfferId = null;
                            formOptions.OfferItems = ctrl.getOfferItems(formOptions.OfferItems, ctrl.data.ids);
                            await ctrl.getOfferItemsPrepared(formOptions);
                            modalService.close('modalSelectOffer');
                            return ctrl.data;
                        }
                    },
                },
            };

            modalService.renderModal(
                'modalSelectOffer',
                'Выберите товар',
                '<div class="blocks-constructor-settings-col--vertical"><offers-selectvizr selectvizr-tree-url="\'adminv3/catalog/categoriestree\'" ' +
                    'selectvizr-grid-url="\'adminv3/catalog/getOffersCatalog\'" ' +
                    'selectvizr-grid-options="modalData.selectvizrGridOptions" ' +
                    'selectvizr-grid-on-fetch="modalData.gridOnFetch(grid)" ' +
                    'selectvizr-on-change="modalData.onChange(categoryId, ids, selectMode)"' +
                    '>' +
                    '</offers-selectvizr></div>',
                '<input type="button" class="blocks-constructor-btn-confirm" data-ng-click="modalData.select()" value="Применить" />' +
                    '<input type="button" class="blocks-constructor-btn-cancel blocks-constructor-btn-mar" data-modal-close="modalSelectOffer" value="Отмена" />',
                {
                    modalClass: 'blocks-constructor-modal',
                    modalOverlayClass: 'blocks-constructor-modal-floating-wrap blocks-constructor-modal--settings',
                    isFloating: true,
                    backgroundEnable: false,
                    destroyOnClose: true,
                },
                parentData,
            );

            modalService.getModal('modalSelectOffer').then(function (modal) {
                modal.modalScope.open();
            });
        };

        ctrl.getOfferItems = function (items, ids) {
            items = items || [];
            var res = items;
            for (var i = 0; i < ids.length; i++) {
                var offerId = ids[i];
                var finded = items.filter(function (x) {
                    return x.offerId == offerId;
                });

                if (finded == null || finded.length == 0) {
                    res.push({ offerId: offerId, offerPrice: '' });
                }
            }
            return res;
        };

        ctrl.getOfferItemsPrepared = async function (formOptions) {
            ctrl.action_offer_items_prepared = await blocksConstructorService.getOfferDataFromId(formOptions.OfferItems || []);
        };

        ctrl.getPreparedOfferItem = function (offerId) {
            if (ctrl.action_offer_items_prepared == null) {
                return {};
            }
            var items = ctrl.action_offer_items_prepared.filter(function (x) {
                return x.offerId == offerId;
            });
            return items != null && items.length > 0 ? items[0] : {};
        };

        ctrl.deleteOfferItem = function (data, preparedItem, index) {
            data.splice(index, 1);

            var items = ctrl.action_offer_items_prepared.filter(function (x) {
                return x.offerId === preparedItem.offerId;
            });
            if (items != null && items.length > 0) {
                ctrl.action_offer_items_prepared.splice(ctrl.action_offer_items_prepared.indexOf(items[0]), 1);
            }
        };

        ctrl.initUseManyOffers = function (formOptions, isForm) {
            if (formOptions.OfferId != null && formOptions.OfferId != '') {
                formOptions.OfferItems = formOptions.OfferItems || [];

                var finded = formOptions.OfferItems.filter(function (x) {
                    return x.offerId === formOptions.OfferId;
                });
                if (finded == null || finded.length == 0) {
                    formOptions.OfferItems.push({ offerId: formOptions.OfferId, offerPrice: '' });
                }

                formOptions.OfferId = null;
            }
        };

        ctrl.setDefaultMinAmount = function (item) {
            if (item.offerId == null || item.offerId === '' || (item.offerAmount != null && item.offerAmount !== '')) {
                if (item.offerAmount != null && item.offerAmount !== '') {
                    item.offerAmount = parseFloat(item.offerAmount);
                }
                return;
            }

            blocksConstructorService.getProductNameByOfferId(item).then(function (data) {
                if (data != null) {
                    item.offerAmount = parseFloat(data.MinAmount);
                }
            });
        };

        ctrl.setDissalowSave = function (state) {
            ctrl.inProgress = state === 'start';
        };

        ctrl.getQuizzes = function () {
            $http.get('quizzesAdmin/GetQuizzesList').then(function (response) {
                ctrl.modalData.settings.quizzes = response.data;
            });
        };

        ctrl.updateQuizCode = function () {
            $http.get('quizzesAdmin/GetQuizHtmlCodeForInsert', { params: { quizId: ctrl.modalData.settings.quizId } }).then(function (response) {
                ctrl.modalData.settings.html = response.data.CodeForInsert;
            });
        };

        ctrl.IsInstallModule = function (stringId) {
            ctrl.isCheckInstallModule = false;
            $http
                .post('adminv3/modules/IsInstallModule', { stringId: stringId })
                .then(function (response) {
                    ctrl.modalData.settings.module_installed = response.data;
                })
                .finally(function () {
                    ctrl.isCheckInstallModule = true;
                });
        };

        ctrl.onBeforeAddNewElementReviewsByUser = function (entity) {
            entity.Date = $filter('ngFlatpickr')(new Date(), 'Y-m-d H:i');
            return entity;
        };

        ctrl.onBeforeAddNewElementVideoColumns = function (entity) {
            if (entity.video != null) {
                entity.video.preview = true;
                entity.video.inModal = true;
            }
            return entity;
        };

        ctrl.isBookingActive = function () {
            ctrl.isCheckBookingActive = false;
            $http
                .post('adminv3/SettingsBooking/IsBookingActive')
                .then(function (response) {
                    ctrl.modalData.settings.bookingIsActive = response.data;
                })
                .finally(function () {
                    ctrl.isCheckBookingActive = true;
                });
        };

        ctrl.onResizePicture = function (item, result) {
            blocksConstructorService.updatePictureFields(item, result, result.type);
        };
        ctrl.onResizeLogo = function (item, result) {
            ng.extend(item, result);
        };
    };

    ng.module('blocksConstructor').controller('BlocksConstructorSettingsBlockCtrl', BlocksConstructorSettingsBlockCtrl);
})(window.angular);
