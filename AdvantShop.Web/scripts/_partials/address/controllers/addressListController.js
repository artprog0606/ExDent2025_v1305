/* @ngInject */
function AddressListCtrl(addressService, modalService, addressListConfig, $transclude, $scope, SweetAlert, $translate) {
    var ctrl = this,
        timerChange,
        processContactTimer;

    ctrl.$onInit = function () {
        ctrl.items = [];
        ctrl.isLoaded = false;
        ctrl.themeAlt = ctrl.themeAlt != null ? ctrl.themeAlt : addressListConfig.themeAlt;
        ctrl.compactMode = ctrl.compactMode != null ? ctrl.compactMode : addressListConfig.compactMode;
        ctrl.autocompleteAlt = ctrl.autocompleteAlt != null ? ctrl.autocompleteAlt : addressListConfig.autocompleteAlt;

        modalService.getModal('modalAddress').then(function (modal) {
            ctrl.formModal = modal.modalScope._form;
        });

        addressService.getAddresses(ctrl.customerId).then(function (response) {
            ctrl.items = [];

            if (response != null && response !== '' && response.length > 0) {
                ctrl.items = response;
            }

            ctrl.addressSelected = addressService.findItemForSelect(ctrl.items, ctrl.contactId);

            ctrl.initAddressFn({
                address: ctrl.addressSelected,
            });

            ctrl.isLoaded = true;
        });
    };

    ctrl.isModalRendered = function () {
        return modalService.hasModal('modalAddress');
    };

    ctrl.change = function (address) {
        if (timerChange != null) {
            clearTimeout(timerChange);
        }

        timerChange = setTimeout(function () {
            ctrl.changeAddressFn({
                address: ctrl.addressSelected,
            });
        }, 600);
    };

    ctrl.onAddEditAddress = (formData, contacts, addressSelected) => {
        ctrl.save(formData, contacts, addressSelected);
    };

    ctrl.add = function () {
        if (ctrl.isModalRendered() === false) {
            addressService.dialogRender(null, ctrl);
        } else {
            addressService.dialogOpen();
        }
    };

    ctrl.edit = function (event, item) {
        event.preventDefault();

        const form = {};

        form.contactId = item.ContactId;
        form.fio = item.Name;
        form.firstName = item.FirstName;
        form.lastName = item.LastName;
        form.patronymic = item.Patronymic;
        form.countryId = item.CountryId;
        form.country = item.Country; //set in ctrl.buildModal()
        form.region = item.Region;
        form.city = item.City;
        form.district = item.District;
        form.zip = item.Zip;
        form.street = item.Street;
        form.house = item.House;
        form.apartment = item.Apartment;
        form.structure = item.Structure;
        form.entrance = item.Entrance;
        form.floor = item.Floor;

        if (ctrl.isModalRendered() === false) {
            addressService.dialogRender('addressList.modalCallbackClose', { ...ctrl, formData: form });
        } else {
            addressService.dialogOpen();
        }
    };

    ctrl.remove = function (contactId, index) {
        SweetAlert.confirm($translate.instant('Js.Address.AreYouSureDelete'), {
            title: $translate.instant('Js.Address.Deleting'),
        }).then(function (result) {
            if (result === true || result.value) {
                addressService.removeAddress(contactId, ctrl.customerId).then(function (response) {
                    if (response === true) {
                        var isItemSeletedRemoved = ctrl.addressSelected === ctrl.items[index];

                        var itemRemoved = ctrl.items.splice(index, 1);

                        if (isItemSeletedRemoved && ctrl.items.length > 0) {
                            ctrl.addressSelected = ctrl.items[0];
                        }

                        if (ctrl.deleteAddressFn != null) {
                            ctrl.deleteAddressFn({
                                items: ctrl.items,
                                itemRemoved: itemRemoved[0],
                                addressSelected: ctrl.addressSelected,
                                isItemSeletedRemoved,
                            });
                        }
                    }
                });
            }
        });
    };

    ctrl.save = function (formData, contacts, addressSelected) {
        ctrl.items = contacts;
        ctrl.addressSelected = addressSelected;

        // addressService.dialogClose();

        ctrl.saveAddressFn({
            address: ctrl.addressSelected,
        });

        // addressService.getAddresses(ctrl.customerId).then(function (response) {
        //     ctrl.items = response;
        //     ctrl.addressSelected = ctrl.findItemForSelect();
        //
        //     addressService.dialogClose();
        //
        //     ctrl.saveAddressFn({
        //         address: ctrl.addressSelected,
        //     });
        // });
    };

    ctrl.cancelAdding = function (modalId) {
        modalService.close(modalId);
    };

    ctrl.getObjectForUpdate = function (form) {
        var account = {};

        if (form.contactId) {
            account.ContactId = form.contactId;
        }

        if (form.fio) {
            account.Fio = form.fio;
        }

        if (form.firstName) {
            account.FirstName = form.firstName;
        }
        if (form.lastName) {
            account.LastName = form.lastName;
        }
        if (form.patronymic) {
            account.Patronymic = form.patronymic;
        }

        if (form.country) {
            account.CountryId = form.country.CountryId;
            account.Country = form.country.Name;
        }

        if (form.region) {
            account.Region = form.region;
        }

        if (form.district) {
            account.District = form.district;
        }

        if (form.city) {
            account.City = form.city;
        }

        if (form.zip) {
            account.Zip = form.zip;
        }

        account.Street = form.street;
        account.House = form.house;
        account.Apartment = form.apartment;
        account.Structure = form.structure;
        account.Entrance = form.entrance;
        account.Floor = form.floor;

        account.IsShowName = ctrl.isShowName;
        account.IsMain = ctrl.addressSelected != null ? form.contactId === ctrl.addressSelected.ContactId : false;

        return account;
    };

    ctrl.addressStringify = function (address) {
        addressService.addressStringify(address);
    };

    ctrl.transcudeContent = function (scope, element) {
        const scopeNew = $scope.$parent.$new();
        scopeNew.address = scope.item;

        $transclude(scopeNew, function (clone, _scope) {
            element.append(clone);
        });
    };
}

export default AddressListCtrl;
