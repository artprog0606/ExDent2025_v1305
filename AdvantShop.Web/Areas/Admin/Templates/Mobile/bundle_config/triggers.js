import appDependency from '../../../../../scripts/appDependency.js';

import '../../../Content/src/triggers/triggers.js';
import '../../../Content/src/triggers/triggers.service.js';
import '../../../Content/src/triggers/components/triggerEdit/triggerEdit.component.js';
import '../../../Content/src/triggers/components/triggerEdit/triggerEditCtrl.js';
import '../../../Content/src/triggers/modal/addEditFilterRule/ModalAddEditTriggerFilterRuleCtrl.js';
import '../../../Content/src/triggers/modal/addTrigger/ModalAddTriggerCtrl.js';
import '../../../Content/src/triggers/modal/addEditCategory/modalAddEditCategoryCtrl.js';
import '../../../Content/src/triggers/modal/changeTriggerName/ModalChangeTriggerName.js';
import '../../../Content/src/triggers/modal/triggerLetterKeys/ModalTriggerLetterKeysCtrl.js';
import '../../../Content/src/triggers/components/triggerActionEditField/triggerActionEditField.component.js';
import '../../../Content/src/triggers/components/triggerActionEditField/triggerActionEditFieldCtrl.js';
import '../../../Content/src/triggers/components/triggerActionSendRequest/triggerActionSendRequest.component.js';
import '../../../Content/src/triggers/components/triggerActionSendRequest/triggerActionSendRequestCtrl.js';
import '../../../Content/src/triggers/components/triggerActionSendNotification/triggerActionSendNotification.component.js';
import '../../../Content/src/triggers/components/triggerActionSendNotification/triggerActionSendNotificationCtrl.js';

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/_shared/chips/chips.scss';

import '../Content/src/_shared/swipe-line/swipe-line.module.js';

import '../../../Content/src/coupons/coupons.js';
import '../../../Content/src/coupons/modal/addEditCoupon/ModalAddEditCouponCtrl.js';
import '../../../Content/src/coupons/modal/coupon-products-selectvizr/ModalCouponProductsSelectvizrCtrl.js';

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/selectSmsTemplate/ModalSelectSmsTemplateCtrl.js';

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
import '../Content/styles/_shared/product-item/product-item.scss';
appDependency.addItem(`productGridItem`);

appDependency.addItem(`triggers`);
appDependency.addItem(`swipeLine`);
