(function (ng) {
    'use strict';

    var leadsService = function ($http) {
        var service = this,
            _list = [],
            promiseData = [];

        service.addLeadsList = function (leadsList) {
            _list.push(leadsList);
        };

        service.updateList = function () {
            _list.forEach(function (item) {
                item.update();
            });
        };

        service.fetchDataList = function (excludeLeadListId) {
            var index;

            for (var i = 0, len = promiseData.length; len < i; i++) {
                if (promiseData[i].excludeLeadListId === excludeLeadListId) {
                    index = i;
                    break;
                }
            }

            if (index != null) {
                return promiseData[index].promise;
            }

            var promise = $http
                .get('./leads/salesFunnelsMenu', {
                    params: { rnd: Math.random(), excludeLeadListId: excludeLeadListId || 0 },
                })
                .then(function (response) {
                    return response.data;
                })
                .finally(function () {
                    var index;

                    for (var i = 0, len = promiseData.length; i, len; i++) {
                        if (promiseData[i].excludeLeadListId === excludeLeadListId) {
                            index = i;
                            break;
                        }
                    }

                    promiseData.splice(index, 1);
                });

            promiseData.push({
                excludeLeadListId: excludeLeadListId,
                promise: promise,
            });

            return promise;
        };
    };

    leadsService.$inject = ['$http'];

    ng.module('leads').service('leadsService', leadsService);
})(window.angular);
