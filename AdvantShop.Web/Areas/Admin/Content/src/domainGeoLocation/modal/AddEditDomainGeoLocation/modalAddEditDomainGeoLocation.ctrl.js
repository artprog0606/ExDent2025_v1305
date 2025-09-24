(function (ng) {
    'use strict';

    var ModalAddEditDomainGeoLocationCtrl = function ($uibModalInstance, domainGeoLocationService, toaster, $translate) {
        var ctrl = this;

        ctrl.$onInit = function () {
            var params = ctrl.$resolve;
            ctrl.id = params.Id != null ? params.Id : 0;
            ctrl.mode = ctrl.id ? 'edit' : 'add';

            if (ctrl.mode === 'edit') {
                ctrl.get(ctrl.id);
            } else {
                ctrl.domain = {
                    Cities: [],
                };
            }
        };

        ctrl.close = function () {
            $uibModalInstance.dismiss('cancel');
        };

        ctrl.get = function (id) {
            return domainGeoLocationService.get(id).then(function (data) {
                if (data.result === true) {
                    ctrl.domain = data.obj;
                }
            });
        };

        ctrl.addCities = function (result) {
            if (result == null || result.cities == null || result.cities.length == 0) {
                return;
            }

            for (let i = 0; i < result.cities.length; i++) {
                if (
                    ctrl.domain.Cities.find(function (x) {
                        return x.CityId == result.cities[i].CityId;
                    }) == null
                ) {
                    ctrl.domain.Cities.push(result.cities[i]);
                }
            }
        };

        ctrl.removeCity = function (city) {
            ctrl.domain.Cities = ctrl.domain.Cities.filter(function (x) {
                return x.CityId != city.CityId;
            });
        };

        ctrl.save = function () {
            if (ctrl.domain.Cities == null || ctrl.domain.Cities.length == 0) {
                toaster.pop('error', '', $translate.instant('Admin.Js.AddEditDomainGeoLocation.Error.City'));
                return;
            }

            if (ctrl.domain.Url == null || ctrl.domain.Url.length == 0) {
                toaster.pop('error', '', $translate.instant('Admin.Js.AddEditDomainGeoLocation.Error.Domain'));
                return;
            }

            if (ctrl.domain.GeoName == null || ctrl.domain.GeoName.length == 0) {
                toaster.pop('error', '', 'Укажите Geo Name');
                return;
            }

            // https://stackoverflow.com/a/30007882/8211722
            const regex = new RegExp('(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]', 'g');
            const result = regex.exec(ctrl.domain.Url);

            if (result == null || result.length !== 1 || result[0] !== result.input) {
                toaster.pop(
                    'error',
                    '',
                    $translate.instant('Admin.Js.AddEditDomainGeoLocation.Error.NonValidDomain') +
                        ' <a href="https://www.punycoder.com/" target="_blank">Punycode</a>',
                );
                return;
            }

            let promise = ctrl.mode === 'add' ? domainGeoLocationService.add(ctrl.domain) : domainGeoLocationService.update(ctrl.domain);

            promise.then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.AddEditDomainGeoLocation.Success'));
                    $uibModalInstance.close();
                } else {
                    data.errors.forEach(function (error) {
                        toaster.pop('error', error);
                    });

                    if (!data.errors) {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.AddEditDomainGeoLocation.Error'),
                            $translate.instant('Admin.Js.AddEditDomainGeoLocation.Error.Save'),
                        );
                    }
                }
            });
        };
    };

    ModalAddEditDomainGeoLocationCtrl.$inject = ['$uibModalInstance', 'domainGeoLocationService', 'toaster', '$translate'];

    ng.module('uiModal').controller('ModalAddEditDomainGeoLocationCtrl', ModalAddEditDomainGeoLocationCtrl);
})(window.angular);
