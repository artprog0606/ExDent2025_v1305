import fileUploaderTemplate from './templates/file-uploader.html';
(function (ng) {
    'use strict';

    ng.module('fileUploader').component('fileUploader', {
        templateUrl: fileUploaderTemplate,
        controller: 'FileUploaderCtrl',
        bindings: {
            startSrc: '@',
            uploadUrl: '@',
            uploadParams: '<?',
            deleteUrl: '@',
            deleteParams: '<?',
            uploadbylinkUrl: '@',
            uploadbylinkParams: '<?',
            onUpdate: '&',
            onSuccess: '&',
            onBeforeSend: '&',
            accept: '@',
            showResult: '<?',
            disabled: '<?',
            goToFirstStepAfterSucces: '<?',
            titleUploadButton: '@',
            titleUploadLinkButton: '@',
            titleDeleteButton: '@',
            notSendImmediately: '<?',
            onInit: '&',
        },
    });
})(window.angular);
