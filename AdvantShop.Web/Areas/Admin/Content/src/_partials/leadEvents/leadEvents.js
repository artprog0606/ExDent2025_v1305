import leadEventsTemplate from './leadEvents.html';
(function (ng) {
    'use strict';

    var FILTER_TYPE_NAME = 'leadEventsFilterType';
    var LeadEventsCtrl = function ($http, $location, toaster, $translate, SweetAlert, adminWebNotificationsEvents, adminWebNotificationsService) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.firstLoadingData = true;
            var search = $location.search();
            ctrl.filterType = (search != null && search[FILTER_TYPE_NAME]) || '';
            ctrl.getLeadEvents();
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    leadEvents: ctrl,
                });
            }
            ctrl.removeCallbackUpdateAdminComment = adminWebNotificationsService.addListener(
                adminWebNotificationsEvents.updateAdminComment,
                function (data) {
                    if ((data.objId === ctrl.objId && data.objType === ctrl.objType) || data.relatedCustomerId === ctrl.customerId) {
                        ctrl.getLeadEvents();
                    }
                },
            );
        };
        ctrl.$onDestroy = function () {
            if (ctrl.removeCallbackUpdateAdminComment) {
                ctrl.removeCallbackUpdateAdminComment();
                ctrl.removeCallbackUpdateAdminComment = null;
            }
        };
        ctrl.toggleselectCurrencyLabel = function (val) {
            ctrl.selectCurrency = val;
        };
        ctrl.getLeadEvents = function () {
            $http
                .get('crmEvents/getEvents', {
                    params: {
                        objId: ctrl.objId,
                        objType: ctrl.objType,
                        customerId: ctrl.customerId,
                    },
                })
                .then(function (response) {
                    var data = response.data;
                    if (data != null) {
                        ctrl.data = data;
                        ctrl.type = ctrl.data.EventTypes[0];
                        if (ctrl.firstLoadingData == true) {
                            ctrl.firstLoadingData = false;
                        }
                    }
                });
        };
        ctrl.getLeadEventsWithDelay = function () {
            setTimeout(ctrl.getLeadEvents, 800);
        };
        ctrl.addLeadEvent = function () {
            $http
                .post('crmEvents/addEvent', {
                    customerId: ctrl.customerId,
                    objId: ctrl.objId,
                    objType: ctrl.objType,
                    type: ctrl.type.value,
                    message: ctrl.message,
                    pageType: ctrl.pageType,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Lead.CommentAdded'));
                        ctrl.message = '';
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Lead.CommentNotAdded'));
                    }
                    ctrl.getLeadEvents();
                });
        };
        ctrl.addEventKeydown = function (e) {
            if (e.keyCode === 13) {
                ctrl.addLeadEvent();
                e.preventDefault();
            }
        };
        ctrl.changeType = function (type) {
            ctrl.type = ctrl.data.EventTypes.filter(function (x) {
                return x.value === type;
            })[0];
        };
        ctrl.getIcon = function (type) {
            if (type == null) return '';
            switch (type.toLocaleLowerCase()) {
                case 'none':
                    return 'far fa-flag';
                case 'comment':
                    return 'far fa-comments';
                case 'call':
                    return 'fas fa-phone';
                case 'sms':
                    return 'far fa-comments';
                case 'email':
                    return 'far fa-envelope';
                case 'task':
                    return 'far fa-calendar-check';
                case 'other':
                    return 'fas fa-list-ul';
                case 'vk':
                    return 'fab fa-vk';
                case 'instagram':
                    return 'fab fa-instagram';
                case 'facebook':
                    return 'fab fa-facebook';
                case 'telegram':
                    return 'fab fa-telegram';
                case 'history':
                    return 'fas fa-history';
                case 'ok':
                    return 'fab fa-odnoklassniki';
            }
        };
        ctrl.setComment = function (event, result) {
            event.Data = event.Data || {};
            event.Data.Id = result.id;
            event.Data.Text = result.text;
            event.Data.ObjId = result.objId;
        };
        ctrl.editComment = function (event, index, text) {
            $http
                .post('adminComments/update', {
                    id: event.Id,
                    text: text,
                })
                .then(function (response) {
                    if (response.data.Result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Partials.ChangesSaved'));
                        event.Message = text;
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Partials.Error'));
                    }
                    event.showEditComment[index] = false;
                    ctrl.getLeadEventsWithDelay();
                });
        };
        ctrl.deleteComment = function (event) {
            SweetAlert.confirm($translate.instant('Admin.Js.Lead.DeleteComment'), {
                title: '',
            }).then(function (result) {
                if (result.value === true) {
                    $http
                        .post('adminComments/delete', {
                            id: event.Id,
                        })
                        .then(function (response) {
                            if (response.data.Result === true) {
                                toaster.pop('success', '', $translate.instant('Admin.Js.Partials.ChangesSaved'));
                            }
                            ctrl.getLeadEventsWithDelay();
                        });
                }
            });
        };
        ctrl.filterTypeChange = function (filterType) {
            $location.search(FILTER_TYPE_NAME, filterType);
        };
    };
    LeadEventsCtrl.$inject = [
        '$http',
        '$location',
        'toaster',
        '$translate',
        'SweetAlert',
        'adminWebNotificationsEvents',
        'adminWebNotificationsService',
    ];
    ng.module('leadEvents', ['callRecord', 'yaru22.angular-timeago'])
        .controller('LeadEventsCtrl', LeadEventsCtrl)
        .component('leadEvents', {
            templateUrl: leadEventsTemplate,
            controller: LeadEventsCtrl,
            transclude: true,
            bindings: {
                objId: '<?',
                objType: '<?',
                customerId: '<?',
                onInit: '&',
                pageType: '@',
            },
        });
})(window.angular);
