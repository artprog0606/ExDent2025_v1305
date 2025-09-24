(function (ng) {
    'use strict';

    var bookingAffiliateService = function ($http) {
        var service = this;

        service.delete = function (id) {
            return $http.post('bookingAffiliate/delete', { Id: id }).then(function (result) {
                return result.data;
            });
        };

        service.getAdditionalTime = function (affiliateId, date) {
            var paramDate = date;
            if (paramDate instanceof Date) {
                paramDate = paramDate.getFullYear() + '-' + (paramDate.getMonth() + 1) + '-' + paramDate.getDate();
            }

            return $http
                .get('bookingAffiliate/getAdditionalTime', { params: { affiliateId: affiliateId, date: paramDate } })
                .then(function (response) {
                    return response.data;
                });
        };

        service.addAdditionalTime = function (affiliateId, date, times) {
            var paramDate = date;
            if (paramDate instanceof Date) {
                paramDate = paramDate.getFullYear() + '-' + (paramDate.getMonth() + 1) + '-' + paramDate.getDate();
            }

            return $http
                .post('bookingAffiliate/addAdditionalTime', { affiliateId: affiliateId, date: paramDate, times: times })
                .then(function (response) {
                    return response.data;
                });
        };

        service.updateAdditionalTime = function (affiliateId, date, times) {
            var paramDate = date;
            if (paramDate instanceof Date) {
                paramDate = paramDate.getFullYear() + '-' + (paramDate.getMonth() + 1) + '-' + paramDate.getDate();
            }

            return $http
                .post('bookingAffiliate/updateAdditionalTime', {
                    affiliateId: affiliateId,
                    date: paramDate,
                    times: times,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.addAdditionalTimes = function (affiliateId, startDate, endDate, times) {
            var paramStartDate = startDate;
            if (paramStartDate instanceof Date) {
                paramStartDate = paramStartDate.getFullYear() + '-' + (paramStartDate.getMonth() + 1) + '-' + paramStartDate.getDate();
            }
            var paramEndDate = endDate;
            if (paramEndDate instanceof Date) {
                paramEndDate = paramEndDate.getFullYear() + '-' + (paramEndDate.getMonth() + 1) + '-' + paramEndDate.getDate();
            }

            return $http
                .post('bookingAffiliate/addAdditionalTime', {
                    affiliateId: affiliateId,
                    startDate: paramStartDate,
                    endDate: paramEndDate,
                    times: times,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.updateAdditionalTimes = function (affiliateId, startDate, endDate, times) {
            var paramStartDate = startDate;
            if (paramStartDate instanceof Date) {
                paramStartDate = paramStartDate.getFullYear() + '-' + (paramStartDate.getMonth() + 1) + '-' + paramStartDate.getDate();
            }
            var paramEndDate = endDate;
            if (paramEndDate instanceof Date) {
                paramEndDate = paramEndDate.getFullYear() + '-' + (paramEndDate.getMonth() + 1) + '-' + paramEndDate.getDate();
            }

            return $http
                .post('bookingAffiliate/updateAdditionalTime', {
                    affiliateId: affiliateId,
                    startDate: paramStartDate,
                    endDate: paramEndDate,
                    times: times,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.deleteAdditionalTime = function (affiliateId, date) {
            var paramDate = date;
            if (paramDate instanceof Date) {
                paramDate = paramDate.getFullYear() + '-' + (paramDate.getMonth() + 1) + '-' + paramDate.getDate();
            }

            return $http.post('bookingAffiliate/deleteAdditionalTime', { affiliateId: affiliateId, date: paramDate }).then(function (response) {
                return response.data;
            });
        };

        service.deleteAdditionalTimes = function (affiliateId, startDate, endDate) {
            var paramStartDate = startDate;
            if (paramStartDate instanceof Date) {
                paramStartDate = paramStartDate.getFullYear() + '-' + (paramStartDate.getMonth() + 1) + '-' + paramStartDate.getDate();
            }
            var paramEndDate = endDate;
            if (paramEndDate instanceof Date) {
                paramEndDate = paramEndDate.getFullYear() + '-' + (paramEndDate.getMonth() + 1) + '-' + paramEndDate.getDate();
            }

            return $http
                .post('bookingAffiliate/deleteAdditionalTime', {
                    affiliateId: affiliateId,
                    startDate: paramStartDate,
                    endDate: paramEndDate,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.getSmsTemplate = function (id) {
            return $http.get('bookingAffiliate/getSmsTemplate', { params: { id: id } }).then(function (response) {
                return response.data;
            });
        };

        service.addSmsTemplate = function (affiliateId, status, text, enabled) {
            return $http
                .post('bookingAffiliate/addSmsTemplate', {
                    affiliateId: affiliateId,
                    status: status,
                    text: text,
                    enabled: enabled,
                })
                .then(function (response) {
                    return response.data;
                });
        };

        service.updateSmsTemplate = function (id, affiliateId, status, text, enabled) {
            return $http
                .post('bookingAffiliate/updateSmsTemplate', {
                    id: id,
                    affiliateId: affiliateId,
                    status: status,
                    text: text,
                    enabled: enabled,
                })
                .then(function (response) {
                    return response.data;
                });
        };
    };

    bookingAffiliateService.$inject = ['$http'];

    ng.module('bookingAffiliate').service('bookingAffiliateService', bookingAffiliateService);
})(window.angular);
