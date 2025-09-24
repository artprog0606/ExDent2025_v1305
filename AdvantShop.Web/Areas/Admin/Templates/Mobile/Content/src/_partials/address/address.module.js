import addressModule from '../../../../../../../../scripts/_partials/address/address.module.js';
import './address.scss';
import '../../../styles/_shared/modal/adv-modal-compability.scss';

angular.module(addressModule).config(
    /* @ngInject */ function (addressListConfig) {
        addressListConfig.autocompleteAlt = true;
        addressListConfig.themeAlt = true;
        addressListConfig.overrideFields = {
            visible: ['IsShowCity', 'IsShowDistrict', 'IsShowState', 'IsShowCountry', 'IsShowZip', 'IsShowAddress', 'IsShowFullAddress'],
        };
        addressListConfig.requiredValidationEnabled = false;
    },
);
export default addressModule;
