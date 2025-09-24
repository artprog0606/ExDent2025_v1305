(function (ng) {
    'use strict';

    var leadService = function ($http, Upload) {
        var service = this;

        service.createOrder = function (leadId) {
            return $http.post('leads/createOrder', { leadId: leadId }).then(function (response) {
                return response.data;
            });
        };

        service.addLeadItems = function (leadId, ids) {
            return $http.post('leads/addLeadItems', { leadId: leadId, offerIds: ids }).then(function (response) {
                return response.data;
            });
        };

        service.saveLead = function (lead) {
            return $http.post('leads/saveLead', lead).then(function (response) {
                return response.data;
            });
        };

        service.createPaymentLink = function (leadId) {
            return $http.post('leads/createOrder', { leadId: leadId, force: true }).then(function (response) {
                return response.data;
            });
        };

        service.deleteLead = function (leadId) {
            return $http.post('leads/deleteLead', { leadId: leadId }).then(function (response) {
                return response.data;
            });
        };

        service.getAttachments = function (leadId) {
            return $http.get('leadsExt/getAttachments', { params: { leadId: leadId } }).then(function (response) {
                return response.data;
            });
        };

        service.uploadAttachment = function (leadId, $files) {
            return Upload.upload({
                url: 'leadsExt/uploadAttachments',
                data: {
                    leadId: leadId,
                },
                file: $files,
            }).then(function (response) {
                return response.data;
            });
        };

        service.deleteAttachment = function (leadId, id) {
            return $http.post('leadsExt/deleteAttachment', { leadId: leadId, id: id }).then(function (response) {
                return response.data;
            });
        };

        service.getLeadInfo = function (leadId) {
            return $http.get('leads/getLeadInfo', { params: { id: leadId } }).then(function (response) {
                return response.data;
            });
        };

        service.changeDealStatus = function (leadId, dealStatusId) {
            return $http.post('leads/changeLeadDealStatus', { leadId: leadId, dealStatusId: dealStatusId }).then(function (response) {
                return response.data;
            });
        };
    };

    leadService.$inject = ['$http', 'Upload'];

    ng.module('lead').service('leadService', leadService);
})(window.angular);
