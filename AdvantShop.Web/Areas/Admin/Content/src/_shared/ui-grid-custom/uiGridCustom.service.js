(function (ng) {
    const isMobile = document.documentElement.classList.contains('mobile-version');

    /* @ngInject */
    const uiGridCustomService = function ($http, $window, $location, uiGridCustomDataScheme, urlHelper, $translate) {
        const service = this,
            STORAGE_ID = `uiGrid_${urlHelper.transformBaseUriToKey()}`,
            storageIds = [];

        //remove old static key
        localStorage.removeItem('uiGrid');

        let uiGridCustomDataSchemeRevert;

        service.getData = function (url, params) {
            return $http.get(url, { params }).then((response) => service.convertToClientParams(response.data));
        };

        service.applyInplaceEditing = function (url, params) {
            return $http.post(url, params).then((response) => response.data);
        };

        service.convertToClientParams = function (resultFromServer, onlyEqual) {
            return service.mapping(resultFromServer, 'client', onlyEqual);
        };

        service.convertToServerParams = function (resultFromClient, onlyEqual) {
            return service.mapping(resultFromClient, 'server', onlyEqual);
        };

        service.mapping = function (data, convertTo, onlyEqual) {
            let result;

            uiGridCustomDataSchemeRevert ||= service.revertDictionary(uiGridCustomDataScheme);

            const dictionary = convertTo === 'server' ? uiGridCustomDataSchemeRevert : uiGridCustomDataScheme;

            for (const key in data) {
                if (Object.hasOwn(data, key) === true) {
                    if (typeof dictionary[key] !== 'undefined') {
                        result ||= {};
                        result[dictionary[key]] = data[key];
                    } else if (typeof onlyEqual === 'undefined' || onlyEqual === null || onlyEqual === false) {
                        result ||= {};
                        result[key] = data[key];
                    }
                }
            }

            if (convertTo === 'client' && typeof result !== 'undefined' && typeof result.data === 'undefined') {
                result.data = [];
            }

            return result;
        };

        service.revertDictionary = function (dictionary) {
            const result = {};

            for (const key in dictionary) {
                if (Object.hasOwn(dictionary, key) === true) {
                    result[dictionary[key]] = key;
                }
            }

            return result;
        };

        service.getParamsByUrl = function (uniqueId) {
            const params = $location.search();
            let result = {};

            if (params[uniqueId]) {
                result = JSON.parse(params[uniqueId]);
            }

            return result;
        };

        service.setParamsByUrl = function (uniqueId, params) {
            return $location.search(uniqueId, JSON.stringify(params));
        };

        service.clearParams = function (uniqueId) {
            return $location.search(uniqueId, null);
        };

        service.export = function (url, params) {
            //get all items
            params.ItemsPerPage = 1000000;
            params.Page = 1;
            params.OutputDataType = 'Csv';
            $window.location.assign(`${url}?${urlHelper.paramsToString(params)}`);
        };

        service.addInStorage = function (uniqueId) {
            storageIds.push(uniqueId);
        };

        service.removeFromStorage = function (uniqueId) {
            const index = storageIds.indexOf(uniqueId);

            if (uniqueId !== -1) {
                storageIds.splice(index, 1);
            }
        };

        service.validateId = function (uniqueId) {
            return typeof uniqueId !== 'undefined' && uniqueId?.length > 0 && storageIds.indexOf(uniqueId) === -1;
        };

        service.saveDataInStorage = function (key, data) {
            const storage = service.getDataInStorage() || {};

            storage[key] = ng.extend(storage[key] || {}, data);

            $window.localStorage.setItem(STORAGE_ID, JSON.stringify(storage));

            return storage[key];
        };

        service.getDataItimFromStorageByKey = function (key) {
            const data = service.getDataInStorage();

            return typeof data !== 'undefined' && data !== null ? data[key] : null;
        };

        service.getDataInStorage = function () {
            const data = $window.localStorage.getItem(STORAGE_ID);

            return typeof data !== 'undefined' && data !== null && data.length > 0 ? JSON.parse(data) : null;
        };

        service.removeDuplicate = function (rowEntity, params) {
            const rowEntityToLowers = [],
                paramsNew = {};

            for (const key in rowEntity) {
                if (Object.hasOwn(rowEntity, key)) {
                    const index = key.indexOf('[');
                    if (index !== -1) {
                        if (Object.hasOwn(rowEntity, key.substr(0, index))) continue;
                    }

                    rowEntityToLowers.push(key.toLowerCase());
                }
            }

            for (const keyParam in params) {
                if (Object.hasOwn(params, keyParam) && rowEntityToLowers.indexOf(keyParam.toLowerCase()) === -1) {
                    paramsNew[keyParam] = params[keyParam];
                }
            }

            return paramsNew;
        };
        service.removePropertyDuplicate = function (rowEntity) {
            const rowEntityToLowers = [],
                paramsNew = {};

            for (const key in rowEntity) {
                if (Object.hasOwn(rowEntity, key)) {
                    const index = key.indexOf('[');
                    if (index !== -1) {
                        if (Object.hasOwn(rowEntity, key.substr(0, index))) continue;
                    }

                    rowEntityToLowers.push(key.toLowerCase());
                }
            }

            for (const keyParam in rowEntity) {
                if (Object.hasOwn(rowEntity, keyParam) && rowEntityToLowers.indexOf(keyParam.toLowerCase()) !== -1) {
                    paramsNew[keyParam] = rowEntity[keyParam];
                }
            }

            return paramsNew;
        };

        const getTemplateCellButton = function (type, url, params, attrs) {
            const additionalAttrs = typeof attrs !== 'undefined' && attrs !== null ? ` ${attrs} ` : '';

            let tpl = '';

            switch (type) {
                case 'link':
                    tpl = isMobile
                        ? `<a class="btn btn-sm flex center-xs middle-xs btn-success" ng-href="${url}" ${additionalAttrs}>${$translate.instant('Admin.Js.Grid.Edit')}</a>`
                        : `<a class="ui-grid-custom-service-icon fas fa-pencil-alt" aria-label="${$translate.instant('Admin.Js.Grid.Edit')}" ng-href="${url}" ${additionalAttrs}></a>`;
                    break;
                case 'edit':
                    tpl = isMobile
                        ? `<button type="button" class="btn btn-sm flex center-xs middle-xs btn-success" ${additionalAttrs}>${$translate.instant('Admin.Js.Grid.Edit')}</button>`
                        : `<button type="button" aria-label="${$translate.instant('Admin.Js.Grid.Edit')}" class="ui-grid-custom-service-icon fas fa-pencil-alt" ${additionalAttrs}></button>`;
                    break;
                case 'delete':
                    tpl = `<ui-grid-custom-delete class="${
                        isMobile ? 'btn btn-sm flex center-xs middle-xs btn-danger' : ''
                    }" url="${url}" params="${params}" ${additionalAttrs}>${isMobile ? $translate.instant('Admin.Js.Grid.Delete') : ''}</ui-grid-custom-delete>`;
                    break;
                default:
                    throw new Error(`Type button "${type}" for ui-grid-custom not register`);
            }

            return (isMobile ? '' : '<div class="ui-grid-cell-contents"><div class="js-grid-not-clicked">') + tpl + (isMobile ? '' : '</div></div>');
        };
        service.getTemplateCellDelete = function (url, params, attrs) {
            return getTemplateCellButton('delete', url, params, attrs);
        };

        service.getTemplateCellEdit = function (attrs) {
            return getTemplateCellButton('edit', null, null, attrs);
        };

        service.getTemplateCellLink = function (url, attrs) {
            return getTemplateCellButton('link', url, null, attrs);
        };

        service.getColsByType = function (columnDefs, type) {
            return columnDefs.filter((it) => it.type === type);
        };

        service.getColByFieldName = function (columnDefs, fieldName) {
            return columnDefs.find((it) => it.name === fieldName);
        };
    };

    ng.module('uiGridCustom').service('uiGridCustomService', uiGridCustomService);
})(window.angular);
