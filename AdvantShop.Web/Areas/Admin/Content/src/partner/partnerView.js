(function (ng) {
    const PartnerViewCtrl = /* @ngInject */ function ($http, SweetAlert, $window, toaster, $translate, uiTabsService) {
        const ctrl = this;

        ctrl.init = function (partnerId) {
            ctrl.partnerId = partnerId;

            localStorage.removeItem(uiTabsService.getNameForStorage());

            ctrl.getPartnerView();
        };

        ctrl.getPartnerView = function () {
            $http.get('partners/getView', { params: { id: ctrl.partnerId } }).then((response) => {
                const { data } = response;
                if (data.result === true) {
                    ctrl.instance = data.obj;
                } else {
                    window.location.assign('partners');
                }
            });
        };

        ctrl.delete = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then((result) => {
                if (result === true || result.value) {
                    $http.post('partners/deletePartner', { id: ctrl.partnerId }).then((response) => {
                        const { data } = response;
                        if (data.result === true) {
                            $window.location.assign('partners');
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        };

        ctrl.updateAdminComment = function (comment) {
            $http
                .post('partners/updateAdminComment', {
                    partnerId: ctrl.partnerId,
                    comment,
                })
                .then((response) => {
                    const { data } = response;
                    if (data.result === true) {
                        toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.addPartnerCoupon = function (couponId) {
            $http
                .post('partners/addPartnerCoupon', {
                    partnerId: ctrl.partnerId,
                    couponId,
                })
                .then((response) => {
                    const { data } = response;
                    if (data.result === true) {
                        ctrl.getPartnerView();
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.deletePartnerCoupon = function (couponId) {
            SweetAlert.confirm('Вы уверены, что хотите удалить купон партнера?', {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then((result) => {
                if (result === true || result.value) {
                    $http.post('coupons/deleteCoupon', { couponId }).then((response) => {
                        const { data } = response;
                        if (data.result === true) {
                            ctrl.getPartnerView();
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
                }
            });
        };

        ctrl.onMoneyProcessed = function () {
            ctrl.getPartnerView();
            if (ctrl.gridTransactions) {
                ctrl.gridTransactions.fetchData();
            }
        };

        ctrl.isNullOrEmpty = function (str) {
            return str == null || str.length == 0;
        };

        ctrl.onSelectTab = function (indexTab) {
            ctrl.tabActiveIndex = indexTab;
        };

        ctrl.getPartnerCustomersCtrl = function (partnerCustomersCtrl) {
            ctrl.partnerCustomersCtrl = partnerCustomersCtrl;
        };

        ctrl.getPartnerActReportsCtrl = function (partnerActReportsCtrl) {
            ctrl.partnerActReportsCtrl = partnerActReportsCtrl;
        };
    };

    ng.module('partnerView', ['uiGridCustom', 'partnerCustomers', 'partnerTransactions', 'partnerActReports', 'isMobile']).controller(
        'PartnerViewCtrl',
        PartnerViewCtrl,
    );
})(window.angular);
