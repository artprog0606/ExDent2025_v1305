/* @ngInject */
function BonusWhatToDoCtrl(bonusService, toaster, $translate, checkoutService) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.bonusAvalable = 'not';
        ctrl.activeView = 'none';

        ctrl.isShowPatronymic = ctrl.isShowPatronymic();
        checkoutService.addCallback('shipping', ctrl.init);
        checkoutService.addCallback('address', ctrl.init);
        checkoutService.addCallback('coupon', ctrl.init);
        ctrl.init();
    };

    ctrl.init = function () {
        bonusService.getBonus().then(function (bonus) {
            ctrl.bonusData = bonus;

            if (ctrl.bonusData == null) {
                ctrl.activeView = ctrl.page === 'myaccount' ? 'myaccount_newcart' : 'form';
            } else if (ctrl.bonusData != null && ctrl.bonusData.bonus != null && ctrl.bonusData.bonus.Blocked === true) {
                ctrl.activeView = 'blocked';
            } else {
                if (ctrl.bonusData.maxBonus < ctrl.appliedBonuses) {
                    ctrl.appliedBonuses = ctrl.bonusData.maxBonus;
                    ctrl.changeBonusInterface(ctrl.appliedBonuses);
                }
                ctrl.activeView = ctrl.page === 'checkout' ? 'apply' : 'info';
            }
        });
    };

    ctrl.signIn = function (bonusData) {
        ctrl.bonusData = bonusData;

        ctrl.activeView = ctrl.page === 'checkout' ? 'apply' : 'info';

        ctrl.autorizeBonus({ cardNumber: bonusData.bonus.CardNumber });
    };

    ctrl.changeBonusInterface = function (appliedBonuses) {
        ctrl.changeBonus({ appliedBonuses: appliedBonuses });
    };

    ctrl.createBonusCard = function () {
        bonusService.createBonusCard().then(function (data) {
            if (data.result === true) {
                toaster.pop('success', '', $translate.instant('Js.Bonus.BonusCartCreated'));
            } else {
                toaster.pop('error', '', data.error);
            }
            ctrl.init();
        });
    };
}

export default BonusWhatToDoCtrl;
