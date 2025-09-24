(function (ng) {
    'use strict';

    var ReviewsFormLandingCtrl = /* @ngInject */ function ($http, toaster, modalService) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.page = 1;
            ctrl.reviewsData = [];
            ctrl.inProgress = false;
        };

        ctrl.submit = function (blockId) {
            ctrl.formSubmitInProcess = true;

            $http
                .post('landing/landing/addReview', {
                    blockId: blockId,
                    name: ctrl.name,
                    email: ctrl.email,
                    text: ctrl.text,
                    rating: ctrl.rating != null ? ctrl.rating : '0',
                })
                .then(function (response) {
                    var data = response.data.obj,
                        MODAL_ID = 'ReviewsForm' + blockId;

                    modalService.renderModal(
                        MODAL_ID,
                        'Спасибо за отзыв!',
                        data.WithoutPreModerate ? 'Ваш отзыв успешно добавлен.' : 'Отзыв будет добавлен после проверки модератором.',
                        '<div class="text-right"><button type="button" type="button" class="lp-btn lp-btn--primary lp-btn--md"  modal-close="">Ок</button></div>',
                        {
                            destroyOnClose: true,
                            modalClass: 'reviews-form-modal color-scheme--light',
                            isShowFooter: true,
                        },
                        null,
                    );

                    modalService.getModal(MODAL_ID).then(function (modal) {
                        modal.modalScope.open();
                    });

                    ctrl.clearForm();
                })
                .finally(function () {
                    ctrl.formSubmitInProcess = false;
                });
        };

        ctrl.clearForm = function () {
            ctrl.name = '';
            ctrl.email = '';
            ctrl.text = '';
            ctrl.rating = '5';

            ctrl.form.$setPristine();
        };

        ctrl.getItems = function (blockId, page, size) {
            ctrl.inProgress = true;

            return ctrl
                .fetchData(blockId, page, size)
                .then(function (data) {
                    ctrl.reviewsData = ctrl.reviewsData.concat(data.obj);
                    ctrl.page += 1;

                    return data;
                })
                .finally(function () {
                    ctrl.inProgress = false;
                });
        };

        ctrl.fetchData = function (blockId, page, size) {
            return $http
                .get('landing/landing/getPagingFromSettings', { params: { blockId: blockId, page: page, size: size } })
                .then(function (response) {
                    return response.data;
                });
        };
    };

    ng.module('reviewsFormLanding').controller('ReviewsFormLandingCtrl', ReviewsFormLandingCtrl);
})(window.angular);
