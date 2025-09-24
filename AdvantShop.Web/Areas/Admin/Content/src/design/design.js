(function (ng) {
    'use strict';

    var DesignCtrl = function ($window, $http, $location, $q, toaster, Upload, designService, SweetAlert, $translate, cmStatService) {
        var ctrl = this;

        var mapWithDesignData = {
            Theme: 'Themes',
            Color: 'ColorSchemes',
            Background: 'BackGrounds',
        };

        ctrl.pageLoaded = false;

        ctrl.templatesProgress = [];

        ctrl.$onInit = function () {
            //ctrl.getData();
            ctrl.pageLoaded = true;
        };

        ctrl.getData = function () {
            return designService.getDesigns(ctrl.urlParametr()).then(function (designData) {
                return (ctrl.designData = designData);
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
                designService.uploadDesign($file, designType).then(function (result) {
                    if (result === true) {
                        ctrl.getData().then(function () {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Design.ArchiveSuccessfullyUploaded'));
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
            console.log('dsfds');
            SweetAlert.confirm($translate.instant('Admin.Js.Design.AreYouSureDelete'), { title: '' }).then(function (result) {
                if (result === true || result.value) {
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

        ctrl.preview = function (id, previewTemplateId, shopUrl) {
            ctrl.templatesProgress[previewTemplateId] = true;

            designService.previewTemplate(id, previewTemplateId).then(function (result) {
                designService.checkPage(shopUrl).then(function () {
                    if (result === true) {
                        SweetAlert.success($translate.instant('Admin.Js.Design.TemplateReadyForPreview'), {
                            title: $translate.instant('Admin.Js.Design.TemplatePreview'),
                            showCancelButton: true,
                            confirmButtonText: $translate.instant('Admin.Js.Design.GoTo'),
                            cancelButtonText: $translate.instant('Admin.Js.Design.Close'),
                        }).then(function (result) {
                            if (result === true) {
                                $window.open(shopUrl);
                            }

                            $window.location.reload(true);
                        });
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorActivatingPreview'));
                    }

                    ctrl.templatesProgress[previewTemplateId] = false;
                });
            });
        };

        ctrl.resizePictures = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Design.DoYouWantSqueezePhotos'), {
                title: $translate.instant('Admin.Js.Design.SqueezePhotosOfProducts'),
            }).then(function (result) {
                if (result === true || result.value) {
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

        ctrl.installTemplate = function (stringId, id, version, redirectUrl) {
            SweetAlert.info(null, {
                title: '<i class="fa fa-spinner fa-spin"></i>&nbsp;' + $translate.instant('Admin.Js.Design.TemplateInstalling'),
                showConfirmButton: false,
                allowOutsideClick: false,
                allowEscapeKey: false,
            });

            return designService
                .enableStore(ctrl.checkStore)

                .then(function (response) {
                    if (response != null && response.errors != null) {
                        response.errors.forEach(function (err) {
                            toaster.pop('error', '', err);
                        });
                        swal.close();
                    }

                    return designService.installTemplate(stringId, id, version);
                })
                .then(function (response) {
                    if (response.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Design.TemplateInstalled'));
                        if (redirectUrl != null && redirectUrl.length > 0) {
                            $window.location.assign(redirectUrl);
                        } else {
                            $window.location.reload(true);
                        }
                    } else {
                        swal.close();
                        toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorInstalledTemplate'));
                    }
                })
                .catch(function () {
                    swal.close();
                });
        };

        ctrl.updateTemplate = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Design.AreYouSureUpdateTemplate'), {
                title: $translate.instant('Admin.Js.Design.TemplateUpdating'),
            }).then(function (result) {
                if (result === true || result.value) {
                    designService.updateTemplate(id).then(function (response) {
                        if (response.result === true) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Design.TemplateUpdated'));
                            $window.location.reload(true);
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileUpdatingTemplate'));
                        }
                    });
                }
            });
        };

        ctrl.deleteTemplate = function (stringid) {
            SweetAlert.confirm($translate.instant('Admin.Js.Design.AreYouSureDeleteTemplate'), {
                title: $translate.instant('Admin.Js.Design.TemplateDeleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    designService.deleteTemplate(stringid).then(function (response) {
                        if (response.result === true) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Design.TemplateDeleted'));
                            $window.location.reload(true);
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileDeletingTemplate'));
                        }
                    });
                }
            });
        };

        ctrl.urlParametr = function () {
            return location.search
                .split(/[?&]/)
                .slice(1)
                .map(function (paramPair) {
                    return paramPair.split(/=(.+)?/).slice(0, 2);
                })
                .reduce(function (obj, pairArray) {
                    obj[pairArray[0]] = pairArray[1];
                    return obj;
                }, {});
        };

        // Theme

        ctrl.initTheme = function (show, theme, design, title, custom) {
            ctrl.show = show;
            ctrl.theme = theme;
            ctrl.design = design;
            ctrl.themeTitle = title;
            ctrl.themeIsCustom = custom;
            ctrl.getFiles();
        };

        ctrl.getFiles = function () {
            $http
                .post('design/themeFiles', {
                    theme: ctrl.theme,
                    design: ctrl.design,
                    action: 'getfiles',
                })
                .then(function (response) {
                    ctrl.themeFiles = response.data.files;
                });
        };

        ctrl.removeFile = function (item) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http
                        .post('design/themeFiles', {
                            theme: ctrl.theme,
                            design: ctrl.design,
                            action: 'remove',
                            removeFile: item.Name,
                        })
                        .then(ctrl.getFiles)
                        .then(function () {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Design.ChangesSaved'));
                        });
                }
            });
        };

        ctrl.addThemeFile = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                Upload.upload({
                    url: 'design/themeFiles',
                    data: {
                        theme: ctrl.theme,
                        design: ctrl.design,
                        themeCss: ctrl.themeCss,
                        action: 'upload',
                        file: $file,
                    },
                }).then(function (result) {
                    ctrl.getFiles();
                    if (result.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.PictureUploader.ImageSaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Design.ErrorWhileLoading'));
                    }
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.Design.ErrorWhileLoading'), $translate.instant('Admin.Js.Design.FileDoesNotMeet'));
            }
        };

        ctrl.saveTheme = function () {
            $http
                .post('design/saveTheme', {
                    theme: ctrl.theme,
                    design: ctrl.design,
                    themeCss: ctrl.themeCss,
                })
                .then(function (response) {
                    if (response.data.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Design.ChangesSaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Design.ChangesNotSaved'));
                    }
                });
        };

        $(window).bind('keydown', function (event) {
            if (event.ctrlKey || event.metaKey) {
                switch (String.fromCharCode(event.which).toLowerCase()) {
                    case 's':
                        event.preventDefault();
                        if (!ctrl.themeIsCustom) {
                            ctrl.showDefaultThemeSweetAlert();
                        } else {
                            ctrl.saveTheme();
                        }
                        break;
                }
            }
        });

        ctrl.showDefaultThemeSweetAlert = function () {
            return SweetAlert.confirm('Данную тему нельзя редактировать, но мы можем создать копию', {
                title: 'Создать копию темы дизайна?',
                input: 'text',
                inputValue: ctrl.themeTitle + ' (изм.)',
                inputValidator: (value) => {
                    return $q.when(!value || ctrl.checkNameAvailability(value)).then(function (invalid) {
                        return invalid ? $q.reject('Название не может быть пустым или совпадать с уже существующими') : false;
                    });
                },
            })
                .then(function (result) {
                    if (result && !result.isDismissed) {
                        ctrl.createCopy(result.value || result);
                    }
                })
                .catch(function (error) {
                    if (error !== 'cancel') {
                        if (error != null) {
                            toaster.pop('error', '', error.message);
                        }
                    }
                });
        };

        ctrl.checkNameAvailability = function (name) {
            return $q.when(ctrl.designData || ctrl.getData()).then(function (data) {
                var nameLowerCase = name.toLowerCase();

                var field = mapWithDesignData[ctrl.design];

                return field != null
                    ? data[field].some(function (item) {
                          return item?.Title.toLowerCase() === nameLowerCase;
                      })
                    : !name;
            });
        };

        ctrl.createCopy = function (newThemeTitle) {
            return $http
                .post('design/copyTheme', {
                    theme: ctrl.theme,
                    design: ctrl.design,
                    newThemeTitle: newThemeTitle,
                    newThemeCss: ctrl.themeCss,
                })
                .then(function (response) {
                    if (response.data.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Design.ChangesSaved'));
                        $window.location.assign(response.data.obj.newThemeEditUrl);
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Design.ChangesNotSaved'));
                    }
                });
        };
    };

    DesignCtrl.$inject = ['$window', '$http', '$location', '$q', 'toaster', 'Upload', 'designService', 'SweetAlert', '$translate', 'cmStatService'];
    ng.module('design', ['ngFileUpload', 'uiAceTextarea']).controller('DesignCtrl', DesignCtrl);
})(window.angular);
