(function (ng) {
    'use strict';

    /* @ngInject */
    var ModalCustomOptionsCtrl = function (
        $uibModalInstance,
        toaster,
        $translate,
        $http,
        uiGridCustomConfig,
        $uibModal,
        isMobileService,
        SweetAlert,
        $q,
        Upload,
    ) {
        const ctrl = this;
        const isMinMaxQuantityNotEmpty =
            'grid.appScope.$ctrl.gridExtendCtrl.customOptions[grid.appScope.$ctrl._params.index].MinQuantity != null || grid.appScope.$ctrl.gridExtendCtrl.customOptions[grid.appScope.$ctrl._params.index].MaxQuantity != null';

        let defaultColumnDefs = [
            {
                name: 'Title',
                headerCellTemplate: '<span class="ui-grid-cell-contents text-required">Название</span>',
                enableSorting: false,
                enableCellEdit: false,
                width: 170,
                cellTemplate:
                    '<div class="ui-grid-cell-contents">' +
                    '<input type="text" name="custom-option-grid-name_{{row.entity.OptionId}}" validation-input-text="Название в таблице значений" class="form-control input-alt"' +
                    'ng-model="row.entity.Title"' +
                    ' required /> ' +
                    '</div>',
                enableHiding: false,
            },
            {
                name: 'BasePrice',
                displayName: 'Цена',
                enableSorting: false,
                enableCellEdit: false,
                width: 100,
                cellTemplate:
                    '<div class="ui-grid-cell-contents">' +
                    '<input type="number" class="form-control input-alt"' +
                    'ng-model="row.entity.BasePrice" ' +
                    ' />' +
                    '</div>',
                enableHiding: false,
            },
        ];

        const descriptionSortColumns = [
            {
                name: 'SortOrder',
                displayName: 'Сортировка',
                enableSorting: false,
                enableCellEdit: false,
                width: 100,
                cellTemplate:
                    '<div class="ui-grid-cell-contents">' +
                    '<input type="number" class="form-control input-alt"' +
                    'ng-model="row.entity.SortOrder"' +
                    ' />' +
                    '</div>',
            },
            {
                name: 'Description',
                displayName: 'Описание',
                enableSorting: false,
                enableCellEdit: false,
                minWidth: 200,
                cellTemplate:
                    '<div class="ui-grid-cell-contents">' +
                    '<input type="text" class="form-control input-alt"' +
                    'ng-model="row.entity.Description"' +
                    ' />' +
                    '</div>',
                enableHiding: true,
            },
            {
                name: '_serviceColumn',
                displayName: '',
                width: 40,
                cellTemplate:
                    '<div class="ui-grid-cell-contents"><div>' +
                    '<a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteOption(row.entity.OptionId, grid.appScope.$ctrl.getParams().customOptionId)" class="ui-grid-custom-service-icon fa fa-times link-invert"></a>' +
                    '</div></div>',
                enableHiding: false,
            },
        ];

        const additionalComboEnabled = [
            {
                name: 'MinQuantity',
                displayName: 'Мин. кол-во',
                enableSorting: false,
                enableCellEdit: false,
                width: 100,
                cellTemplate:
                    '<div class="ui-grid-cell-contents">' +
                    '<input type="number" min="0" class="form-control input-alt"' +
                    ' name="custom-option-grid-min_{{row.entity.CustomOptionsId}}" ng-change="grid.appScope.$ctrl.gridExtendCtrl.changeOption(row.entity, grid.appScope.$ctrl.gridExtendCtrl.customOptions[grid.appScope.$ctrl._params.index])" ng-model="row.entity.MinQuantity" ' +
                    'ng-required="' +
                    isMinMaxQuantityNotEmpty +
                    '" validation-input-text="Мин. количество"/>' +
                    '</div>',
                enableHiding: true,
            },
            {
                name: 'MaxQuantity',
                displayName: 'Макс. кол-во',
                enableSorting: false,
                enableCellEdit: false,
                width: 100,
                cellTemplate:
                    '<div class="ui-grid-cell-contents">' +
                    '<input type="number" min="0" class="form-control input-alt"' +
                    '  name="custom-option-grid-max_{{row.entity.CustomOptionsId}}" ng-change="grid.appScope.$ctrl.gridExtendCtrl.changeOption(row.entity, grid.appScope.$ctrl.gridExtendCtrl.customOptions[grid.appScope.$ctrl._params.index])" ng-model="row.entity.MaxQuantity" ' +
                    ' ng-required="' +
                    isMinMaxQuantityNotEmpty +
                    '" validation-input-text="Макс. количество"/>' +
                    '</div>',
                enableHiding: true,
            },
            {
                name: 'DefaultQuantity',
                displayName: 'Кол-во по умолчанию',
                enableSorting: false,
                enableCellEdit: false,
                width: 100,
                cellTemplate:
                    '<div class="ui-grid-cell-contents">' +
                    '<input type="number" min="0" class="form-control input-alt"' +
                    ' name="custom-option-grid-default_{{row.entity.CustomOptionsId}}" ng-change="grid.appScope.$ctrl.gridExtendCtrl.changeOption(row.entity, grid.appScope.$ctrl.gridExtendCtrl.customOptions[grid.appScope.$ctrl._params.index])" ng-model="row.entity.DefaultQuantity" ' +
                    ' ng-required="' +
                    isMinMaxQuantityNotEmpty +
                    '" validation-input-text="Количество по умолчанию"/>' +
                    '</div>',
                enableHiding: true,
            },
        ];

        const pictureGridOption = {
            name: 'PictureData.PictureUrl',
            headerCellClass: 'ui-grid-custom-header-cell-center',
            displayName: 'Изображение',
            cellTemplate:
                '<div ng-if="row.entity.OptionId > 0"><a href="" class= "link-invert link-decoration-none fas fa-times pull-right" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteImage(row.entity)"></a></div>' +
                '<div class="ui-grid-cell-contents pointer">' +
                '<a class="ui-grid-custom-flex-center ui-grid-custom-link-for-img"' +
                'ng-click="grid.appScope.$ctrl.gridExtendCtrl.addPictureToGrid(row.entity)"> ' +
                '<img class= "ui-grid-custom-col-img" ng-src="{{row.entity.PictureData.PictureUrl}}"></a></div>',
            width: 80,
            enableSorting: false,
            enableHiding: true,
        };

        ctrl.$onInit = function () {
            ctrl.optionInputType = {
                DropDownList: 0,
                RadioButton: 1,
                CheckBox: 2,
                TextBoxSingleLine: 3,
                TextBoxMultiLine: 4,
                // ChoiceOfProduct: 5,
                MultiCheckBox: 6,
            };

            const params = ctrl.$resolve;
            ctrl.comboEnabled = params.comboEnabled;
            ctrl.isMobile = isMobileService.getValue();
            ctrl.productId = params.productId;
            ctrl.rnd = Math.random() * 10000;
            ctrl.gridOptionsDict = {};
            ctrl.isEmptyOptionsFields = true;

            // ctrl.gridOptionsChoiceOfProduct = ng.extend({}, uiGridCustomConfig, {
            //     enableGridMenu: !ctrl.isMobile,
            //     columnDefs: getColumnDefsChoiceOfProductOptions(),
            // });
            if (params.customOptionsPictureEnabled) {
                defaultColumnDefs.unshift(pictureGridOption);
            }

            ctrl.getCustomOptions().then(function (result) {
                if (result) {
                    ctrl.btnLoading = false;
                    ctrl.getFormData().then(function (result) {
                        if (ctrl.comboEnabled === false) {
                            ctrl.customOptions = ctrl.customOptions.filter((x) => x.InputType !== 5);
                        }
                    });
                }
            });
        };

        //#region CustomOptions
        ctrl.getCustomOptions = function () {
            return $http.get('CustomOptions/GetCustomOptions', { params: { productId: ctrl.productId } }).then(function (response) {
                var data = response.data;
                if (data.result) {
                    if (ctrl.customOptions != null) {
                        mergeData(ctrl.customOptions, data.obj);
                    } else {
                        ctrl.customOptions = data.obj;
                    }
                } else {
                    if (data.errors) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        toaster.pop('error', 'Не удалось получить настройки');
                    }
                }
                return data.result;
            });
        };

        ctrl.getFormData = function () {
            return $http.get('CustomOptions/GetFormData').then(function (response) {
                var data = response.data;
                ctrl.inputTypeList = data.inputTypeList;
                ctrl.priceTypeList = data.priceTypeList;
                ctrl.usePicture = data.usePicture;
                ctrl.noPhotoUrl = data.noPhotoUrl;
                ctrl.comboEnabled = data.comboEnabled;
                return true;
            });
        };

        ctrl.addNewCustomOption = function () {
            $http.get('CustomOptions/GetDefaultCustomOptionValues', { params: { productId: ctrl.productId } }).then(function (response) {
                var data = response.data;

                var maxSortOrder = ctrl.customOptions.length > 0 ? null : 0;
                var minId = ctrl.customOptions.length > 0 ? null : 0;

                for (var i = 0; i < ctrl.customOptions.length; i++) {
                    var item = ctrl.customOptions[i];
                    if (item.SortOrder > maxSortOrder) {
                        maxSortOrder = item.SortOrder;
                    }
                    if (item.CustomOptionsId < minId) {
                        minId = item.CustomOptionsId;
                    }
                }

                data.obj.CustomOptionsId = minId - 1;
                data.obj.SortOrder = maxSortOrder + 10;

                let defaultOption = ctrl.getDefaultOption(data.obj.CustomOptionsId, data.obj.InputType);
                data.obj.Options = [defaultOption];
                data.obj.isNew = true;
                ctrl.customOptions.push(data.obj);
            });
        };

        ctrl.deleteCustomOption = function (customOptionId) {
            const defer = $q.defer();

            SweetAlert.confirm($translate.instant('Admin.Js.Product.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Product.Deleting'),
            }).then(function (result) {
                if (result.value) {
                    if (customOptionId > 0) {
                        $http
                            .post('CustomOptions/DeleteCustomOption', { customOptionId })
                            .then(() => {
                                defer.resolve();
                            })
                            .catch((error) => {
                                defer.reject(error);
                            });
                    } else {
                        defer.resolve();
                    }
                    defer.promise
                        .then(() => {
                            ctrl.removeByAttr(ctrl.customOptions, 'CustomOptionsId', customOptionId);
                            toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                        })
                        .catch((error) => {
                            console.error(error);
                        });
                }
            });
        };

        ctrl.changeOption = function (option, item) {
            if (item.Options) {
                for (let option of item.Options) {
                    if (option.MinQuantity != null || option.MaxQuantity != null || option.DefaultQuantity != null) {
                        item._isEmptyOptionsFields = false;
                        break;
                    } else {
                        item._isEmptyOptionsFields = true;
                    }
                }
            }
        };

        ctrl.save = function () {
            let isValid = true;

            ctrl.customOptions.forEach((item) => {
                delete item.isNew;
                if (item.Options && item.InputType !== 2) {
                    item.Options.forEach((option) => {
                        if (!ctrl.validateOption(option, item)) {
                            isValid = false;
                        }
                    });
                }
                if (item.InputType === 2 || item.InputType === 6) {
                    if (!ctrl.validateItem(item)) {
                        isValid = false;
                    }
                }
            });

            if (!isValid) return;

            ctrl.btnLoading = true;

            $http
                .post('CustomOptions/SaveCustomOptions', {
                    customOptions: ctrl.customOptions,
                    productId: ctrl.productId,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result) {
                        if (ctrl.form != null) {
                            ctrl.form.$setPristine();
                        }
                        toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                        return ctrl.getCustomOptions();
                    } else {
                        if (data.errors) {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                        } else {
                            toaster.pop('error', $translate.instant('Admin.Js.Product.ErorWhileSaving'));
                        }
                    }
                })
                .finally(() => (ctrl.btnLoading = false));
        };

        ctrl.getCustomOption = function (customOptionId) {
            return ctrl.customOptions.find((x) => x.CustomOptionsId === customOptionId);
        };

        //#endregion

        //#region Grid

        var getColumnDefsWithPictureOptions = function (item) {
            let columns = [
                ...defaultColumnDefs,
                {
                    name: 'PriceType',
                    displayName: 'Тип цены',
                    enableSorting: false,
                    enableCellEdit: false,
                    width: 170,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<select class="form-control input-alt"' +
                        'ng-model="row.entity.PriceType"' +
                        ' ng-options="option.Value as option.Text for option in grid.appScope.$ctrl.gridExtendCtrl.priceTypeList"></select >' +
                        '</div>',
                    enableHiding: true,
                },
            ];

            if (ctrl.comboEnabled || (item != null && (item.MinQuantity != null || item.MaxQuantity))) {
                columns = [...columns, ...additionalComboEnabled];
            }

            return [...columns, ...descriptionSortColumns];
        };

        var getColumnDefsChoiceOfProductOptions = function () {
            var columns = Object.assign([], defaultColumnDefs);
            columns.splice(0, 0, {
                name: 'ProductPhoto.ProductImageSrcBig',
                headerCellClass: 'ui-grid-custom-header-cell-center',
                displayName: 'Товар',
                cellTemplate:
                    '<div ng-if="row.entity.ProductId != null"><a href="" class= "link-invert link-decoration-none fas fa-times pull-right" ng-click="grid.appScope.$ctrl.gridExtendCtrl.deleteProductFromGrid(row.entity)"></a></div>' +
                    '<div ng-if="row.entity.ProductId != null" class="ui-grid-cell-contents">' +
                    '<a class="ui-grid-custom-flex-center ui-grid-custom-link-for-img">' +
                    '<img class="ui-grid-custom-col-img" ng-src="{{row.entity.ProductPhoto.ProductImageSrcBig}}"></a></div></div > ' +
                    '<div ng-if="row.entity.ProductId == null" class="ui-grid-cell-contents" ng-click="grid.appScope.$ctrl.gridExtendCtrl.selectProduct(row.entity)"' +
                    'ng-model="row.entity.ProductId" required validation-input-text="Выберите товар"> <a>Выбрать товар</a></div> ',
                width: 90,
                enableSorting: false,
            });
            return columns;
        };

        ctrl.getNewGridOptions = function (inputType, item) {
            return ng.extend({}, ctrl.getGridOptionsByInputType(inputType, item));
        };

        ctrl.getGridOptionsByInputType = function (inputType, item) {
            uiGridCustomConfig.enableHorizontalScrollbar = 1;
            switch (inputType) {
                case ctrl.optionInputType.DropDownList:
                case ctrl.optionInputType.RadioButton:
                case ctrl.optionInputType.MultiCheckBox:
                    // let column = [];
                    // if (ctrl.comboEnabled) {
                    //     defaultColumnDefs = [...defaultColumnDefs, ...additionalComboEnabled];
                    // }
                    //
                    // defaultColumnDefs = [...defaultColumnDefs, ...descriptionSortColumns];

                    // ctrl.gridOptionsWithPicture = ng.extend({}, uiGridCustomConfig, {
                    //     enableGridMenu: !ctrl.isMobile,
                    //     columnDefs: getColumnDefsWithPictureOptions(item),
                    // });
                    return ng.extend({}, uiGridCustomConfig, {
                        enableGridMenu: !ctrl.isMobile,
                        columnDefs: getColumnDefsWithPictureOptions(item),
                    });
                // case ctrl.optionInputType.ChoiceOfProduct:
                //     return ctrl.gridOptionsChoiceOfProduct;
            }
        };

        ctrl.getGridOptions = function (customOptionId, inputType) {
            if (Object.prototype.hasOwnProperty.call(ctrl.gridOptionsDict, customOptionId) === false) {
                const item = ctrl.customOptions.find((x) => x.ID === customOptionId);
                setGridOption(customOptionId, ng.extend({}, ctrl.getNewGridOptions(inputType, item), { data: item?.Options }));
            }

            return ctrl.gridOptionsDict[customOptionId];
        };

        ctrl.gridOptionsOnInit = function (grid) {
            var id = grid.getParams().customOptionId;
            var customOption = ctrl.getCustomOption(parseInt(id));
            var gridOptions = ctrl.getNewGridOptions(customOption.InputType, customOption);
            gridOptions.data = customOption.Options;
            setGridOption(id, gridOptions);
        };

        function getGridData(customOptionId) {
            if (Object.prototype.hasOwnProperty.call(ctrl.gridOptionsDict, customOptionId)) {
                return ctrl.gridOptionsDict[customOptionId].data;
            }
        }

        function setGridData(customOptionId, data) {
            ctrl.gridOptionsDict[customOptionId].data = data;
        }

        function setGridOption(customOptionId, gridOption) {
            ctrl.gridOptionsDict[customOptionId] = gridOption;
        }

        //#endregion

        //#region Options

        ctrl.addOption = function (customOptionId) {
            var customOption = ctrl.getCustomOption(customOptionId);

            let option = ctrl.getDefaultOption(customOptionId, customOption.InputType);
            const gridData = getGridData(customOptionId);
            option = ctrl.setOptionDefaultValues(option, customOptionId, gridData);

            gridData.push(option);
        };

        ctrl.deleteOption = function (optionId, customOptionId) {
            var gridData = ctrl.gridOptionsDict[customOptionId].data;
            if (gridData.length === 1) {
                toaster.pop('error', 'В таблице значений должен быть хотя бы 1 элемент');
                return;
            }

            if (optionId > 0) {
                $http.post('CustomOptions/DeleteOption', { optionId: optionId });
            }

            ctrl.removeByAttr(gridData, 'OptionId', optionId);
            toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
        };

        ctrl.updateOption = function (option, item) {
            if (option.OptionId <= 0) {
                return;
            }

            if (!ctrl.validateOption(option, item)) {
                return;
            }
            var params = {
                Title: option.Title,
                PriceBC: option.PriceBC,
                PriceType: option.PriceType,
                SortOrder: option.SortOrder,
                ProductId: option.ProductId,
                OfferId: option.OfferId,
                MinQuantity: option.MinQuantity,
                MaxQuantity: option.MaxQuantity,
                DefaultQuantity: option.DefaultQuantity,
                Description: option.Description,
            };
            $http.post('CustomOptions/UpdateOption', params).then(function (response) {
                var data = response.data;
                if (data.result) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                } else {
                    if (data.errors) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Product.ErorWhileSaving'));
                    }
                }
            });
        };

        ctrl.inputTypeChange = function (customOptionId, inputType, item) {
            if (inputType === ctrl.optionInputType.TextBoxSingleLine || inputType === ctrl.optionInputType.TextBoxMultiLine) {
                return;
            }

            var defaultOption = ctrl.getDefaultOption(customOptionId, inputType);

            if (inputType === ctrl.optionInputType.CheckBox) {
                var customOption = ctrl.getCustomOption(customOptionId);
                if (customOption.Options == null || customOption.Options.length == 0) {
                    customOption.Options = [defaultOption];
                }
            }

            if (
                inputType === ctrl.optionInputType.DropDownList ||
                inputType === ctrl.optionInputType.RadioButton ||
                // inputType === ctrl.optionInputType.ChoiceOfProduct ||
                inputType === ctrl.optionInputType.MultiCheckBox
            ) {
                var gridData = getGridData(customOptionId);
                if (gridData == null || gridData.length === 0) {
                    gridData = [defaultOption];
                } else {
                    // if (inputType === ctrl.optionInputType.ChoiceOfProduct) {
                    //     gridData = ctrl.setOptionListQuantityToZero(gridData);
                    // }

                    var gridOption = ctrl.getNewGridOptions(inputType, item);
                    gridOption.data = gridData;
                    setGridOption(customOptionId, gridOption);
                }
            }
        };

        ctrl.getDefaultOption = function (customOptionId, inputType) {
            let defaultOption = {
                _isEmptyOptionsFields: true,
                CustomOptionsId: customOptionId,
                OptionId: -1,
                Title: '',
                BasePrice: 0,
                PriceType: 0,
                SortOrder: 10,
                MinQuantity: null,
                MaxQuantity: null,
                DefaultQuantity: null,
                PictureData: { PictureUrl: ctrl.noPhotoUrl },
                ProductPhoto: { ProductImageSrcBig: ctrl.noPhotoUrl },
            };
            // if (inputType === ctrl.optionInputType.ChoiceOfProduct || inputType === ctrl.optionInputType.MultiCheckBox) {
            if (inputType === ctrl.optionInputType.MultiCheckBox) {
                defaultOption = ctrl.setOptionQuantityToZero(defaultOption);
            }

            return defaultOption;
        };

        ctrl.validateItem = function (item) {
            if (item.MinQuantity > item.MaxQuantity) {
                toaster.pop('error', '', `Минимальное количество "${item.Title}" не должно быть больше максимального количества.`);
                return false;
            }

            if (item.InputType === ctrl.optionInputType.MultiCheckBox) {
                let optionsMinQuantitySum = 0;
                let defaultQuantitySum = 0;
                item.Options?.forEach((x) => {
                    optionsMinQuantitySum += x.MinQuantity;
                    defaultQuantitySum += x.DefaultQuantity;
                });

                if (optionsMinQuantitySum > item.MaxQuantity) {
                    toaster.pop(
                        'error',
                        '',
                        `Сумма значений полей "Минимальное кол-во" в таблице значений опции больше, чем максимальное количество у опции "${item.Title}"`,
                    );
                    return false;
                }
                if (defaultQuantitySum > item.MaxQuantity) {
                    toaster.pop(
                        'error',
                        '',
                        `Сумма значений полей "Кол-во по умолчанию" в таблице значений опции больше, чем максимальное количество у опции "${item.Title}"`,
                    );
                    return false;
                }

                if (optionsMinQuantitySum > item.MinQuantity) {
                    toaster.pop(
                        'error',
                        '',
                        'Минимальное количество опции не должно быть меньше суммы минимального количества записей. ' + item.Title,
                    );
                    return false;
                }
            }

            return true;
        };

        ctrl.validateOption = function (option, item) {
            // if (option.Title == null) {
            //     toaster.pop('error', '', 'Заполните название');
            //     return false;
            // }

            // const customOption = ctrl.getCustomOption(option.CustomOptionsId);
            // const gridData = getGridData(customOption.CustomOptionsId);

            const gridData = getGridData(item.CustomOptionsId);

            if (item.MaxQuantity > 0 || item.MinQuantity > 0) {
                if (option.MinQuantity == null) {
                    toaster.pop('error', '', 'Заполните минимальное количество');
                    return false;
                }
                if (option.MaxQuantity == null) {
                    toaster.pop('error', '', 'Заполните максимальное количество');
                    return false;
                }
                if (option.DefaultQuantity == null) {
                    toaster.pop('error', '', 'Заполните количество по умолчанию');
                    return false;
                }
                if (option.MaxQuantity < 0) {
                    toaster.pop('error', '', 'Максимальное количество должно быть больше 0');
                    return false;
                }
                if (option.MinQuantity < 0) {
                    toaster.pop('error', '', 'Минимальное количество должно быть больше 0');
                    return false;
                }
                if (option.DefaultQuantity < 0) {
                    toaster.pop('error', '', 'Количество по умолчанию должно быть больше 0');
                    return false;
                }
                if (option.DefaultQuantity > option.MaxQuantity) {
                    toaster.pop('error', '', `Максимальное количество "${option.Title}" не должно быть меньше количества по умолчанию.`);
                    return false;
                }
                if (option.DefaultQuantity < option.MinQuantity) {
                    toaster.pop('error', '', `Минимальное количество "${option.Title}" не должно быть больше количества по умолчанию.`);
                    return false;
                }
                if (option.MinQuantity > item.MaxQuantity) {
                    toaster.pop('error', '', `Минимальное количество "${option.Title}" не должно быть больше максимального количества записи.`);
                    return false;
                }
                // if (option.MinQuantity < item.MinQuantity) {
                //     toaster.pop('error', '', `Минимальное количество "${option.Title}" не должно быть меньше минимального количества записи.`);
                //     return false;
                // }
                if (option.MaxQuantity > item.MaxQuantity) {
                    toaster.pop('error', '', `Максимальное количество "${option.Title}" не должно быть больше максимального количества записи.`);
                    return false;
                }
            } else if (item.MaxQuantity == null || item.MinQuantity == null) {
                if (gridData) {
                    const filledQuantityCount = gridData.filter(
                        (x) => x.MaxQuantity != null || x.MinQuantity != null || x.DefaultQuantity != null,
                    ).length;
                    if (filledQuantityCount > 0) {
                        toaster.pop('error', '', 'Заполните минимальное и максимальное количество. ' + item.Title);
                        return false;
                    }
                }
            }

            return true;
        };

        // #endregion

        //#region Выбор товара

        ctrl.deleteProductFromGrid = function (option) {
            option.ProductPhoto = { ProductImageSrcBig: ctrl.noPhotoUrl };
            option.OfferId = null;
            option.ProductId = null;
        };

        ctrl.selectProduct = function (option) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalOffersSelectvizrCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: '../areas/admin/content/src/_shared/modal/offers-selectvizr/offersSelectvizrModal.html',
                    size: 'xs-11',
                })
                .result.then(
                    function (result) {
                        ctrl.addOfferListToOption(option, result.ids);
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };

        ctrl.addOfferListToOption = function (option, offerIds) {
            const customOption = ctrl.getCustomOption(option.CustomOptionsId);
            const customOptionId = customOption.CustomOptionsId;

            const existingOfferIds = customOption.Options.map((x) => x.OfferId);
            offerIds = offerIds.filter((x) => !existingOfferIds.includes(x));

            if (offerIds.length === 0) {
                return;
            }

            $http.get('CustomOptions/GetProductPhotosByOfferIds', { params: { offerIds: offerIds } }).then(function (response) {
                const data = response.data;
                if (data.result) {
                    const optionsToAdd = data.obj;

                    // удаляем запись, вместо которой добавляем записи с инфой о товаре
                    customOption.Options = customOption.Options.filter((x) => x.OptionId != option.OptionId);
                    setGridData(customOptionId, customOption.Options);

                    let isFirstElement = true;
                    optionsToAdd.forEach((newOption) => {
                        newOption = ctrl.setOptionValues(newOption, option, customOptionId, isFirstElement);
                        if (isFirstElement) {
                            isFirstElement = false;
                        }

                        customOption.Options.push(newOption);
                        setGridData(customOptionId, customOption.Options);
                    });

                    ctrl.form.$setDirty();
                } else {
                    toaster.pop('error', 'Не удалось получить настройки');
                }
            });
        };

        ctrl.setOptionValues = function (newOption, existOption, customOptionId, needSetOptionId = false) {
            newOption = ctrl.setOptionDefaultValues(newOption, customOptionId);
            newOption.Title = existOption.Title || newOption.Title;
            newOption.BasePrice = existOption.BasePrice;
            newOption.MinQuantity = existOption.MinQuantity;
            newOption.MaxQuantity = existOption.MaxQuantity;
            newOption.DefaultQuantity = existOption.DefaultQuantity;

            if (needSetOptionId) {
                newOption.OptionId = existOption.OptionId;
                needSetOptionId = false;
            }

            return newOption;
        };

        ctrl.setOptionListQuantityToZero = function (optionList) {
            optionList.forEach((option) => {
                option = ctrl.setOptionQuantityToZero(option);
            });

            return optionList;
        };

        ctrl.setOptionQuantityToZero = function (option) {
            option.MinQuantity = null;
            option.MaxQuantity = null;
            option.DefaultQuantity = null;

            return option;
        };

        //#endregion

        //#region Images

        ctrl.addPictureToGrid = function (option) {
            if (option.OptionId <= 0) {
                toaster.pop('error', $translate.instant('Admin.Js.Product.CannotUploadPhoto'));
                return;
            }
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalCropImageCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: '../areas/admin/content/src/_shared/modal/cropImage/cropImage.html',
                })
                .result.then(
                    function (result) {
                        ctrl.updateImage(result, option);
                        return result;
                    },
                    function (result) {
                        return result;
                    },
                );
        };

        ctrl.updateImage = function (params, option) {
            if (option.OptionId <= 0) {
                toaster.pop('error', $translate.instant('Admin.Js.Product.CannotUploadPhoto'));
                return;
            }
            if (params != null && option != null) {
                option.PictureData.PhotoName = params.fileName;
                option.PictureData.PictureUrl = URL.createObjectURL(params.file);
                option.PictureData.PictureFile = params.file;
                option.PictureData.NeedUpdate = true;

                if (option.OptionId > 0) {
                    ctrl.updateOptionPhoto(option, params.file);
                }
            }
        };

        ctrl.deleteImage = function (option) {
            option.PictureData.PhotoName = null;
            option.PictureData.PictureUrl = ctrl.noPhotoUrl;
            option.PictureData.PictureFile = null;
            option.PictureData.NeedUpdate = true;

            if (option.OptionId > 0) {
                ctrl.updateOptionPhoto(option);
            }
        };

        ctrl.updateOptionPhoto = function (option, file) {
            var url = 'CustomOptions/UpdatePhoto';
            Upload.upload({
                url: url,
                data: { optionId: option.OptionId },
                file: file,
            }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Product.ChangesSaved'));
                } else {
                    if (data.errors) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', error);
                        });
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Product.ErorWhileSaving'));
                    }
                }
            });
        };

        //#endregion

        //#region helpers

        ctrl.setOptionDefaultValues = function (option, customOptionId, gridData) {
            option.CustomOptionsId = customOptionId;
            gridData = gridData || getGridData(customOptionId);

            // set SortOrder and OptionId
            var haveOptions = gridData.length > 0;
            var maxSortOrder = haveOptions ? null : -10;
            var minOptionId = haveOptions ? null : 0;

            for (var i = 0; i < gridData.length; i++) {
                var item = gridData[i];
                if (item.SortOrder > maxSortOrder) {
                    maxSortOrder = item.SortOrder;
                }
                if (item.OptionId < minOptionId) {
                    minOptionId = item.OptionId;
                }
            }

            option.SortOrder = maxSortOrder + 10;
            option.OptionId = minOptionId - 1;

            return option;
        };

        ctrl.removeByAttr = function (arr, attr, value) {
            var i = arr.length;
            while (i--) {
                if (arr[i] && Object.prototype.hasOwnProperty.call(arr[i], attr) && arguments.length > 2 && arr[i][attr] === value) {
                    arr.splice(i, 1);
                }
            }
            return arr;
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        //#endregion
    };

    ng.module('uiModal').controller('ModalCustomOptionsCtrl', ModalCustomOptionsCtrl);

    function mergeData(source, data) {
        let itemNew, itemNewIndex;

        if (data == null) {
            return source;
        }

        for (const item of source) {
            itemNewIndex = data.findIndex((itemNew) =>
                item.ID === 0 || item._isEmptyOptionsFields ? itemNew.Title === item.Title : itemNew.ID === item.ID,
            );

            if (itemNewIndex === -1) {
                throw new Error('customOptions(merge): not found item in response');
            }

            itemNew = data[itemNewIndex];

            for (const key in itemNew) {
                if (Array.isArray(item[key])) {
                    mergeData(item[key], itemNew[key]);
                } else {
                    item[key] = itemNew[key];
                }
            }

            data.splice(itemNewIndex, 1);

            if (data.length > 0) {
                source = source.concat(data);
            }
        }
        return source;
    }
})(window.angular);
