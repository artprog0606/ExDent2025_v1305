import './styles/autocompleter.scss';
import './templates/common.html';
import './templates/location.html';
import './templates/properties.html';
import './templates/categoriesProducts.html';

import autocompleterService from './services/autocompleterService.js';

import {
    autocompleterDirective,
    autocompleterInputDirective,
    autocompleterListDirective,
    autocompleterItemDirective,
} from './directives/autocompleterDirectives.js';

import AutocompleterCtrl from './controllers/autocompleterController.js';
import AutocompleterInputCtrl from './controllers/autocompleterInputController.js';
import AutocompleterListCtrl from './controllers/autocompleterListController.js';
import AutocompleterItemCtrl from './controllers/autocompleterItemController.js';

const moduleName = 'autocompleter';

angular
    .module(moduleName, [])
    .constant('autocompleterConfig', {
        minLength: 3,
        requestUrl: undefined,
        field: undefined,
        templatePath: undefined,
        linkAll: undefined,
        maxHeightList: undefined,
        minHeightList: undefined,
    })
    .directive('autocompleter', autocompleterDirective)
    .directive('autocompleterInput', autocompleterInputDirective)
    .directive('autocompleterList', autocompleterListDirective)
    .directive('autocompleterItem', autocompleterItemDirective)
    .service('autocompleterService', autocompleterService)
    .controller('AutocompleterCtrl', AutocompleterCtrl)
    .controller('AutocompleterInputCtrl', AutocompleterInputCtrl)
    .controller('AutocompleterListCtrl', AutocompleterListCtrl)
    .controller('AutocompleterItemCtrl', AutocompleterItemCtrl);

export default moduleName;
