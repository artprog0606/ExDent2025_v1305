(function (ng) {
    'use strict';

    var countId = 0;

    ng.module('advBaguetteBox').directive('advBaguetteBox', [
        '$timeout',
        function ($timeout) {
            return {
                restrict: 'AE',
                controller: 'AdvBaguetteBoxCtrl',
                controllerAs: 'advBaguetteBox',
                bindToController: {
                    captionsFn: '&',
                    captions: '<?',
                    buttons: '<?',
                    fullScreen: '<?',
                    noScrollbars: '<?',
                    titleTag: '<?',
                    async: '<?',
                    preload: '<?',
                    ignoreClass: '@',
                    animation: '<?',
                    afterShow: '&',
                    afterHide: '&',
                    onChange: '&',
                    overlayBackgroundColor: '@',
                    filter: '@',
                    //опции самого компонента
                    onInit: '&',
                },
                scope: true,
                link: function (scope, element, attrs, ctrl) {
                    var idStr = 'baguettDef',
                        flags,
                        pattern,
                        id,
                        options;

                    if (attrs.id) {
                        id = attrs.id;
                    } else {
                        id = idStr + countId;
                        attrs.$set('id', id);
                        countId++;
                    }

                    ctrl.id = id;

                    if (ctrl.filter != null) {
                        flags = ctrl.filter.replace(/.*\/([gimy]*)$/, '$1');
                        pattern = ctrl.filter.replace(new RegExp('^/(.*?)/' + flags + '$'), '$1');
                    }

                    options = {
                        buttons: ctrl.buttons != null ? ctrl.buttons : 'auto',
                        fullScreen: ctrl.fullScreen != null ? ctrl.fullScreen : false,
                        noScrollbars: ctrl.noScrollbars != null ? ctrl.noScrollbars : false,
                        titleTag: ctrl.titleTag != null ? ctrl.titleTag : false,
                        async: ctrl.async != null ? ctrl.async : false,
                        preload: ctrl.preload ? ctrl.preload : 2,
                        ignoreClass: ctrl.ignoreClass,
                        animation: ctrl.animation != null ? ctrl.animation : 'slideIn',
                        afterShow: ctrl.afterShow ? ctrl.afterShow : null,
                        afterHide: ctrl.afterHide ? ctrl.afterHide : null,
                        onChange: function (currentIndex, imagesCount) {
                            ctrl.onChange({ currentIndex: currentIndex, imagesCount: imagesCount });
                        },
                        overlayBackgroundColor: ctrl.overlayBackgroundColor !== 0 ? ctrl.overlayBackgroundColor : 'rgba(0,0,0,0.8)',
                        filter: ctrl.filter ? new RegExp(pattern, flags) : /.+\.(gif|jpe?g|png|webp)/i,
                    };

                    if (ctrl.captions != null) {
                        options.captions = ctrl.captions != null ? ctrl.captions : true;
                    } else if (ctrl.captionsFn != null && attrs.captionsFn != null && attrs.captionsFn.length > 0) {
                        options.captions = function (el) {
                            ctrl.captionsFn({ el: el });
                        };
                    } else {
                        options.captions = true;
                    }

                    ctrl.options = options;

                    $timeout(function () {
                        ctrl.init(ctrl);
                    });
                },
            };
        },
    ]);
})(window.angular);
