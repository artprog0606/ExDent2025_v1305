(function (ng) {
    const CustomerViewCtrl = /* @ngInject */ function ($http, SweetAlert, $window, toaster, $translate, uiTabsService) {
        const ctrl = this;

        ctrl.init = function (customerId, customerCode) {
            ctrl.customerId = customerId;
            ctrl.customerCode = customerCode;

            localStorage.removeItem(uiTabsService.getNameForStorage());

            ctrl.getCustomerView();
        };

        ctrl.getCustomerView = function () {
            $http
                .get('customers/getView', {
                    params: { id: ctrl.customerId, code: ctrl.customerCode },
                })
                .then((response) => {
                    const { data } = response;
                    if (data == null) {
                        window.location.assign('customers');
                    }
                    ctrl.instance = data;
                    ctrl.customerAddress =
                        ctrl.instance.Customer.Contacts[0] != null ? ctrl.getCustomerFullAddress(ctrl.instance.Customer.Contacts[0]) : null;
                    ctrl.customerName = !data.CustomerCompanyName
                        ? `${data.Customer.LastName} ${data.Customer.FirstName} ${data.Customer.Patronymic}`
                        : data.CustomerCompanyName;
                });
        };

        ctrl.getCustomerLocationAddress = function (contact) {
            let result = contact.City;

            if (!ctrl.isNullOrEmpty(contact.Region)) {
                result += (!ctrl.isNullOrEmpty(result) ? ' ' : '') + contact.Region;
            }

            if (!ctrl.isNullOrEmpty(contact.Country)) {
                result += (!ctrl.isNullOrEmpty(result) ? ', ' : '') + contact.Country;
            }
            return result;
        };

        ctrl.getCustomerAddress = function (contact) {
            let result = '';

            if (!ctrl.isNullOrEmpty(contact.Street)) result += `${$translate.instant('Admin.Js.CustomerView.Street')} ${contact.Street}`;

            if (!ctrl.isNullOrEmpty(contact.House)) result += `, ${$translate.instant('Admin.Js.CustomerView.House')} ${contact.House}`;

            if (!ctrl.isNullOrEmpty(contact.Apartment)) result += `, ${$translate.instant('Admin.Js.CustomerView.Ap')} ${contact.Apartment}`;

            if (!ctrl.isNullOrEmpty(contact.Structure)) result += `, ${$translate.instant('Admin.Js.CustomerView.Struct')} ${contact.Structure}`;

            if (!ctrl.isNullOrEmpty(contact.Entrance)) result += `, ${$translate.instant('Admin.Js.CustomerView.Entrance')} ${contact.Entrance}`;

            if (!ctrl.isNullOrEmpty(contact.Floor)) result += `, ${$translate.instant('Admin.Js.CustomerView.Floor')} ${contact.Floor}`;

            return result;
        };

        ctrl.getCustomerFullAddress = function (contact) {
            const array = [];

            if (contact.Zip != null && contact.Zip.length > 0 && contact.Zip !== '-') {
                array.push(contact.Zip);
            }

            if (contact.Country != null && contact.Country.length > 0) {
                array.push(contact.Country);
            }

            if (contact.Region != null && contact.Region.length > 0 && contact.Region !== '-') {
                array.push(contact.Region);
            }

            if (contact.City != null && contact.City.length > 0) {
                array.push(contact.City);
            }

            if (contact.Street != null && contact.Street.length > 0) {
                array.push(ctrl.getCustomerAddress(contact));
            }

            return array.join(', ');
        };

        ctrl.isNullOrEmpty = function (str) {
            return str == null || str.length == 0;
        };

        ctrl.delete = function () {
            SweetAlert.confirm($translate.instant('Admin.Js.Customer.AreYouSure'), {
                title: $translate.instant('Admin.Js.Customer.Deleting'),
            }).then((result) => {
                if (result === true || result.value) {
                    $http
                        .post('customers/deleteCustomer', {
                            customerId: ctrl.customerId,
                        })
                        .then((response) => {
                            const { data } = response;
                            if (data) {
                                $window.location.assign('customers');
                            } else {
                                toaster.pop('error', '', $translate.instant('Admin.Js.Customer.UserCantBeDeleted'));
                            }
                        });
                }
            });
        };

        ctrl.updateAdminComment = function (comment) {
            $http
                .post('customers/updateAdminComment', {
                    id: ctrl.customerId,
                    comment,
                })
                .then((response) => {
                    const { data } = response;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Customer.ChangesSaved'));
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Customer.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.updateStatus = function () {
            $http
                .post('customers/updateClientStatus', {
                    id: ctrl.customerId,
                    clientStatus: ctrl.clientStatus,
                })
                .then((response) => {
                    const { data } = response;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Customer.ChangesSaved'));
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Customer.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.leadEventsOnInit = function (leadEvents) {
            ctrl.leadEvents = leadEvents;
        };

        ctrl.updateLeadEvents = function () {
            ctrl.leadEvents.getLeadEvents();
        };

        ctrl.updateLeadEventsWithDelay = function () {
            setTimeout(ctrl.updateLeadEvents, 1000);
        };

        ctrl.changeStatusClient = function (currentStatus) {
            ctrl.clientStatus = ctrl.clientStatus === currentStatus ? 'none' : currentStatus;
            ctrl.updateStatus();
        };

        ctrl.taskGridOnInit = function (taskGrid) {
            ctrl.taskGrid = taskGrid;
        };

        ctrl.fetchGrid = function () {
            ctrl.taskGrid.modalClose();
        };

        ctrl.addSocialUser = function (type, link) {
            if (ctrl.btnSocialAdding != null || !link) return;

            let url = '';
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
                .post(url, { customerId: ctrl.customerId, link })
                .then((response) => {
                    const { data } = response;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Customer.ChangesSaved'));
                        location.reload();
                    } else if (data.errors != null) {
                        data.errors.forEach((error) => {
                            toaster.pop('error', '', error);
                        });
                    } else {
                        toaster.pop('error', $translate.instant('Admin.Js.Customer.ErrorWhileSaving'));
                    }
                })
                .finally(() => {
                    ctrl.btnSocialAdding = null;
                });
        };

        ctrl.deleteSocialLink = function (type) {
            SweetAlert.confirm($translate.instant('Admin.Js.Customer.AreYouSureDelete'), {
                title: $translate.instant('Admin.Js.Customer.Deleting'),
            }).then((result) => {
                if (result === true || result.value) {
                    let url = '';
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

                    $http.post(url, { customerId: ctrl.customerId }).then((response) => {
                        const { data } = response;
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
                    fromCustomerId: ctrl.customerId,
                    toCustomerId: result.customerId,
                    type,
                })
                .then((response) => {
                    $window.location.reload(true);
                });
        };

        ctrl.mouseoverOptions = function (event) {
            angular.element(event.currentTarget).click();
        };

        ctrl.filterEvents = function (filterBy) {
            ctrl.leadEvents.filterType = filterBy;
        };

        ctrl.editCustomerClose = function () {
            ctrl.getCustomerView();
        };

        ctrl.createOrderFromCart = function () {
            ctrl.addingOrder = false;
            $http
                .post('orders/addOrderFromCart', {
                    customerId: ctrl.customerId,
                })
                .then((response) => {
                    const { data } = response;
                    if (data.result == true && response.data.obj != 0) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Customer.DraftOrder') + response.data.obj);
                        $window.location.assign(`orders/edit/${response.data.obj}`);
                    } else {
                        toaster.pop(
                            'error',
                            $translate.instant('Admin.Js.Customer.ErrorCreatingOrder'),
                            data.errors != null ? `</br>${data.errors.join('</br>')}` : '',
                        );
                        ctrl.addingOrder = false;
                    }
                });
        };

        ctrl.activityOnInit = function (activityActions) {
            ctrl.activityActions = activityActions;
        };

        // #region partners

        ctrl.bindToPartner = function (partner) {
            if (partner == null || partner.partnerId == null) {
                return false;
            }
            $http
                .post('partners/bindCustomer', {
                    partnerId: partner.partnerId,
                    customerId: ctrl.customerId,
                })
                .then((response) => {
                    const { data } = response;
                    if (data.result === true) {
                        ctrl.getCustomerView();
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                    }
                });
        };

        ctrl.unbindFromPartner = function () {
            SweetAlert.confirm('Вы уверены, что хотите отвязать покупателя от партнера?', {
                title: $translate.instant('Admin.Js.Deleting'),
            }).then((result) => {
                if (result === true || result.value) {
                    $http
                        .post('partners/unbindCustomer', {
                            customerId: ctrl.customerId,
                        })
                        .then((response) => {
                            const { data } = response;
                            if (data.result === true) {
                                ctrl.getCustomerView();
                            } else {
                                toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                            }
                        });
                }
            });
        };

        // #endregion

        ctrl.saveManager = function (customerId, managerId) {
            if (managerId !== null) {
                $http
                    .post('customers/changeCustomerManager', {
                        customerId,
                        managerId,
                    })
                    .then((response) => {
                        const { data } = response;
                        if (data.result === true) {
                            toaster.pop('success', '', $translate.instant('Admin.Js.Customer.ChangesSaved'));
                            ctrl.getCustomerView();
                        } else {
                            toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.ErrorWhileSaving'));
                        }
                    });
            }
        };

        ctrl.sendNotification = function () {
            $http
                .post('customers/sendMobileAppNotification', {
                    customerId: ctrl.customerId,
                })
                .then((response) => {
                    const { data } = response;
                    if (data.result === true) {
                        toaster.pop('success', '', $translate.instant('Admin.Js.Customer.PushSended'));
                        ctrl.getCustomerView();
                    } else {
                        toaster.error('', (data.errors || [])[0] || $translate.instant('Admin.Js.Customer.ErrorWhileSending'));
                    }
                });
        };
    };

    ng.module('customerView', ['uiGridCustom', 'customerOrders', 'customerLeads', 'customerBookings']).controller(
        'CustomerViewCtrl',
        CustomerViewCtrl,
    );
})(window.angular);
