/* @ngInject */
function MyAccountCtrl($http, $window, $location, toaster, $translate, $timeout, tabsService) {
    var ctrl = this,
        activeTabHeader,
        processCompanyNameTimer;

    ctrl.orderHistoryMode = 'all';
    ctrl.commonInfo = {};

    ctrl.showContent = function (tabHeader, fromUrl) {
        if (fromUrl) {
            ctrl.showTabs = false;
        }
        activeTabHeader = tabHeader.headerTab;
    };

    ctrl.changeTempEmail = function (email) {
        $http.post('myaccount/updatecustomeremail', { email: email }).then(function (response) {
            if (response.data === true) {
                ctrl.modalWrongNewEmail = false;
                $window.location.reload(true);
            } else {
                ctrl.modalWrongNewEmail = true;
            }
        });
    };

    ctrl.onChangeOrderHistoryMode = function (orderHistoryCtrl, ordernumber) {
        ctrl.orderHistoryCtrl = orderHistoryCtrl;
        $location.search('mode', orderHistoryCtrl.mode);
        $location.search('ordernumber', ordernumber);
        if (orderHistoryCtrl.mode === 'details') {
            ctrl.myaccountTitlePageText = activeTabHeader;
        }
    };

    ctrl.backToFromTabs = function () {
        if (ctrl.orderHistoryMode === 'all') {
            ctrl.showTabs = true;
            $location.search('tab', null);
        } else {
            ctrl.orderHistoryCtrl.changeModeAll();
            ctrl.myaccountTitlePageText = null;
        }
        $location.search('mode', null);
        $location.search('ordernumber', null);
    };

    ctrl.saveUser = function () {
        $http.post('myaccount/saveUserInfo', { userInfo: ctrl.user }).then(function (response) {
            let data = response.data;
            if (data != null && data.result) {
                toaster.pop('success', '', $translate.instant('Js.MyAccount.ChangesSaved'));
            } else if (data != null && data.errors != null) {
                data.errors.forEach(function (err) {
                    toaster.pop('error', '', err);
                });
            } else {
                toaster.pop('error', '', $translate.instant('Js.MyAccount.ChangesNotSaved'));
            }
        });
    };

    ctrl.changePassword = function () {
        $http.post('myaccount/changePassword', ctrl.password).then(function (response) {
            if (response.data.result) {
                toaster.pop('success', '', $translate.instant('Js.MyAccount.ChangesSaved'));
            } else if (response.data.errors != null) {
                response.data.errors.forEach(function (err) {
                    toaster.pop('error', '', err);
                });
            } else {
                toaster.pop('error', '', $translate.instant('Js.MyAccount.ChangesNotSaved'));
            }
        });
    };

    ctrl.processCompanyName = function (item) {
        if (processCompanyNameTimer != null) {
            $timeout.cancel(processCompanyNameTimer);
        }

        return (processCompanyNameTimer = $timeout(
            function () {
                if (item != null && item.CompanyData) {
                    ctrl.user.CustomerFields.forEach(function (field) {
                        if (field.FieldAssignment == 1) field.Value = item.CompanyData.CompanyName;
                        else if (field.FieldAssignment == 2) field.Value = item.CompanyData.LegalAddress;
                        else if (field.FieldAssignment == 3) field.Value = item.CompanyData.INN;
                        else if (field.FieldAssignment == 4) field.Value = item.CompanyData.KPP;
                        else if (field.FieldAssignment == 5) field.Value = item.CompanyData.OGRN;
                        else if (field.FieldAssignment == 6) field.Value = item.CompanyData.OKPO;
                    });
                } else if (item != null && item.BankData) {
                    ctrl.user.CustomerFields.forEach(function (field) {
                        if (field.FieldAssignment == 7) field.Value = item.BankData.BIK;
                        else if (field.FieldAssignment == 8) field.Value = item.BankData.BankName;
                        else if (field.FieldAssignment == 9) field.Value = item.BankData.CorrespondentAccount;
                    });
                }
            },
            item != null ? 0 : 700,
        ));
    };

    ctrl.changeAddressList = function (address) {
        $http.post('myaccount/setMainContact', { contactId: address.ContactId }).then((response) => {
            if (response.data.result === true) {
                toaster.pop('success', $translate.instant('Js.MyAccount.AddressList.SetMainSuccess'));
            } else {
                toaster.pop('error', 'Error set main contact');
            }
        });
    };

    ctrl.resetUrlParams = function (tabHeader) {
        if (tabHeader != null && tabHeader.id !== 'orderhistory') {
            $location.search('mode', null);
            $location.search('ordernumber', null);
        }
    };

    ctrl.showDetailsOrder = function (orderNumber, tabHeader) {
        if (ctrl.myAccountTabs && tabHeader) {
            tabsService.changeUrl('orderhistory');
            ctrl.showTabs = false;
            ctrl.myAccountTabs.selectFromUrl();
        }
        ctrl.showContent(tabHeader);
        $location.search('mode', 'details');
        $location.search('ordernumber', orderNumber);
    };

    ctrl.addTabs = (tabs) => {
        ctrl.myAccountTabs = tabs;
    };
}

export default MyAccountCtrl;
