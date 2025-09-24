/* @ngInject */
function InplaceRichCtrl(inplaceService, $q, $scope, $element, toaster, $translate) {
    var ctrl = this;
    ctrl.callbacks = [];
    //ctrl.inplaceParams = ctrl.inplaceParams();

    ctrl.active = function () {
        ctrl.isShow = true;
        ctrl.startContent = ctrl.editor.getData();
    };

    ctrl.destroy = function () {
        if (ctrl.buttons) {
            ctrl.buttons.element.remove();
        }

        if (ctrl.editor) {
            ctrl.editor.destroy();
            $scope.$destroy();
        }

        $element.removeAttr('contenteditable');
    };

    ctrl.save = function (content) {
        var params;

        if (ctrl.startContent === content) {
            return true;
        }

        if (ctrl.inplaceUrl != null) {
            params = angular.extend(ctrl.getParams(), { content: content });

            return inplaceService.save(ctrl.inplaceUrl, params).finally(function () {
                ctrl.isShow = false;

                if (ctrl.inplaceOnSave != null) {
                    ctrl.inplaceOnSave({ value: content, $scope: $scope });
                }
            });
        } else {
            if (ctrl.inplaceOnSave != null) {
                return ctrl.inplaceOnSave({ value: content, $scope: $scope });
            }
        }

        return true;
    };

    ctrl.cancel = function () {
        ctrl.isShow = false;
        ctrl.editor.setData(ctrl.startContent);
    };

    ctrl.addCallback = function (callback) {
        ctrl.callbacks.push(callback);
    };

    ctrl.callCallbacks = function (callbacksArray) {
        callbacksArray.forEach(function (callback) {
            callback();
        });
    };
}

export default InplaceRichCtrl;
