import addressModalTemplate from '../templates/addressModal.html';
import addressModalFooterTemplate from '../templates/addressModalFooter.html';

/* @ngInject */
function addressService($http, $q, modalService, $translate, urlHelper) {
    var service = this,
        fields;

    service.dialogRender = function (callbackClose, parentScope) {
        var options = {
            modalClass: parentScope.themeAlt ? 'address-dialog address-dialog--admin-mode' : 'address-dialog',
            modalOverlayClass: 'address-dialog-overlay',
            callbackClose: callbackClose,
            isOpen: true,
            destroyOnClose: true,
        };

        modalService.renderModal(
            'modalAddress',
            $translate.instant(parentScope.formData ? 'Js.Address.EditTitle' : 'Js.Address.AddTitle'),
            `<div>
                <div data-add-edit-address-form=""
                    data-type="{{addEditAddress.type}}"
                    data-theme-alt="addEditAddress.themeAlt"
                    data-customer-id="addEditAddress.customerId"
                    data-form-data="addEditAddress.formData"
                    data-address-selected="addEditAddress.addressSelected"
                    data-on-add-edit-address="addEditAddress.onAddEditAddress(formData, contacts, addressSelected)"
                    data-autocomplete-alt="addEditAddress.autocompleteAlt"
                    data-zone-update-on-add="addEditAddress.zoneUpdateOnAdd"
                    data-contact-id="addEditAddress.contactId"
                    data-is-show-name="addEditAddress.isShowName"></div>
            </div>`,
            null,
            options,
            { addEditAddress: parentScope },
        );
    };
    // `<div data-ng-include="'${addressModalFooterTemplate}'"></div>`
    service.dialogOpen = function () {
        modalService.open('modalAddress');
    };

    service.dialogClose = function () {
        modalService.close('modalAddress');
    };

    service.getDialogScope = function () {
        return modalService.getModal('modalAddress').then(function (dialog) {
            return dialog.modalScope;
        });
    };

    service.removeAddress = function (contactId, customerId) {
        return $http
            .post(customerId == null ? urlHelper.getAbsUrl('MyAccount/DeleteCustomerContact', true) : 'Customers/DeleteCustomerContact', {
                customerId,
                contactId,
                rnd: Math.random(),
            })
            .then(function (response) {
                return response.data;
            });
    };

    service.getAddresses = function (customerId) {
        return $http
            .get(customerId == null ? urlHelper.getAbsUrl('MyAccount/GetCustomerContacts', true) : 'Customers/GetCustomerContacts', {
                params: { customerId, rnd: Math.random() },
            })
            .then(function (response) {
                return response.data;
            });
    };

    service.getFields = function (isShowName, customerId) {
        return fields != null
            ? $q.when(fields)
            : $http
                  .get(
                      customerId == null
                          ? urlHelper.getAbsUrl('MyAccount/GetFieldsForCustomerContacts', true)
                          : 'Customers/GetFieldsForCustomerContacts',
                      { params: { customerId, isShowName } },
                  )
                  .then(function (response) {
                      return (fields = response.data);
                  });
    };

    service.processAddress = function (address, customerId) {
        return $http
            .post(customerId == null ? urlHelper.getAbsUrl('MyAccount/processAddress', true) : 'Customers/processAddress', { customerId, address })
            .then(function (response) {
                return response.data;
            });
    };

    service.addUpdateCustomerContact = function (account, customerId) {
        return $http
            .post(customerId == null ? urlHelper.getAbsUrl('MyAccount/AddUpdateCustomerContact', true) : 'Customers/AddUpdateCustomerContact', {
                customerId,
                account,
                rnd: Math.random(),
            })
            .then(function (response) {
                return response.data;
            });
    };

    service.addressStringify = function (address, short = false) {
        var array = [];

        if (address.Zip != null && address.Zip.length > 0 && address.Zip !== '-' && !short) {
            array.push(address.Zip);
        }

        if (address.Country != null && address.Country.length > 0 && !short) {
            array.push(address.Country);
        }

        if (address.Region != null && address.Region.length > 0 && address.Region !== '-' && !short) {
            array.push(address.Region);
        }

        if (address.City != null && address.City.length > 0) {
            array.push(address.City);
        }

        if (address.Street != null && address.Street.length > 0) {
            array.push(address.Street);
        }

        if (address.House != null && address.House.length > 0) {
            array.push(address.House);
        }

        return array.join(', ');
    };

    service.showAddressListModal = (parentScope, options) => {
        const modalId = 'addressListModal';
        const modalOptions = Object.assign(
            {
                destroyOnClose: true,
                isOpen: true,
                modalClass: 'address-list-modal',
            },
            options,
        );

        let selectedAddress = null;

        const onCloseModalCb = {
            initAddressFn: (address) => {
                selectedAddress = address;
            },
            changeAddressFn: (address) => {
                selectedAddress = address;
            },
            saveAddressFn: (address) => {
                selectedAddress = address;
            },
            deleteAddressFn: (items, itemRemoved, addressSelected, isItemSeletedRemoved) => {
                selectedAddress = addressSelected;
                parentScope.onApply(selectedAddress, modalId);
            },
            onApply: (modalId) => {
                parentScope.onApply(selectedAddress, modalId);
                modalService.close(modalId);
            },
            clearGeoModeUrlParam: () => {
                parentScope.clearGeoModeUrlParam();
            },
        };

        modalService.renderModal(
            modalId,
            $translate.instant('Js.Address.SelectAddress'),
            `<div data-address-list
                      data-type="change"
                      data-is-show-name="false"
                      data-zone-update-on-add="true"
                      data-init-address-fn="addressList.initAddressFn(address)"
                      data-delete-address-fn="addressList.deleteAddressFn(items, itemRemoved, addressSelected, isItemSeletedRemoved)"
                      data-save-address-fn="addressList.saveAddressFn(address)"
                      data-change-address-fn="addressList.changeAddressFn(address)">
                </div>`,
            `<div class="address-list-modal__footer">
                    <button data-ng-click="addressList.onApply('${modalId}')" class="btn btn-middle btn-submit" type="button">Ок</button>
                </div>`,
            modalOptions,
            { addressList: onCloseModalCb },
        );
    };

    service.findItemForSelect = function (items, contactId) {
        let result = null;

        if (contactId != null && items != null && items.length > 0) {
            result = items.find((x) => x.ContactId == contactId);
        }

        if (result == null) {
            result = items.find((x) => x.IsMain);
        }

        if (result == null) {
            result = items[0];
        }

        return result;
    };
}

export default addressService;
