import './pictureUploaderModal.html';

(function (ng) {
    'use strict';
    const regexpUrlParams = '\\??[\\w\\d=&~_\\-\\!\\.\\,\\)\\(]*';
    var ModalPictureUploaderCtrl = function ($uibModalInstance, $window, $timeout) {
        var ctrl = this;

        ctrl.$onInit = function () {
            const allowExts = ctrl.$resolve?.options?.allowExts;
            if (ctrl.$resolve != null && allowExts?.length) {
                ctrl.pattern = new RegExp('https?://.*.(?:' + allowExts.join('|') + ')' + regexpUrlParams, 'i');
            } else {
                ctrl.pattern = new RegExp('https?://.*.(?:jpg|gif|png|jpeg)' + regexpUrlParams, 'i');
            }
        };

        ctrl.save = function (url) {
            $uibModalInstance.close(url);
        };

        ctrl.dismiss = function () {
            $uibModalInstance.dismiss('cancel');
        };
        //to do: ngModel не обновляется если мы вставили текст в поле через контекстное меню мышкой
        ctrl.pasteUrl = function ($event) {
            var paste =
                $event.originalEvent.clipboardData && $event.originalEvent.clipboardData.getData
                    ? $event.originalEvent.clipboardData.getData('text/plain') // Standard
                    : $window[0].clipboardData && $window[0].clipboardData.getData
                      ? $window[0].clipboardData.getData('Text') // MS
                      : null;

            $timeout(function () {
                ctrl.url = paste;
            });
        };
    };

    ModalPictureUploaderCtrl.$inject = ['$uibModalInstance', '$window', '$timeout'];

    ng.module('uiModal').controller('ModalPictureUploaderCtrl', ModalPictureUploaderCtrl);
})(window.angular);
