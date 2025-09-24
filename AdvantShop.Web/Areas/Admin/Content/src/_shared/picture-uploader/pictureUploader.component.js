import pictureUploaderTemplate from './templates/picture-uploader.html';

(function (ng) {
    'use strict';

    ng.module('pictureUploader').directive(
        'pictureUploader',
        /* @ngInject */
        function ($templateRequest, urlHelper, $compile) {
            return {
                bindToController: true,
                controllerAs: '$ctrl',
                transclude: true,
                controller: 'PictureUploaderCtrl',
                scope: {
                    startSrc: '@',
                    pictureId: '@',
                    uploadUrl: '@',
                    uploadParams: '<?',
                    deleteUrl: '@',
                    deleteParams: '<?',
                    uploadbylinkUrl: '@',
                    uploadbylinkParams: '<?',
                    onUpdate: '&',
                    onDelete: '&',
                    onInit: '&',
                    startPictureId: '@',
                    uploaderDestination: '@', //название input type=file для тестов
                    fileTypes: '<?', //вариант из pictureUploaderFileTypes
                },
                link: function (scope, element, attrs, ctrl, transclude) {
                    $templateRequest(pictureUploaderTemplate).then(function (tpl) {
                        var fragment = document.createDocumentFragment();
                        var innerEl = document.createElement('div');
                        innerEl.innerHTML = tpl;
                        var clone = transclude().clone();
                        var transcludeBlock = innerEl.querySelector('.transclude-block');
                        for (var i = 0; i < clone.length; i++) {
                            fragment.appendChild(clone[i]);
                        }
                        transcludeBlock.appendChild(fragment);

                        var buttonAdd = innerEl.querySelector('.picture-uploader-buttons-add');
                        buttonAdd.setAttribute('data-e2e', 'imgAdd' + ctrl.uploaderDestination);
                        $compile(element.html(innerEl).contents())(scope);
                    });
                },
            };
        },
    );
})(window.angular);
