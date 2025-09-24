import * as tpls from '../templates/iframeResponsiveTemplates.js';

/*@ngInject*/
export default function iframeResponsiveDirective(iframeResponsiveService, $templateRequest, $compile, urlHelper) {
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
            loop: '<?',
            disabledStop: '<?',
            fromUpload: '<?',
            asBackground: '<?',
        },
        transclude: true,
        link: function (scope, element, attrs, ctrl) {
            //$templateRequest возможно использовать
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
            ctrl.useVk = ctrl.src.indexOf('vk.com') !== -1;
            ctrl.isVkClip = iframeResponsiveService.checkIsVkClip(ctrl.src);
            ctrl.useDzen = ctrl.src.indexOf('dzen.ru') !== -1;
            ctrl.isPlayerCode = iframeResponsiveService.isPlayerCode(ctrl.src);
            ctrl.deviceMobile = /Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent);

            ctrl.getContentUrl = function () {
                ctrl.tplUrl = tpls.iframeResponsiveVideoTemplate;

                if (ctrl.isPlayerCode) {
                    ctrl.tplUrl = tpls.iframeResponsivePlayerCodeTemplate;
                }

                if (ctrl.fromUpload) {
                    ctrl.tplUrl = tpls.iframeResponsiveUploadTemplate;
                }

                if (ctrl.asBackground === true) {
                    ctrl.tplUrl = tpls.iframeResponsiveVideoBackgroundTemplate;
                }

                if (ctrl.fromUpload === true && ctrl.inModal === true) {
                    ctrl.tplUrl = tpls.iframeResponsiveUploadModalTemplate;
                }

                return ctrl.tplUrl;
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
            '<div data-lozad-adv="$ctrl.showContent()" data-ng-class="{\'iframe-responsive--vk-clip\': $ctrl.useVk && $ctrl.isVkClip}" class="iframe-responsive__container-wrap"><div class="iframe-responsive__container embed-container ng-cloak" data-ng-if="!$ctrl.deviceMobile && !$ctrl.asBackground || !ctrl.fromUpload" data-ng-class="{\'iframe-responsive__container-upload\': $ctrl.fromUpload}" data-ng-include="$ctrl.getContentUrl()"></div><div ng-style="{\'background-image\':\'url(\'+$ctrl.coverVideoPath+\')\'}" class="ng-cloak iframe-responsive__container--image" data-ng-if="$ctrl.asBackground"></div></div>',
    };
}
