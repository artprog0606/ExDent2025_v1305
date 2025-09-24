(function (ng) {
    'use strict';

    var SettingsTemplateCtrl = function (
        SweetAlert,
        toaster,
        $translate,
        designService,
        cmStatService,
        $http,
        $location,
        isMobileService,
        zoneService,
        $timeout,
    ) {
        var ctrl = this;
        var isMobile = isMobileService.getValue();

        ctrl.$onInit = function () {
            ctrl.getData();
        };

        ctrl.getData = function () {
            return designService.getThemes().then(function (designData) {
                ctrl.designData = designData;

                ctrl.CurrentTheme = ctrl.designData.Themes.find((theme) => theme.Name === ctrl.designData.CurrentTheme);
                ctrl.CurrentBackGround = ctrl.designData.BackGrounds.find((backGround) => backGround.Name === ctrl.designData.CurrentBackGround);
                ctrl.CurrentColorScheme = ctrl.designData.ColorSchemes.find((colorScheme) => colorScheme.Name === ctrl.designData.CurrentColorScheme);

                if (ctrl.form != null) {
                    ctrl.form.$setPristine();
                }
            });
        };

        ctrl.changeDesign = function (designType, name) {
            designService.saveDesign(designType, name).then(function (result) {
                if (result === true) {
                    ctrl.getData().then(function () {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Design.ChangesSaved'));
                    });
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileSavingDesign'));
                }
            });
        };

        ctrl.addDesign = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event, designType) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                designService.uploadDesign($file, designType).then(function (data) {
                    let result = data.obj;
                    if (result === true) {
                        ctrl.getData().then(function () {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Design.ArchiveSuccessfullyUploaded'));
                        });
                    } else if (data.errors != null) {
                        data.errors.forEach(function (error) {
                            toaster.pop('error', '', error);
                        });
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileLoading'));
                    }
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.Design.ErrorWhileLoading'), $translate.instant('Admin.Js.Design.FileDoesNotMeet'));
            }
        };

        ctrl.deleteDesign = function (designType, designName) {
            SweetAlert.confirm($translate.instant('Admin.Js.Design.AreYouSureDelete'), { title: '' }).then(function (result) {
                if (result === true || (result.value && !result.isDismissed)) {
                    designService.deleteDesign(designName, designType).then(
                        function (result) {
                            if (result === true) {
                                ctrl.getData().then(function () {
                                    toaster.pop('success', '', $translate.instant('Admin.Js.Design.SuccessfullyDeleted'));
                                });
                            } else {
                                toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileDeleting'));
                            }
                        },
                        function () {
                            toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileDeleting'));
                        },
                    );
                }
            });
        };

        // resize product pictures

        ctrl.resizePictures = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Design.DoYouWantSqueezePhotos'), {
                title: $translate.instant('Admin.Js.Design.SqueezePhotosOfProducts'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    designService.resizePictures().finally(function () {
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

        // resize big category pictures

        ctrl.resizeCategoryPictures = function (type) {
            SweetAlert.confirm($translate.instant('Admin.Js.Design.DoYouWantResizeCategoryPhotosText'), {
                title: $translate.instant('Admin.Js.Design.DoYouWantResizeCategoryPhotosTitle'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    designService.resizeCategoryPictures(type).finally(function () {
                        if (ctrl.startResizeCategoryPictures == null) {
                            ctrl.startResizeCategoryPictures = {};
                        }
                        ctrl.startResizeCategoryPictures[type] = true;
                    });
                }
            });
        };

        ctrl.cmResizeCategoryStatOnTick = function (data, type) {
            if (data.IsRun === false && data.ProcessedPercent === 100) {
                ctrl.startResizeCategoryPictures[type] = false;
                cmStatService.deleteObsevarable();
            }
        };

        ctrl.setAllProductsManualRatio = function () {
            if (ctrl.ManualRatio == null) return;
            if (ctrl.ManualRatio < 0 || ctrl.ManualRatio > 5) {
                toaster.pop('error', '', 'Значение рейтинга может быть от 0 до 5');
                return;
            }
            $http.post('product/setAllProductsManualRatio', { manualRatio: ctrl.ManualRatio }).then(function (response) {
                if (response.data.result == true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.ChangesSaved'));
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.memoryForm = function (form) {
            ctrl.form = form;
        };

        ctrl.isHidden = function (setting) {
            return ctrl.HiddenSettings != null && ctrl.HiddenSettings.length > 0 && ctrl.HiddenSettings.indexOf(setting) > -1;
        };

        ctrl.scrollIntoView = function () {
            if (isMobile) {
                setTimeout(function () {
                    angular.element('li.active')[0].scrollIntoView({
                        inline: 'center',
                        block: 'nearest',
                        behavior: 'smooth',
                    });
                }, 10);
            }
        };

        ctrl.processCity = function (zone) {
            if (zone && zone.CityId) {
                ctrl.DefaultCityIdIfNotAutodetect = zone.CityId;

                if (zone.Name) {
                    ctrl.DefaultCityIfNotAutodetect = zone.Name;
                    ctrl.DefaultCityDescription = zone.Country + ', ' + zone.Region;
                }
            }
        };

        ctrl.changeCityName = function () {
            if (ctrl.DefaultCityIfNotAutodetect) {
                zoneService.setCurrentZone(ctrl.DefaultCityIfNotAutodetect).then(function (response) {
                    if (response) {
                        if (response.CityId === 0) {
                            ctrl.DefaultCityIdIfNotAutodetect = 0;
                        }
                    }
                });
            }
        };
    };

    SettingsTemplateCtrl.$inject = [
        'SweetAlert',
        'toaster',
        '$translate',
        'designService',
        'cmStatService',
        '$http',
        '$location',
        'isMobileService',
        'zoneService',
        '$timeout',
    ];

    ng.module('settingsTemplate', ['design', 'csseditor', 'isMobile']).controller('SettingsTemplateCtrl', SettingsTemplateCtrl);
})(window.angular);
