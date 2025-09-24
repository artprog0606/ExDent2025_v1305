import appDependency from '../../../../../scripts/appDependency.js';

import 'ng-file-upload';

import '../Content/styles/views/settings.scss';
import '../Content/styles/views/settings-list.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';
import 'tinycolor2';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../../../Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';

import '../Content/styles/_shared/color-picker/color-picker.scss';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';

import '../../../Content/src/settingsTasks/settingsTasks.js';
import '../../../Content/src/settingsTasks/modal/addEditRule/ModalAddEditRuleCtrl.js';
import '../../../Content/src/settingsTasks/modal/addEditFilterRule/ModalAddEditFilterRuleCtrl.js';
import '../../../Content/src/settingsTasks/modal/addEditManagerFilterRule/ModalAddEditManagerFilterRuleCtrl.js';

import '../../../Content/src/taskgroups/taskgroups.js';
import '../../../Content/src/taskgroups/modal/ModalAddEditTaskGroupCtrl.js';
import '../../../Content/src/taskgroups/modal/copyTaskGroup/ModalCopyTaskGroupCtrl.js';
import '../../../Content/src/taskgroups/projectStatuses/projectStatuses.js';
import '../../../Content/src/taskgroups/projectStatuses/modal/ModalEditProjectStatusCtrl.js';

import '../../../Content/src/tasks/tasks.js';
import '../../../Content/src/tasks/tasks.service.js';
import '../../../Content/src/tasks/modal/editTask/ModalEditTaskCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskStatuses/ModalChangeTaskStatusesCtrl.js';
import '../../../Content/src/tasks/modal/completeTask/ModalCompleteTaskCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskGroup/ModalChangeTaskGroupCtrl.js';
import '../../../Content/src/tasks/task-create.ctrl.js';
import '../../../Content/src/tasks/task-create.component.js';

import '../../../Content/src/_partials/admin-comments/adminComments.js';
import '../../../Content/src/_partials/admin-comments/adminCommentsForm.js';
import '../../../Content/src/_partials/admin-comments/adminCommentsItem.js';
import '../../../Content/src/_partials/admin-comments/adminComments.component.js';
import '../../../Content/src/_partials/admin-comments/adminComments.service.js';

import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.js';
import '../../../Content/src/_partials/products-selectvizr/productsSelectvizr.component.js';
import '../../../Content/src/_shared/modal/products-selectvizr/ModalProductsSelectvizrCtrl.js';
import '../../../Content/src/_shared/modal/selectCategories/ModalSelectCategoriesCtrl.js';
appDependency.addItem(`productsSelectvizr`);

import '../../../Content/src/_shared/ngClickCapture/ngClickCapture.directive.js';
appDependency.addItem(`ngClickCapture`);

import '../../../Content/src/_shared/modal/editableGridRow/ModalEditableGridRow.js';

import '../Content/src/_shared/product-grid-item/productGridItem.directive.js';
import '../Content/styles/_shared/product-item/product-item.scss';
appDependency.addItem(`productGridItem`);

appDependency.addItem(`swipeLine`);
appDependency.addItem('settingsTasks');
