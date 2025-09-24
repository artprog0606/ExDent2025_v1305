import leadFieldsListTemplate from './leadFieldsList.html';
(function (ng) {
    'use strict';

    var LeadFieldsListCtrl = function (toaster, SweetAlert, $translate, leadFieldsService, isMobileService) {
        var ctrl = this;
        ctrl.$onInit = function () {
            ctrl.fetch();
        };
        ctrl.fetch = function () {
            if (ctrl.salesFunnelId != null) {
                leadFieldsService.getLeadFields(ctrl.salesFunnelId, false).then(function (data) {
                    ctrl.items = data.obj || [];
                });
            } else {
                ctrl.items = [];
            }
        };
        ctrl.sortableOptions = {
            orderChanged: function (event) {
                if (ctrl.salesFunnelId == null) {
                    return;
                }
                var id = event.source.itemScope.item.Id,
                    prev = ctrl.items[event.dest.index - 1],
                    next = ctrl.items[event.dest.index + 1];
                leadFieldsService
                    .changeLeadFieldSorting(ctrl.salesFunnelId, id, prev != null ? prev.Id : null, next != null ? next.Id : null)
                    .then(function (data) {
                        if (data.result === true) {
                            toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                        }
                    });
            },
        };
        ctrl.deleteItem = function (item) {
            if (ctrl.salesFunnelId != null) {
                SweetAlert.confirm($translate.instant('Admin.Js.SettingsCrm.AreYouSureDelete'), {
                    title: $translate.instant('Admin.Js.SettingsCrm.Delete'),
                }).then(function (result) {
                    if (result === true || result.value === true) {
                        leadFieldsService.deleteLeadField(item.Id).then(function (data) {
                            if (data.result === true) {
                                ctrl.fetch();
                                toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                            }
                        });
                    }
                });
            } else {
                var index = ctrl.items.indexOf(item);
                if (index !== -1) {
                    ctrl.items.splice(index, 1);
                }
            }
            if (isMobileService.getValue()) {
                document.querySelector('.swal2-container').style.zIndex = 2000;
            }
        };
        ctrl.onAddEditLeadField = function (index, edited, added) {
            if (ctrl.salesFunnelId != null) {
                ctrl.fetch();
            } else {
                if (edited != null) {
                    ctrl.items[index] = edited;
                }
                if (added != null) {
                    ctrl.items.push(added);
                }
            }
        };
        ctrl.inplace = function (item, field, checked) {
            item[field] = checked;
            if (ctrl.salesFunnelId == null) {
                return;
            }
            leadFieldsService.inplaceLeadField(item).then(function (data) {
                if (data.result == true) {
                    toaster.success('', $translate.instant('Admin.Js.ChangesSaved'));
                } else {
                    toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                }
            });
        };
    };
    LeadFieldsListCtrl.$inject = ['toaster', 'SweetAlert', '$translate', 'leadFieldsService', 'isMobileService'];
    ng.module('leadFieldsList', ['as.sortable', 'isMobile'])
        .controller('LeadFieldsListCtrl', LeadFieldsListCtrl)
        .component('leadFieldsList', {
            templateUrl: leadFieldsListTemplate,
            controller: 'LeadFieldsListCtrl',
            transclude: true,
            bindings: {
                onInit: '&',
                salesFunnelId: '<',
                items: '=',
            },
        });
})(window.angular);
