(function (ng) {
    'use strict';
    var MODAL_ID_GAllERY = 'modalPictureLoaderGallerCloud';

    var galleryCloudService = function ($http, toaster, modalService) {
        var service = this;

        service.getSearchImages = function (page, term) {
            return $http.get('landinginplace/searchImages', { params: { term: term, page: page, rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };

        service.getPopularImages = function (page) {
            return $http.get('landinginplace/getPopularImages', { params: { page: page, rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };

        service.showModal = function (options) {
            modalService.renderModal(
                MODAL_ID_GAllERY,
                'Выбрать изображение',
                '<gallery-cloud class="gallery-cloud" on-select="modalData.onSelect(photo)"></gallery-cloud>',
                '<div class="gallery-cloud__copyright">Все изображения являются полностью <b> бесплатными</b> для коммерческого использования.Источник <a href="https://www.pexels.com" target="_blank"> www.pexels.com</a></div>',
                { destroyOnClose: true, modalClass: 'gallery-cloud-modal' },
                { modalData: options },
            );
            //}

            modalService.getModal(MODAL_ID_GAllERY).then(function (modal) {
                modal.modalScope.open();
            });
        };

        service.closeModal = function () {
            modalService.close(MODAL_ID_GAllERY);
        };
    };

    ng.module('galleryCloud').service('galleryCloudService', galleryCloudService);

    galleryCloudService.$inject = ['$http', 'toaster', 'modalService'];
})(window.angular);
