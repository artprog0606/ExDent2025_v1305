import './styles/address.scss';

import AddressListCtrl from './controllers/addressListController.js';
import { AddEditAddressFormController } from './controllers/addEditAddressFormController.ts';
import { addressListDirective, addressListTransclude, addEditAddressFormDirective } from './directives/addressDirectives.js';
import addressService from './services/addressService.js';
import '../zone/zone.js';
import zone from '../zone/zone.module.ts';

const moduleName = 'address';

angular
    .module(moduleName, [zone])
    .constant('addressListConfig', {
        autocompleteAlt: false,
        themeAlt: false,
        compactMode: false,
        overrideFields: {},
        requiredValidationEnabled: true,
    })
    .service('addressService', addressService)
    .controller('AddressListCtrl', AddressListCtrl)
    .controller('AddEditAddressFormCtrl', AddEditAddressFormController)
    .directive('addressList', addressListDirective)
    .directive('addEditAddressForm', addEditAddressFormDirective)
    .directive('addressListTransclude', addressListTransclude);

export default moduleName;
