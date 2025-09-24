(function (ng) {
    'use strict';

    var MODAL_ID = 'modalPictureLoader';

    var pictureLoaderService = function ($http, $q, modalService, Upload, toaster, galleryCloudService, galleryIconsService) {
        var service = this;

        service.openModal = function (params, onInit, onApply) {
            const deletePicture = params.deletePicture === true ? ' delete-picture="true" ' : '';

            const alllowChangeSize = params.alllowChangeSize != null ? `alllow-change-size="${params.alllowChangeSize}"` : '';
            const parentData = {
                modalData: params,
                onInit,
                onApply,
            };

            modalService.renderModal(
                MODAL_ID,
                params.current != null ? 'Обновить изображение' : 'Загрузить изображение',
                '<picture-loader cropper-params="modalData.cropperParams" lp-id="modalData.lpId" block-id="modalData.blockId" ' +
                    'on-init="onInit(pictureLoader)" ' +
                    'on-upload-file="modalData.onUploadFile(result)" ' +
                    'on-upload-by-url="modalData.onUploadByUrl(result)" ' +
                    'on-upload-icon="modalData.onUploadIcon(result)" ' +
                    'on-delete="modalData.onDelete(result)" ' +
                    'on-change-state="modalData.onChangeState(state, pictureLoader)"' +
                    'on-resize="modalData.onResize(result)"' +
                    deletePicture +
                    alllowChangeSize +
                    'current="modalData.current" type="modalData.type" upload-url-file="modalData.uploadUrlFile" upload-url-by-address="modalData.uploadUrlByAddress" delete-url="modalData.deleteUrl" max-width-picture="modalData.maxWidthPicture" max-width="modalData.maxWidth" max-height="modalData.maxHeight" max-height-picture="modalData.maxHeightPicture" parameters="modalData.parameters"' +
                    'use-external-save="modalData.useExternalSave" external-save="modalData.externalSave(pictureLoader, saveFn, base64String)"' +
                    'lazy-load-enabled="modalData.lazyLoadEnabled" on-lazy-load-change="modalData.onLazyLoadChange(result)"' +
                    'gallery-icons-enabled="' +
                    params.galleryIconsEnabled +
                    '" no-photo="modalData.noPhoto" width-picture="modalData.widthPicture" height-picture="modalData.heightPicture"></picture-loader>',
                '<div class="text-left"><button type="button" type="button" class="blocks-constructor-btn-confirm"  modal-close="" data-e2e="SaveCroperBtn" modal-close-callback="onApply()">Применить</button></div>',
                { destroyOnClose: true, modalClass: 'picture-upload-modal', appendModalClass: '', isShowFooter: false },
                parentData,
            );

            modalService.getModal(MODAL_ID).then(function (modal) {
                modal.modalScope.open();
            });
        };

        service.showGalleryCloud = function (callback) {
            galleryCloudService.showModal({ onSelect: callback });
        };

        service.showGalleryIcons = function (callback) {
            galleryIconsService.showModal({ onSelect: callback });
        };

        service.setVisibleFooter = function (visibility) {
            modalService.setVisibleFooter(MODAL_ID, visibility);
        };

        service.closeModal = function () {
            modalService.close(MODAL_ID);
        };

        service.uploadFile = function (lpId, blockId, maxWidth, maxHeight, parameters, $file, uploadUrlFile, current) {
            var data = {
                lpId: lpId,
                blockId: blockId,
                maxWidth: maxWidth,
                maxHeight: maxHeight,
                parameters: parameters,
            };

            if (current != null) {
                data.picture = current;
            }

            return Upload.upload({
                url: uploadUrlFile,
                data: data,
                file: $file,
            }).then(function (response) {
                return response.data;
            });
        };

        service.uploadByUrl = function (lpId, blockId, maxWidth, maxHeight, parameters, url, uploadUrlByAddress, current) {
            var data = {
                lpId: lpId,
                blockId: blockId,
                maxWidth: maxWidth,
                maxHeight: maxHeight,
                parameters: parameters,
                url: url,
            };

            if (current != null) {
                data.picture = current;
            }

            return Upload.upload({
                url: uploadUrlByAddress,
                data: data,
            }).then(function (response) {
                return response.data;
            });
        };

        service.uploadCropped = function (lpId, blockId, maxWidth, maxHeight, parameters, base64String, ext, uploadUrlCropped, current) {
            var data = {
                lpId: lpId,
                blockId: blockId,
                maxWidth: maxWidth,
                maxHeight: maxHeight,
                parameters: parameters,
                base64String: base64String,
                ext: ext,
            };

            if (current != null) {
                data.picture = current;
            }

            return Upload.upload({
                url: uploadUrlCropped,
                data: data,
            }).then(function (response) {
                return response.data;
            });
        };

        service.resize = function (lpId, blockId, maxWidth, maxHeight, parameters, resizeUrl, current) {
            var data = {
                lpId: lpId,
                blockId: blockId,
                maxWidth: maxWidth,
                maxHeight: maxHeight,
                parameters: parameters,
            };

            if (current != null) {
                data.picture = current;
            }

            return Upload.upload({
                url: resizeUrl,
                data: data,
            }).then(function (response) {
                return response.data;
            });
        };

        service.delete = function (lpId, blockId, picture, parameters, deleteUrl) {
            var data = { lpId: lpId, blockId: blockId, picture: picture, parameters: parameters };

            return Upload.upload({
                url: deleteUrl,
                data: data,
            }).then(function (response) {
                return response.data;
            });
        };

        service.getBase64PictureByUrl = function (url) {
            return $http.post('landinginplace/getBase64PictureByUrl', { url: url }).then(function (response) {
                return response.data;
            });
        };

        service.getExt = function (filename) {
            return '.' + filename.split('.').pop().split('?').shift();
        };

        service.getPictureType = function (original, postfix) {
            if (original == null) {
                return null;
            }

            var array = original.split('/');
            var dir = array.slice(0, array.length - 1).join('/');
            var filename = array[array.length - 1];
            var ext = service.getExt(filename);
            var name = filename.replace(ext, '');

            return dir + '/' + name + '_' + postfix + ext;
        };

        service.getFileName = function (path) {
            const parts = path.split(`/`);
            return parts[parts.length - 1].split('?')[0];
        };
    };

    ng.module('pictureLoader').service('pictureLoaderService', pictureLoaderService);

    pictureLoaderService.$inject = ['$http', '$q', 'modalService', 'Upload', 'toaster', 'galleryCloudService', 'galleryIconsService'];
})(window.angular);
