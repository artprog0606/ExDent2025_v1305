/* @ngInject */
function OrderHistoryCtrl(
    $element,
    $sce,
    $window,
    orderService,
    windowService,
    toaster,
    $translate,
    SweetAlert,
    $location,
    Upload,
    scrollToBlockService,
    $q,
) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.setViewFromUrl()
            .then(() => {
                ctrl.isGetOrdersData = false;
                return orderService.getOrders();
            })
            .then(function (orders) {
                ctrl.items = orders;
                if ($location.search().mode == null) {
                    ctrl.changeModeAll();
                }
            })
            .finally(() => {
                ctrl.isGetOrdersData = true;
            });
    };

    ctrl.changeModeAll = function () {
        ctrl.mode = 'all';
    };

    ctrl.changeModeDetails = function () {
        ctrl.mode = 'details';
    };

    ctrl.resetOrderNumber = function () {
        $location.search('ordernumber', null);
    };

    ctrl.resetMode = function () {
        $location.search('mode', null);
    };

    ctrl.prepareDetails = function (order) {
        var paymentSelected;

        ctrl.orderDetails = order;

        if (ctrl.orderDetails.Payments != null && ctrl.orderDetails.Payments.length > 0) {
            for (var i = 0, l = ctrl.orderDetails.Payments.length; i < l; i++) {
                if (ctrl.orderDetails.Payments[i].Id == ctrl.orderDetails.PaymentMethodId) {
                    paymentSelected = ctrl.orderDetails.Payments[i];
                    break;
                }
            }

            if (paymentSelected == null) {
                paymentSelected = ctrl.orderDetails.Payments[0];
            }

            ctrl.orderDetails.paymentSelected = paymentSelected;
            ctrl.orderDetails.PaymentForm = $sce.trustAsHtml(ctrl.orderDetails.PaymentForm);
        }

        return ctrl.orderDetails;
    };

    ctrl.view = function (ordernumber) {
        return orderService.getOrderDetails(ordernumber).then(function (order) {
            ctrl.prepareDetails(order);
            ctrl.changeModeDetails();
            if (ctrl.onChangeView != null) {
                ctrl.onChangeView({ orderHistoryCtrl: ctrl, ordernumber: ordernumber });
            }
            setTimeout(function () {
                if ($element[0].getBoundingClientRect().top < 0) {
                    scrollToBlockService.scrollToBlock($element[0]);
                }
            });
            return order;
        });
    };

    ctrl.cancelOrder = function (ordernumber) {
        return SweetAlert.confirm($translate.instant('Js.Order.AreYouWantCancelOrder'), {
            title: $translate.instant('Js.Order.OrderCancel'),
        }).then(function (result) {
            if (result.value === true) {
                return orderService
                    .cancelOrder(ordernumber)
                    .then(orderService.getOrderDetails.bind(orderService, ordernumber))
                    .then(function (order) {
                        return (ctrl.orderDetails = order);
                    });
            }
        });
    };

    ctrl.print = function (ordernumber) {
        windowService.print('PrintOrder/' + ordernumber, 'printOrder', 'menubar=no,location=no,resizable=yes,scrollbars=yes');
    };

    ctrl.changePaymentMethod = function (ordernumber, paymentId) {
        return orderService.changePaymentMethod(ordernumber, paymentId).then(function (response) {
            if (response != null) {
                return orderService.getOrderDetails(ordernumber).then(ctrl.prepareDetails);
            }
        });
    };

    ctrl.changeOrderComment = function (ordernumber, customercomment) {
        return orderService.changeOrderComment(ordernumber, customercomment).then(function (response) {
            if (response === true) {
                toaster.pop('success', '', $translate.instant('Js.Order.CommentSaved'));
            } else {
                toaster.pop('error', '', $translate.instant('Js.Order.CommentNotSaved'));
            }
        });
    };

    ctrl.setViewFromUrl = function () {
        if ($location.search().mode != null && $location.search().ordernumber != null) {
            return ctrl.view($location.search().ordernumber).then(() => (ctrl.mode = $location.search().mode));
        }
        return $q.resolve(null);
    };

    ctrl.selectedFiles = function ($files, $file, $newFiles, $duplicateFiles, $invalidFiles, $event) {
        if ($files && $files.length > 0) {
            var exceededLimit = false;
            if (ctrl.orderDetails.CustomerOrderAttachments && ctrl.orderDetails.CustomerOrderAttachments.length + $files.length > 10) {
                exceededLimit = true;
                $files.splice(10 - ctrl.orderDetails.CustomerOrderAttachments.length);
            }
            ctrl.loadingFiles = true;
            Upload.upload({
                url: '/myaccount/uploadAttachments',
                file: $files,
                data: {
                    orderId: ctrl.orderDetails.OrderID,
                },
            }).then(function (response) {
                var data = response.data;
                if (exceededLimit) toaster.error($translate.instant('Js.Checkout.File.ExceededLimit'));
                if (data.result) {
                    for (var i in data.obj) {
                        if (data.obj[i].Result === true) {
                            ctrl.orderDetails.CustomerOrderAttachments.push(data.obj[i].Attachment);
                            toaster.success(
                                $translate.instant('Js.Checkout.File') +
                                    data.obj[i].Attachment.FileName +
                                    $translate.instant('Js.Checkout.FileWasAdded'),
                            );
                        } else {
                            toaster.error(
                                $translate.instant('Js.ErrorLoading'),
                                (data.obj[i].Attachment != null ? data.obj[i].Attachment.FileName + ': ' : '') + data.obj[i].Error,
                            );
                        }
                    }
                } else {
                    if (data.errors) {
                        data.errors.forEach(function (error) {
                            toaster.error(error);
                        });
                    } else {
                        toaster.error($translate.instant('Js.ErrorLoading'));
                    }
                }
                $files.splice(0);
                ctrl.loadingFiles = false;
            });
        } else if ($invalidFiles.length > 0) {
            toaster.pop('error', $translate.instant('Js.ErrorLoading'), $translate.instant('Js.Order.FileDoesNotMeet'));
            ctrl.loadingFiles = false;
        } else {
            ctrl.loadingFiles = false;
        }
    };
}

export default OrderHistoryCtrl;
