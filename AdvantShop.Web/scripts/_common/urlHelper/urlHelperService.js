import { urlHelper } from './urlHelper.js';
import { config } from './urlHelper.config.js';

/*@ngInject*/
const urlHelperService = function ($document, $location, $window, urlHelperConfig) {
    const urlHelperInstance = new urlHelper($document[0].head.querySelector('base').getAttribute('href'), urlHelperConfig);

    const service = this;

    service.getUrlParam = function (paramName, toLower) {
        return urlHelperInstance.getUrlParam(paramName, toLower);
    };

    service.getUrlParamByName = function (name) {
        return urlHelperInstance.getUrlParamByName(name);
    };

    service.getUrlParamDictionaryByNameFunc = function (fn, paramsAsObject) {
        return urlHelperInstance.getUrlParamDictionaryByNameFunc(fn, paramsAsObject);
    };

    service.getUrlParamsAsObject = function (string) {
        return urlHelperInstance.getUrlParamsAsObject(string);
    };

    service.getUrlParamsUniversalAsObject = function () {
        const result = {};
        const items = [new URLSearchParams(decodeURIComponent($window.location.search).slice(1))];

        items.splice($location.$$html5 ? 0 : 1, 0, new URLSearchParams(decodeURIComponent($window.location.hash).slice(2)));

        for (let i = 0, len = items.length; i < len; i++) {
            items[i].forEach(function (value, key) {
                result[key] = value;
            });
        }

        return result;
    };

    service.getBaseHref = function () {
        return urlHelperInstance.getBaseHref();
    };

    service.hasDomain = function (url) {
        return urlHelperInstance.hasDomain(url);
    };

    service.getAbsUrl = function (url, excludeAdmin) {
        return urlHelperInstance.getAbsUrl(url, excludeAdmin);
    };

    service.getUrl = function (url, excludeAdmin) {
        return urlHelperInstance.getUrl(url, excludeAdmin);
    };

    service.paramsToString = function (object) {
        return urlHelperInstance.paramsToString(object);
    };

    service.updateQueryStringParameter = function (uri, key, value) {
        return urlHelperInstance.updateQueryStringParameter(uri, key, value);
    };

    service.setLocationQueryParams = function (key, value, replace) {
        return urlHelperInstance.setLocationQueryParams(key, value, replace);
    };

    service.getHashFromString = function (string, withHash) {
        return urlHelperInstance.getHashFromString(string, withHash);
    };

    service.getPhysicalPath = function (area, isMobile) {
        return urlHelperInstance.getPhysicalPath(area, isMobile);
    };
    service.normalizeBundleFilesUrl = function (projectArea, url) {
        return urlHelperInstance.normalizeBundleFilesUrl(projectArea, url);
    };

    service.toAnchor = function (url) {
        return urlHelperInstance.toAnchor(url);
    };

    service.transformUrlToKey = function (url) {
        return urlHelperInstance.transformUrlToKey(url);
    };

    service.transformBaseUriToKey = function () {
        return urlHelperInstance.transformBaseUriToKey();
    };
};

angular.module(`urlHelper`, []).constant(`urlHelperConfig`, config).service(`urlHelper`, urlHelperService);
