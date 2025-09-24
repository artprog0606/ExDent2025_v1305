import sendSmsTemplate from '../_shared/modal/sendSms/sendSms.html';
import sendLetterToCustomerTemplate from '../_shared/modal/sendLetterToCustomer/sendLetterToCustomer.html';
import completeLeadTemplate from '../lead/modal/completeLead/completeLead.html';
import changeLeadManagerTemplate from './modal/changeLeadManager/ChangeLeadManager.html';
import changeLeadSalesFunnelTemplate from './modal/changeLeadSalesFunnel/ChangeLeadSalesFunnel.html';
(function (ng) {
    'use strict';

    var LeadsCtrl = /* @ngInject */ function (
        $cookies,
        $document,
        $http,
        $location,
        $q,
        $uibModal,
        $window,
        adminWebNotificationsEvents,
        adminWebNotificationsService,
        leadService,
        SweetAlert,
        toaster,
        uiGridConstants,
        uiGridCustomConfig,
        leadInfoService,
        $translate,
        urlHelper,
        customerFieldsService,
        leadFieldsService,
        uiGridCustomService,
        isMobileService,
    ) {
        var ctrl = this,
            showSalesFunnelName = urlHelper.getUrlParam('salesFunnelId') == '-1';

        ctrl.$onInit = function () {
            var urlSearch = $location.search();

            ctrl.viewMode = urlSearch != null && urlSearch.viewmode != null && urlSearch.viewmode.length > 0 ? urlSearch.viewmode : 'leads';

            adminWebNotificationsService.addListener(adminWebNotificationsEvents.updateLeads, function () {
                ctrl.fetchData(true);
            });
        };

        ctrl.init = function (useKanban, isAdmin) {
            ctrl.useKanban = ctrl.viewMode !== 'leads' ? null : useKanban;
            ctrl.isAdmin = isAdmin;

            var columnDefs = [
                {
                    name: 'Id',
                    displayName: $translate.instant('Admin.Js.Leads.Number'),
                    enableCellEdit: false,
                    cellTemplate:
                        '<div class="ui-grid-cell-contents">' +
                        '<a href=\'leads?{{ row.entity.SalesFunnelId != 0 ? "salesFunnelId=" + row.entity.SalesFunnelId + "&" : ""}}leadIdInfo={{row.entity.Id}}\' ng-click="grid.appScope.$ctrl.gridExtendCtrl.openLead(row.entity.Id, $event)">{{COL_FIELD}}</a>' +
                        '</div>',
                    width: 90,
                },
            ];

            if (showSalesFunnelName) {
                columnDefs.push({
                    name: 'SalesFunnelName',
                    displayName: $translate.instant('Admin.Js.Leads.SalesFunnel'),
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Leads.SalesFunnel'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'FunnelId',
                        fetch: 'salesFunnels/getSalesFunnels',
                    },
                });
            }

            if (ctrl.useKanban) {
                columnDefs.push({
                    name: '_noopColumnStatus',
                    visible: false,
                    enableHiding: false,
                    width: 170,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Leads.DealStage'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'StatusId',
                        fetch: 'salesFunnels/getDealStatuses',
                    },
                });
            }

            columnDefs = columnDefs.concat([
                {
                    name: 'DealStatusName',
                    displayName: $translate.instant('Admin.Js.Leads.DealStage'),
                    enableCellEdit: false,
                    width: 170,
                },
                {
                    name: 'FullName',
                    displayName: $translate.instant('Admin.Js.Leads.Contact'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Leads.Contact'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Name',
                    },
                },
                {
                    name: 'ManagerName',
                    displayName: $translate.instant('Admin.Js.Leads.Manager'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Leads.Manager'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'ManagerId',
                        fetch: 'managers/getManagersSelectOptions?includeEmpty=true',
                    },
                },
                {
                    name: 'ProductsCount',
                    displayName: $translate.instant('Admin.Js.Leads.Products'),
                    enableCellEdit: false,
                    width: 90,
                },
                {
                    name: 'SumFormatted',
                    displayName: $translate.instant('Admin.Js.Leads.Budget'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Leads.Budget'),
                        type: 'range',
                        rangeOptions: {
                            from: {
                                name: 'SumFrom',
                            },
                            to: {
                                name: 'SumTo',
                            },
                        },
                    },
                    width: 100,
                },
                {
                    name: 'CreatedDateFormatted',
                    displayName: $translate.instant('Admin.Js.Leads.DateOfCreation'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Leads.DateOfCreation'),
                        type: 'datetime',
                        term: {
                            from: new Date(new Date().setMonth(new Date().getMonth() - 1)),
                            to: new Date(),
                        },
                        datetimeOptions: {
                            from: {
                                name: 'CreatedDateFrom',
                            },
                            to: {
                                name: 'CreatedDateTo',
                            },
                        },
                    },
                    width: 150,
                },
                {
                    name: 'DescriptionCut',
                    displayName: $translate.instant('Admin.Js.Leads.Description'),
                    enableCellEdit: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Leads.Description'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Description',
                    },
                },
                {
                    name: '_noopColumnOrganization',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Leads.Organization'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'Organization',
                    },
                },
                {
                    name: '_noopColumnSources',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Leads.LeadSource'),
                        type: uiGridConstants.filter.SELECT,
                        name: 'OrderSourceId',
                        fetch: 'leads/getordersources',
                    },
                },
                {
                    name: '_noopColumnCity',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Customers.City'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'City',
                    },
                },
                {
                    name: '_noopColumnProduct',
                    visible: false,
                    enableHiding: false,
                    filter: {
                        placeholder: $translate.instant('Admin.Js.Leads.NameOrVendorCodeOfProduct'),
                        type: uiGridConstants.filter.INPUT,
                        name: 'ProductNameArtNo',
                    },
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 80,
                    useInSwipeBlock: true,
                    enableHiding: false,
                    cellTemplate:
                        '<div ng-if="!grid.appScope.$ctrl.isMobile" class="ui-grid-cell-contents"><div>' +
                        '<button type="button" class="btn-icon ui-grid-custom-service-icon fas fa-pencil-alt" ng-click="grid.appScope.$ctrl.gridExtendCtrl.openLead(row.entity.Id, $event)" aria-label="Редактировать"></button>' +
                        (ctrl.isAdmin
                            ? '<ui-grid-custom-delete url="leads/deleteLeads" params="{\'Ids\': row.entity.Id}"></ui-grid-custom-delete>'
                            : '') +
                        '</div></div>' +
                        '<ui-grid-custom-delete ng-if="grid.appScope.$ctrl.isMobile" url="leads/deleteLeads" params="{\'Ids\': row.entity.Id}" class="btn btn-sm btn-danger btn--as-swipe-line flex center-xs middle-xs">Удалить</ui-grid-custom-delete>',
                },
            ]);

            ctrl.gridOptions = ng.extend({}, uiGridCustomConfig, {
                enableGridMenu: !isMobileService.getValue(),
                columnDefs: columnDefs,
                uiGridCustom: {
                    rowClick: function ($event, row) {
                        ctrl.openLead(row.entity.Id);
                    },
                    selectionOptions: (!ctrl.isAdmin
                        ? []
                        : [
                              {
                                  text: $translate.instant('Admin.Js.Leads.DeleteSelected'),
                                  url: 'leads/deleteLeads',
                                  field: 'Id',
                                  before: function () {
                                      return SweetAlert.confirm($translate.instant('Admin.Js.Leads.AreYouSureDelete'), {
                                          title: $translate.instant('Admin.Js.Leads.Deleting'),
                                      }).then(function (result) {
                                          return result === true || result.value ? $q.resolve('sweetAlertConfirm') : $q.reject('sweetAlertCancel');
                                      });
                                  },
                              },
                          ]
                    ).concat([
                        {
                            template:
                                '<ui-modal-trigger data-on-close="$ctrl.gridActionWithCallback($ctrl.clearStorage);" data-controller="\'ModalChangeLeadManagerCtrl\'" data-controller-as="ctrl" ' +
                                'data-resolve="{params: $ctrl.getSelectedParams(\'Id\') }" ' +
                                'template-url="' +
                                changeLeadManagerTemplate +
                                '">' +
                                $translate.instant('Admin.Js.Leads.SetManagerToSelected') +
                                '</ui-modal-trigger>',
                        },
                        {
                            template:
                                '<ui-modal-trigger data-on-close="$ctrl.gridActionWithCallback($ctrl.clearStorage);" data-controller="\'ModalChangeLeadSalesFunnelCtrl\'" data-controller-as="ctrl" ' +
                                'data-resolve="{params: $ctrl.getSelectedParams(\'Id\') }" ' +
                                'template-url="' +
                                changeLeadSalesFunnelTemplate +
                                '">' +
                                $translate.instant('Admin.Js.Leads.ChangeDealStatusToSelected') +
                                '</ui-modal-trigger>',
                        },
                    ]),
                },
            });
        };

        ctrl.leadsParam = { dealStatusId: null };

        ctrl.changeParam = function (statusId) {
            ctrl.leadsParam['dealStatusId'] = statusId;
            ctrl.grid.setParams(ctrl.leadsParam);
            ctrl.grid.fetchData();
        };

        ctrl.changeSalesFunnel = function (id) {
            ctrl.salesFunnelId = id;
            ctrl.leadsParam['salesFunnelId'] = id;
            if (ctrl.grid != null) {
                ctrl.grid.setParams(ctrl.leadsParam);
                ctrl.grid.fetchData();
            }
        };

        ctrl.gridOnInit = function (grid) {
            ctrl.grid = grid;

            // grid.setParams({
            //     customerField: ctrl.gridColumnsCustomerField.map(x => x.),
            //     leadField:  ctrl.gridColumnsLeadField
            // })
        };

        ctrl.gridOnFilterInit = function (filter) {
            ctrl.gridFilter = filter;
            customerFieldsService.getFilterColumns().then(function (columns) {
                Array.prototype.push.apply(ctrl.gridOptions.columnDefs, columns);

                ctrl.gridColumnsCustomerField = columns;

                if (ctrl.salesFunnelId) {
                    leadFieldsService.getFilterColumns(ctrl.salesFunnelId).then(function (columns) {
                        Array.prototype.push.apply(ctrl.gridOptions.columnDefs, columns);

                        ctrl.gridColumnsLeadField = columns;

                        ctrl.gridFilter.updateColumns();
                    });
                } else {
                    ctrl.gridFilter.updateColumns();
                }
            });
        };

        ctrl.fetchData = function (ignoreHistory) {
            if (!ctrl.useKanban) {
                ctrl.grid.fetchData(ignoreHistory);
            } else {
                ctrl.kanban.fetchData();
            }
        };

        ctrl.modalAddLeadClose = function () {
            ctrl.fetchData(true);
        };

        ctrl.changeBuyInOneClickCreateOrder = function () {
            $http.post('leads/changeBuyInOneClickCreateOrder').then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.Leads.ChangesSaved'));
                window.location.reload();
            });
        };

        ctrl.closeOrderFromBuyInOneClickMsg = function () {
            ctrl.hideOrderFromBuyInOneClickMsg = true;
            $http.post('leads/hideOrderFromBuyInOneClickMsg').then(function (response) {});
        };

        ctrl.changeView = function (view) {
            ctrl.setCookie('leads_viewmode', view);
            $location.search('grid', null);
            $location.search('kanban', null);

            ctrl.reload();
        };

        ctrl.reload = function () {
            var url = $window.location.href.split('#')[0];
            url = urlHelper.updateQueryStringParameter(url, 'rnd', Math.random());
            url = urlHelper.updateQueryStringParameter(url, 'useKanban', undefined);
            url = urlHelper.updateQueryStringParameter(url, 'viewmode', undefined);
            $window.location.href = url;
        };

        ctrl.setCookie = function (name, value) {
            var date = new Date();
            date.setFullYear(date.getFullYear() + 1);
            $cookies.put(name, value, { expires: date });
        };

        // kanban

        ctrl.sortableOptions = {
            containment: '#kanban',
            containerPositioning: 'relative',
            additionalPlaceholderClass: 'kanban__placeholder',
            itemMoved: function (event) {
                var lead = event.source.itemScope.modelValue,
                    columnId = event.dest.sortableScope.$parent.column.Id;
                if (columnId == 'CompleteLead') {
                    ctrl.completeLead(lead.Id).then(function (result) {
                        if (result == 'cancel') {
                            event.dest.sortableScope.removeItem(event.dest.index);
                            event.source.itemScope.sortableScope.insertItem(event.source.index, event.source.itemScope.modelValue);
                        } else if (result != 'redirect') {
                            ctrl.fetchData();
                        }
                    });
                } else {
                    leadService.changeDealStatus(lead.Id, columnId).then(function (data) {
                        toaster.pop('success', $translate.instant('Admin.Js.Leads.TransactionStageChanged'));
                        ctrl.onOrderChanged(event);
                        ctrl.fetchData();
                    });
                }
            },
            orderChanged: function (event) {
                ctrl.onOrderChanged(event, true);
            },
        };

        ctrl.onOrderChanged = function (event, showMessage) {
            var leadId = event.source.itemScope.card.Id,
                prev = event.dest.sortableScope.modelValue[event.dest.index - 1],
                next = event.dest.sortableScope.modelValue[event.dest.index + 1];
            $http
                .post('leads/changeSorting', {
                    id: leadId,
                    prevId: prev != null ? prev.Id : null,
                    nextId: next != null ? next.Id : null,
                })
                .then(function (response) {
                    if (showMessage && response.data != null && response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Leads.ChangesSaved'));
                    }
                });
        };

        ctrl.kanbanOnInit = function (kanban) {
            ctrl.kanban = kanban;
        };

        ctrl.kanbanOnFilterInit = function (filter) {
            ctrl.kanbanFilter = filter;
            customerFieldsService.getFilterColumns().then(function (columns) {
                Array.prototype.push.apply(ctrl.gridOptions.columnDefs, columns);
                if (ctrl.salesFunnelId) {
                    leadFieldsService.getFilterColumns(ctrl.salesFunnelId).then(function (columns) {
                        Array.prototype.push.apply(ctrl.gridOptions.columnDefs, columns);
                        ctrl.kanbanFilter.updateColumns();
                    });
                } else {
                    ctrl.kanbanFilter.updateColumns();
                }
            });
        };

        ctrl.completeLead = function (leadId) {
            return $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalCompleteLeadCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: completeLeadTemplate,
                    resolve: {
                        id: leadId,
                    },
                })
                .result.then(
                    function (result) {
                        return result;
                    },
                    function (result) {
                        return 'cancel';
                    },
                );
        };

        ctrl.openLead = function (leadId, $event) {
            if ($event) {
                $event.preventDefault();
            }
            leadInfoService.addInstance(
                {
                    leadId: leadId,
                },
                {
                    onClose: function (result) {
                        ctrl.fetchData();
                    },
                },
            );
        };

        ctrl.getCommunicationParams = function () {
            if (ctrl.useKanban) {
                if (ctrl.kanban != null) {
                    return ctrl.kanban.getRequestParams();
                }
            } else if (ctrl.grid != null) {
                return ctrl.grid.getRequestParams();
            } else {
                return urlHelper.getUrlParamsAsObject($window.location.href);
            }
        };

        ctrl.getCustomerIds = function () {
            if (ctrl.useKanban) {
                return $http.post('leads/getKanbanCustomerIds', ctrl.getCommunicationParams()).then(function (response) {
                    return response.data;
                });
            } else {
                var params = ctrl.grid.selectionCustom.getSelectedParams('CustomerId');
                if (params.selectMode == 'none' && params.ids != null && params.ids.length > 0) {
                    return $q.resolve(params.ids);
                }

                return $http.post('leads/getLeadCustomerIds', params).then(function (response) {
                    return response.data;
                });
            }
        };

        ctrl.getLetterRecipients = function () {
            if (ctrl.useKanban) {
                return $http.post('leads/getKanbanLetterRecipients', ctrl.getCommunicationParams()).then(function (response) {
                    return response.data;
                });
            } else {
                var params = ctrl.grid.selectionCustom.getSelectedParams('Id');

                return $http.post('leads/getLetterRecipients', params).then(function (response) {
                    return response.data;
                });
            }
        };

        ctrl.getSmsRecipients = function () {
            if (ctrl.useKanban) {
                return $http.post('leads/getKanbanSmsRecipients', ctrl.getCommunicationParams()).then(function (response) {
                    return response.data;
                });
            } else {
                var params = ctrl.grid.selectionCustom.getSelectedParams('Id');

                return $http.post('leads/getSmsRecipients', params).then(function (response) {
                    return response.data;
                });
            }
        };

        ctrl.export = function () {
            if (ctrl.grid != null) {
                ctrl.grid.export();
            } else {
                var params = ctrl.getCommunicationParams();
                $http.post('leads/kanbanExport', params).then(function (response) {
                    var data = response.data;
                    if (data != null && data.url != null) {
                        $window.location.assign(data.url);
                    }
                });
            }
        };

        ctrl.sendEmail = function () {
            ctrl.getLetterRecipients().then(function (recipients) {
                $uibModal.open({
                    animation: false,
                    bindToController: true,
                    controller: 'ModalSendLetterToCustomerCtrl',
                    controllerAs: 'ctrl',
                    size: 'lg',
                    templateUrl: sendLetterToCustomerTemplate,
                    resolve: {
                        params: {
                            recipients: recipients,
                            pageType: 'leads',
                        },
                    },
                });
            });
        };

        ctrl.sendSms = function () {
            ctrl.getSmsRecipients().then(function (recipients) {
                $uibModal.open({
                    animation: false,
                    bindToController: true,
                    controller: 'ModalSendSmsAdvCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: sendSmsTemplate,
                    resolve: {
                        params: {
                            recipients: recipients,
                            pageType: 'leads',
                        },
                    },
                });
            });
        };

        ctrl.setViewMode = function (mode) {
            ctrl.viewMode = mode;
            $location.search('viewmode', mode);
        };

        ctrl.updateSalesFunnel = function (result) {
            ctrl.salesFunnelName = result.Name;
            ctrl.fetchData();
        };
    };

    ng.module('leads', ['uiGridCustom', 'urlHelper']).controller('LeadsCtrl', LeadsCtrl);
})(window.angular);
