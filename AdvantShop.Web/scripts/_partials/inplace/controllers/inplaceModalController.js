import inplaceModalTemplate from '../templates/inplaceModal.html';
import inplaceModalButtonsTemplate from '../templates/inplaceModalButtons.html';

/* @ngInject */
function InplaceModalCtrl($http, $q, $window, inplaceService, modalService, $translate) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.inplaceParams = ctrl.inplaceParams();
    };

    ctrl.modalOpen = function () {
        var modal, data;

        if (modalService.hasModal('inplaceModal') === false) {
            modalService.renderModal(
                'inplaceModal',
                $translate.instant('Js.Inplace.SeoSettings'),
                `<div data-ng-include="'${inplaceModalTemplate}'"></div>`,
                `<div data-ng-include="'${inplaceModalButtonsTemplate}'"></div>`,
                null,
                {
                    inplace: ctrl,
                },
            );
        }

        modal = modalService.getModal('inplaceModal');

        data = $http.get('inplaceeditor/getmeta', {
            params: angular.extend(ctrl.inplaceParams, { rnd: Math.random() }),
        });

        $q.all([modal, data]).then(function (results) {
            modalService.open('inplaceModal');

            ctrl.startData = results[1].data;
            ctrl.current = results[1].data;

            results[0].modalScope.formData = ctrl.current;
        });

        ctrl.apply = function () {
            inplaceService.save(ctrl.inplaceUrl, angular.extend(ctrl.inplaceParams, ctrl.current)).then(function () {
                $window.location.reload();
            });
        };
    };

    ctrl.reset = function () {
        modalService.close('inplaceModal');
        ctrl.current = ctrl.startData;
    };
}

export default InplaceModalCtrl;
