(function (ng) {
    'use strict';

    var cartService = function ($q, $http) {
        var service = this,
            isRequestProcess = false,
            isInitilaze = false,
            deferes = [],
            cart = {},
            callbacks = {};

        service.getData = function (cache, queryParams) {
            var defer = $q.defer();

            if (cache == null) {
                cache = true;
            }

            if (isInitilaze === true && cache === true) {
                defer.resolve(cart);
            } else {
                if (isRequestProcess === true) {
                    deferes.push(defer);
                } else {
                    isRequestProcess = true;

                    deferes.push(defer);

                    $http.post('landing/landing/getBookingCart', ng.extend({ rnd: Math.random() }, queryParams || {})).then(function (response) {
                        ng.extend(cart, response.data);

                        isInitilaze = true;
                        isRequestProcess = false;

                        for (var i = deferes.length - 1; i >= 0; i--) {
                            deferes[i].resolve(cart);
                        }

                        deferes.length = 0;
                    });
                }
            }

            return defer.promise.then(function (result) {
                service.processCallback('get', result);
                return result;
            });
        };

        service.removeItem = function (shoppingCartItemId, queryParams) {
            return $http
                .post('landing/landing/removeFromBookingCart', ng.extend({ itemId: shoppingCartItemId }, queryParams || {}))
                .then(function (response) {
                    service.processCallback('remove', response.data);
                    service.getData(false, queryParams);
                    return response.data;
                });
        };

        service.addItem = function (beginDate, endDate, affiliateId, resourceId, selectedServices, queryParams) {
            var params = ng.extend(
                {
                    beginDate: beginDate,
                    endDate: endDate,
                    affiliateId: affiliateId,
                    resourceId: resourceId,
                    selectedServices: selectedServices,
                },
                queryParams || {},
            );

            return $http.post('landing/landing/addToBookingCart', params).then(function (response) {
                var result = response.data,
                    defer,
                    promise;

                if (response.data.status !== 'redirect') {
                    return service.getData(false, params).then(function () {
                        service.processCallback('add', [result]);
                        return [result];
                    });
                } else {
                    defer = $q.defer();
                    promise = defer.promise;
                    defer.resolve([result]);
                    return promise;
                }
            });
        };

        service.clear = function (queryParams) {
            return $http.post('landing/landing/clearBookingCart', queryParams || {}).then(function () {
                service.processCallback('clear');
                return service.getData(false, queryParams);
            });
        };

        service.addCallback = function (name, func, targetName) {
            callbacks[name] = callbacks[name] || [];

            if (func == null) {
                throw Error('Callback for cart equal null');
            }

            callbacks[name].push({ callback: func, targetName: targetName || null });
        };

        service.processCallback = function (name, params, targetName) {
            if (callbacks[name] == null) {
                return;
            }

            for (var i = callbacks[name].length - 1; i >= 0; i--) {
                if (targetName != null && callbacks[name][i].targetName === targetName) {
                    callbacks[name][i].callback(cart, params);
                } else {
                    callbacks[name][i].callback(cart, params);
                }
            }
        };

        service.removeCallback = function (name, targetName) {
            var arrayCallbacksByName = callbacks[name],
                index;

            if (arrayCallbacksByName == null) {
                return;
            }

            if (targetName != null) {
                for (var i = 0, len = arrayCallbacksByName.length; i < len; i++) {
                    if (arrayCallbacksByName[i].targetName === targetName) {
                        arrayCallbacksByName.splice(i, 1);
                    }
                }
            } else {
                index = callbacks.indexOf(callbacks[name]);

                if (index !== -1) {
                    callbacks.splice(index, 1);
                }
            }
        };
    };

    cartService.$inject = ['$q', '$http'];

    ng.module('bookingCart').service('bookingCartService', cartService);
})(window.angular);
