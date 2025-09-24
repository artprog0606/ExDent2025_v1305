import videoFileUploaderTemplate from './video-file-uploader.html';
(function (ng) {
    'use strict';

    ng.module('videoFileUploader').directive('videoFileUploader', [
        function () {
            return {
                restrict: 'A',
                transclude: true,
                controller: 'VideoFileUploaderCtrl',
                bindToController: true,
                controllerAs: 'videoFileUploader',
                templateUrl: videoFileUploaderTemplate,
                scope: {
                    settings: '<',
                    pattern: '<?',
                    accept: '@',
                    upload: '&',
                    showProgress: '<?',
                    drop: '<?',
                    dropSize: '<?',
                    uploadUrl: '@',
                    deleteUrl: '@',
                    urlListVideo: '@',
                },
                link: function (scope, element, attrs, ctrl) {
                    ctrl.pattern = ctrl.pattern || 'video/*';
                    ctrl.accept = ctrl.accept || '.webm';
                    ctrl.dropSize = ctrl.dropSize || {
                        width: 200,
                        height: 300,
                    };
                },
            };
        },
    ]);
})(window.angular);
