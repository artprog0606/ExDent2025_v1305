(function (ng) {
    'use strict';
    ng.module('iframeResponsive').directive('iframeResponsive', [
        'iframeResponsiveService',
        '$templateRequest',
        '$compile',
        function (iframeResponsiveService, $templateRequest, $compile) {
            return {
                controller: 'IframeResponsiveCtrl',
                controllerAs: '$ctrl',
                bindToController: true,
                priority: 100,
                scope: {
                    src: '@',
                    videoWidth: '@',
                    videoHeight: '@',
                    autoplay: '<?',
                    inModal: '<?',
                    loop: '<?',
                    disabledStop: '<?',
                    fromUpload: '<?',
                    asBackground: '<?',
                },
                transclude: true,
                link: function (scope, element, attrs, ctrl) {
                    //$templateRequest возможно использовать
                    ctrl.inModal = ctrl.inModal === true;
                    ctrl.fromUpload = ctrl.fromUpload === true;
                    ctrl.asBackground = ctrl.asBackground === true;

                    ctrl.videoLoaded = null;
                    ctrl.playerCode = null;
                    ctrl.visibleVideo = true;
                    ctrl.visibleCover = true;
                    ctrl.stylesPlayIcon = {};

                    ctrl.useYouTube = ctrl.src.indexOf('youtu.be') !== -1 || ctrl.src.indexOf('youtube.com') !== -1;
                    ctrl.useVimeo = ctrl.src.indexOf('vimeo.com') !== -1;
                    ctrl.useRutube = ctrl.src.indexOf('rutube.ru') !== -1;
                    ctrl.isPlayerCode = iframeResponsiveService.isPlayerCode(ctrl.src);
                    ctrl.deviceMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);

                    ctrl.getContentUrl = function () {
                        ctrl.nameTemplate = 'Video';

                        if (ctrl.isPlayerCode) {
                            ctrl.nameTemplate = 'PlayerCode';
                        }

                        if (ctrl.fromUpload) {
                            ctrl.nameTemplate = 'Upload';
                        }

                        if (ctrl.inModal === true) {
                            ctrl.nameTemplate = 'InModal';
                        }

                        if (ctrl.asBackground === true) {
                            ctrl.nameTemplate = 'VideoBackground';
                        }

                        if (ctrl.fromUpload === true && ctrl.inModal === true) {
                            ctrl.nameTemplate = 'UploadModal';
                        }

                        return '../areas/admin/content/src/_shared/iframe-responsive/iframeResponsive' + ctrl.nameTemplate + '.html';
                    };

                    $templateRequest(ctrl.getContentUrl()).then(function (html) {
                        if (ctrl.fromUpload) {
                            var template = angular.element(html);
                            var video = template[0].querySelector('video');
                            if (video) {
                                if (ctrl.loop) {
                                    video.setAttribute('loop', null);
                                }

                                if (ctrl.autoplay || ctrl.inModal) {
                                    video.setAttribute('autoplay', null);
                                    video.setAttribute('mute', null);
                                }
                            }

                            ctrl.showContent();
                            $compile(template)(scope);
                        }
                    });
                },
                template:
                    '<div data-lozad-adv="$ctrl.showContent()" class="iframe-responsive__container-wrap"><div class="iframe-responsive__container embed-container ng-cloak" data-ng-if="!$ctrl.deviceMobile && !$ctrl.asBackground || !ctrl.fromUpload" data-ng-class="{\'iframe-responsive__container-upload\': $ctrl.fromUpload}" data-ng-include="$ctrl.getContentUrl()"></div><div ng-style="{\'background-image\':\'url(\'+$ctrl.coverVideoPath+\')\'}" class="ng-cloak iframe-responsive__container--image" data-ng-if="$ctrl.asBackground"></div></div>',
            };
        },
    ]);
})(window.angular);
