import addressListTemplate from '../templates/addressList.html';
import addAddressFormTemplate from '../templates/addressModal.html';
function addressListDirective() {
    return {
        restrict: 'A',
        scope: {
            type: '@', // change, view, viewRo
            initAddressFn: '&',
            changeAddressFn: '&',
            saveAddressFn: '&',
            deleteAddressFn: '&',
            isShowFullAddress: '<?',
            isShowName: '<?',
            contactId: '<?',
            themeAlt: '<?',
            compactMode: '<?',
            customerId: '<?',
            autocompleteAlt: '<?',
            zoneUpdateOnAdd: '<?',
            requiredValidationEnabled: '<?',
        },
        controller: 'AddressListCtrl',
        controllerAs: 'addressList',
        bindToController: true,
        replace: true,
        templateUrl: addressListTemplate,
        transclude: true,
        link: function (scope, element, attr, ctrl, transclude) {},
    };
}

function addressListTransclude() {
    return {
        restrict: 'A',
        require: '^addressList',
        transclude: true,
        link: function (scope, element, attrs, parentCtrl) {
            parentCtrl.transcudeContent(scope, element);
        },
    };
}

function addEditAddressFormDirective() {
    return {
        restrict: 'A',
        transclude: true,
        templateUrl: addAddressFormTemplate,
        controller: 'AddEditAddressFormCtrl',
        controllerAs: 'addEditAddressForm',
        bindToController: true,
        scope: {
            type: '@',
            isShowName: '<?',
            onAddEditAddress: '&',
            customerId: '<?',
            themeAlt: '<?',
            autocompleteAlt: '<?',
            formData: '<?',
            addressSelected: '<?',
            zoneUpdateOnAdd: '<?',
            contactId: '<?',
        },
        link: function (scope, element, attrs, parentCtrl) {},
    };
}

export { addressListDirective, addressListTransclude, addEditAddressFormDirective };
