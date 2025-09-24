import bonusInfoTemplate from '../templates/modalEdit.html';

/* @ngInject */
function BonusInfoCtrl(toaster, bonusService, modalService, $translate) {
    var ctrl = this,
        isRenderModal = false;

    ctrl.bonusDataMaster = {};
    ctrl.transactions = [];

    ctrl.save = function () {
        bonusService.updateCard(ctrl.bonusData).then(function (response) {
            if (response.error != null && response.error.length > 0) {
                toaster.pop('error', $translate.instant('Js.Bonus.BonusCartTitle'), response.error);
            } else {
                ctrl.modalDataSave = true;
                ctrl.dialogClose();
                toaster.pop('success', $translate.instant('Js.Bonus.BonusCartTitle'), $translate.instant('Js.Bonus.ChangesSaved'));
            }
        });
    };

    ctrl.dialogOpen = function () {
        ctrl.modalDataSave = false;

        angular.copy(ctrl.bonusData, ctrl.bonusDataMaster);

        if (isRenderModal === false) {
            modalService.renderModal(
                'modalBonusInfo',
                $translate.instant('Js.Bonus.BonusCartTitle'),
                `<div data-ng-include="'${bonusInfoTemplate}'"></div>`,
                '<input data-ng-click="bonusInfo.save()" class="btn btn-middle btn-buy" type="button" value="' +
                    $translate.instant('Js.Bonus.Save') +
                    '">',
                {
                    isOpen: true,
                    modalClass: 'modal-bonus-info',
                    callbackClose: 'bonusInfo.dialogReset',
                },
                {
                    bonusInfo: ctrl,
                },
            );

            isRenderModal = true;
        } else {
            modalService.open('modalBonusInfo');
        }
    };

    ctrl.dialogClose = function () {
        modalService.close('modalBonusInfo');
    };

    ctrl.dialogReset = function () {
        if (ctrl.modalDataSave === false) {
            angular.copy(ctrl.bonusDataMaster, ctrl.bonusData);
        }
    };

    ctrl.setTransactions = function (mode = ctrl.bonusData.TransactionsMode) {
        switch (mode) {
            case 0:
                ctrl.transactions = ctrl.bonusData.Transactions;
                ctrl.bonusData.TransactionsMode = 0;
                break;
            case 1:
                ctrl.transactions = ctrl.bonusData.Transactions.filter((transaction) => transaction.IsAdd === true);
                ctrl.bonusData.TransactionsMode = 1;
                break;
            case 2:
                ctrl.transactions = ctrl.bonusData.Transactions.filter((transaction) => transaction.IsAdd === false);
                ctrl.bonusData.TransactionsMode = 2;
                break;
        }
    };
}

export default BonusInfoCtrl;
