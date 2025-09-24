(function (ng) {
    'use strict';

    var TemplateDocxService = function ($http) {
        var service = this;
    };

    TemplateDocxService.$inject = ['$http'];

    ng.module('templateDocx').service('templateDocxService', TemplateDocxService);
})(window.angular);
