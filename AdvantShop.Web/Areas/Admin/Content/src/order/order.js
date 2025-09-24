import changeOrderStatusTemplate from './modal/changeOrderStatus/changeOrderStatus.html';
import editCustomOptionsTemplate from './modal/editCustomOptions/editCustomOptions.html';
import changeMarkingTemplate from './modal/changeMarking/changeMarking.html';
import iconAmount from '../../images/icon/dolly-solid.svg?raw';
import iconInfo from '../../images/icon/info.svg?raw';
import changeMarkingDirectiveTemplate from './templates/changeMarkingTemplate.html';
import distributionOfOrderItemTemplate from './modal/distributionOfOrderItem/distributionOfOrderItem.html';

(function (ng) {
    'use strict';

    var OrderCtrl = /* @ngInject */ function (
        uiGridCustomConfig,
        $http,
        $httpParamSerializer,
        toaster,
        $timeout,
        $uibModal,
        $q,
        SweetAlert,
        lastStatisticsService,
        $translate,
        $window,
        isMobileService,
        uiGridCustomService,
        Upload,
    ) {
        var ctrl = this;
        var timerChangeCustomer, timerProcessAddress, timerProcessCompanyName;
        ctrl.$onInit = function () {
            ctrl.isMobile = isMobileService.getValue();
            ctrl.getOrderStatuses();
            ctrl.isSave = false;
        };
        ctrl.initOrder = function (orderId, isEditMode, isDraft, customerId, standardPhone, hasContacts) {
            ctrl.orderId = orderId;
            ctrl.isEditMode = isEditMode;
            ctrl.isDraft = isDraft;
            ctrl.customerId = customerId;
            ctrl.standardPhone = standardPhone;
            ctrl.hasContacts = hasContacts;
            ctrl.getOrderAttachments(orderId);
        };
        ctrl.startGridOrderItems = function (isPaied, hasCerticates, availableWarehouses) {
            ctrl.isPaied = isPaied;
            ctrl.hasCerticates = hasCerticates;
            ctrl.availableWarehouses = availableWarehouses;
            var paginationPageSize = 20;
            var paginationPageSizeMobile = 10;
            var paginationPageSizes = [20, 50, 100];
            if (ctrl.isMobile) {
                paginationPageSize = paginationPageSizeMobile;
                paginationPageSizes.unshift(paginationPageSizeMobile);
            }
            ctrl.gridOrderItemsOptions = ng.extend({}, uiGridCustomConfig, {
                enableGridMenu: !ctrl.isMobile,
                rowHeight: 95,
                columnDefs: [
                    {
                        name: 'Position',
                        displayName: $translate.instant('Admin.Js.Order.Position'),
                        cellTemplate: '<div class="ui-grid-cell-contents"><span class="order-grid__css-counter"></span></div>',
                        width: 45,
                        enableSorting: false,
                        enableHiding: false,
                    },
                    {
                        name: 'ImageSrc',
                        displayName: '',
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><img class="ui-grid-custom-col-img" ng-src="{{row.entity.ImageSrc}}"/></div>',
                        width: 100,
                        enableSorting: false,
                        enableCellEdit: false,
                        enableHiding: false,
                    },
                    {
                        name: 'ArtNo',
                        width: 150,
                        displayName: $translate.instant('Admin.Js.Order.ArtNo'),
                        enableSorting: true,
                        enableCellEdit: false,
                        visible: false,
                        enableHiding: true,
                    },
                    {
                        name: 'Name',
                        displayName: $translate.instant('Admin.Js.Order.Name'),
                        cellTemplate:
                            '<div class="ui-grid-cell-contents ui-grid-cell-contents-order-items">' +
                            '<div>' +
                            '<div ng-if="row.entity.ProductLink != null"><a href="{{row.entity.ProductLink}}" target="_blank" ng-class="{\'product-item-not-enabled\': !row.entity.Enabled}">{{row.entity.Name}}</a></div> ' +
                            '<div ng-if="row.entity.ProductLink == null">{{row.entity.Name}}</div> ' +
                            '<div data-ng-if="!grid.getColDef(\'ArtNo\').visible" class="order-item-artno">' +
                            $translate.instant('Admin.Js.Order.VendorCode') +
                            '{{row.entity.ArtNo}}</div> ' +
                            '<div class="order-item-artno" ng-if="row.entity.BarCode != null && row.entity.BarCode.length > 0">' +
                            $translate.instant('Admin.Js.Order.BarCode') +
                            '{{row.entity.BarCode}}</div> ' +
                            '<div ng-if="row.entity.Color != null && row.entity.Color.length > 0">{{row.entity.Color}}</div>' +
                            '<div ng-if="row.entity.Size != null && row.entity.Size.length > 0">{{row.entity.Size}}</div>' +
                            '<div ng-if="row.entity.CustomOptions != null && row.entity.CustomOptions.length > 0"> <div ng-bind-html="row.entity.CustomOptions"></div> </div>' +
                            '<div ng-if="!row.entity.Available"><div ng-repeat="error in row.entity.AvailableText" class="order-notavalable" ng-bind="error"></div></div>' +
                            '<div ng-if="row.entity.Length != 0 && row.entity.Width != 0 && row.entity.Height != 0">' +
                            $translate.instant('Admin.Js.Order.Dimensions') +
                            ': {{row.entity.Length}} x {{row.entity.Width}} x {{row.entity.Height}} мм </div>' +
                            '<div ng-if="row.entity.Weight != 0">' +
                            $translate.instant('Admin.Js.Order.Weight') +
                            ': {{row.entity.Weight}} кг </div>' +
                            '<div ng-if="!grid.appScope.$ctrl.gridExtendCtrl.isPaied && row.entity.ShowEditCustomOptions"> ' +
                            '<ui-modal-trigger data-controller="\'ModalEditCustomOptionsCtrl\'" data-controller-as="ctrl" ' +
                            'data-resolve="{params: { orderItemId: row.entity.OrderItemId, productId: row.entity.ProductId, artno: row.entity.ArtNo}}" ' +
                            'data-on-close="grid.appScope.$ctrl.gridExtendCtrl.gridOrderItemUpdate()" ' +
                            'template-url="' +
                            editCustomOptionsTemplate +
                            '">' +
                            '<a href="" class="order-item-edit-custom-options">изменить доп. опции</a></ui-modal-trigger>' +
                            '</div>' +
                            '</div>' +
                            '</div>',
                        enableCellEdit: false,
                        enableHiding: false,
                        minWidth: 150,
                    },
                    {
                        name: 'PriceWhenOrdering',
                        displayName: $translate.instant('Admin.Js.Order.PriceWhenOrdering'),
                        cellTemplate:
                            '<div class="ui-grid-cell-contents ui-grid-cell-contents-order-items__price-when-ordering">' +
                            '<div ng-bind-html="row.entity.PriceWhenOrdering"></div>' +
                            '</div>',
                        enableCellEdit: false,
                        width: 120,
                        visible: false,
                        enableHiding: true,
                    },
                    {
                        name: 'DiscountWhenOrdering',
                        displayName: $translate.instant('Admin.Js.Order.DiscountWhenOrdering'),
                        cellTemplate: '<div class="ui-grid-cell-contents" ng-bind-html="row.entity.DiscountWhenOrdering"></div>',
                        enableCellEdit: false,
                        width: 120,
                        visible: false,
                        enableHiding: true,
                    },
                    {
                        name: 'IsCustomPrice',
                        displayName: $translate.instant('Admin.Js.Order.IsCustomPrice'),
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><div class="adv-checkbox-label">' +
                            '<input type="checkbox" ng-model="row.entity.IsCustomPrice" readonly class="adv-checkbox-input control-checkbox pointer-events-none" />' +
                            '<span class="adv-checkbox-emul"></span>' +
                            '</div></div>',
                        enableCellEdit: false,
                        width: 100,
                        visible: false,
                        enableHiding: true,
                    },
                    {
                        name: 'PriceString',
                        displayName: $translate.instant('Admin.Js.Order.Price'),
                        enableCellEdit: true,
                        width: 100,
                        enableHiding: false,
                    },
                    {
                        name: 'Amount',
                        displayName: $translate.instant('Admin.Js.Order.Amount'),
                        enableCellEdit: true,
                        width: 70,
                        enableHiding: false,
                    },
                    {
                        name: 'Stocks',
                        displayName: $translate.instant('Admin.Js.Order.Stoks'),
                        cellTemplate: `<div class="ui-grid-cell-contents" ng-init="grid.appScope.$ctrl.gridExtendCtrl.setLimitItemsStocks(row, 3)">
                                <div class="order-grid-stocks">
                                    <div class="order-grid-stocks__item" ng-repeat="stock in row.entity.StocksText | limitTo:row.gridColStocksLimitItems" ng-bind="stock"></div>
                                    <div class="order-grid-stocks__button" data-ng-if="row.entity.StocksText.length > row.gridColStocksLimitItems"><button class="btn-link italic" ng-click="grid.appScope.$ctrl.gridExtendCtrl.setLimitItemsStocks(row, 3)" type="button" ng-bind="grid.appScope.$ctrl.gridExtendCtrl.getButtonTextMore(row)"></button> </div>
                                </div>
                            </div>`,
                        enableCellEdit: false,
                        width: 220,
                        visible: false,
                        enableSorting: false,
                        enableHiding: true,
                    },
                    {
                        name: 'Marking',
                        displayName: '',
                        cellTemplate:
                            '<div class="ui-grid-cell-contents p-n" ng-if="row.entity.MarkingRequiredValidation != null">' +
                            '<div ng-click="grid.appScope.$ctrl.gridExtendCtrl.openChangeMarkingModal(row.entity)">' +
                            '<change-marking is-mobile="false" is-required-validation="row.entity.MarkingRequiredValidation"></change-marking>' +
                            '</div>' +
                            '</div>',
                        width: 20,
                        enableHiding: false,
                    },
                    {
                        name: 'Cost',
                        displayName: $translate.instant('Admin.Js.Order.Cost'),
                        width: 100,
                        enableHiding: false,
                    },
                    {
                        name: 'Info',
                        displayName: '',
                        cellTemplate: `<div class="ui-grid-cell-contents p-n flex">
                            <div>
                            <a href="" ng-click="grid.appScope.$ctrl.gridExtendCtrl.showDistributionOfOrderItem(row.entity); $event.preventDefault();" class="ui-grid-custom-service-icon fas fa-dolly" ng-if="grid.appScope.$ctrl.gridExtendCtrl.availableWarehouses && row.entity.CountWarehouses !== 1"></a>
                            <help-trigger use-template="true" help-trigger-icon-bg="false">
                            <help-trigger-icon>${iconInfo}</help-trigger-icon>
                            <p><span>Ед. измерения:</span> <span ng-bind-html="row.entity.UnitName"></span></p>
                            <p><span>Мера (ФФД 1.2):</span> <span ng-bind-html="row.entity.MeasureName"></span></p>
                            <p><span>Предмет расчета:</span> <span ng-bind-html="row.entity.PaymentSubjectName"></span></p>
                            <p><span>Способ расчета:</span> <span ng-bind-html="row.entity.PaymentMethodName"></span></p>
                            <p ng-show="row.entity.DownloadLink !== ''"><span>Ссылка на скачивание:</span> <a class="product-download-link" ng-href="{{row.entity.DownloadLink}}" target="_blank"><span class="product-download-link__text" ng-bind-html="row.entity.DownloadLink"></span></a></p>
                            </help-trigger>
                            </div>
                            </div>`,
                        width: 60,
                        enableSorting: false,
                        enableHiding: false,
                    },
                    {
                        name: 'Delete',
                        displayName: '',
                        useInSwipeBlock: true,
                        onClickSwipeElement: ctrl.checkStopEdit,
                        cellTemplate:
                            '<div class="ui-grid-cell-contents"><div>' +
                            '<div' +
                            (!ctrl.isMobile ? ' ng-if="!grid.appScope.$ctrl.gridExtendCtrl.isPaied"' : '') +
                            '>' +
                            uiGridCustomService.getTemplateCellDelete(
                                'orders/deleteOrderItem',
                                '{orderId: row.entity.OrderId, orderItemId: row.entity.OrderItemId }',
                                'on-delete="grid.appScope.$ctrl.gridExtendCtrl.onChangeHistory()"',
                            ) +
                            '</div>' +
                            '</div></div>',
                        width: 50,
                        enableSorting: false,
                        enableHiding: false,
                    },
                ],
                paginationPageSize: paginationPageSize,
                paginationPageSizes: paginationPageSizes,
            });
            ctrl.gridOrderCertificatesOptions = ng.extend({}, uiGridCustomConfig, {
                columnDefs: [
                    {
                        name: 'CustomName',
                        displayName: '',
                        cellTemplate: '<div class="ui-grid-cell-contents">' + $translate.instant('Admin.Js.Order.Certificate') + '</div>',
                    },
                    {
                        name: 'CertificateCode',
                        displayName: $translate.instant('Admin.Js.Order.CertificateCode'),
                    },
                    {
                        name: 'Sum',
                        displayName: $translate.instant('Admin.Js.Order.Sum'),
                    },
                    {
                        name: 'Price',
                        displayName: $translate.instant('Admin.Js.Order.UsedInOrderN'),
                    },
                ],
            });
            ctrl.isShowGridOrderItem = true;
        };
        ctrl.gridOrderItemsOnInit = function (grid) {
            ctrl.gridOrderItems = grid;
            ctrl.gridOrderOnFetch();
        };
        ctrl.gridOrderItemsSelectionOnInit = function (selectionCustom) {
            ctrl.selectionCustom = selectionCustom;
        };
        ctrl.gridOrderOnFetch = function () {
            if (ctrl.gridOrderItems == null) return;
            var params = ctrl.gridOrderItems.getRequestParams();
            if (params.sorting != null && params.sortingType != null) {
                ctrl.gridOrderItemsSorting = '&sorting=' + params.sorting + '&sortingType=' + params.sortingType;
            } else {
                ctrl.gridOrderItemsSorting = null;
            }
        };
        ctrl.addOrderItems = function (result) {
            if (result == null || result.ids == null || result.ids.length == 0) return;
            ctrl.saveDraft().then(function () {
                ctrl.gridOrderItems.isProcessing = true;
                $http
                    .post('orders/addOrderItems', {
                        orderId: ctrl.orderId,
                        offerIds: result.ids,
                    })
                    .then(function (response) {
                        if (response.data.result === true) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Order.ProductSuccessfullyAdded'));
                        }
                    })
                    .then(ctrl.gridOrderItemUpdate)
                    .then(ctrl.onChangeHistory)
                    .catch(function () {
                        toaster.pop('error', $translate.instant('Admin.Js.Order.ErrorWhileAddingOrder'));
                    })
                    .finally(function () {
                        ctrl.gridOrderItems.isProcessing = false;
                    });
            });
        };
        ctrl.gridOrderItemUpdate = function () {
            ctrl.gridOrderItems
                .fetchData()
                .then(function () {
                    ctrl.orderItemsSummaryUpdate();
                })
                .finally(() => ctrl.gridOrderItems?.setStateProcess(false));
        };

        ctrl.gridOrderItemBeforeDelete = function () {
            ctrl.gridOrderItems?.setStateProcess(true);
        };

        ctrl.gridOrderItemDelete = function () {
            ctrl.orderItemsSummaryUpdate().finally(() => {
                ctrl.gridOrderItems?.setStateProcess(false);
            });
        };
        ctrl.initOrderItemsSummary = function (orderItemsSummary) {
            ctrl.orderItemsSummary = orderItemsSummary;
        };
        ctrl.orderItemsSummaryUpdate = function () {
            if (ctrl.orderItemsSummary != null) {
                return ctrl.orderItemsSummary.getOrderItemsSummary();
            }
            return Promise.resolve();
        };
        ctrl.updateByOrderItemsSummary = function () {
            ctrl.gridOrderItems.fetchData();
        };
        ctrl.getStatusText = function () {
            const selectedOption = [...ctrl.statusDropdownEl.options].find((it) => {
                return it.value === ctrl.orderStatus;
            });
            return selectedOption != null ? selectedOption.text : '';
        };
        ctrl.getOrderStatuses = function () {
            return $http.get('orderStatuses/getAllStatuses').then(function (response) {
                ctrl.orderStatuses = response.data;
            });
        };
        ctrl.getStatusColor = function (name) {
            ctrl.orderStatuses.find(function (elem) {
                if (name == elem.StatusName) {
                    ctrl.colorStatusOrder = elem.Color;
                    return true;
                }
            });
        };
        ctrl.changeStatus = function (e) {
            const event = e != null ? e : window.event;
            ctrl.statusDropdownEl = event.target;
            ctrl.statusText = ctrl.getStatusText();
            ctrl.getStatusColor(ctrl.statusText);
            ctrl.isSave = false;
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalChangeOrderStatusCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: changeOrderStatusTemplate,
                    resolve: {
                        params: {
                            orderId: ctrl.orderId,
                            statusId: ctrl.orderStatus,
                            statusName: ctrl.statusText,
                            orderCtrl: ctrl,
                        },
                    },
                })
                .result.then(
                    function (result) {
                        if (result == null) {
                            ctrl.orderStatus = ctrl.orderStatusOld;
                            ctrl.statusText = ctrl.getStatusText();
                        } else {
                            ctrl.colorStatus = result.color;
                            ctrl.statusComment = result.basis;
                        }
                        ctrl.orderStatusOld = ctrl.orderStatus;
                        ctrl.onChangeStatusHistory();
                        //ctrl.modalClose(); //тут при успешном закрытии
                        return result;
                    },
                    function (result) {
                        if ((result === 'cancelChangeOrderStatus' && !ctrl.isSave) || (result === 'backdrop click' && !ctrl.isSave)) {
                            ctrl.orderStatus = ctrl.orderStatusOld;
                            ctrl.statusText = ctrl.getStatusText();
                            ctrl.getStatusColor(ctrl.statusText);
                        }
                        ctrl.onChangeStatusHistory();
                        //ctrl.modalDismiss();  //тут при неудачном закрытии, отмене
                        return result;
                    },
                );
        };
        ctrl.setPaied = function (checked) {
            $http
                .post('orders/setPaied', {
                    orderId: ctrl.orderId,
                    paid: checked,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                        ctrl.isPaied = checked;
                        ctrl.onChangeHistory();
                    }
                });
        };
        ctrl.sendDateToServer = function (date) {
            if (date == null || date == '') {
                return;
            }
            $http
                .post('orders/setDate', {
                    orderId: ctrl.orderId,
                    date: date,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    }
                });
        };
        ctrl.setDate = function (selectedDates, date, instance) {
            ctrl.sendDateToServer(date);
        };
        ctrl.setDateMobile = function (date) {
            ctrl.sendDateToServer(date);
        };
        ctrl.setManagerConfirmer = function (isManagerConfirmed) {
            $http
                .post('orders/setManagerConfirmed', {
                    orderId: ctrl.orderId,
                    isManagerConfirmed: isManagerConfirmed,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    }
                });
        };
        ctrl.set1CExportOrder = function (useIn1C) {
            $http
                .post('orders/setUseIn1C', {
                    orderId: ctrl.orderId,
                    useIn1C: useIn1C,
                })
                .then(function (response) {
                    if (response.data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    }
                });
        };
        ctrl.selectCustomer = function (result) {
            ctrl.getCustomer(result)
                .then(function (result) {
                    return result || $q.reject('error');
                })
                .then(function (result) {
                    if (ctrl.isDraft) {
                        ctrl.saveDraft().then(ctrl.gridOrderItemUpdate);
                    } else {
                        $timeout(function () {
                            document.getElementById('orderForm').submit();
                        });
                    }
                });
        };
        ctrl.changeCustomer = function (orderCustomerForm, timeout) {
            if (timerChangeCustomer) {
                clearTimeout(timerChangeCustomer);
            }
            timerChangeCustomer = setTimeout(
                function () {
                    ctrl.saveDraft().then(function () {
                        orderCustomerForm.$setPristine();
                    });
                },
                timeout != null ? timeout : 300,
            );
        };
        ctrl.processCity = function (orderCustomerForm, zone) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }
            return (timerProcessAddress = $timeout(
                function () {
                    if (zone != null) {
                        ctrl.country = zone.Country;
                        ctrl.region = zone.Region;
                        ctrl.district = zone.District;
                        ctrl.zip = zone.Zip;
                        if (zone.Zip && ctrl.isDraft) {
                            ctrl.changeCustomer(orderCustomerForm, 0);
                        }
                    }
                    if (zone == null || !zone.Zip) {
                        ctrl.processCustomerContact(zone == null).then(function (data) {
                            if (data.result === true) {
                                ctrl.country = data.obj.Country;
                                ctrl.region = data.obj.Region;
                                ctrl.district = data.obj.District;
                                ctrl.zip = data.obj.Zip;
                            }
                            if (ctrl.isDraft) {
                                ctrl.changeCustomer(orderCustomerForm, 0);
                            }
                        });
                    }
                },
                zone != null ? 0 : 300,
            ));
        };
        ctrl.processAddress = function (orderCustomerForm, data) {
            if (timerProcessAddress != null) {
                $timeout.cancel(timerProcessAddress);
            }
            return (timerProcessAddress = $timeout(
                function () {
                    if (data != null && data.Zip) {
                        ctrl.zip = data.Zip;
                        if (ctrl.isDraft) {
                            ctrl.changeCustomer(orderCustomerForm, 0);
                        }
                    } else {
                        ctrl.processCustomerContact().then(function (data) {
                            if (data.result === true) {
                                ctrl.zip = data.obj.Zip;
                            }
                            if (ctrl.isDraft) {
                                ctrl.changeCustomer(orderCustomerForm, 0);
                            }
                        });
                    }
                },
                data != null ? 0 : 300,
            ));
        };
        ctrl.processCustomerContact = function (byCity) {
            var contact = {
                country: ctrl.country,
                region: ctrl.region,
                district: ctrl.district,
                city: ctrl.city,
                zip: ctrl.zip,
                street: ctrl.street,
                house: ctrl.house,
                byCity: byCity,
            };
            return $http.post('customers/processCustomerContact', contact).then(function (response) {
                return response.data;
            });
        };
        ctrl.saveChanges = function (form) {
            // order.isDraft
            if (ctrl.isEditMode === true && !ctrl.isDraft) {
                ctrl.saveOrderInfo(form);
            } else {
                ctrl.saveDraft(form);
            }
        };
        ctrl.updateAdminComment = function (form) {
            var params = {
                orderId: ctrl.orderId,
                adminOrderComment: ctrl.adminOrderComment,
            };
            return $http.post('orders/updateAdminComment', params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    if (form != null) {
                        form.$setPristine();
                    }
                }
                ctrl.onChangeHistory();
                return data;
            });
        };
        // #region Attachments

        ctrl.getOrderAttachments = function (orderId) {
            $http.get('orders/getOrderAttachments', { params: { orderId: orderId } }).then(function (response) {
                if (response.data) {
                    ctrl.adminOrderAttachments = response.data.AdminOrderAttachments;
                    ctrl.customerOrderAttachments = response.data.CustomerOrderAttachments;
                }
            });
        };

        ctrl.updateAdminOrderAttachments = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if ($files && $files.length > 0) {
                var exceededLimit = false;
                if (ctrl.adminOrderAttachments && ctrl.adminOrderAttachments.length + $files.length > 10) {
                    exceededLimit = true;
                    $files.splice(10 - ctrl.adminOrderAttachments.length);
                }
                ctrl.loadingFiles = true;
                Upload.upload({
                    url: 'orders/updateAdminOrderAttachments',
                    data: { orderId: ctrl.orderId },
                    file: $files,
                }).then(function (response) {
                    var data = response.data;
                    if (exceededLimit) toaster.error($translate.instant('Admin.Js.Order.File.ExceededLimit'));
                    for (var i in data) {
                        if (data[i].Result === true) {
                            ctrl.adminOrderAttachments.push(data[i].Attachment);
                            toaster.success(
                                $translate.instant('Admin.Js.Order.File') +
                                    data[i].Attachment.OriginFileName +
                                    $translate.instant('Admin.Js.Order.FileWasAdded'),
                            );
                        } else {
                            toaster.error(
                                $translate.instant('Admin.Js.ErrorLoading'),
                                (data[i].Attachment != null ? data[i].Attachment.OriginFileName + ': ' : '') + data[i].Error,
                            );
                        }
                    }
                    $files.splice(0);
                    ctrl.loadingFiles = false;
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.ErrorLoading'), $translate.instant('Admin.Js.Order.FileDoesNotMeet'));
                ctrl.loadingFiles = false;
            } else {
                ctrl.loadingFiles = false;
            }
        };

        ctrl.deleteAdminOrderAttachment = function (id, index) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('orders/deleteAdminOrderAttachment', { attachmentId: id }).then(function (response) {
                        if (response.data) {
                            ctrl.adminOrderAttachments.splice(index, 1);
                            toaster.pop('success', '', $translate.instant('Admin.Js.FileUploader.FileWasDeleted'));
                        } else {
                            toaster.pop('error', $translate.instant('Admin.Js.DeletingError'));
                        }
                    });
                }
            });
        };

        ctrl.updateCustomerOrderAttachments = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
            if ($files && $files.length > 0) {
                var exceededLimit = false;
                if (ctrl.customerOrderAttachments && ctrl.customerOrderAttachments.length + $files.length > 10) {
                    exceededLimit = true;
                    $files.splice(10 - ctrl.customerOrderAttachments.length);
                }
                ctrl.loadingFiles = true;
                Upload.upload({
                    url: 'orders/updateCustomerOrderAttachments',
                    data: { orderId: ctrl.orderId },
                    file: $files,
                }).then(function (response) {
                    var data = response.data;
                    if (exceededLimit) toaster.error($translate.instant('Admin.Js.Order.File.ExceededLimit'));
                    for (var i in data) {
                        if (data[i].Result === true) {
                            ctrl.customerOrderAttachments.push(data[i].Attachment);
                            toaster.success(
                                $translate.instant('Admin.Js.Order.File') +
                                    data[i].Attachment.OriginFileName +
                                    $translate.instant('Admin.Js.Order.FileWasAdded'),
                            );
                        } else {
                            toaster.error(
                                $translate.instant('Admin.Js.ErrorLoading'),
                                (data[i].Attachment != null ? data[i].Attachment.OriginFileName + ': ' : '') + data[i].Error,
                            );
                        }
                    }
                    $files.splice(0);
                    ctrl.loadingFiles = false;
                });
            } else if ($invalidFiles.length > 0) {
                toaster.pop('error', $translate.instant('Admin.Js.ErrorLoading'), $translate.instant('Admin.Js.Order.FileDoesNotMeet'));
                ctrl.loadingFiles = false;
            } else {
                ctrl.loadingFiles = false;
            }
        };

        ctrl.deleteCustomerOrderAttachment = function (id, index) {
            SweetAlert.confirm($translate.instant('Admin.Js.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http.post('orders/deleteCustomerOrderAttachment', { attachmentId: id }).then(function (response) {
                        if (response.data) {
                            ctrl.customerOrderAttachments.splice(index, 1);
                            toaster.pop('success', '', $translate.instant('Admin.Js.FileUploader.FileWasDeleted'));
                        } else {
                            toaster.pop('error', $translate.instant('Admin.Js.DeletingError'));
                        }
                    });
                }
            });
        };

        // #endregion

        ctrl.getCustomer = function (result) {
            if (result == null || result.customerId == null) {
                return false;
            }

            if (result.managerId) {
                ctrl.managerId = result.managerId;
            }
            return $http
                .get('customers/getCustomerWithContact', {
                    params: {
                        customerId: result.customerId,
                    },
                })
                .then(function (response) {
                    var customer = response.data;
                    if (customer == null) return false;
                    ctrl.customerId = customer.Id;
                    ctrl.firstName = ctrl.selectedFirstName = customer.FirstName;
                    ctrl.lastName = ctrl.selectedLastName = customer.LastName;
                    ctrl.patronymic = customer.Patronymic;
                    ctrl.email = customer.Email;
                    ctrl.phone = customer.Phone;
                    ctrl.standardPhone = customer.StandardPhone;
                    ctrl.organization = customer.Organization;
                    ctrl.bonusCardNumber = customer.BonusCardNumber;
                    ctrl.customerGroup = customer.CustomerGroup;
                    var contacts = customer.Contacts;
                    ctrl.customerType = customer.CustomerType;
                    ctrl.customerFields = customer.customerFields;
                    if (contacts != null && contacts.length > 0) {
                        ctrl.hasContacts = true;
                        var contact = contacts[0];
                        ctrl.country = contact.Country;
                        ctrl.region = contact.Region;
                        ctrl.district = contact.District;
                        ctrl.city = contact.City;
                        ctrl.zip = contact.Zip;
                        ctrl.street = contact.Street;
                        ctrl.entrance = contact.Entrance;
                        ctrl.floor = contact.Floor;
                        ctrl.house = contact.House;
                        ctrl.structure = contact.Structure;
                        ctrl.apartment = contact.Apartment;
                        ctrl.customField1 = contact.CustomField1;
                        ctrl.customField2 = contact.CustomField2;
                        ctrl.customField3 = contact.CustomField3;
                    } else {
                        ctrl.hasContacts = false;
                    }
                    ctrl.getCustomerFields();
                    return true;
                });
        };

        // save draft
        ctrl.saveDraft = function (form) {
            if (!ctrl.isDraft) {
                return $q.resolve();
            }
            var orderId = ctrl.orderId;
            var params = {
                orderId: ctrl.orderId,
                orderCustomer: {
                    customerId: ctrl.customerId,
                    firstName: ctrl.firstName,
                    lastName: ctrl.lastName,
                    patronymic: ctrl.patronymic,
                    email: ctrl.email,
                    phone: ctrl.phone,
                    standardPhone: ctrl.standardPhone,
                    country: ctrl.country,
                    region: ctrl.region,
                    district: ctrl.district,
                    city: ctrl.city,
                    zip: ctrl.zip,
                    customField1: ctrl.customField1,
                    customField2: ctrl.customField2,
                    customField3: ctrl.customField3,
                    street: ctrl.street,
                    house: ctrl.house,
                    apartment: ctrl.apartment,
                    structure: ctrl.structure,
                    entrance: ctrl.entrance,
                    floor: ctrl.floor,
                    organization: ctrl.organization,
                    customerType: ctrl.customerType,
                },
                statusComment: ctrl.statusComment,
                adminOrderComment: ctrl.adminOrderComment,
                orderSourceId: ctrl.orderSourceId,
                managerId: ctrl.managerId,
                trackNumber: ctrl.trackNumber,
            };
            return $http.post('orders/saveDraft', params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    ctrl.customerId = data.customerId;
                    if (orderId === 0 && data.orderId !== 0) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.CreatedDraftOrder') + data.orderId);
                        ctrl.orderId = data.orderId;
                        ctrl.gridOrderItems.setParams({
                            OrderId: data.orderId,
                        });
                    } else {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    }
                    if (form != null) {
                        form.$setPristine();
                    }
                    ctrl.isEditMode = true;
                    ctrl.isDraft = true;
                    ctrl.onChangeHistory();
                }
                return data;
            });
        };

        // save order information in edit mode
        ctrl.saveOrderInfo = function (form) {
            if (ctrl.isDraft) {
                return $q.resolve();
            }
            var params = {
                orderId: ctrl.orderId,
                managerId: ctrl.managerId,
                statusComment: ctrl.statusComment,
                adminOrderComment: ctrl.adminOrderComment,
                trackNumber: ctrl.trackNumber,
                orderSourceId: ctrl.orderSourceId,
            };
            return $http.post('orders/saveOrderInfo', params).then(function (response) {
                var data = response.data;
                if (data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    if (form != null) {
                        form.$setPristine();
                    }
                }
                ctrl.onChangeHistory();
                return data;
            });
        };
        ctrl.updateOrderBonusCard = function () {
            $http
                .post('orders/updateOrderBonusCard', {
                    orderId: ctrl.orderId,
                })
                .then(function (response) {
                    window.location.reload();
                });
        };
        ctrl.deleteOrder = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Order.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Order.Deleting'),
            }).then(function (result) {
                if (result === true || result.value === true) {
                    $http
                        .post('orders/deleteOrder', {
                            orderId: ctrl.orderId,
                        })
                        .then(function (response) {
                            lastStatisticsService.getLastStatistics();
                            window.location.assign('orders');
                        });
                }
            });
        };
        ctrl.getMapAddress = function () {
            var address = ctrl.country != null ? ctrl.country : '';
            address += (address.length > 0 ? ', ' : '') + (ctrl.region != null ? ctrl.region : '');
            address += (address.length > 0 ? ', ' : '') + (ctrl.district != null ? ctrl.district : '');
            address += (address.length > 0 ? ', ' : '') + (ctrl.city != null ? ctrl.city : '');
            if (ctrl.address != null && ctrl.address !== '') {
                address += (address.length > 0 ? ', ' : '') + (ctrl.address != null ? ctrl.address : '');
            } else {
                address += (address.length > 0 ? ', ' : '') + (ctrl.street != null ? ctrl.street : '');
                address += (address.length > 0 ? ', ' : '') + (ctrl.house != null ? ctrl.house : '');
                address += (address.length > 0 ? ', ' : '') + (ctrl.structure != null ? ctrl.structure : '');
            }
            return encodeURIComponent(address);
        };
        ctrl.findCustomers = function (val) {
            if (ctrl.isDraft && val != null && val.length > 1) {
                return $http.get('customers/getCustomersAutocomplete?q=' + val).then(function (response) {
                    return response.data;
                });
            }
        };
        ctrl.findCustomersByFullNameParts = function (val) {
            if (ctrl.isDraft && val != null && val.length > 1) {
                return $http
                    .post('customers/getCustomersAutocompleteByFullNameParts', {
                        lastName: ctrl.lastName,
                        firstName: ctrl.firstName,
                        patronymic: ctrl.patronymic,
                    })
                    .then(function (response) {
                        return response.data;
                    });
            }
        };
        ctrl.findCustomersByPhone = function (val) {
            if (ctrl.isDraft && val != null && val.length > 2) {
                return $http.get('customers/getCustomersByPhoneAutocomplete?q=' + val).then(function (response) {
                    return response.data;
                });
            }
        };
        ctrl.selectCustomerByAutocomplete = function ($item, $model, $label, $event) {
            var customerId = $item.value;
            return ctrl.getCustomer({
                customerId: customerId,
            });
        };
        ctrl.dateChange = function (date) {
            alert($translate.instant('Admin.Js.Order.ChangeTheOrderDate') + date.toString());
        };
        ctrl.checkStopEdit = function (event) {
            var result = true;
            if (ctrl.isPaied === true) {
                if (event) {
                    //для мобильной версии
                    //не даём в swipe-line в гриде изменить элемент
                    event.preventDefault();
                    event.stopPropagation();
                }
                SweetAlert.alert($translate.instant('Admin.Js.Order.PaidOrderCantBeChanged'), {
                    title: $translate.instant('Admin.Js.Order.ChangingOrder'),
                });
                result = false;
            }
            return result;
        };
        ctrl.gridOnInplaceBeforeApply = function () {
            const isStopEdit = ctrl.checkStopEdit();
            if (isStopEdit) {
                ctrl.gridOrderItems?.setStateProcess(true);
            }
            return isStopEdit;
        };
        ctrl.resetOrderCustomer = function () {
            ctrl.customerId = null;
            ctrl.firstName = ctrl.selectedFirstName = null;
            ctrl.lastName = ctrl.selectedLastName = null;
            ctrl.managerId = ctrl.defaultManagerId;
            ctrl.patronymic = null;
            ctrl.email = null;
            ctrl.phone = null;
            ctrl.standardPhone = null;
            ctrl.country = null;
            ctrl.region = null;
            ctrl.district = null;
            ctrl.city = null;
            ctrl.zip = null;
            ctrl.street = null;
            ctrl.entrance = null;
            ctrl.floor = null;
            ctrl.house = null;
            ctrl.structure = null;
            ctrl.apartment = null;
            ctrl.organization = null;
            ctrl.hasContacts = false;
            ctrl.saveDraft().then(ctrl.gridOrderItemUpdate);
        };
        ctrl.updateStatus = function () {
            $http
                .post('customers/updateClientStatus', {
                    id: ctrl.customerId,
                    clientStatus: ctrl.clientStatus,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Customer.ChangesSaved'));
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Customer.ErrorWhileSaving'));
                    }
                    ctrl.onChangeHistory();
                });
        };
        ctrl.changeStatusClient = function (currentStatus) {
            ctrl.clientStatus = ctrl.clientStatus === currentStatus ? 'none' : currentStatus;
            ctrl.updateStatus();
        };
        ctrl.leadEventsOnInit = function (leadEvents) {
            ctrl.leadEvents = leadEvents;
        };
        ctrl.updateLeadEvents = function () {
            ctrl.leadEvents.getLeadEvents();
        };
        ctrl.updateLeadEventsWithDelay = function () {
            setTimeout(ctrl.updateLeadEvents, 800);
        };
        ctrl.statusHistoryOnInit = function (orderStatusHistory) {
            ctrl.orderStatusHistory = orderStatusHistory;
        };
        ctrl.onChangeStatusHistory = function () {
            if (ctrl.orderStatusHistory != null) {
                ctrl.orderStatusHistory.update();
            }
        };
        ctrl.orderHistoryOnInit = function (orderHistory) {
            ctrl.orderHistory = orderHistory;
        };
        ctrl.onChangeHistory = function () {
            if (ctrl.orderHistory != null) {
                ctrl.orderHistory.update();
            }
        };
        /* templateDocx */

        ctrl.createDocx = function () {
            var params = {
                orderId: ctrl.orderId,
                templatesDocx: ctrl.selectedTemplatesDocx.slice(),
                attach: ctrl.templatesDocxAttach,
            };
            var url = 'orders/generateTemplates';
            if (ctrl.templatesDocxAttach) {
                ctrl.creatingDocxs = true;
                $http.post(url, params).then(function (response) {
                    var data = response.data;
                    if (data.result === false) {
                        if (!data.errors) {
                            toaster.pop('error', 'Ошибка', 'Ошибка при генерации документов');
                        } else {
                            data.errors.forEach(function (error) {
                                toaster.pop('error', error);
                            });
                        }
                    } else {
                        for (var i in data.obj) {
                            if (data.obj[i].Result === true) {
                                ctrl.adminOrderAttachments.push(data.obj[i].Attachment);
                                toaster.pop('success', '', 'Файл "' + data.obj[i].Attachment.OriginFileName + '" добавлен');
                            } else {
                                toaster.pop(
                                    'error',
                                    'Ошибка при загрузке',
                                    (data.obj[i].Attachment != null ? data.obj[i].Attachment.OriginFileName + ': ' : '') + data.obj[i].Error,
                                );
                            }
                        }
                    }
                    ctrl.creatingDocxs = false;
                    ctrl.selectedTemplatesDocx = [];
                    ctrl.templatesDocxAttach = false;
                });
            } else {
                $window.location.href = url + '?' + $httpParamSerializer(params);
            }
        };

        /* end templateDocx */

        ctrl.editCustomerClose = function () {
            $timeout(function () {
                $window.location.reload(true);
            }, 100);
        };
        ctrl.taskGridOnInit = function (taskGrid) {
            ctrl.taskGrid = taskGrid;
        };
        ctrl.formatDateString = function (date) {
            return new Intl.DateTimeFormat('ru', {
                timeStyle: 'medium',
                dateStyle: 'short',
            })
                .format(new Date(date))
                .replace(/, /g, ' ');
        };
        ctrl.getCustomerFields = function () {
            return ctrl.getFunctionGetCustomerFields().then(function () {
                return ctrl.getCustomerFieldsFn();
            });
        };
        ctrl.getFunctionGetCustomerFields = function () {
            if (ctrl.getCustomerFieldsFn) {
                return $q.resolve();
            } else {
                ctrl.functionGetCustomerFieldsPromise = ctrl.functionGetCustomerFieldsPromise ? ctrl.functionGetCustomerFieldsPromise : $q.defer();
                return ctrl.functionGetCustomerFieldsPromise.promise;
            }
        };
        ctrl.onCustomerFieldsInit = function (reloadFn) {
            ctrl.getCustomerFieldsFn = reloadFn || function () {};
            if (ctrl.functionGetCustomerFieldsPromise) {
                ctrl.functionGetCustomerFieldsPromise.resolve();
            }
        };
        ctrl.initCustomerFields = function (customerFields) {
            return (ctrl.customerFields = !customerFields ? [] : customerFields);
        };
        ctrl.processCompanyName = function (item) {
            if (timerProcessCompanyName != null) {
                $timeout.cancel(timerProcessCompanyName);
            }
            return (timerProcessCompanyName = $timeout(
                function () {
                    if (item != null && item.CompanyData) {
                        ctrl.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 1) field.Value = item.CompanyData.CompanyName;
                            else if (field.FieldAssignment == 2) field.Value = item.CompanyData.LegalAddress;
                            else if (field.FieldAssignment == 3) field.Value = item.CompanyData.INN;
                            else if (field.FieldAssignment == 4) field.Value = item.CompanyData.KPP;
                            else if (field.FieldAssignment == 5) field.Value = item.CompanyData.OGRN;
                            else if (field.FieldAssignment == 6) field.Value = item.CompanyData.OKPO;
                        });
                    } else if (item != null && item.BankData) {
                        ctrl.customerfieldsJs.forEach(function (field) {
                            if (field.FieldAssignment == 7) field.Value = item.BankData.BIK;
                            else if (field.FieldAssignment == 8) field.Value = item.BankData.BankName;
                            else if (field.FieldAssignment == 9) field.Value = item.BankData.CorrespondentAccount;
                        });
                    }
                },
                item != null ? 0 : 700,
            ));
        };
        ctrl.showDistributionOfOrderItem = function (orderItem) {
            var amount = orderItem.Amount;
            var itemInfo = '';
            itemInfo += orderItem.ArtNo;
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalDistributionOfOrderItemCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: distributionOfOrderItemTemplate,
                    resolve: {
                        orderItemId: orderItem.OrderItemId,
                        amount: amount,
                        itemInfo: function () {
                            return itemInfo;
                        },
                    },
                    backdrop: 'static',
                })
                .result.then(function (result) {
                    if (result) {
                        var newAmount = result.reduce((prev, current) => prev + current.Amount, 0);
                        if (newAmount !== amount) {
                            ctrl.onChangeHistory();
                        }
                        ctrl.gridOrderItemUpdate();
                    }
                });
        };
        ctrl.updateOrderRecipient = function (data) {
            if (!data) return;
            ctrl.RecipientFullName = [data.LastName, data.FirstName, data.Patronymic].filter((x) => !!x).join(' ');
            ctrl.RecipientPhone = data.Phone;
        };
        ctrl.changeAddress = function (address) {
            ctrl.city = address.City;
            ctrl.region = address.Region;
            ctrl.country = address.Country;
            ctrl.district = address.District;
            ctrl.street = address.Street;
            ctrl.zip = address.Zip;
            ctrl.house = address.House;
            ctrl.structure = address.Structure;
            ctrl.apartment = address.Apartment;
            ctrl.entrance = address.Entrance;
            ctrl.floor = address.Floor;
        };
        ctrl.deleteAddress = function (items, itemRemoved, addressSelected, isItemSeletedRemoved) {
            ctrl.hasContacts = items.length > 0;
            if (ctrl.hasContacts && isItemSeletedRemoved) {
                ctrl.changeAddress(addressSelected);
            }
        };
        ctrl.openChangeMarkingModal = function (entity = null) {
            $uibModal
                .open({
                    bindToController: true,
                    controller: 'ModalChangeMarkingCtrl',
                    controllerAs: 'ctrl',
                    templateUrl: changeMarkingTemplate,
                    resolve: {
                        params: {
                            orderItemId: entity.OrderItemId,
                        },
                    },
                })
                .result.then(ctrl.gridOrderItemUpdate);
        };

        ctrl.setLimitItemsStocks = function (row, count) {
            row.gridColStocksLimitItems = row.gridColStocksLimitItems == null ? count : null;
        };

        ctrl.getButtonTextMore = function (row) {
            return $translate.instant(
                row.gridColStocksLimitItems == null
                    ? 'Admin.Js.Orders.GetOrderItems.StocksInWarehouseCollapse'
                    : 'Admin.Js.Orders.GetOrderItems.StocksInWarehouseExpand',
            );
        };
    };
    ng.module('order', [
        'uiGridCustom',
        'urlHelper',
        'orderItemsSummary',
        'spinbox',
        'shipping',
        'payment',
        'orderStatusHistory',
        'orderHistory',
        'orderItemCustomOptions',
        'rating',
    ])
        .controller('OrderCtrl', OrderCtrl)
        .directive('changeMarking', [
            function () {
                return {
                    scope: {
                        isRequiredValidation: '<?',
                        isMobile: '<?',
                        openModal: '<?',
                    },
                    controller: function () {
                        this.$postLink = function () {
                            this.color = this.isRequiredValidation ? '#f4ec3e' : '#fd5a52';
                            this.helpText = this.isRequiredValidation ? 'Маркировка успешно задана' : 'Требуется задать маркировку';
                        };
                    },
                    bindToController: true,
                    controllerAs: 'ctrl',
                    restrict: 'E',
                    templateUrl: changeMarkingDirectiveTemplate,
                };
            },
        ]);
})(window.angular);
