import popoverBootstrap from 'angular-ui-bootstrap/src/popover/index.js';
import '../../../vendors/ui-bootstrap-custom/styles/ui-popover.css';

import rangeSliderModule from '../../../vendors/rangeSlider/rangeSlider.module.js';
import popoverModule from '../../_common/popover/popover.module.js';
import './styles/catalogFilter.scss';

import catalogFilterService from './services/catalogFilterService.js';
import { catalogFilterDirective, catalogFilterSortDirective, catalogFilterSelectSortDirective } from './directives/catalogFilterDirectives.js';
import CatalogFilterCtrl from './controllers/catalogFilterController.js';
import CatalogFilterSortCtrl from './controllers/catalogFilterSortController.js';

const moduleName = 'catalogFilter';

angular
    .module(moduleName, [rangeSliderModule, popoverBootstrap, popoverModule])
    .constant('catalogFilterAdvPopoverOptionsDefault', {
        position: 'left',
        isFixed: false,
        showOnLoad: false,
        overlayEnabled: false,
    })
    .service('catalogFilterService', catalogFilterService)
    .controller('CatalogFilterCtrl', CatalogFilterCtrl)
    .controller('CatalogFilterSortCtrl', CatalogFilterSortCtrl)
    .directive('catalogFilter', catalogFilterDirective)
    .directive('catalogFilterSort', catalogFilterSortDirective)
    .directive('catalogFilterSelectSort', catalogFilterSelectSortDirective);

export default moduleName;
