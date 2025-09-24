import getBillingLinkTemplate from './../order/modal/getBillingLink/getBillingLink.html';
(function (ng) {
    'use strict';

    /* @ngInject */
    var LeadCtrl = function (
        uiGridCustomConfig,
        uiGridCustomService,
        $http,
        toaster,
        $timeout,
        $uibModal,
        $q,
        SweetAlert,
        $window,
        Upload,
        leadService,
        $translate,
    ) {
        var ctrl = this,
            timerProcessAddress,
            timerProcessCompanyName;
        ctrl.init = function (Id, form, readOnly) {
            ctrl.instance = {};
            ctrl.instance.Id = Id;
            ctrl.instance.lead = ctrl.instance.lead || {};
            ctrl.instance.lead.Id = Id;
            ctrl.instance.lead.customer = ctrl.instance.lead.customer || {};
            ctrl.getAttachments();
            ctrl.formLead = form;
            ctrl.readOnly = readOnly;
        };
        ctrl.gridLeadItemsOptions = ng.extend({}, uiGridCustomConfig, {
            rowHeight: 90,
            columnDefs: [
                {
                    name: 'ImageSrc',
                    displayName: '',
                    cellTemplate: '<div class="ui-grid-cell-contents"><img class="ui-grid-custom-col-img" ng-src="{{row.entity.ImageSrc}}"></div>',
                    width: 100,
                    enableSorting: false,
                },
                {
                    name: 'Name',
                    displayName: $translate.instant('Admin.Js.Lead.Name'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents ui-grid-cell-contents-order-items">' +
                        '<div ng-if="row.entity.ProductLink != null" class="product-item__title"><a ng-class="{\'product-item-not-enabled\': !row.entity.Enabled}" href="{{row.entity.ProductLink}}" target="_blank">{{row.entity.Name}}</a></div> ' +
                        '<div ng-if="row.entity.ProductLink == null" class="product-item__title" ng-class="{\'product-item-not-enabled\': !row.entity.Enabled}">{{row.entity.Name}}</div> ' +
                        '<div class="order-item-artno product-item__art">' +
                        $translate.instant('Admin.Js.Lead.VendorCode') +
                        '{{row.entity.ArtNo}}</div> ' +
                        '<div class="order-item-artno product-item__art" ng-if="row.entity.BarCode != null && row.entity.BarCode.length > 0">' +
                        $translate.instant('Admin.Js.Lead.BarCode') +
                        '{{row.entity.BarCode}}</div> ' +
                        '<div class="card__row card__row--middle product-item__row" ng-if="row.entity.Color != null && row.entity.Color.length > 0 || row.entity.Size != null && row.entity.Size.length > 0">' +
                        '<div class="product-item__color product-item__col" ng-if="row.entity.Color != null && row.entity.Color.length > 0">{{row.entity.Color}}</div>' +
                        '<div class="product-item__size product-item__col product-item__col--text-right" ng-if="row.entity.Size != null && row.entity.Size.length > 0">{{row.entity.Size}}</div>' +
                        '</div>' +
                        '<div ng-if="row.entity.CustomOptions != null && row.entity.CustomOptions.length > 0"> <div ng-bind-html="row.entity.CustomOptions"></div> </div>' +
                        '</div>',
                },
                {
                    name: 'Dimensions',
                    displayName: $translate.instant('Admin.Js.Lead.Dimensions'),
                    cellTemplate:
                        '<div class="ui-grid-cell-contents" ng-if="row.entity.Length != 0 && row.entity.Width != 0 && row.entity.Height != 0">' +
                        "{{row.entity.Length}} x {{row.entity.Width}} x {{row.entity.Height}} {{'Admin.Js.Lead.Mm'|translate}}" +
                        '</div>',
                    enableCellEdit: false,
                    enableSorting: false,
                    width: 80,
                },
                {
                    name: 'Weight',
                    cellTemplate:
                        '<div class="ui-grid-cell-contents" ng-if="row.entity.Weight != 0">{{row.entity.Weight}} {{\'Admin.Js.Lead.Kg\'|translate}} </div>',
                    displayName: $translate.instant('Admin.Js.Lead.Weight'),
                    enableCellEdit: false,
                    width: 60,
                },
                {
                    name: 'Price',
                    displayName: $translate.instant('Admin.Js.Lead.Price'),
                    enableCellEdit: true,
                    cellEditableCondition: function () {
                        return ctrl.readOnly !== true;
                    },
                    width: 100,
                },
                {
                    name: 'Amount',
                    displayName: $translate.instant('Admin.Js.Lead.Amount'),
                    enableCellEdit: true,
                    cellEditableCondition: function () {
                        return ctrl.readOnly !== true;
                    },
                    width: 80,
                },
                {
                    name: 'Cost',
                    displayName: $translate.instant('Admin.Js.Lead.Cost'),
                    width: 90,
                },
                {
                    name: '_serviceColumn',
                    displayName: '',
                    width: 35,
                    enableSorting: false,
                    useInSwipeBlock: true,
                    cellTemplate: uiGridCustomService.getTemplateCellDelete(
                        'leads/deleteLeadItem',
                        '{LeadId: row.entity.LeadId, leadItemId: row.entity.LeadItemId }',
                    ),
                },
            ],
        });
        ctrl.gridLeadItemsOnInit = function (grid) {
            ctrl.gridLeadItems = grid;
            ctrl.updateReadOnlyStatus();
        };
        ctrl.addLeadItems = function (result) {
            if (result == null || result.ids == null || result.ids.length == 0) return;
            leadService
                .addLeadItems(ctrl.instance.lead.Id, result.ids)
                .then(function (data) {
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Lead.ItemSuccessfullyAdded'));
                    }
                })
                .then(ctrl.gridLeadItemUpdate)
                .then(ctrl.updateLeadEventsWithDelay);
        };
        ctrl.gridLeadItemUpdate = function () {
            ctrl.gridLeadItems.fetchData();
            ctrl.leadItemsSummaryUpdate();
        };
        ctrl.gridLeadItemDelete = function () {
            ctrl.leadItemsSummaryUpdate();
        };
        ctrl.initLeadItemsSummary = function (leadItemsSummary) {
            ctrl.leadItemsSummary = leadItemsSummary;
            ctrl.leadItemsSummaryUpdate(true);
        };
        ctrl.leadItemsSummaryUpdate = function (firstTime) {
            if (ctrl.leadItemsSummary != null) {
                ctrl.leadItemsSummary.getLeadItemsSummary().then(function (data) {
                    if (data != null) {
                        ctrl.hasProducts = data.ProductsCost > 0;
                        if (ctrl.hasProducts || (data.ProductsCost === 0 && ctrl.ProductsCost !== 0 && ctrl.ProductsCost != null)) {
                            ctrl.instance.lead.sum = ctrl.sum = data.SumValueFormat;
                            ctrl.ProductsCost = data.ProductsCost;
                        }
                    }
                });
                if (firstTime != true) {
                    ctrl.updateLeadEventsWithDelay();
                }
            }
        };
        ctrl.onCompleteLead = function (result, close) {
            if (result === true && close) {
                close();
            }
        };
        ctrl.createPaymentLink = function (cb) {
            SweetAlert.confirm($translate.instant('Admin.Js.Lead.CreatePaymentLinkConfirm'), {
                title: '',
            }).then(function (result) {
                if (result === true || result.value === true) {
                    leadService.createPaymentLink(ctrl.instance.lead.Id).then(function (data) {
                        if (data.result === true && data.orderId != null) {
                            ctrl.orderId = data.orderId;
                            $uibModal
                                .open({
                                    bindToController: true,
                                    controller: 'ModalGetBillingLinkCtrl',
                                    controllerAs: 'ctrl',
                                    templateUrl: getBillingLinkTemplate,
                                    resolve: {
                                        params: {
                                            orderId: ctrl.orderId,
                                        },
                                    },
                                })
                                .result.then(
                                    () => {},
                                    (result) => {
                                        if (cb) {
                                            cb();
                                        }
                                    },
                                );
                        }
                    });
                }
            });
        };
        ctrl.deleteLead = function (onDelete) {
            SweetAlert.confirm($translate.instant('Admin.Js.Lead.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Lead.Deleting'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    leadService.deleteLead(ctrl.instance.lead.Id).then(function (data) {
                        if (data.result === true) {
                            toaster.pop('success', $translate.instant('Admin.Js.Lead.ChangesSaved'));
                            if (onDelete) {
                                onDelete();
                                //if ($window.location.href.includes('customers/view')) {
                                //    $window.location.href = $window.location.href;
                                //}
                            } else {
                                $window.location.assign('leads');
                            }
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.error(error);
                            });
                        }
                    });
                }
            });
        };

        /* attachments */
        ctrl.getAttachments = function () {
            leadService.getAttachments(ctrl.instance.lead.Id).then(function (data) {
                ctrl.attachments = data;
            });
        };
        ctrl.uploadAttachment = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            ctrl.loadingFiles = true;
            if (($event.type === 'change' || $event.type === 'drop') && $files != null && $files.length > 0) {
                leadService.uploadAttachment(ctrl.instance.lead.Id, $files).then(function (data) {
                    for (var i in data) {
                        if (data[i].Result === true) {
                            ctrl.attachments.push(data[i].Attachment);
                            toaster.pop(
                                'success',
                                '',
                                $translate.instant('Admin.Js.Lead.File') + data[i].Attachment.OriginFileName + $translate.instant('Admin.Js.Lead.Added'),
                            );
                            ctrl.updateLeadEvents();
                        } else {
                            toaster.pop(
                                'error',
                                $translate.instant('Admin.Js.Lead.ErrorLoading'),
                                (data[i].Attachment != null ? data[i].Attachment.OriginFileName + ': ' : '') + data[i].Error,
                            );
                        }
                    }
                    ctrl.loadingFiles = false;
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.Lead.ErrorLoading'), $translate.instant('Admin.Js.Lead.FileDoesNotMeet'));
                ctrl.loadingFiles = false;
            } else {
                ctrl.loadingFiles = false;
            }
        };
        ctrl.deleteAttachment = function (id) {
            SweetAlert.confirm($translate.instant('Admin.Js.Lead.AreYouSureDeleteFile'), {
                title: $translate.instant('Admin.Js.Lead.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    leadService.deleteAttachment(ctrl.instance.lead.Id, id).then(function () {
                        ctrl.getAttachments();
                        ctrl.updateLeadEvents();
                    });
                }
            });
        };
        /* end attachments */

        ctrl.leadEventsOnInit = function (leadEvents) {
            ctrl.leadEvents = leadEvents;
        };
        ctrl.updateLeadEvents = function () {
            ctrl.leadEvents.getLeadEvents();
        };
        ctrl.updateLeadEventsWithDelay = function () {
            setTimeout(ctrl.updateLeadEvents, 800);
        };
        ctrl.taskGridOnOnit = function (taskGrid) {
            ctrl.taskGrid = taskGrid;
        };
        ctrl.updateTasks = function () {
            ctrl.updateLeadEventsWithDelay();
            leadService.getLeadInfo(ctrl.instance.lead.Id).then(function (data) {
                if (data == null) return;
                ctrl.instance.lead.dealStatusId = data.Lead.DealStatusId.toString();
                ctrl.statuses = data.Statuses;
            });
        };
        ctrl.saveLead = function (lead) {
            let leadValus = [];

            getAllValus(lead);

            function getAllValus(obj) {
                for (var item in obj) {
                    if (typeof obj[item] === 'object') {
                        getAllValus(obj[item]);
                    } else if (Array.isArray(obj[item])) {
                        for (let elem of obj[item]) {
                            getAllValus(elem);
                        }
                    } else {
                        leadValus.push(obj[item]);
                        continue;
                    }
                }
            }

            if (leadValus.includes(undefined)) {
                toaster.pop('error', '', $translate.instant('Admin.Js.Lead.ErrorWhileSaving'));
                return;
            }

            return leadService.saveLead(lead).then(function (data) {
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Lead.ChangesSuccessfullySaved'));
                    ctrl.updateLeadEvents();
                    ctrl.formLead.$setPristine();
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Lead.ErrorWhileSaving'));
                }
            });
        };
        ctrl.updateTitle = function (value) {
            ctrl.instance.lead.title = value;
            ctrl.saveLead(ctrl.instance);
        };
        ctrl.onAfterAddLead = function () {};
        ctrl.saveCustomerField = function () {
            ctrl.saveLead(ctrl.instance);
        };
        ctrl.saveLeadField = function () {
            ctrl.saveLead(ctrl.instance);
        };
        ctrl.changeLeadStatus = function (prevStatusId) {
            ctrl.saveLead(ctrl.instance);
            if (
                prevStatusId !== ctrl.instance.lead.dealStatusId &&
                ctrl.instance.lead.dealStatusId === ctrl.finalStatusId &&
                ctrl.instance.lead.dealStatusId !== ctrl.canceledStatusId &&
                ctrl.finalSuccessAction === 0
            ) {
                SweetAlert.confirm($translate.instant('Admin.Js.Lead.CreateAnOrder'), {
                    title: '',
                }).then(function (result) {
                    if (result === true || result.value) {
                        ctrl.completeLead();
                    }
                });
            }
            ctrl.updateReadOnlyStatus();
        };
        ctrl.completeLead = function () {
            leadService.createOrder(ctrl.instance.lead.Id).then(function (data) {
                if (data.result === true) {
                    if (data.orderId != null && data.orderId != 0) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Lead.OrderSuccessfullyCreated'));
                        $window.location.assign('orders/edit/' + data.orderId);
                    } else {
                        $window.location.reload();
                    }
                } else {
                    toaster.pop('error', '', $translate.instant('Admin.Js.Lead.UnableToCreateOrder'));
                }
            });
        };
        ctrl.changeSalesFunnel = function () {
            ctrl.instance.leadfieldsJs = [];
            ctrl.saveLead(ctrl.instance).then(function () {
                leadService.getLeadInfo(ctrl.instance.lead.Id).then(function (data) {
                    if (data == null) return;
                    ctrl.instance.lead.dealStatusId = data.Lead.DealStatusId.toString();
                    ctrl.statuses = data.Statuses;
                    ctrl.finalStatusId = data.FinalStatusId;
                    ctrl.canceledStatusId = data.CanceledStatusId;
                    ctrl.updateReadOnlyStatus();
                    if (ctrl.leadFieldsReloadFn) {
                        ctrl.leadFieldsReloadFn();
                    }
                });
            });
        };
        ctrl.updateReadOnlyStatus = function () {
            if (ctrl.instance.lead.dealStatusId == ctrl.finalStatusId || ctrl.instance.lead.dealStatusId == ctrl.canceledStatusId) {
                ctrl.readOnly = true;
                ctrl.gridLeadItems.hideColumn('_serviceColumn');
                ctrl.gridLeadItemUpdate();
            } else {
                ctrl.readOnly = false;
                ctrl.gridLeadItems.showColumn('_serviceColumn');
                ctrl.gridLeadItemUpdate();
            }
        };
        ctrl.addSocialUser = function (type, link) {
            if (ctrl.btnSocialAdding != null || !link) return;
            var url = '';
            switch (type) {
                case 'vk':
                    url = 'vk/addVkUser';
                    break;
                case 'facebook':
                    url = 'facebook/addFacebookUser';
                    break;
                case 'instagram':
                    url = 'instagram/addInstagramUser';
                    break;
                case 'ok':
                    url = 'ok/addOkUser';
                    break;
            }
            ctrl.btnSocialAdding = type;
            $http
                .post(url, {
                    customerId: ctrl.instance.lead.customer.customerId,
                    link: link,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Lead.ChangesSaved'));
                        location.reload();
                    } else {
                        if (data.errors != null) {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', '', error);
                            });
                        } else {
                            toaster.pop('error', $translate.instant('Admin.Js.Lead.ErrorWhileSaving'));
                        }
                    }
                })
                .finally(function () {
                    ctrl.btnSocialAdding = null;
                });
        };
        ctrl.deleteSocialLink = function (type) {
            SweetAlert.confirm($translate.instant('Admin.Js.Customer.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Customer.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    var url = '';
                    switch (type) {
                        case 'vk':
                            url = 'vk/deleteVkLink';
                            break;
                        case 'facebook':
                            url = 'facebook/deleteLink';
                            break;
                        case 'instagram':
                            url = 'instagram/deleteLink';
                            break;
                        case 'ok':
                            url = 'ok/deleteOkLink';
                            break;
                        case 'telegram':
                            url = 'telegram/deleteLink';
                            break;
                    }
                    $http
                        .post(url, {
                            customerId: ctrl.instance.lead.customer.customerId,
                        })
                        .then(function (response) {
                            var data = response.data;
                            if (data) {
                                $window.location.reload(true);
                            }
                        });
                }
            });
        };
        ctrl.changeSocialCustomer = function (result, type) {
            if (result == null || result.customerId == null) {
                return;
            }
            $http
                .post('customers/changeSocialCustomer', {
                    fromCustomerId: ctrl.instance.lead.customer.customerId,
                    toCustomerId: result.customerId,
                    type: type,
                })
                .then(function (response) {
                    $window.location.reload(true);
                });
        };
        ctrl.changeCustomer = function (result) {
            if (result == null || result.customerId == null) {
                return;
            }
            $http
                .post('leads/changeCustomer', {
                    leadId: ctrl.instance.lead.Id,
                    newCustomerId: result.customerId,
                })
                .then(function (response) {
                    $window.location.reload(true);
                });
        };
        ctrl.mouseoverOptions = function (event) {
            angular.element(event.currentTarget).click();
        };
        ctrl.filterEvents = function (filterBy) {
            ctrl.leadEvents.filterType = filterBy;
        };
        ctrl.saveAddress = function () {
            var params = {
                leadId: ctrl.instance.lead.Id,
                country: ctrl.instance.lead.Country,
                region: ctrl.instance.lead.Region,
                district: ctrl.instance.lead.District,
                city: ctrl.instance.lead.City,
                zip: ctrl.instance.lead.Zip,
            };
            return $http.post('leads/saveShippingCity', params).then(function (response) {
                toaster.pop('success', '', $translate.instant('Admin.Js.Lead.ChangesSuccessfullySaved'));
                ctrl.leadItemsSummaryUpdate();
            });
        };
        ctrl.processCompanyName = function (item) {
            if (timerProcessCompanyName != null) {
                $timeout.cancel(timerProcessCompanyName);
            }
            return (timerProcessCompanyName = $timeout(
                function () {
                    if (item != null && item.CompanyData) {
                        ctrl.instance.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 1) field.Value = item.CompanyData.CompanyName;
                            else if (field.FieldAssignment == 2) field.Value = item.CompanyData.LegalAddress;
                            else if (field.FieldAssignment == 3) field.Value = item.CompanyData.INN;
                            else if (field.FieldAssignment == 4) field.Value = item.CompanyData.KPP;
                            else if (field.FieldAssignment == 5) field.Value = item.CompanyData.OGRN;
                            else if (field.FieldAssignment == 6) field.Value = item.CompanyData.OKPO;
                        });
                        ctrl.saveCustomerField();
                    } else if (item != null && item.BankData) {
                        ctrl.instance.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 7) field.Value = item.BankData.BIK;
                            else if (field.FieldAssignment == 8) field.Value = item.BankData.BankName;
                            else if (field.FieldAssignment == 9) field.Value = item.BankData.CorrespondentAccount;
                        });
                        ctrl.saveCustomerField();
                    }
                },
                item != null ? 0 : 700,
            ));
        };
        ctrl.processCity = function (leadForm, zone) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }
            return (timerProcessAddress = $timeout(
                function () {
                    if (zone != null) {
                        ctrl.instance.lead.Country = zone.Country;
                        ctrl.instance.lead.Region = zone.Region;
                        ctrl.instance.lead.District = zone.District;
                        ctrl.instance.lead.Zip = zone.Zip;
                        ctrl.saveAddress();
                    }
                    if (zone == null || !zone.Zip) {
                        ctrl.saveAddress(zone == null).then(function (data) {
                            if (data.result === true) {
                                ctrl.instance.lead.Country = data.obj.Country;
                                ctrl.instance.lead.Region = data.obj.Region;
                                ctrl.instance.lead.District = data.obj.District;
                                ctrl.instance.lead.Zip = data.obj.Zip;
                            }
                        });
                    }
                },
                zone != null ? 0 : 300,
            ));
        };
        ctrl.onChangeAddress = function (address) {
            if (!address) {
                return;
            }
            ctrl.instance.lead.Country = address.Country;
            ctrl.instance.lead.Region = address.Region;
            ctrl.instance.lead.District = address.District;
            ctrl.instance.lead.City = address.City;
        };
        ctrl.onSelectAddress = function (address) {
            ctrl.instance.lead.Country = address.Country;
            ctrl.instance.lead.Region = address.Region;
            ctrl.instance.lead.District = address.District;
            ctrl.instance.lead.City = address.City;
            ctrl.instance.lead.Zip = address.Zip;
            ctrl.instance.lead.Street = address.Street;
            ctrl.saveAddress();
        };
    };
    ng.module('lead', ['uiGridCustom', 'leadItemsSummary', 'leadEvents', 'ngFileUpload']).controller('LeadCtrl', LeadCtrl);
})(window.angular);
