import pictureLoaderTemplate from './../templates/picture-loader.html';
(function (ng) {
    'use strict';

    ng.module('pictureLoader').directive('pictureLoader', [
        '$parse',
        function ($parse) {
            return {
                controller: 'PictureLoaderCtrl',
                controllerAs: 'pictureLoader',
                bindToController: true,
                templateUrl: pictureLoaderTemplate,
                scope: {
                    lpId: '<?',
                    blockId: '<?',
                    onUploadFile: '&',
                    onUploadByUrl: '&',
                    onDelete: '&',
                    onInit: '&',
                    onUploadIcon: '&',
                    parameters: '<?',
                    current: '<?',
                    deletePicture: '<?',
                    uploadUrlFile: '<?',
                    uploadUrlByAddress: '<?',
                    uploadUrlCropped: '<?',
                    deleteUrl: '<?',
                    maxWidth: '<?',
                    maxHeight: '<?',
                    maxWidthPicture: '<?',
                    maxHeightPicture: '<?',
                    cropperParams: '<?',
                    galleryIconsEnabled: '<?',
                    type: '<?',
                    noPhoto: '<?',
                    useExternalSave: '<?',
                    externalSave: '&',
                    onLazyLoadChange: '&',
                    lazyLoadEnabled: '<?',
                    pictureShowType: '@',
                    onChangeState: '&',
                    onResize: '&',
                    backgroundMode: '<?',
                    widthPicture: '<?',
                    heightPicture: '<?',
                    alllowChangeSize: '<?',
                },
            };
        },
    ]);
    ng.module('pictureLoader').directive('pictureLoaderElementTrigger', [
        '$parse',
        function ($parse) {
            return {
                controller: 'PictureLoaderTriggerCtrl',
                controllerAs: 'pictureLoaderTrigger',
                require: '^pictureLoaderTrigger',
                scope: true,
                link: function (scope, element, attrs, pictureLoaderTriggerCtrl) {
                    pictureLoaderTriggerCtrl.addElement(element);
                    element.on('click', function (event) {
                        pictureLoaderTriggerCtrl.open();
                        event.stopPropagation();
                    });
                },
            };
        },
    ]);
    ng.module('pictureLoader').directive('pictureLoaderTrigger', [
        '$parse',
        function ($parse) {
            return {
                controller: 'PictureLoaderTriggerCtrl',
                controllerAs: 'pictureLoaderTrigger',
                scope: true,
                link: function (scope, element, attrs, ctrl, transclude) {
                    if (!(ctrl.pictureLoaderElementTrigger != null)) {
                        element.on('click', function () {
                            ctrl.open();
                            event.stopPropagation();
                        });
                    }
                },
            };
        },
    ]);
    ng.module('pictureLoader').directive('pictureLoaderReplacement', [
        '$parse',
        function ($parse) {
            return {
                require: '^pictureLoaderTrigger',
                compile: function (cElement, cAttrs) {
                    var content = cElement.innerHTML;
                    return function (scope, element, attrs, pictureLoaderTrigger) {
                        //replacementMode : default, compile
                        pictureLoaderTrigger.addReplacement(attrs.replacementMode || 'default', element[0], content);
                    };
                },
            };
        },
    ]);
})(window.angular);
