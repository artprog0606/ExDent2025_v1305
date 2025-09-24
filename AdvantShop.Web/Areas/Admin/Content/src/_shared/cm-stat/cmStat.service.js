(function (ng) {
    'use strict';

    var cmStatService = function CmStatCTrl($http, $timeout) {
        var service = this,
            isRuning = false,
            countObsevarable = 0,
            data = {},
            callbacks = {};

        service.getData = function getData() {
            countObsevarable += 1;

            data.Processed = 0;
            data.Total = 0;
            data.Update = 0;
            data.Add = 0;
            data.Error = 0;

            if (isRuning === false) {
                isRuning = true;

                service.startPooling();
            }

            return data;
        };

        service.makeRequest = function makeRequest() {
            return $http.get('ExportImportCommon/GetCommonStatistic').then(function (response) {
                data = ng.extend(data, response.data);

                for (var key in callbacks) {
                    if (Object.prototype.hasOwnProperty.call(callbacks, key)) {
                        callbacks[key](data);
                    }
                }

                return data;
            });
        };

        service.startPooling = function startPooling() {
            service.makeRequest().then(function () {
                if (data.ProcessedPercent === 100 && !data.IsRun && countObsevarable === 1) {
                    isRuning = false;
                }

                return $timeout(function () {
                    if (isRuning === true && countObsevarable > 0) {
                        service.startPooling();
                    }
                }, 1000);
            });
        };

        service.stopPooling = function stopPooling() {
            isRuning = false;
        };

        service.deleteObsevarable = function deleteObsevarable() {
            if (countObsevarable === 0) {
                return;
            }

            countObsevarable -= 1;

            if (countObsevarable === 0) {
                service.stopPooling();
            }
        };

        service.addCallback = function (uid, callback) {
            callbacks[uid] = callback;
        };
    };

    cmStatService.$inject = ['$http', '$timeout'];

    ng.module('cmStat').service('cmStatService', cmStatService);
})(window.angular);
