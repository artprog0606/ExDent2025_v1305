import { PubSub } from '../../../_common/PubSub/PubSub.js';
/* @ngInject */
function buyOneClickService($http, $sce, modalService) {
    var service = this,
        modalId = 'modalBuyOneClick',
        isRenderDialog = false;

    service.showDialog = function (modalId) {
        modalService.open(modalId);
        PubSub.publish('buy_one_click_pre');
    };

    service.getFieldsOptions = function () {
        return $http.get('checkout/checkoutbuyinoneclickfields').then(function (response) {
            return response.data;
        });
    };

    service.getCustomerInfo = function () {
        return $http.get('checkout/checkoutbuyinoneclickcustomer').then(function (response) {
            return response.data;
        });
    };

    service.modalFooterShow = function (modalId, show) {
        modalService.setVisibleFooter(modalId, show);
    };

    service.checkout = function (
        page,
        orderType,
        offerId,
        productId,
        amount,
        attributesXml,
        name,
        email,
        phone,
        comment,
        captchaCode,
        captchaSource,
        isAgreeForPromotionalNewsletter,
    ) {
        let params = {
            page: page,
            orderType: orderType,
            offerId: offerId,
            productId: productId,
            amount: amount,
            attributesXml: attributesXml,
            name: name,
            email: email,
            phone: phone,
            comment: comment,
            captchaCode: captchaCode,
            captchaSource: captchaSource,
            isAgreeForPromotionalNewsletter: isAgreeForPromotionalNewsletter,
        };

        return $http.post('checkout/checkoutbuyinoneclick', params).then(function (response) {
            if (response.data.error === null || response.data.length === 0) {
                PubSub.publish('buy_one_click_confirm');
            }

            return response.data;
        });
    };
}

export default buyOneClickService;
