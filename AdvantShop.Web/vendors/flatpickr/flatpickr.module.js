import flatpickr from 'flatpickr';
import 'flatpickr/dist/flatpickr.min.css';
import { Russian } from "flatpickr/dist/l10n/ru.js";
flatpickr.localize(Russian); // default locale is now Russian

import './flatpickr.custom.css';

import './ng-flatpickr.js';

const moduleName = 'angular-flatpickr';

angular.module(moduleName)
    .config(['$localeProvider', 'ngFlatpickrDefaultOptions', function ($localeProvider, ngFlatpickrDefaultOptions) {
        ngFlatpickrDefaultOptions.locale = $localeProvider.$get().id.split('-')[0];
        ngFlatpickrDefaultOptions.disableMobile = true;
    }]);

export default moduleName;