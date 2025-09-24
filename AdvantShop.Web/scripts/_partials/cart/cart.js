import helpTriggerModule from '../../../Areas/Admin/Content/src/_partials/help-trigger/helpTrigger.module.js';
import '../../../Areas/Admin/Content/src/_shared/is-mobile/is-mobile.js';
import '../../../Areas/Admin/Templates/Mobile/Content/vendors/ui-bootstrap/angular-popover-decorator/angular-popover-decorator.js';
import customOptionsModule from '../custom-options/customOptions.module.js';
import { cartAddConfigDefault } from './cartConfigDefault.ts';

(function (ng) {
    'use strict';

    angular.module('cart', ['isMobile', helpTriggerModule, customOptionsModule]).constant('cartConfig', cartAddConfigDefault);
})(window.angular);
