import bodyTemplate from '../templates/newBuilder/body.html';
/*@ngInject*/
function NewBuilderController(
    $window,
    builderService,
    builderTypes,
    cmStatService,
    urlHelper,
    sidebarsContainerService,
    $translate,
    toaster,
    SweetAlert,
    $document,
    $scope,
) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.colors = [];
        ctrl.design = {};

        var urlParams = urlHelper.getUrlParamByName('showTransformer');

        if (urlParams != null && urlParams === 'true') {
            ctrl.openInSidebar();
        }
    };

    ctrl.setBackgroundSelectElement = function (element) {
        ctrl.backgroundSelectElement = element;
    };

    ctrl.setThemeSelectElement = function (element) {
        ctrl.themeSelectElement = element;
    };

    ctrl.openInSidebar = function () {
        ctrl.isLoaded = false;

        ctrl.getSetting().then(() => (ctrl.isLoaded = true));

        sidebarsContainerService
            .open({
                sidebarClass: 'sidebar--builder',
                contentId: 'new-builder',
                templateUrl: bodyTemplate,
                title: $translate.instant('Js.Builder.Settings'),
                showFooter: true,
                onSave: ctrl.save,
                scope: $scope.$new(),
            })
            .then(() => sidebarsContainerService.addCallback('onClose', ctrl.onCloseSidebarHandler, true));
    };

    ctrl.getSetting = function () {
        return builderService.getSettings().then(function (data) {
            ctrl.design = data;
            ctrl.design.OthersSectionOther = ctrl.design.OtherSettingsSections.filter((x) => x.IsOther);
            ctrl.design.OthersSectionMain = ctrl.design.OtherSettingsSections.filter((x) => x.Key == 'MainPage');
            ctrl.design.OthersSectionCategory = ctrl.design.OtherSettingsSections.filter((x) => x.Key == 'Category');
            ctrl.design.OthersSectionDesign = ctrl.design.OtherSettingsSections.filter((x) => x.Key == 'Design');
            ctrl.design.OthersSectionBrands = ctrl.design.OtherSettingsSections.filter((x) => x.Key == 'Brands');
            ctrl.design.OthersSectionNews = ctrl.design.OtherSettingsSections.filter((x) => x.Key == 'News');
            ctrl.design.OthersSectionProduct = ctrl.design.OtherSettingsSections.filter((x) => x.Key == 'Product');
            ctrl.design.OthersSectionCheckout = ctrl.design.OtherSettingsSections.filter((x) => x.Key == 'Checkout');
            ctrl.design.mainPageModeCurrent =
                ctrl.design.MainPageModeImageOptions != null
                    ? ctrl.design.MainPageModeImageOptions.find((x) => x.Value === ctrl.design.MainPageMode)
                    : null;

            builderService.setDesignVariants(ctrl.design);

            return ctrl.design;
        });
    };

    ctrl.changeBackground = function (name) {
        builderService.newBuilderApply(builderTypes.background, name);

        //reset theme
        ctrl.design.CurrentTheme = '_none';
        builderService.newBuilderApply(builderTypes.theme, '_none');
    };

    ctrl.changeTheme = function (name) {
        builderService.newBuilderApply(builderTypes.theme, name);

        //reset background
        ctrl.design.CurrentBackGround = '_none';
        builderService.newBuilderApply(builderTypes.background, '_none');
    };

    ctrl.changeColor = function (color) {
        $document[0].body.classList.add('color-scheme-preview');
        builderService.newBuilderApply(builderTypes.colorScheme, color.Name);
    };

    ctrl.changeMenuStyle = function (menuStyleName) {
        ctrl.design.MenuStyle = menuStyleName;
        builderService.newBuilderApplyMenuStyle(menuStyleName);
    };

    ctrl.changeMainPage = function (mode) {
        ctrl.design.MainPageMode = mode.Value;

        ctrl.design.mainPageModeCurrent = mode;

        if (mode.Value == 'Default') {
            ctrl.design.CountMainPageProductInLine =
                ctrl.design.CountMainPageCategoriesInLine =
                ctrl.design.CountMainPageProductInSection =
                ctrl.design.CountMainPageCategoriesInSection =
                    4;
        } else if (mode.Value == 'TwoColumns') {
            ctrl.design.CountMainPageProductInLine =
                ctrl.design.CountMainPageCategoriesInLine =
                ctrl.design.CountMainPageProductInSection =
                ctrl.design.CountMainPageCategoriesInSection =
                    3;
        }
    };

    ctrl.changeCheckbox = function (checked, name) {
        var objectProperty = name.split('.');
        objectProperty.reduce(function (obj, property) {
            if (obj[property] != null && Object.keys(obj[property]).length) {
                return obj[property];
            } else {
                obj[property] = checked;
            }
        }, ctrl.design);
    };

    ctrl.isHidden = function (setting) {
        return ctrl.design.HiddenSettings != null && ctrl.design.HiddenSettings.length > 0 && ctrl.design.HiddenSettings.indexOf(setting) > -1;
    };

    ctrl.cancel = function () {
        var oldDesign = builderService.newBuilderReturn();

        for (var i = ctrl.colors.length - 1; i >= 0; i--) {
            if (oldDesign.CurrentColorScheme == ctrl.colors[i].ColorName) {
                ctrl.colorSelected = ctrl.colors[i];
                break;
            }
        }

        builderService.newBuilderDialogClose();
    };

    ctrl.close = function () {
        sidebarsContainerService.close();
    };

    ctrl.save = function () {
        return builderService
            .newBuilderSave()
            .then(function (design) {
                toaster.pop('success', '', $translate.instant('Js.Design.SuccessSavingTemplate'));
                urlHelper.setLocationQueryParams('tab', null);
                $window.location.reload();
            })
            .catch(function (error) {
                toaster.pop({
                    type: 'error',
                    title: $translate.instant('Js.Design.ErrorSavingTemplate'),
                    timeout: 5000,
                });
            });
    };

    ctrl.setAllProductsManualRatio = function () {
        if (ctrl.ManualRatio == null) return;
        if (ctrl.ManualRatio < 0 || ctrl.ManualRatio > 5) {
            toaster.pop('error', '', 'Значение рейтинга может быть от 0 до 5');
            return;
        }
        builderService.setAllProductsManualRatio(ctrl.ManualRatio).then(function (response) {
            if (response.data.result == true) {
                toaster.pop('success', '', $translate.instant('Js.Design.SuccessSavingTemplate'));
            } else {
                toaster.pop('error', '', $translate.instant('Js.Design.ErrorSavingTemplate'));
            }
        });
    };

    ctrl.resizePictures = function () {
        SweetAlert.confirm($translate.instant('Js.Design.DoYouWantSqueezePhotos'), {
            title: $translate.instant('Js.Design.SqueezePhotosOfProducts'),
            cancelButtonText: $translate.instant('Js.Builder.Cancel'),
            customClass: {
                confirmButton: 'sidebar-container__btn',
                cancelButton: 'builder-btn-cancel',
            },
            buttonsStyling: false,
        }).then(function (result) {
            if (result.value === true) {
                builderService.resizePictures().finally(function () {
                    ctrl.startResizePictures = true;
                });
            }
        });
    };

    ctrl.cmStatOnTick = function (data) {
        if (data.IsRun === false && data.ProcessedPercent === 100) {
            ctrl.startResizePictures = false;
            cmStatService.deleteObsevarable();
        }
    };

    ctrl.resizeCategoryPictures = function (type) {
        SweetAlert.confirm($translate.instant('Js.Design.DoYouWantResizeCategoryPhotosText'), {
            title: $translate.instant('Js.Design.DoYouWantResizeCategoryPhotosTitle'),
            cancelButtonText: $translate.instant('Js.Builder.Cancel'),
            customClass: {
                confirmButton: 'sidebar-container__btn',
                cancelButton: 'builder-btn-cancel',
            },
            buttonsStyling: false,
        }).then(function (result) {
            if (result.value === true) {
                builderService.resizeCategoryPictures(type).finally(function () {
                    if (ctrl.startResizeCategoryPictures == null) {
                        ctrl.startResizeCategoryPictures = {};
                    }
                    ctrl.startResizeCategoryPictures[type] = true;
                });
            }
        });
    };

    ctrl.cmCategoryStatOnTick = function (data, type) {
        if (data.IsRun === false && data.ProcessedPercent === 100) {
            ctrl.startResizeCategoryPictures[type] = false;
            cmStatService.deleteObsevarable();
        }
    };

    ctrl.addPhone = function () {
        ctrl.design.AdditionalPhones = ctrl.design.AdditionalPhones || [];
        ctrl.design.AdditionalPhones.push({ Phone: '', StandardPhone: '', Description: '', Icon: '', Type: 0 });
    };
    ctrl.deletePhone = function (index) {
        ctrl.design.AdditionalPhones.splice(index, 1);
    };

    var debounceFormatPhone;
    ctrl.formatPhone = function (item) {
        if (debounceFormatPhone != null) {
            clearTimeout(debounceFormatPhone);
        }

        debounceFormatPhone = setTimeout(function () {
            builderService.convertToStandardPhone(item.Phone).then(function (phone) {
                item.StandardPhone = phone;
            });
        }, 300);
    };

    ctrl.onCloseSidebarHandler = function () {
        $document[0].body.classList.remove('color-scheme-preview');
    };

    ctrl.processCity = function (zone) {
        if (zone && zone.CityId) {
            ctrl.design.DefaultCityIdIfNotAutodetect = zone.CityId;

            if (zone.Name) {
                ctrl.design.DefaultCityIfNotAutodetect = zone.Name;
                ctrl.design.DefaultCityDescription = zone.Country + ', ' + zone.Region;
            }
        }
    };
}

/*@ngInject*/
function BuilderOtherSettingsController() {
    var ctrl = this;
}

export { NewBuilderController, BuilderOtherSettingsController };
