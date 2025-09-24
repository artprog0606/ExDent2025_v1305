import orderItemsSummaryTemplate from './orderItemsSummary.html';
import customsDeclarationProductDataTemplate from './../../modal/shippings/russianPost/customsDeclarationProductData/customsDeclarationProductData.html';
import changeDeliveryDateTemplate from './../../modal/shippings/yandex/changeDeliveryDate/changeDeliveryDate.html';
(function (ng) {
    'use strict';

    var OrderItemsSummaryCtrl = function ($http, $timeout, toaster, SweetAlert, $translate, $uibModal, $templateRequest, urlHelper) {
        var ctrl = this,
            popoverShippingTimer,
            popoverPaymentTimer;
        ctrl.$onInit = function () {
            ctrl.shippingContent = null;
            ctrl.grastinActionsUrl = 'grastin/getorderactions?orderid=' + ctrl.orderId;
            ctrl.russianPostActionsUrl = 'orders/getOrderActionsRussianPost?orderid=' + ctrl.orderId;
            ctrl.shiptorActionsUrl = 'orders/getOrderActionsShiptor?orderid=' + ctrl.orderId;
            ctrl.sdekActionsUrl = 'orders/getOrderActionsSdek?orderid=' + ctrl.orderId;
            ctrl.hermesActionsUrl = 'orders/getOrderActionsHermes?orderid=' + ctrl.orderId;
            ctrl.measoftActionsUrl = 'orders/getOrderActionsMeasoft?orderid=' + ctrl.orderId;
            ctrl.pecEasywayActionsUrl = 'orders/getOrderActionsPecEasyway?orderid=' + ctrl.orderId;
            ctrl.pecActionsUrl = 'orders/getOrderActionsPec?orderid=' + ctrl.orderId;
            ctrl.pickPointActionsUrl = 'orders/getOrderActionsPickPoint?orderid=' + ctrl.orderId;
            ctrl.ozonRocketActionsUrl = 'orders/getOrderActionsOzonRocket?orderid=' + ctrl.orderId;
            ctrl.yandexActionsUrl = 'orders/getOrderActionsYandex?orderid=' + ctrl.orderId;
            ctrl.sberlogisticActionsUrl = 'orders/getOrderActionsSberlogistic?orderid=' + ctrl.orderId;
            ctrl.fivePostActionsUrl = 'orders/GetOrderActionsFivePost?orderid=' + ctrl.orderId;
            ctrl.apiShipActionsUrl = 'orders/getOrderActionsApiShip?orderid=' + ctrl.orderId;

            //ctrl.toggleselectCurrencyLabel('1');

            ctrl.CheckDdeliveryOrder();
            ctrl.getOrderItemsSummary();
            if (ctrl.onInit != null) {
                ctrl.onInit({
                    orderItemsSummary: ctrl,
                });
            }
        };
        ctrl.toggleselectCurrencyLabel = function (val) {
            ctrl.typeDiscountPercent = val === '1' ? true : false;
            ctrl.selectCurrency = val;
        };

        //for mobile version
        ctrl.updateFormFields = function () {
            ctrl.commentField = [
                {
                    label: $translate.instant('Admin.Js.OrderItemsSummary.UsersComment'),
                    name: 'CustomerComment',
                    controlType: 'textarea',
                    model: ctrl.Summary.CustomerComment,
                },
            ];
            ctrl.weightFormFields = [
                {
                    label: $translate.instant('Admin.Js.OrderItemsSummary.TotalWeight'),
                    attributes: [
                        {
                            'validation-input-float': '',
                        },
                        {
                            'validation-input-min': 0,
                        },
                        {
                            'validation-input-text': 'Вес',
                        },
                    ],
                    name: 'TotalWeight',
                    controlType: 'input',
                    type: 'text',
                    model: ctrl.Summary.TotalWeight,
                },
            ];
            ctrl.demensionsFields = [
                {
                    label: $translate.instant('Admin.Js.AddEditOffer.Length'),
                    name: 'TotalLength',
                    controlType: 'input',
                    type: 'text',
                    model: ctrl.Summary.TotalLength,
                    attributes: [
                        {
                            'validation-input-float': '',
                        },
                        {
                            'validation-input-min': 0,
                        },
                        {
                            'validation-input-text': 'Длина',
                        },
                    ],
                },
                {
                    label: $translate.instant('Admin.Js.AddEditOffer.Width'),
                    name: 'TotalWidth',
                    controlType: 'input',
                    type: 'text',
                    model: ctrl.Summary.TotalWidth,
                    attributes: [
                        {
                            'validation-input-float': '',
                        },
                        {
                            'validation-input-min': 0,
                        },
                        {
                            'validation-input-text': 'Ширина',
                        },
                    ],
                },
                {
                    label: $translate.instant('Admin.Js.AddEditOffer.Height'),
                    name: 'TotalHeight',
                    controlType: 'input',
                    type: 'text',
                    model: ctrl.Summary.TotalHeight,
                    attributes: [
                        {
                            'validation-input-float': '',
                        },
                        {
                            'validation-input-min': 0,
                        },
                        {
                            'validation-input-text': 'Высота',
                        },
                    ],
                },
            ];
        };
        ctrl.getOrderItemsSummary = function () {
            return $http
                .get('orders/getOrderItemsSummary', {
                    params: {
                        orderId: ctrl.orderId,
                    },
                })
                .then(function (response) {
                    var data = response.data;
                    if (data != null) {
                        ctrl.Summary = data;
                        ctrl.toggleselectCurrencyLabel(ctrl.Summary.OrderDiscount > 0 ? '1' : '0');
                        ctrl.updateFormFields();
                    }
                });
        };
        ctrl.changeShipping = function (result) {
            ctrl.getOrderItemsSummary();
            ctrl.grastinUpdateActions();
            ctrl.russianPostUpdateActions();
            ctrl.shiptorUpdateActions();
            ctrl.sdekUpdateActions();
            ctrl.hermesUpdateActions();
            ctrl.pecEasywayUpdateActions();
            ctrl.pecUpdateActions();
            ctrl.pickPointUpdateActions();
            ctrl.ozonRocketUpdateActions();
            ctrl.yandexUpdateActions();
            ctrl.apiShipUpdateActions();
            if (result != null) {
                toaster.pop('success', '', $translate.instant('Admin.Js.Order.ShippingMethodSaved'));
            }
        };
        ctrl.changePayment = function (result) {
            ctrl.getOrderItemsSummary();
            if (result != null) {
                toaster.pop('success', '', $translate.instant('Admin.Js.Order.PaymentMethodSaved'));
            }
        };

        /* shippings */

        ctrl.getOrderTrackNumber = function () {
            $http
                .post('orders/getOrderTrackNumber', {
                    orderId: ctrl.orderId,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.trackNumber = data.object;
                    }
                });
        };
        ctrl.baseShippingRequest = function (url, params, flag, onSuccess, onError) {
            if (ctrl[flag]) {
                return;
            }
            ctrl[flag] = true;
            $http
                .post(url, params)
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        if (data.message) {
                            toaster.success('', data.message);
                        }
                        ctrl.getOrderTrackNumber();
                        if (onSuccess) {
                            onSuccess(data);
                        }
                    } else {
                        if (data.errors) {
                            data.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        } else if (data.error) {
                            toaster.error('', data.error);
                        }
                        if (onError) {
                            onError(data);
                        }
                    }
                })
                .finally(function () {
                    ctrl[flag] = false;
                    toaster.clear(toasterWait);
                });
            var toasterWait = toaster.pop('wait', '', 'Задача выполняется');
        };
        ctrl.createYandexDeliveryOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createYandexDeliveryOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateYandexDeliveryOrder',
            );
        };
        ctrl.createYandexNewDeliveryOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createYandexNewDeliveryOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateYandexNewDeliveryOrder',
            );
        };
        ctrl.createUpdateSberlogisticOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createUpdateSberlogisticOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateUpdateSberlogisticOrder',
            );
        };
        ctrl.cancelSberlogisticOrder = function () {
            ctrl.baseShippingRequest(
                'orders/cancelSberlogisticOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCancelSberlogisticOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.createYandexOrder = function (additionalAction, additionalActionData) {
            ctrl.baseShippingRequest(
                'orders/createYandexOrder',
                {
                    orderId: ctrl.orderId,
                    additionalAction: additionalAction,
                    additionalActionData: additionalActionData,
                },
                'sendingCreateYandexDeliveryOrder',
                function (data) {
                    if (data.obj) {
                        if (data.obj.additionalAction) {
                            if (data.obj.additionalAction === 'select_valid_delivery_date') {
                                $uibModal
                                    .open({
                                        bindToController: true,
                                        controller: 'ModalYandexChangeDeliveryDateCtrl',
                                        controllerAs: 'ctrl',
                                        size: 'lg',
                                        backdrop: 'static',
                                        templateUrl: changeDeliveryDateTemplate,
                                        resolve: {
                                            params: {
                                                datesOfDelivery: data.obj.additionalActionData.datesOfDelivery,
                                                timesOfDelivery: data.obj.additionalActionData.timesOfDelivery,
                                            },
                                        },
                                    })
                                    .result.then(function (result) {
                                        if (result && result.selectedInterval) {
                                            ctrl.createYandexOrder(data.obj.additionalAction, JSON.stringify(result));
                                        }
                                        return result;
                                    });
                            } else if (data.obj.additionalAction === 'confirm_modify_artno') {
                                SweetAlert.confirm(
                                    'Яндекс.Доставка не позволяет передавать в одной коробке позиции с одинаковым артикулом.<br>' +
                                        'Передать заказ еще раз с видоизмененными артикулами?',
                                    {
                                        title: 'Дублирующиеся артикулы в заказе',
                                    },
                                ).then(function (result) {
                                    if (result === true || result.value) {
                                        ctrl.createYandexOrder(data.obj.additionalAction, true);
                                    }
                                });
                            }
                        }
                        if (data.obj.errors) {
                            data.obj.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        } else if (data.obj.error) {
                            toaster.error('', data.obj.error);
                        }
                    } else {
                        ctrl.grastinUpdateActionsAndSummary();
                    }
                },
            );
        };
        ctrl.cancelYandexOrder = function () {
            ctrl.baseShippingRequest(
                'orders/cancelYandexOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCancelYandexOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.createCheckoutRuOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createCheckoutRuOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateCheckoutRuOrder',
            );
        };
        ctrl.sdekOrderPrintForm = function () {
            window.location = 'orders/sdekOrderPrintForm?orderId=' + ctrl.orderId;
        };
        ctrl.createSdekOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createSdekOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateSdekOrder',
                function (data) {
                    ctrl.grastinUpdateActionsAndSummary();
                },
            );
        };
        ctrl.sdekDeleteOrder = function () {
            ctrl.baseShippingRequest(
                'orders/sdekDeleteOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingSdekDeleteOrder',
                function (data) {
                    ctrl.grastinUpdateActionsAndSummary();
                },
            );
        };
        ctrl.createApiShipOrder = function () {
            ctrl.baseShippingRequest('orders/createApiShipOrder', { orderId: ctrl.orderId }, 'sendingCreateApiShipOrder', function (data) {
                ctrl.grastinUpdateActionsAndSummary();
            });
        };

        ctrl.deleteApiShipOrder = function () {
            ctrl.baseShippingRequest('orders/deleteApiShipOrder', { orderId: ctrl.orderId }, 'sendingApiShipDeleteOrder', function (data) {
                ctrl.grastinUpdateActionsAndSummary();
            });
        };
        ctrl.createBoxberryOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createBoxberryOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateBoxberryOrder',
            );
        };
        ctrl.deleteBoxberryOrder = function () {
            ctrl.baseShippingRequest(
                'orders/deleteBoxberryOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingDeleteBoxberryOrder',
            );
        };
        ctrl.createMeasoftOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createMeasoftOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateMeasoftOrder',
            );
        };
        ctrl.deleteMeasoftOrder = function () {
            ctrl.baseShippingRequest(
                'orders/deleteMeasoftOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingDeleteMeasoftOrder',
            );
        };
        ctrl.grastinOrderPrintMark = function () {
            ctrl.baseShippingRequest(
                'orders/grastinSendRequestForMark',
                {
                    orderId: ctrl.orderId,
                },
                'sendingGrastinOrderPrintMark',
                function (data) {
                    window.location = 'orders/GrastinOrderPrintMark?filename=' + encodeURIComponent(data.obj.FileName);
                },
            );
        };
        ctrl.createShiptorOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createShiptorOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateShiptorOrder',
                function (data) {
                    ctrl.grastinUpdateActionsAndSummary();
                },
            );
        };
        ctrl.createRussianPostOrder = function (additionalAction, additionalActionData) {
            ctrl.baseShippingRequest(
                'orders/createRussianPostOrder',
                {
                    orderId: ctrl.orderId,
                    additionalAction: additionalAction,
                    additionalActionData: additionalActionData,
                },
                'sendingCreateRussianPostOrder',
                function (data) {
                    if (data.obj) {
                        if (data.obj.additionalAction) {
                            if (data.obj.additionalAction === 'fill_additional_data_for_customs_declaration') {
                                $uibModal
                                    .open({
                                        bindToController: true,
                                        controller: 'ModalRussianPostCustomsDeclarationProductDataCtrl',
                                        controllerAs: 'ctrl',
                                        size: 'lg',
                                        backdrop: 'static',
                                        templateUrl: customsDeclarationProductDataTemplate,
                                        resolve: {
                                            params: {
                                                products: data.obj.additionalActionData.Products,
                                            },
                                        },
                                    })
                                    .result.then(function (result) {
                                        if (result && result.products) {
                                            ctrl.createRussianPostOrder(data.obj.additionalAction, JSON.stringify(result.products));
                                        }
                                        return result;
                                    });
                            }
                        }
                        if (data.obj.errors) {
                            data.obj.errors.forEach(function (error) {
                                toaster.error('', error);
                            });
                        } else if (data.obj.error) {
                            toaster.error('', data.obj.error);
                        }
                    } else {
                        ctrl.grastinUpdateActionsAndSummary();
                    }
                },
            );
        };
        ctrl.deleteRussianPostOrder = function () {
            ctrl.baseShippingRequest(
                'orders/deleteRussianPostOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingDeleteRussianPostOrder',
                function (data) {
                    ctrl.grastinUpdateActionsAndSummary();
                },
            );
        };
        ctrl.russianPostGetDocumentsBeforShipment = function () {
            ctrl.baseShippingRequest(
                'orders/russianPostGetDocumentsBeforShipment',
                {
                    orderId: ctrl.orderId,
                },
                'sendingRussianPostGetDocumentsBeforShipment',
                function (data) {
                    window.location = 'orders/russianPostGetFileDocuments?filename=' + encodeURIComponent(data.obj.FileName);
                },
            );
        };
        ctrl.russianPostGetDocuments = function () {
            ctrl.baseShippingRequest(
                'orders/russianPostGetDocuments',
                {
                    orderId: ctrl.orderId,
                },
                'sendingRussianPostGetDocuments',
                function (data) {
                    window.location = 'orders/russianPostGetFileDocuments?filename=' + encodeURIComponent(data.obj.FileName);
                },
            );
        };
        ctrl.createHermesOrderStandart = function () {
            ctrl.baseShippingRequest(
                'orders/createHermesOrderStandart',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateHermesOrderStandart',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.createHermesOrderVSD = function () {
            ctrl.baseShippingRequest(
                'orders/createHermesOrderVsd',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateHermesOrderVsd',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.createHermesOrderDrop = function () {
            ctrl.baseShippingRequest(
                'orders/createHermesOrderDrop',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateHermesOrderDrop',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.deleteHermesOrder = function () {
            ctrl.baseShippingRequest(
                'orders/deleteHermesOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingDeleteHermesOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.createPecEasywayOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createPecEasywayOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreatePecEasywayOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.cancelPecEasywayOrder = function () {
            ctrl.baseShippingRequest(
                'orders/cancelPecEasywayOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCancelPecEasywayOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.createPecOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createPecOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreatePecOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.cancelPecOrder = function () {
            ctrl.baseShippingRequest(
                'orders/cancelPecOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCancelPecOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.createPickPointOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createPickPointOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreatePickPointOrder',
                function (data) {
                    ctrl.pickPointUpdateActions();
                },
            );
        };
        ctrl.deletePickPointOrder = function () {
            ctrl.baseShippingRequest(
                'orders/deletePickPointOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingDeletePickPointOrder',
                function (data) {
                    ctrl.pickPointUpdateActions();
                },
            );
        };
        ctrl.createDdeliveryOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createDDeliveryOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateDdeliveryOrder',
                function (data) {
                    ctrl.CheckDdeliveryOrder();
                },
            );
        };
        ctrl.createOzonRocketOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createOzonRocketOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateOzonRocketOrder',
                function (data) {
                    ctrl.updateActionsAndSummary();
                },
            );
        };
        ctrl.cancelOzonRocketOrder = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Order.CancelOzonRocketOrder'), {
                title: 'Ozon Rocket',
            }).then(function (result) {
                if (result === true || result.value) {
                    ctrl.baseShippingRequest(
                        'orders/cancelOzonRocketOrder',
                        {
                            orderId: ctrl.orderId,
                        },
                        'sendingCancelOzonRocketOrder',
                        function (data) {
                            ctrl.updateActionsAndSummary();
                        },
                    );
                }
            });
        };
        ctrl.createFivePostOrder = function () {
            ctrl.baseShippingRequest(
                'orders/createFivePostOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCreateFivePostOrder',
            );
        };
        ctrl.deleteFivePostOrder = function () {
            ctrl.baseShippingRequest(
                'orders/deleteFivePostOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingDeleteFivePostOrder',
            );
        };
        ctrl.getDdeliveryOrderInfo = function () {
            window.location = 'orders/ddeliveryOrderInfo?orderId=' + ctrl.orderId;
            //$http.post('orders/DDeliveryOrderInfo', { orderId: ctrl.orderId }).then(function (response) {
            //    var data = response.data;
            //    if (data.result === true) {
            //        toaster.pop('success', '', data.message);
            //    } else {
            //        toaster.pop('error', '', data.error);
            //    }
            //});
        };
        ctrl.canselDdeliveryOrder = function () {
            ctrl.baseShippingRequest(
                'orders/CanselDDeliveryOrder',
                {
                    orderId: ctrl.orderId,
                },
                'sendingCancelDdeliveryOrder',
                function (data) {
                    ctrl.CheckDdeliveryOrder();
                },
            );
        };
        ctrl.CheckDdeliveryOrder = function () {
            ctrl.isExistDdeliveryOrder = false;
            $http
                .post('orders/IsExistDDeliveryOrder', {
                    orderId: ctrl.orderId,
                })
                .then(function (response) {
                    var data = response.data;
                    ctrl.isExistDdeliveryOrder = data.result;
                });
        };
        ctrl.grastinUpdateActions = function () {
            ctrl.grastinActionsUrl = 'grastin/getorderactions?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.russianPostUpdateActions = function () {
            ctrl.russianPostActionsUrl = 'orders/getOrderActionsRussianPost?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.sdekUpdateActions = function () {
            ctrl.sdekActionsUrl = 'orders/getOrderActionsSdek?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.apiShipUpdateActions = function () {
            ctrl.apiShipActionsUrl = 'orders/getOrderActionsApiShip?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.sdekUpdatedDispatchNumber = function () {
            ctrl.sdekUpdateActions();
        };
        ctrl.shiptorUpdateActions = function () {
            ctrl.shiptorActionsUrl = 'orders/getOrderActionsShiptor?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.hermesUpdateActions = function () {
            ctrl.hermesActionsUrl = 'orders/getOrderActionsHermes?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.pecEasywayUpdateActions = function () {
            ctrl.pecEasywayActionsUrl = 'orders/getOrderActionsPecEasyway?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.pecUpdateActions = function () {
            ctrl.pecActionsUrl = 'orders/getOrderActionsPec?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.pickPointUpdateActions = function () {
            ctrl.pickPointActionsUrl = 'orders/getOrderActionsPickPoint?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.ozonRocketUpdateActions = function () {
            ctrl.ozonRocketActionsUrl = 'orders/getOrderActionsOzonRocket?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.yandexUpdateActions = function () {
            ctrl.yandexActionsUrl = 'orders/getOrderActionsYandex?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.sberlogisticUpdateActions = function () {
            ctrl.sberlogisticActionsUrl = 'orders/getOrderActionsSberlogistic?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.fivePostUpdateActions = function () {
            ctrl.fivePostActionsUrl = 'orders/GetOrderActionsFivePost?orderid=' + ctrl.orderId + '&rnd=' + Math.random();
        };
        ctrl.updateActionsAndSummary = function () {
            ctrl.getOrderItemsSummary();
            ctrl.grastinUpdateActions();
            ctrl.russianPostUpdateActions();
            ctrl.shiptorUpdateActions();
            ctrl.sdekUpdateActions();
            ctrl.apiShipUpdateActions();
            ctrl.hermesUpdateActions();
            ctrl.pecEasywayUpdateActions();
            ctrl.pecUpdateActions();
            ctrl.pickPointUpdateActions();
            ctrl.ozonRocketUpdateActions();
            ctrl.yandexUpdateActions();
            ctrl.sberlogisticUpdateActions();
            ctrl.fivePostUpdateActions();
        };
        ctrl.grastinUpdateActionsAndSummary = function () {
            ctrl.updateActionsAndSummary();
        };

        /* end shippings */

        /* discount */
        ctrl.discountPopoverOpen = function () {
            ctrl.OrderDiscountNew = ctrl.typeDiscountPercent ? ctrl.Summary.OrderDiscount : ctrl.Summary.ProductsDiscountPrice;
            ctrl.discountPopoverIsOpen = true;
        };
        ctrl.discountPopoverClose = function () {
            ctrl.discountPopoverIsOpen = false;
        };
        ctrl.discountPopoverToggle = function () {
            ctrl.discountPopoverIsOpen === true ? ctrl.discountPopoverClose() : ctrl.discountPopoverOpen();
        };
        ctrl.changeDiscountHandler = function (discountValue) {
            ctrl.OrderDiscountNew = discountValue != null ? discountValue : 0;
        };
        ctrl.changeDiscount = function (discount) {
            if (ctrl.orderId === 0) return;
            $http
                .post('orders/changeDiscount', {
                    orderId: ctrl.orderId,
                    orderDiscount: discount,
                    isValue: ctrl.selectCurrency === '0',
                })
                .then(function (response) {
                    if (response.data.result == true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.DiscountSaved'));
                        if (ctrl.selectCurrency === '1') {
                            ctrl.Summary.OrderDiscount = discount;
                        } else {
                            ctrl.Summary.OrderDiscountValue = discount;
                        }
                    } else if (response.data.errors != null) {
                        response.data.errors.forEach(function (error) {
                            toaster.pop('error', '', error);
                        });
                    }
                    return response.data;
                })
                .finally(function () {
                    ctrl.getOrderItemsSummary();
                    ctrl.discountPopoverClose();
                });
        };
        ctrl.getPaymentDetailsLink = function (withoutStamp) {
            var link = ctrl.Summary.PrintPaymentDetailsLink;
            if (ctrl.Summary.PaymentDetails != null) {
                if (ctrl.Summary.PaymentDetails.CompanyName != null && ctrl.Summary.PaymentDetails.CompanyName.length > 0) {
                    link += '&bill_CompanyName=' + ctrl.Summary.PaymentDetails.CompanyName;
                }
                if (ctrl.Summary.PaymentDetails.INN != null && ctrl.Summary.PaymentDetails.INN.length > 0) {
                    link += '&bill_INN=' + ctrl.Summary.PaymentDetails.INN;
                }
                if (ctrl.Summary.PaymentDetails.Kpp != null && ctrl.Summary.PaymentDetails.Kpp.length > 0) {
                    link += '&bill_kpp=' + ctrl.Summary.PaymentDetails.Kpp;
                }
                if (ctrl.Summary.PaymentDetails.Contract != null && ctrl.Summary.PaymentDetails.Contract.length > 0) {
                    link += '&bill_Contract=' + ctrl.Summary.PaymentDetails.Contract;
                }
            }

            if (withoutStamp) {
                link += '&withoutStamp=true';
            }

            return link;
        };

        /* bonuses */
        ctrl.bonusesPopoverOpen = function () {
            ctrl.bonusesPopoverIsOpen = true;
        };
        ctrl.bonusesPopoverClose = function () {
            ctrl.bonusesPopoverIsOpen = false;
        };
        ctrl.bonusesPopoverToggle = function () {
            ctrl.bonusesPopoverIsOpen === true ? ctrl.bonusesPopoverClose() : ctrl.bonusesPopoverOpen();
        };
        ctrl.useBonuses = function (bonusesAmount) {
            SweetAlert.confirm($translate.instant('Admin.Js.Order.WriteOffBonuses'), {
                title: $translate.instant('Admin.Js.Order.WritingOffBonuses'),
            }).then(function (result) {
                if (result === true || result.value) {
                    $http
                        .post('orders/useBonuses', {
                            orderId: ctrl.orderId,
                            bonusesAmount: bonusesAmount,
                        })
                        .then(function (response) {
                            var data = response.data;
                            if (data.result === true) {
                                toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                            } else {
                                toaster.pop('error', '', data.error);
                            }
                        })
                        .finally(function () {
                            ctrl.getOrderItemsSummary();
                            ctrl.bonusesPopoverClose();
                        });
                }
            });
        };
        ctrl.popoverShippingOpen = function () {
            if (popoverShippingTimer != null) {
                $timeout.cancel(popoverShippingTimer);
            }
            ctrl.popoverShippingIsOpen = true;
        };
        ctrl.popoverShippingClose = function () {
            popoverShippingTimer = $timeout(function () {
                ctrl.popoverShippingIsOpen = false;
            }, 500);
        };
        ctrl.popoverPaymentOpen = function () {
            if (popoverPaymentTimer != null) {
                $timeout.cancel(popoverPaymentTimer);
            }
            ctrl.popoverPaymentIsOpen = true;
        };
        ctrl.popoverPaymentClose = function () {
            popoverPaymentTimer = $timeout(function () {
                ctrl.popoverPaymentIsOpen = false;
            }, 500);
        };
        ctrl.savePaymentDetails = function () {
            var params = ng.extend(ctrl.Summary.PaymentDetails, {
                orderId: ctrl.orderId,
            });
            $http.post('orders/updatePaymentDetails', params).then(function (response) {
                if (response.data.result === true) {
                    toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                }
            });
        };

        /* coupons */

        ctrl.couponsPopoverOpen = function () {
            ctrl.couponsPopoverIsOpen = true;
        };
        ctrl.couponsPopoverClose = function () {
            ctrl.couponsPopoverIsOpen = false;
        };
        ctrl.couponsPopoverToggle = function () {
            ctrl.couponsPopoverIsOpen === true ? ctrl.couponsPopoverClose() : ctrl.couponsPopoverOpen();
        };
        ctrl.changeCoupon = function (couponCode) {
            if (ctrl.orderId === 0) return;
            SweetAlert.confirm($translate.instant('Admin.Js.Order.ChangeCoupon'), {
                title: '',
            }).then(function (result) {
                if (result === true || result.value) {
                    $http
                        .post('orders/changeCoupon', {
                            orderId: ctrl.orderId,
                            couponCode: couponCode,
                        })
                        .then(function (response) {
                            if (response.data.result == true) {
                                toaster.pop('success', '', $translate.instant('Admin.Js.Order.CouponSaved'));
                            } else if (response.data.errors != null) {
                                response.data.errors.forEach(function (error) {
                                    toaster.pop('error', '', error);
                                });
                            } else {
                                toaster.pop('error', '', $translate.instant('Admin.Js.Order.CouponSavingError'));
                            }
                            return response.data;
                        })
                        .finally(function () {
                            ctrl.getOrderItemsSummary();
                            ctrl.couponsPopoverClose();
                            ctrl.onUpdateOrderItems();
                        });
                }
            });
        };
        ctrl.removeCoupon = function () {
            $http
                .post('orders/removeCoupon', {
                    orderId: ctrl.orderId,
                })
                .then(function (response) {
                    if (response.data.result == true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.CouponRemoved'));
                    } else if (response.data.errors != null) {
                        response.data.errors.forEach(function (error) {
                            toaster.pop('error', '', error);
                        });
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.Order.CouponRemoveError'));
                    }
                })
                .finally(function () {
                    ctrl.getOrderItemsSummary();
                    ctrl.couponsPopoverClose();
                    ctrl.onUpdateOrderItems();
                });
        };

        /* certificate */

        ctrl.certificatePopoverOpen = function () {
            ctrl.certificatePopoverIsOpen = true;
        };
        ctrl.certificatePopoverClose = function () {
            ctrl.certificatePopoverIsOpen = false;
        };
        ctrl.certificatePopoverToggle = function () {
            ctrl.certificatePopoverIsOpen === true ? ctrl.certificatePopoverClose() : ctrl.certificatePopoverOpen();
        };
        ctrl.changeCertificate = function (code) {
            if (ctrl.orderId === 0) return;
            $http
                .post('orders/changeCertificate', {
                    orderId: ctrl.orderId,
                    code: code,
                })
                .then(function (response) {
                    if (response.data.result == true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    } else {
                        if (response.data.errors != null && response.data.errors.length > 0) {
                            toaster.pop('error', $translate.instant('Admin.Js.Order.CertificateSavingError'), response.data.errors[0]);
                        } else {
                            toaster.pop('error', '', $translate.instant('Admin.Js.Order.CertificateSavingError'));
                        }
                    }
                    return response.data;
                })
                .finally(function () {
                    ctrl.getOrderItemsSummary();
                    ctrl.certificatePopoverClose();
                });
        };
        ctrl.removeCertificate = function () {
            $http
                .post('orders/removeCertificate', {
                    orderId: ctrl.orderId,
                })
                .then(function (response) {
                    if (response.data.result == true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.OrderCertificateRemoveError'));
                    }
                })
                .finally(function () {
                    ctrl.getOrderItemsSummary();
                    ctrl.certificatePopoverClose();
                });
        };

        /* end certificate */

        ctrl.demensionsStartEdit = function () {
            ctrl.demensionsBackup = {
                height: ctrl.Summary.TotalHeight,
                width: ctrl.Summary.TotalWidth,
                length: ctrl.Summary.TotalLength,
            };
            ctrl.demensionsEdit = true;
        };
        ctrl.demensionsCancelEdit = function () {
            ctrl.Summary.TotalHeight = ctrl.demensionsBackup.height;
            ctrl.Summary.TotalWidth = ctrl.demensionsBackup.width;
            ctrl.Summary.TotalLength = ctrl.demensionsBackup.length;
            ctrl.demensionsEdit = false;
        };
        ctrl.demensionsApplyEdit = function (form) {
            ctrl.Summary.TotalWidth = form != null ? form.TotalWidth.$modelValue : ctrl.Summary.TotalWidth;
            ctrl.Summary.TotalHeight = form != null ? form.TotalHeight.$modelValue : ctrl.Summary.TotalHeight;
            ctrl.Summary.TotalLength = form != null ? form.TotalLength.$modelValue : ctrl.Summary.TotalLength;
            var listValues = [ctrl.Summary.TotalWidth, ctrl.Summary.TotalHeight, ctrl.Summary.TotalLength];
            if (
                listValues.every(function (item) {
                    return item != null && item !== '';
                }) === false &&
                listValues.every(function (item) {
                    return item == null || item === '';
                }) === false
            ) {
                toaster.pop('error', $translate.instant('Admin.Js.OrdersItemsSummary.InvalidDimensionsValues'));
                return;
            }
            $http
                .post('orders/updateDimensions', {
                    orderId: ctrl.orderId,
                    width: ctrl.Summary.TotalWidth,
                    height: ctrl.Summary.TotalHeight,
                    length: ctrl.Summary.TotalLength,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.Summary.TotalWidth = data.obj.width;
                        ctrl.Summary.TotalHeight = data.obj.height;
                        ctrl.Summary.TotalLength = data.obj.length;
                        ctrl.Summary.IsNotEditedDimensions = data.obj.IsNotEditedDimensions;
                        toaster.pop('success', '', $translate.instant('Admin.Js.OrdersItemsSummary.DimensionsSaved'));
                        ctrl.demensionsEdit = false;
                        ctrl.getOrderItemsSummary();
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.OrdersItemsSummary.ErrorDimensionsSaved'));
                    }
                });
        };
        ctrl.weightStartEdit = function () {
            ctrl.weightBackup = ctrl.Summary.TotalWeight;
            ctrl.weightEdit = true;
        };
        ctrl.weightCancelEdit = function () {
            ctrl.Summary.TotalWeight = ctrl.weightBackup;
            ctrl.weightEdit = false;
        };
        ctrl.weightApplyEdit = function (form) {
            ctrl.Summary.TotalWeight = form != null && form.TotalWeight ? form.TotalWeight.$modelValue : ctrl.Summary.TotalWeight;
            $http
                .post('orders/updateWeight', {
                    orderId: ctrl.orderId,
                    weight: ctrl.Summary.TotalWeight,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        ctrl.Summary.TotalWeight = data.obj.weight;
                        ctrl.Summary.IsNotEditedWeight = data.obj.IsNotEditedWeight;
                        toaster.pop('success', '', $translate.instant('Admin.Js.OrdersItemsSummary.WeightSaved'));
                        ctrl.weightEdit = false;
                        ctrl.getOrderItemsSummary();
                    } else {
                        toaster.pop('error', '', $translate.instant('Admin.Js.OrdersItemsSummary.ErrorWeightSaved'));
                    }
                });
        };
        ctrl.customerCommentStartEdit = function () {
            ctrl.customerCommentBackup = ctrl.Summary.CustomerComment;
            ctrl.customerCommentEdit = true;
        };
        ctrl.customerCommentApplyEdit = function (form) {
            ctrl.Summary.CustomerComment = form != null ? form.CustomerComment.$modelValue : ctrl.Summary.CustomerComment;
            ctrl.commentField[0].model = ctrl.Summary.CustomerComment;
            $http
                .post('orders/updateCustomerComment', {
                    orderId: ctrl.orderId,
                    customerComment: ctrl.Summary.CustomerComment,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                        ctrl.customerCommentEdit = false;
                    }
                });
        };
        ctrl.customerCommentCancelEdit = function () {
            ctrl.Summary.CustomerComment = ctrl.customerCommentBackup;
            ctrl.customerCommentEdit = false;
        };
        ctrl.countDevicesStartEdit = function () {
            ctrl.countDevicesBackup = ctrl.Summary.CountDevices;
            ctrl.countDevicesEdit = true;
        };
        ctrl.countDevicesApplyEdit = function (form) {
            $http
                .post('orders/updateCountDevices', {
                    orderId: ctrl.orderId,
                    countDevices: ctrl.Summary.CountDevices,
                })
                .then(function (response) {
                    var data = response.data;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Order.ChangesSaved'));
                        ctrl.countDevicesEdit = false;
                    }
                });
        };
        ctrl.countDevicesCancelEdit = function () {
            ctrl.Summary.CountDevices = ctrl.countDevicesBackup;
            ctrl.countDevicesEdit = false;
        };
        ctrl.setShippingTypeContent = function (src) {
            $templateRequest(src)
                .then((response) => {
                    ctrl.shippingContent = response?.trim();
                })
                .catch((error) => console.error(error));
        };

        ctrl.goToCustomerOrderAttachments = function () {
            var customerOrderAttachmentsTab = document.getElementById('customerOrderAttachments');
            if (customerOrderAttachmentsTab.children.length == 0) return;
            customerOrderAttachmentsTab.children[0].click();
            $timeout(function () {
                var elem = document.createElement('a');
                elem.href = urlHelper.getAbsUrl('orders/edit/' + ctrl.orderId + window.location.search + '#customerOrderAttachments', false);
                elem.click();
            }, 100);
        };
    };
    OrderItemsSummaryCtrl.$inject = ['$http', '$timeout', 'toaster', 'SweetAlert', '$translate', '$uibModal', '$templateRequest', 'urlHelper'];
    ng.module('orderItemsSummary', [])
        .controller('OrderItemsSummaryCtrl', OrderItemsSummaryCtrl)
        .component('orderItemsSummary', {
            templateUrl: orderItemsSummaryTemplate,
            controller: OrderItemsSummaryCtrl,
            transclude: {
                footerLeft: '?footerLeft',
            },
            bindings: {
                orderId: '=',
                onInit: '&',
                country: '=',
                region: '=',
                district: '=',
                city: '=',
                zip: '=',
                street: '=',
                house: '=',
                structure: '=',
                apartment: '=',
                entrance: '=',
                floor: '=',
                isEdit: '<',
                onStopEdit: '&',
                onUpdateOrderItems: '&',
                statusComment: '@',
                adminComment: '@',
                trackNumber: '=',
            },
        });
})(window.angular);
