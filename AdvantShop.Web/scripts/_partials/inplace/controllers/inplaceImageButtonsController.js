/* @ngInject */
function InplaceImageButtonsCtrl($element, inplaceService) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.inplaceImage = inplaceService.getInplaceImage(ctrl.inplaceImageButtons);

        ctrl.inplaceImage.onLoadButtons(ctrl, $element);
    };

    ctrl.add = function (files, event) {
        if (event.type === 'change') {
            //ng-upload-file call callback two events: click and change
            ctrl.inplaceImage.fileChange(files, event, 'add');
        }
    };

    ctrl.update = function (files, event) {
        if (event.type === 'change') {
            //ng-upload-file call callback two events: click and change
            ctrl.inplaceImage.fileChange(files, event, 'update');
        }
    };

    ctrl.delete = function (event) {
        ctrl.inplaceImage.fileChange(null, event, 'delete');
    };
}

export default InplaceImageButtonsCtrl;
