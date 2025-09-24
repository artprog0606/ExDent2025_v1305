import quckviewModalTemplate from '../templates/quckviewModal.html';

/* @ngInject */
function quickviewService(modalService, $location, $timeout, urlHelper) {
    var service = this,
        needOpenDialogId,
        dialogIdOpen;

    service.dialogRender = function (parentScope) {
        modalService.renderModal(
            parentScope.modalId || 'modalQuickView',
            null,
            `<div data-ng-include="'${quckviewModalTemplate}'"></div>`,
            null,
            {
                isOpen: false,
                modalClass: 'modal-quickview' + ' ' + (parentScope.modalClass || ''),
                backgroundEnable: true,
                modalOverlayClass: 'modal-quickview-wrap',
                spyAddress: parentScope.spyAddress,
                anchor: parentScope.modalId || 'modalQuickView',
                callbackOpen: parentScope.spyAddress ? 'quickview.onOpenModalCallback()' : '',
                callbackClose: 'quickview.onModalClose()',
                destroyOnClose: true,
            },
            { quickview: parentScope },
        );
        modalService.getModal(parentScope.modalId || 'modalQuickView').then(function (modal) {
            modal.modalScope.open();
        });
    };

    service.getUrl = function (
        productId,
        colorId,
        typeView,
        landingId,
        hideShipping,
        showLeadButton,
        blockId,
        showVideo,
        sizeId,
        descriptionMode,
        cartAddType,
    ) {
        return (
            'product/productquickview' +
            '?productId=' +
            productId +
            (colorId != null ? '&color=' + colorId : '') +
            (sizeId != null ? '&size=' + sizeId : '') +
            (typeView != null ? '&from=' + typeView : '') +
            (landingId != null ? '&landingId=' + landingId : '') +
            (hideShipping != null ? '&hideShipping=' + hideShipping : '') +
            (showLeadButton != null ? '&showLeadButton=' + showLeadButton : '') +
            (blockId != null ? '&blockId=' + blockId : '') +
            (showVideo != null ? '&showVideo=' + showVideo : '') +
            (descriptionMode != null ? '&descriptionMode=' + descriptionMode : '') +
            (cartAddType != null ? '&cartAddType=' + cartAddType : '') +
            '&rnd=' +
            Math.random()
        );
    };

    service.dialogOpen = function (
        itemData,
        productId,
        colorId,
        typeView,
        modalClass,
        landingId,
        hideShipping,
        showLeadButton,
        blockId,
        showVideo,
        modalId,
        openFromHash,
        sizeId,
        onOpenModalCallback,
        spyAddress,
        descriptionMode,
        cartAddType,
    ) {
        dialogIdOpen = modalId;
        const data = {};
        data.url = service.getUrl(
            productId,
            colorId,
            typeView,
            landingId,
            hideShipping,
            showLeadButton,
            blockId,
            showVideo,
            sizeId,
            descriptionMode,
            cartAddType,
        );
        data.itemData = itemData;
        data.productId = productId;
        data.next = service.next;
        data.prev = service.prev;
        data.modalClass = modalClass;
        data.typeView = typeView;
        data.landingId = landingId;
        data.hideShipping = hideShipping;
        data.showLeadButton = showLeadButton;
        data.blockId = blockId;
        data.showVideo = showVideo;
        data.modalId = modalId;
        data.onOpenModalCallback = onOpenModalCallback;
        data.spyAddress = spyAddress;
        data.descriptionMode = descriptionMode;
        data.onModalClose = () => {
            data.url = null;
            modalService.destroy('modalProductRotate');

            $location.search({ color: undefined, size: undefined });
            //remove hash
            history.pushState(null, document.title, location.pathname + location.search);

            dialogIdOpen = null;
        };
        data.cartAddType = cartAddType;

        var hash = $location.hash();
        var splitedHash = hash.split('?');
        var originalHash = splitedHash[0];

        if (!modalService.hasModal(modalId)) {
            service.dialogRender(data);
        } else {
            modalService.open(modalId);
        }

        if (modalId != null) {
            $timeout(() => $location.hash(modalId), 100);
        }

        service.removeNeedOpenDialog();
    };

    service.dialogClose = function (modalQuickViewId) {
        modalService.close(modalQuickViewId || 'modalQuickView');
        dialogIdOpen = null;
    };

    service.goTo = function (quickview, index) {
        if (quickview.itemData.siblings[index] != null) {
            quickview.productId = quickview.itemData.siblings[index];
            if (quickview.itemData.modalIds[quickview.productId] != null) {
                $location.hash(quickview.itemData.modalIds[quickview.productId]);
            }
            $location.search({ color: undefined, size: undefined });
            quickview.url = service.getUrl(
                quickview.productId,
                null,
                quickview.typeView,
                quickview.landingId,
                quickview.hideShipping,
                quickview.showLeadButton,
                quickview.blockId,
                quickview.showVideo,
                null,
                quickview.descriptionMode,
                quickview.cartAddType,
            );
        }
    };

    service.prev = function (quickview) {
        modalService.destroy('modalProductRotate');
        service.goTo(quickview, quickview.itemData.siblings.indexOf(quickview.productId) - 1);
    };

    service.next = function (quickview) {
        modalService.destroy('modalProductRotate');
        service.goTo(quickview, quickview.itemData.siblings.indexOf(quickview.productId) + 1);
    };

    service.dialogIsExist = function (modalQuickViewId) {
        return modalService.hasModal(modalQuickViewId) || needOpenDialogId != null;
    };

    service.checkDialogOpenById = function (modalQuickViewId) {
        return dialogIdOpen === modalQuickViewId;
    };

    service.needOpenDialog = function (modalQuickViewId) {
        needOpenDialogId = modalQuickViewId;
    };

    service.removeNeedOpenDialog = function () {
        needOpenDialogId = null;
    };
}

export default quickviewService;
