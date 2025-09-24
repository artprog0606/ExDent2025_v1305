(function (ng) {
    'use strict';

    var blocksConstructorService = function ($http, $q, $filter) {
        var service = this,
            blockMain,
            blockContainersStorage = {},
            blockContainersDeferStorage = {},
            colorSchemeList = [
                {
                    name: 'Светлая',
                    value: 'color-scheme--light',
                },
                {
                    name: 'Умеренная',
                    value: 'color-scheme--medium',
                },
                {
                    name: 'Темная',
                    value: 'color-scheme--dark',
                },
                {
                    name: 'Пользовательская',
                    value: 'color-scheme--custom',
                },
            ];

        service.getBlocks = function (landingPageId) {
            return $http.get('landinginplace/getblocks', { params: { landingPageId: landingPageId } }).then(function (response) {
                return response.data;
            });
        };

        service.addBlock = function (lpId, name, sortOrder, productId, top, blockIdSibling) {
            return $http
                .post('landinginplace/addblock', {
                    lpId: lpId,
                    name: name,
                    sortOrder: sortOrder,
                    productId: productId,
                    top: top,
                    blockIdSibling: blockIdSibling,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.addListBlock = function (lpId, blocks, sortOrder, productId, top, blockIdSibling) {
            return $http
                .post('landinginplace/addAllBlocksByCategory', {
                    lpId: lpId,
                    blocks: blocks,
                    sortOrder: sortOrder,
                    productId: productId,
                    top: top,
                    blockIdSibling: blockIdSibling,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.saveBlockSortOrder = function (blockId, top) {
            return $http.post('landinginplace/saveblocksortorder', { blockId: blockId, top: top }).then(function (response) {
                return response.data;
            });
        };

        service.getBlockData = function (blockId) {
            return $http.get('landinginplace/GetBlockSettings', { params: { blockId: blockId, rnd: Math.random() } }).then(function (response) {
                return response.data;
            });
        };

        service.saveProductsIds = function (blockId, ids) {
            return $http.post('/landinginplace/saveproductids', { blockId: blockId, ids: ids }).then(function (response) {
                return response.data;
            });
        };

        service.saveBlockSettings = function (blockId, settings) {
            return $http.post('landinginplace/saveblockSettings', { blockId: blockId, settings: JSON.stringify(settings) }).then(function (response) {
                return response.data;
            });
        };

        service.removeBlock = function (blockId) {
            return $http.post('landinginplace/removeblock', { blockId: blockId }).then(function (response) {
                return response.data;
            });
        };

        service.removeAllBlockByCategory = function (lpId, category) {
            return $http.post('landinginplace/removeAllBlockByCategory', { lpId: lpId, category: category }).then(function (response) {
                return response.data;
            });
        };

        service.getColorSchemeList = function () {
            return colorSchemeList;
        };

        service.createFormHidden = function (lpId) {
            return $http.post('landinginplace/createFormHidden', { lpId: lpId }).then(function (response) {
                return response.data;
            });
        };

        service.saveMain = function (blockConstructorMain) {
            blockMain = blockConstructorMain;
        };

        service.getMain = function () {
            return blockMain;
        };

        service.activateSelectMode = function () {
            return blockMain.activateSelectMode();
        };

        service.deactivateSelectMode = function () {
            return blockMain.deactivateSelectMode();
        };

        service.enabledSelectMode = function () {
            return blockMain.enabledSelectMode;
        };

        service.getIndexSubblockByName = function (subblockList, name) {
            var index;

            for (var i = 0, len = subblockList.length; i < len; i++) {
                if (name === subblockList[i].Name) {
                    index = i;
                    break;
                }
            }

            //if (index == null) {
            //    subblockList.push({ Name: name });
            //    index = subblockList.length - 1;
            //}

            return index;
        };

        service.convertToHtmlBlock = function (blockId) {
            return $http.post('landinginplace/convertToHtmlBlock', { blockId: blockId }).then(function (response) {
                return response.data;
            });
        };

        service.copyBlock = function (blockId) {
            return $http.post('landinginplace/copyBlock', { blockId: blockId }).then(function (response) {
                return response.data;
            });
        };

        service.tryUpdatelBlock = function (blockId) {
            return $http.post('landinginplace/tryUpdateBlock', { blockId: blockId }).then(function (response) {
                return response.data;
            });
        };

        service.recreateBlock = function (blockId) {
            return $http.post('landinginplace/recreateBlock', { blockId: blockId }).then(function (response) {
                return response.data;
            });
        };

        service.addBlockConstructorContainer = function (blockId, blockConstructorContainer) {
            blockContainersStorage[blockId] = blockConstructorContainer;

            if (blockContainersDeferStorage[blockId] != null) {
                blockContainersDeferStorage[blockId].resolve(blockConstructorContainer);
                delete blockContainersDeferStorage[blockId];
            }
        };

        service.getBlockConstructorContainer = function (blockId) {
            if (blockContainersStorage[blockId] != null) {
                return $q.resolve(blockContainersStorage[blockId]);
            } else {
                blockContainersDeferStorage[blockId] = blockContainersDeferStorage[blockId] || $q.defer();

                return blockContainersDeferStorage[blockId].promise;
            }
        };

        service.updatePictureFields = function (item, data, pictureType) {
            pictureType = pictureType || 'picture';
            var defaultValue = {};
            defaultValue[pictureType] = { src: null };

            item = $filter('blocksConstructorPictureAsObj')(item, pictureType) || defaultValue;

            item[pictureType] = service.mapPictureField(item[pictureType], data);

            if (data.width != null && data.width > 0) {
                item[pictureType].width = data.width;
            } else {
                delete item[pictureType].width;
            }

            if (data.height != null && data.height > 0) {
                item[pictureType].height = data.height;
            } else {
                delete item[pictureType].height;
            }

            return item;
        };

        service.mapPictureField = function (item, data) {
            item.src = data.picture;
            item.type = data.type || 'image';

            if (data.processedPictures != null) {
                Object.keys(data.processedPictures).forEach(function (key) {
                    item[key] = data.processedPictures[key];
                });
            }

            return item;
        };

        service.deletePictureFields = function (item) {
            item.picture = null;

            Object.keys(item.picture).forEach(function (key) {
                item.picture[key] = null;
            });

            return item;
        };

        service.getProductNameByOfferId = function (item) {
            return $http.get('adminv3/product/getProductNameByOfferId', { params: { offerId: item.offerId } }).then(function (response) {
                return response.data;
            });
        };

        service.getOfferDataFromId = async function (itemList) {
            const result = [];
            const itemsFiltered = itemList.filter((item) => item.offerId != null && item.offerId !== '');

            let itemData;
            for (let item of itemsFiltered) {
                itemData = await service.getProductNameByOfferId(item);
                result.push({
                    offerId: item.offerId,
                    offerPrice: item.offerPrice,
                    name: '[' + itemData.ArtNo + '] ' + itemData.Name,
                    productId: itemData.ProductId,
                    enabled: itemData.Enabled,
                    minAmount: parseFloat(itemData.MinAmount),
                    maxAmount: parseFloat(itemData.MaxAmount),
                    multiplicity: parseFloat(itemData.Multiplicity),
                    color: itemData.Color != null ? 'Цвет: ' + itemData.Color.ColorName : '',
                    size: itemData.Size != null ? 'Размер: ' + itemData.Size.SizeName : '',
                });
            }

            return result;
        };
    };

    ng.module('blocksConstructor').service('blocksConstructorService', blocksConstructorService);

    blocksConstructorService.$inject = ['$http', '$q', '$filter'];
})(window.angular);
