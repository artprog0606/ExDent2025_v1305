(function (ng) {
    'use strict';

    var SettingsPartnersCtrl = function ($http, $q, $window, SweetAlert, toaster, $translate, Upload, isMobileService, $location) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.categoryIds = [];
            ctrl.getFormData();
            ctrl.getRewardPercentCategories();
            ctrl.getActTplsData();
        };

        ctrl.getFormData = function () {
            return $http.post('settingsPartners/getFormData').then(function (response) {
                var data = response.data.obj;

                ctrl.coupon = data.coupon;
                ctrl.paymentTypes = data.paymentTypes || [];
                ctrl.paymentTypes.push({ Id: null, Name: '' });
            });
        };

        // region payment types

        ctrl.sortableOptions = {
            orderChanged: function (event) {
                var id = event.source.itemScope.item.Id,
                    prev = ctrl.paymentTypes[event.dest.index - 1],
                    next = ctrl.paymentTypes[event.dest.index + 1];

                $http
                    .post('settingsPartners/changePaymentTypesSorting', {
                        id: id,
                        prevId: prev != null ? prev.Id : null,
                        nextId: next != null ? next.Id : null,
                    })
                    .then(function (response) {
                        if (response.data.result === true) {
                            toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                        }
                    });
            },
        };

        ctrl.deletePaymentType = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('settingsPartners/deletePaymentType', { id: id }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            ctrl.getFormData();
                            toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        };

        ctrl.addPaymentType = function (name) {
            $http.post('settingsPartners/addPaymentType', { name: name }).then(function (response) {
                if (response.data.result === true) {
                    ctrl.getFormData();
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                }
            });
        };

        ctrl.setPaymentTypeName = function (item, name) {
            if (!name) return;

            item.Name = name;

            $http.post('settingsPartners/updatePaymentType', { id: item.Id, name: name }).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        // endregion

        // region categories

        ctrl.addCategoryRewardPercent = function (item) {
            if (item.RewardPercent == null || item.RewardPercent < 0) {
                toaster.error('', 'Укажите процент');
                return;
            }
            if (item.categoryIds == null || item.categoryIds.length == 0) {
                toaster.error('', 'Выберите категории');
                return;
            }

            $http
                .post('settingsPartners/addCategoryRewardPercent', {
                    categoryIds: item.categoryIds,
                    rewardPercent: item.RewardPercent,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                        ctrl.getRewardPercentCategories();
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.getRewardPercentCategories = function () {
            $http.post('settingsPartners/getRewardPercentCategories').then(function (response) {
                if (response.data.result === true) {
                    ctrl.categories = response.data.obj || [];
                    ctrl.categories.push({ CategoryId: null, RewardPercent: 0 });
                }
            });
        };

        ctrl.updateCategoryRewardPercent = function (item) {
            $http
                .post('settingsPartners/updateCategoryRewardPercent', {
                    categoryId: item.CategoryId,
                    rewardPercent: item.RewardPercent,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.deleteCategoryRewardPercent = function (categoryId) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            })
                .catch((err) => console.error(err))
                .then(function (result) {
                    if (result === true || result.value) {
                        $http.post('settingsPartners/deleteCategoryRewardPercent', { categoryId: categoryId }).then(function (response) {
                            var data = response.data;
                            if (data.result === true) {
                                toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                                ctrl.getRewardPercentCategories();
                            } else {
                                toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                            }
                        });
                    }
                });
        };

        // endregion

        // region docx templates

        ctrl.getActTplsData = function () {
            return $http.post('settingsPartners/getActReportTplsData').then(function (response) {
                var data = response.data.obj;

                ctrl.tplFiles = data.tplFiles;
                ctrl.actTplsHelpText = data.actTplsHelpText;
            });
        };

        ctrl.saveActTplFile = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event, type) {
            if (($event.type === 'change' || $event.type === 'drop') && $files != null && $files.length > 0) {
                ctrl.files = $files;
                Upload.upload({
                    url: 'settingsPartners/saveActReportTplFile',
                    data: { type: type },
                    file: ctrl.files,
                }).then(function (result) {
                    var data = result.data;

                    if (data.result === true) {
                        ctrl.getActTplsData().then(function () {
                            toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                        });
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
            } else if ($invalidFiles.length > 0) {
                toaster.error('Файл не соответствует требованиям');
            }
        };

        ctrl.setDefaultTpl = function (type) {
            SweetAlert.confirm('Вы уверены, что хотите восстановить шаблон акт-отчета по умолчанию? Текущий шаблон будет удален.', {
                title: 'Восстановление шаблона по умолчанию',
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('settingsPartners/resetActReportTpl', { type: type }).then(function (response) {
                        var data = response.data;
                        if (data.result == true) {
                            ctrl.getActTplsData().then(function () {
                                toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                            });
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        };

        ctrl.modalTplDescription = function () {
            var descrCtrl = this;

            descrCtrl.$onInit = function () {
                return $http.post('settingsPartners/GetActReportTplDescription', { type: descrCtrl.$resolve.params.type }).then(function (response) {
                    var data = response.data;
                    if (data.result == true) {
                        descrCtrl.Childs = data.obj.Fields;
                    }
                });
            };
        };

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };

        // endregion
    };

    SettingsPartnersCtrl.$inject = ['$http', '$q', '$window', 'SweetAlert', 'toaster', '$translate', 'Upload', 'isMobileService', '$location'];

    ng.module('settingsPartners', ['fileUploader', 'isMobile', 'as.sortable', 'productsSelectvizr']).controller(
        'SettingsPartnersCtrl',
        SettingsPartnersCtrl,
    );
})(window.angular);
