/* @ngInject */
function preOrderService($http, $sce, modalService, toaster) {
    var service = this,
        modalId = 'modalPreOrder',
        isRenderDialog = false;

    service.showDialog = function (modalId) {
        modalService.open(modalId);
    };

    service.getFormData = function () {
        return $http.get('checkout/getpreorderformdata').then(function (response) {
            return response.data;
        });
    };

    service.modalFooterShow = function (modalId, show) {
        modalService.setVisibleFooter(modalId, show);
    };

    service.send = function (data) {
        return $http.post('checkout/checkoutpreorder', data).then(function (response) {
            return response.data;
        });
    };
}

export default preOrderService;
