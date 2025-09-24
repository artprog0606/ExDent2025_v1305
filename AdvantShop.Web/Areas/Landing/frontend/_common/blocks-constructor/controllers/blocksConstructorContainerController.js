(function (ng) {
    'use strict';

    var BlocksConstructorContainerCtrl = function ($attrs, $document, $element, $q, $window, toaster, blocksConstructorService) {
        var ctrl = this;
        var constructorEl;
        var emptyObject = {};

        ctrl.$onInit = function () {
            ctrl.id = $attrs.stringId;
            ctrl.blockId = $attrs.blockId;
            ctrl.blocksConstructorMain.addContainer(ctrl.id, ctrl);
            ctrl.data = emptyObject;

            blocksConstructorService.addBlockConstructorContainer(ctrl.blockId, ctrl);

            constructorEl = $document[0].querySelector('.lp-blocks-constructor[data-block-id="' + ctrl.blockId + '"]');
        };

        ctrl.getData = function () {
            //return $q.when(ctrl.isSettingsLoad ? ctrl.data : ctrl.getDataAjax(ctrl.blockId));
            return ctrl.getDataAjax(ctrl.blockId); /* ломается отзывы так как обращается к одному и тому же объекту*/
        };

        ctrl.getDataAjax = function (blockId) {
            return blocksConstructorService
                .getBlockData(blockId)
                .then(function (data) {
                    return ng.extend(ctrl.data, data);
                })
                .finally(function () {
                    ctrl.isSettingsLoad = true;
                });
        };

        ctrl.$postLink = function () {
            $element.on('click', function () {
                if (ctrl.blocksConstructorMain.enabledSelectMode) {
                    ctrl.blocksConstructorMain.selectBlock(ctrl);
                }
            });
        };

        ctrl.removeBlock = function () {
            ctrl.blocksConstructorMain.removeContainer(ctrl.id);
            var parentNode = $element.parent();

            if (parentNode[0].id === 'wrap_' + ctrl.id) {
                parentNode.remove();
            } else {
                $element.remove();
            }

            constructorEl.remove();
        };

        ctrl.moveUpBlock = function () {
            $element[0].parentNode.insertBefore($element[0], $element.prevAll('.blocks-constructor-container').prevAll('.lp-blocks-constructor')[0]);
            constructorEl.parentNode.insertBefore(constructorEl, $element[0]);
        };

        ctrl.moveDownBlock = function () {
            $element[0].parentNode.insertBefore($element[0], $element.nextAll('.blocks-constructor-container').nextAll('.lp-blocks-constructor')[0]);
            constructorEl.parentNode.insertBefore(constructorEl, $element[0]);
        };

        ctrl.onApplyPicture = function (items, result, index, pictureType) {
            pictureType = pictureType || 'picture';
            var isNew = items[index] == null;
            var itemUpdated = ng.extend(items[index] || {}, blocksConstructorService.updatePictureFields(items[index], result, pictureType));

            if (isNew === true) {
                items.push(itemUpdated);
            }

            blocksConstructorService.saveBlockSettings(ctrl.blockId, ctrl.data);
        };

        ctrl.onDeletePicture = function (items, index, pictureType) {
            pictureType = pictureType || 'picture';
            if (items != null && items.length > 0 && items[index] != null && items[index][pictureType] != null) {
                items[index][pictureType].src = null;
                items[index][pictureType].type = null;
                blocksConstructorService.saveBlockSettings(ctrl.blockId, ctrl.data);
            }
        };

        ctrl.onResizePicture = function (items, result, index, pictureType) {
            pictureType = pictureType || 'picture';
            const { picture, width, height } = result;
            if (items != null && items.length > 0 && items[index] != null) {
                items[index] = ng.extend(items[index], blocksConstructorService.updatePictureFields(items[index], result, pictureType));
                blocksConstructorService.saveBlockSettings(ctrl.blockId, ctrl.data);
            }
        };

        ctrl.recreateBlock = function (blockId) {
            blocksConstructorService.recreateBlock(blockId).then(function (data) {
                $window.location.reload(true);
            });
        };

        ctrl.onLazyLoadChange = function (dataSettings, result, index) {
            dataSettings.Settings.items = dataSettings.Settings.items || [];
            dataSettings.Settings.items[index] = dataSettings.Settings.items[index] || {};

            //обратная совместимость
            if (ng.isString(dataSettings.Settings.items[index]) === true) {
                dataSettings.Settings.items[index] = {
                    picture: {
                        src: dataSettings.Settings.items[index],
                    },
                };
            }

            dataSettings.Settings.items[index] = dataSettings.Settings.items[index] || {};
            dataSettings.Settings.items[index].picture = dataSettings.Settings.items[index].picture || {};
            dataSettings.Settings.items[index].picture.lazyLoadEnabled = result;

            blocksConstructorService
                .saveBlockSettings(ctrl.blockId, dataSettings)
                .then(function () {
                    toaster.pop('success', 'Изменения успешно сохранены');
                })
                .catch(function () {
                    toaster.pop('error', 'Ошибка при сохранении');
                });
        };

        ctrl.onInplaceSaveSubblock = function (name, field, value) {
            $q.when(ctrl.data !== emptyObject ? ctrl.data : ctrl.getData()).then(function (data) {
                var item;

                for (var i = 0, len = data.Subblocks.length; i < len; i++) {
                    if (data.Subblocks[i].Name === name) {
                        item = data.Subblocks[i];
                        break;
                    }
                }

                if (item != null) {
                    item[field] = value;
                    return blocksConstructorService.saveBlockSettings(ctrl.blockId, data);
                }

                return data;
            });
        };
    };

    ng.module('blocksConstructor').controller('BlocksConstructorContainerCtrl', BlocksConstructorContainerCtrl);

    BlocksConstructorContainerCtrl.$inject = ['$attrs', '$document', '$element', '$q', '$window', 'toaster', 'blocksConstructorService'];
})(window.angular);
