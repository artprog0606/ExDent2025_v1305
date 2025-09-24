import './styles/shipping.scss';
import './styles/shippingPointsList.scss';

import './extendTemplate/extendTemplate.js';

import yandexMaps from '../../_common/yandexMaps/yandexMaps.module.js';
import '../../_common/modal/modal.module.js';

import ShippingService from './services/shipping.service.js';
import ShippingListCtrl from './controllers/shippingListController.js';
import ShippingVariantsCtrl from './controllers/shippingVariantsController.js';
import ShippingTemplateCtrl from './controllers/shippingTemplateController.js';
import ShippingItemErrorCtrl from './controllers/shippingItemError.js';
import {
    shippingListDirective,
    shippingTemplateDirective,
    shippingVariantsDirective,
    shippingItemError,
    shippingPointsListDirective,
} from './directives/shippingDirectives.js';
import ShippingPointsListCtrl from './controllers/shippingPointsListController.ts';

const moduleName = 'shipping';

angular
    .module(moduleName, [yandexMaps, 'modal'])
    .service('shippingService', ShippingService)
    .controller('ShippingListCtrl', ShippingListCtrl)
    .controller('ShippingTemplateCtrl', ShippingTemplateCtrl)
    .controller('ShippingVariantsCtrl', ShippingVariantsCtrl)
    .controller('ShippingItemErrorCtrl', ShippingItemErrorCtrl)
    .controller('ShippingPointsListCtrl', ShippingPointsListCtrl)
    .directive('shippingList', shippingListDirective)
    .directive('shippingTemplate', shippingTemplateDirective)
    .directive('shippingVariants', shippingVariantsDirective)
    .directive('shippingPointsList', shippingPointsListDirective)
    .component('shippingItemError', shippingItemError);

export default moduleName;
