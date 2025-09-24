/* @ngInject */
function giftcertificateService(modalService) {
    var service = this;

    service.dialogOpen = function () {
        modalService.open('giftcertificatePreview');
    };

    service.dialogClose = function () {
        modalService.close('giftcertificatePreview');
    };
}

export default giftcertificateService;
