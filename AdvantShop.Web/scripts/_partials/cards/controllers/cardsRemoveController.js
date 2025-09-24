/* @ngInject */
function CardsRemoveCtrl(cardsService, $translate) {
    var ctrl = this;

    ctrl.remove = function (type) {
        var request;

        switch (type) {
            case 'coupon':
                request = cardsService.deleteCoupon();
                break;
            case 'certificate':
                request = cardsService.deleteCertificate();
                break;
            default:
                throw Error($translate.instant('Js.Cards.NotFoundTypeToRemove'));
        }

        request.then(function () {
            ctrl.applyFn();
        });
    };
}

export default CardsRemoveCtrl;
