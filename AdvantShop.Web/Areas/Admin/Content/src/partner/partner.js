(function (ng) {
    'use strict';

    var PartnerCtrl = function ($http, SweetAlert, $window, toaster, $translate) {
        var ctrl = this,
            timerSaveChanges;

        ctrl.initPartner = function (partnerId, isEditMode) {
            ctrl.instance = {};
            ctrl.instance.isEditMode = isEditMode;
            ctrl.instance.partnerId = partnerId;
            ctrl.instance.partner = {};
            ctrl.instance.partner.id = partnerId;
        };

        ctrl.delete = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('partners/deletePartner', { id: ctrl.instance.partnerId }).then(function (response) {
                        var data = response.data;
                        if (data.result === true) {
                            $window.location.assign('partners');
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        };

        ctrl.savePartner = function (form) {
            $http.post('partners/savePopup', ctrl.instance).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    if (!ctrl.instance.isEditMode && data.obj != null) {
                        $window.location.assign('partners/view/' + data.obj);
                    } else {
                        form.$setPristine();
                    }
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };

        ctrl.changeEnabled = function (checked) {
            ctrl.instance.partner.enabled = checked;
        };

        ctrl.cancelPopap = function () {
            console.log(ctrl);
        };
    };

    PartnerCtrl.$inject = ['$http', 'SweetAlert', '$window', 'toaster', '$translate'];

    ng.module('partner', ['uiGridCustom']).controller('PartnerCtrl', PartnerCtrl);
})(window.angular);
