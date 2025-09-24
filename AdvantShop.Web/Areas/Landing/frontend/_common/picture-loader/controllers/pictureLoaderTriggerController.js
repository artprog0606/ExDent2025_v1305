(function (ng) {
    'use strict';
    var wrapFn = function wrapFn(fn, scope) {
        return function (locals) {
            fn(scope, locals);
        };
    };

    var PictureLoaderTriggerCtrl = /* @ngInject */ function (pictureLoaderService, $attrs, $parse, $scope, $interpolate, toaster) {
        var ctrl = this;
        var pictureShowType;

        ctrl.$onInit = function () {
            ctrl.noPhoto = $attrs.noPhoto != null ? $interpolate($attrs.noPhoto)($scope) : null;
            pictureShowType = $attrs.pictureShowType != null && $attrs.pictureShowType.length > 0 ? $parse($attrs.pictureShowType)($scope) : null;
            var current = $attrs.current != null ? $parse($attrs.current)($scope) : null;

            if (current != null) {
                if (
                    pictureShowType != null &&
                    pictureShowType.length > 0 &&
                    current.toLowerCase().indexOf('pictures/product/') === -1 &&
                    current.toLowerCase().indexOf('http') != 0
                ) {
                    ctrl.imgSrc = pictureLoaderService.getPictureType(current, pictureShowType);
                } else {
                    ctrl.imgSrc = current;
                }
            } else {
                ctrl.imgSrc = ctrl.noPhoto;
            }

            ctrl.type = $attrs.type != null ? $interpolate($attrs.type)($scope) : 'image';
        };

        ctrl.open = function () {
            ctrl.options = ctrl.options || ctrl.getParams();
            pictureLoaderService.openModal(
                ctrl.options,
                (pictureLoader) => (ctrl.pictureLoader = pictureLoader),
                () => ctrl.pictureLoader.apply(ctrl.options.onApply),
            );
        };

        ctrl.changePicture = function (result, callback) {
            var pathNew, additionalElement;
            if (result != null) {
                ctrl.type = result.type || ctrl.options.type || ctrl.type;

                if (ctrl.type === 'svg') {
                    pathNew = result.picture || ctrl.noPhoto;
                } else {
                    if (pictureShowType != null && pictureShowType.length > 0) {
                        pathNew = pictureLoaderService.getPictureType(result.picture, pictureShowType);
                    } else {
                        pathNew = result.picture;
                    }

                    pathNew = pathNew != null && pathNew.length > 0 ? pathNew : ctrl.noPhoto;

                    ctrl.imgSrc = pathNew;
                }
            } else {
                ctrl.imgSrc = ctrl.noPhoto;
            }

            if (ctrl.replacementMode === 'default') {
                if (ctrl.type === 'svg') {
                    ctrl.replacementElement.innerHTML = pathNew;
                } else {
                    if (ctrl.options.backgroundMode === true) {
                        additionalElement = document.createElement('span');
                        additionalElement.classList.add('picture-loader-trigger-image-background');
                        additionalElement.style.backgroundImage = 'url(' + ctrl.imgSrc + ')';
                    } else {
                        additionalElement = document.createElement('img');
                        additionalElement.src = ctrl.imgSrc;

                        if (result.width != null) {
                            additionalElement.style.width = result.width + 'px';
                        }

                        if (result.height != null) {
                            additionalElement.style.height = result.height + 'px';
                        }
                    }

                    if (ctrl.replacementElement) {
                        ctrl.replacementElement.innerHTML = '';
                        ctrl.replacementElement.appendChild(additionalElement);
                    }
                }
            } else if (ctrl.type === 'svg') {
                ctrl.imgSrc = pathNew;
            } else if (ctrl.type === 'image') {
                if (result.width != null) {
                    ctrl.widthPicture = result.width + 'px';
                }

                if (result.height != null) {
                    ctrl.heightPicture = result.height + 'px';
                }
            }

            if (callback != null) {
                callback({ result: result });
            }
        };

        ctrl.addElement = function (pictureLoaderElementTrigger) {
            ctrl.pictureLoaderElementTrigger = pictureLoaderElementTrigger;
        };

        ctrl.addReplacement = function (replacementMode, element, content) {
            ctrl.replacementMode = replacementMode;
            ctrl.replacementElement = element;
            ctrl.replacementContent = content;
        };

        ctrl.updateOptions = function () {
            var applyFn = ctrl.options.onApply;

            if (ctrl.options != null && ctrl.pictureLoader != null) {
                Object.keys(ctrl.options).forEach((key) => {
                    if (typeof ctrl.pictureLoader[key] !== 'function' && ctrl.options[key] !== ctrl.pictureLoader[key]) {
                        ctrl.options[key] = ctrl.pictureLoader[key];
                    }
                });

                if (ctrl.options.onApply == null && applyFn != null) {
                    ctrl.options.onApply = applyFn;
                }
            }
        };

        ctrl.getParams = function () {
            var attrs = $attrs,
                onUploadFile = attrs.onUploadFile != null ? wrapFn($parse(attrs.onUploadFile), $scope) : null,
                onUploadByUrl = attrs.onUploadByUrl != null ? wrapFn($parse(attrs.onUploadByUrl), $scope) : null,
                onUploadIcon = attrs.onUploadIcon != null ? wrapFn($parse(attrs.onUploadIcon), $scope) : null,
                onDelete = attrs.onDelete != null ? wrapFn($parse(attrs.onDelete), $scope) : null,
                onApply = attrs.onApply != null ? wrapFn($parse(attrs.onApply), $scope) : null,
                onLazyLoadChange = attrs.onLazyLoadChange != null ? wrapFn($parse(attrs.onLazyLoadChange), $scope) : null,
                onChangeState = attrs.onChangeState != null ? wrapFn($parse(attrs.onChangeState), $scope) : null,
                onResize = attrs.onResize != null ? wrapFn($parse(attrs.onResize), $scope) : null;

            let options = {
                lpId: attrs.lpId,
                blockId: $parse(attrs.blockId)($scope),
                cropperParams: $parse(attrs.cropperParams)($scope),
                parameters: $parse(attrs.parameters)($scope),
                current: attrs.current != null ? $parse(attrs.current)($scope) : null,
                uploadUrlFile: attrs.uploadUrlFile,
                uploadUrlByAddress: attrs.uploadUrlByAddress,
                uploadUrlCropped: attrs.uploadUrlCropped,
                deleteUrl: attrs.deleteUrl,
                deletePicture: attrs.deletePicture == null || attrs.deletePicture === 'true',
                maxWidth: $parse(attrs.maxWidth)($scope),
                maxHeight: $parse(attrs.maxHeight)($scope),
                maxWidthPicture: $parse(attrs.maxWidthPicture)($scope),
                maxHeightPicture: $parse(attrs.maxHeightPicture)($scope),
                type: ctrl.type,
                galleryIconsEnabled: attrs.galleryIconsEnabled == null || $parse(attrs.galleryIconsEnabled)($scope) === true,
                noPhoto: ctrl.noPhoto,
                useExternalSave: attrs.useExternalSave === 'true',
                lazyLoadEnabled:
                    attrs.lazyLoadEnabled == null || $parse(attrs.lazyLoadEnabled)($scope) === true || $parse(attrs.lazyLoadEnabled)($scope) == null,
                pictureShowType: $parse(attrs.pictureShowType)($scope),
                backgroundMode: attrs.backgroundMode === 'true',
                widthPicture: $parse(attrs.widthPicture)($scope),
                heightPicture: $parse(attrs.heightPicture)($scope),
                alllowChangeSize: $parse(attrs.alllowChangeSize)($scope),
                onUploadFile: function (result) {
                    ctrl.pictureLoader.widthPicture = null;
                    ctrl.pictureLoader.heightPicture = null;
                    ctrl.updateOptions();
                    ctrl.changePicture(result, onUploadFile);
                },
                onUploadByUrl: function (result) {
                    ctrl.pictureLoader.widthPicture = null;
                    ctrl.pictureLoader.heightPicture = null;
                    ctrl.updateOptions();
                    ctrl.changePicture(result, onUploadByUrl);
                },
                onUploadIcon: function (result) {
                    ctrl.pictureLoader.widthPicture = null;
                    ctrl.pictureLoader.heightPicture = null;
                    ctrl.updateOptions();
                    ctrl.changePicture(result, onUploadIcon);

                    if (onApply != null) {
                        onApply({ result });
                    }
                },
                onDelete: function (result) {
                    ctrl.updateOptions();
                    ctrl.changePicture(result, onDelete);
                },
                onApply: function (result) {
                    ctrl.updateOptions();
                    if (onApply != null) {
                        onApply({ result });
                    }
                },
                onLazyLoadChange: function (result) {
                    ctrl.updateOptions();
                    if (onLazyLoadChange != null) {
                        onLazyLoadChange({ result });
                    }
                },
                onResize: function (result) {
                    ctrl.updateOptions();
                    ctrl.changePicture(result, onResize);
                },
                externalSave: function (pictureLoader, saveFn, base64String) {
                    if (attrs.externalSave != null) {
                        return $parse(attrs.externalSave)($scope, {
                            pictureLoader: pictureLoader,
                            saveFn: saveFn,
                            base64String: base64String,
                        });
                    }
                },
                onChangeState: function (state, pictureLoader) {
                    if (onChangeState != null) {
                        onChangeState({ state: state, pictureLoader: pictureLoader });
                    }
                },
            };

            return options;
        };
    };

    ng.module('pictureLoader').controller('PictureLoaderTriggerCtrl', PictureLoaderTriggerCtrl);
})(window.angular);
