(function (ng) {
    'use strict';
    ng.module('modalVideo')
        //.component('modalVideo', {
        //    controller: 'ModalVideoCtrl',
        //    controllerAs: 'modalVideo',
        //    bindToController: true,
        //    bindings: {
        //        src: '@',
        //        fromUpload: '<?'
        //    }
        //});
        .directive('modalVideo', [
            '$window',
            'modalVideoService',
            function () {
                return {
                    controller: 'ModalVideoCtrl',
                    controllerAs: 'modalVideo',
                    bindToController: {
                        src: '@',
                        fromUpload: '<?',
                        videoCover: '@',
                    },
                    scope: true,
                    link: function (scope, elem, attrs, ctrl) {
                        ctrl.htmlForMobile =
                            '<iframe-responsive  data-from-upload="' +
                            ctrl.fromUpload +
                            '" data-video-cover="' +
                            ctrl.videoCover +
                            '" src="' +
                            ctrl.src +
                            '"></iframe-responsive>';
                    },
                };
            },
        ]);
})(window.angular);
