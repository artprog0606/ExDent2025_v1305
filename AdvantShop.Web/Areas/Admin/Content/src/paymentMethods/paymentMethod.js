(function (ng) {
    'use strict';

    var PaymentMethodCtrl = function ($location, $window, toaster, SweetAlert, $http, Upload, $translate) {
        var ctrl = this;

        ctrl.init = function (methodId, icon) {
            ctrl.methodId = methodId;
            ctrl.icon = icon;

            ctrl.getAvailableLocations();
        };

        ctrl.getAvailableLocations = function () {
            $http.get('paymentMethods/getAvailableLocations', { params: { methodId: ctrl.methodId } }).then(function (response) {
                var data = response.data;
                ctrl.AvailableCountries = data.countries;
                ctrl.AvailableCities = data.cities;
            });
        };

        ctrl.deleteAvailableCountry = function (countryId) {
            $http
                .post('paymentMethods/deleteAvailableCountry', { methodId: ctrl.methodId, countryId: countryId })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.PaymentMethods.ChangesSuccessfullySaved'));
                    }
                })
                .then(ctrl.getAvailableLocations);
        };

        ctrl.deleteAvailableCity = function (cityId) {
            $http
                .post('paymentMethods/deleteAvailableCity', { methodId: ctrl.methodId, cityId: cityId })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.PaymentMethods.ChangesSuccessfullySaved'));
                    }
                })
                .then(ctrl.getAvailableLocations);
        };

        ctrl.addAvailableCountry = function () {
            $http
                .post('paymentMethods/addAvailableCountry', {
                    methodId: ctrl.methodId,
                    countryName: ctrl.newAvailableCountry,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.newAvailableCountry = '';
                        toaster.pop('success', '', $translate.instant('Admin.Js.PaymentMethods.ChangesSuccessfullySaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.PaymentMethods.AddingImpossible') + ctrl.newAvailableCountry + '" ');
                    }
                })
                .then(ctrl.getAvailableLocations);
        };

        ctrl.selectAvailableCity = function (item) {
            if (item == null) return;
            ctrl.newAvailableCity = {
                id: item.CityId,
                name: item.City + (item.Region && item.Region.length ? ' (' + item.Region + ')' : ''),
            };
        };

        ctrl.addAvailableCity = function () {
            $http
                .post('paymentMethods/addAvailableCity', {
                    methodId: ctrl.methodId,
                    cityName: ctrl.newAvailableCity.name,
                    cityId: ctrl.newAvailableCity.id,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        ctrl.newAvailableCity = {};
                        toaster.pop('success', '', $translate.instant('Admin.Js.PaymentMethods.ChangesSuccessfullySaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.PaymentMethods.AddingImpossible') + ctrl.newAvailableCity.name + '" ');
                    }
                })
                .then(ctrl.getAvailableLocations);
        };

        ctrl.findCity = function (val) {
            return $http.get('cities/getCitiesAutocompleteExt', { params: { q: val, rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };

        ctrl.uploadIcon = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if (($event.type === 'change' || $event.type === 'drop') && $file != null) {
                ctrl.sendIcon($file);
            } else if ($invalidFiles.length > 0) {
                toaster.pop(
                    'error',
                    $translate.instant('Admin.Js.PaymentMethods.ErrorWhileUploading'),
                    $translate.instant('Admin.Js.PaymentMethods.FileNotMeet'),
                );
            }
        };

        ctrl.deleteIcon = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.PaymentMethods.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.PaymentMethods.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    return $http.post('paymentMethods/deleteIcon', { methodId: ctrl.methodId }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            ctrl.icon = null;
                            toaster.pop('success', '', $translate.instant('Admin.Js.PaymentMethods.ImageDeleted'));
                        } else {
                            toaster.pop('error', $translate.instant('Admin.Js.PaymentMethods.ErrorWhileDeleting'), data.error);
                        }
                    });
                }
            });
        };

        ctrl.sendIcon = function (file) {
            return Upload.upload({
                url: 'paymentMethods/uploadIcon',
                data: {
                    file: file,
                    methodId: ctrl.methodId,
                    rnd: Math.random(),
                },
            }).then(function (response) {
                var data = response.data;

                if (data.Result === true) {
                    ctrl.icon = data.Picture;
                    toaster.pop('success', '', $translate.instant('Admin.Js.PaymentMethods.ImageSaved'));
                } else {
                    toaster.pop('error', $translate.instant('Admin.Js.PaymentMethods.ErrorWhileUploading'), data.error);
                }
            });
        };

        ctrl.deleteMethod = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.PaymentMethods.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.PaymentMethods.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('paymentMethods/deleteMethod', { methodId: ctrl.methodId }).then(function (response) {
                        $window.location.assign('settings/paymentMethods');
                    });
                }
            });
        };

        ctrl.findChoseItem = function (arr, comparer) {
            return arr.find((it) => it.Value === comparer);
        };

        ctrl.uiSelectGroupByFn = function (item) {
            if (item.Group && item.Group.Name) return item.Group.Name;
        };

        ctrl.containsInArrayElement = function (arr, text) {
            if (arr == null) return false;
            return arr.some((x) => x.indexOf(text) != -1);
        };
    };

    PaymentMethodCtrl.$inject = ['$location', '$window', 'toaster', 'SweetAlert', '$http', 'Upload', '$translate'];

    ng.module('paymentMethod', ['checklist-model']).controller('PaymentMethodCtrl', PaymentMethodCtrl);
})(window.angular);
