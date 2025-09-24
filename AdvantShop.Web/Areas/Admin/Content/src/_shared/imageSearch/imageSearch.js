import imageSearchTemplate from './imageSearch.html';
(function (ng) {
    'use strict';

    var ImageSearchCtrl = function () {
        var ctrl = this;
        ctrl.apply = function (params) {
            if (ctrl.PictureUploaderCtrl != null) {
                ctrl.PictureUploaderCtrl.updatePhotoData(params.result.pictureId, params.result.picture);
            }
            if (ctrl.onApply != null) {
                ctrl.onApply(params);
            }
        };
    };
    ng.module('product')
        .controller('ImageSearchCtrl', ImageSearchCtrl)
        .component('imageSearch', {
            require: {
                PictureUploaderCtrl: '?^pictureUploader',
            },
            templateUrl: imageSearchTemplate,
            controller: 'ImageSearchCtrl',
            bindings: {
                uploadbylinkUrl: '@',
                uploadbylinkParams: '<?',
                selectMode: '@',
                onApply: '&',
            },
        });
})(window.angular);
