import appDependency from '../../../../../scripts/appDependency.js';

import 'ng-file-upload';
import 'angular-timeago';

import '../../../Content/src/tasks/tasks.js';
import '../../../Content/src/tasks/task-create.ctrl.js';
import '../../../Content/src/tasks/task-create.component.js';
import '../../../Content/src/tasks/tasks.service.js';

import '../../../Content/src/tasks/styles/tasks.scss';

import '../../../Content/src/tasks/modal/changeTaskGroup/ModalChangeTaskGroupCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskStatus/ModalChangeTaskStatusCtrl.js';
import '../../../Content/src/tasks/modal/changeTaskStatuses/ModalChangeTaskStatusesCtrl.js';
import '../../../Content/src/tasks/modal/completeTask/ModalCompleteTaskCtrl.js';
import '../../../Content/src/tasks/modal/editTask/ModalEditTaskCtrl.js';
import '../../../Content/src/tasks/modal/finishTask/ModalFinishTaskCtrl.js';
import '../../../Content/src/tasks/modal/editTaskObserver/ModalEditTaskObserverCtrl.js';

import '../../../Content/src/taskgroups/taskgroups.js';
import '../../../Content/src/taskgroups/modal/ModalAddEditTaskGroupCtrl.js';
import '../../../Content/src/taskgroups/modal/copyTaskGroup/ModalCopyTaskGroupCtrl.js';
import '../../../Content/src/taskgroups/projectStatuses/projectStatuses.js';
import '../../../Content/src/taskgroups/projectStatuses/modal/ModalEditProjectStatusCtrl.js';

import '../../../Content/src/_shared/modal/addTask/ModalAddTaskCtrl.js';

import '../../../Content/src/_partials/admin-comments/adminComments.js';
appDependency.addItem('adminComments');
import '../../../Content/src/_partials/admin-comments/adminCommentsForm.js';
appDependency.addItem('adminCommentsForm');
import '../../../Content/src/_partials/admin-comments/adminCommentsItem.js';
appDependency.addItem('adminCommentsItem');
import '../../../Content/src/_partials/admin-comments/adminComments.component.js';
import '../../../Content/src/_partials/admin-comments/adminComments.service.js';

import '../../../../../scripts/_common/modal/modal.module.js';

import '../../../Content/src/_shared/kanban/kanban.module.js';
appDependency.addItem('kanban');

import '../../../Content/src/_shared/file-uploader/fileUploader.js';
import '../../../Content/src/_shared/file-uploader/fileUploader.component.js';
import '../../../Content/src/_shared/file-uploader/modal/fileUploaderModal.js';

import '../../../Content/src/lead/lead.js';
import '../../../Content/src/lead/lead.service.js';
import '../../../Content/src/lead/components/leadItemsSummary/leadItemsSummary.js';
import '../Content/styles/_shared/order-items-summary/order-items-summary.scss';
appDependency.addItem('lead');

import '../../../Content/src/_partials/leadEvents/leadEvents.js';
import '../../../Content/src/_partials/leadEvents/modals/addEditCallComent/ModalAddEditCallComentCtrl.js';
import '../../../Content/src/_partials/leadEvents/modals/showEmail/ModalShowEmailCtrl.js';
appDependency.addItem('leadEvents');

import '../../../Content/src/calls/components/callRecord/callRecord.js';
appDependency.addItem('callRecord');

import '../Content/styles/views/settings.scss';
import '../Content/styles/_shared/chips/chips.scss';
import '../Content/styles/_shared/card/card.scss';
import '../Content/src/_shared/swipe-line/swipe-line.module.js';
import '../Content/styles/views/tasks.scss';

import '../Content/vendors/ng-sortable-custom/ng-sortable.module.js';
appDependency.addItem(`as.sortable`);

import '../../../Content/src/_partials/sidebar-user/sidebarUser.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.service.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUser.component.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.js';
import '../../../Content/src/_partials/sidebar-user/sidebarUserTrigger.component.js';
import '../Content/src/_partials/sidebar-user/styles/sidebar-user.scss';
appDependency.addItem('sidebarUser');

import 'tinycolor2';

import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.cjs';
import '../../../Content/vendors/angular-color-picker/angularjs-color-picker.css';
import '../../../Content/vendors/angular-color-picker/themes/angularjs-color-picker-bootstrap.css';
import '../Content/styles/_shared/color-picker/color-picker.scss';

import scrollIntoViewModule from '../../../../../scripts/_common/scroll-into-view/scroll-into-view.js';
appDependency.addItem(scrollIntoViewModule);

appDependency.addItem('tasks');
appDependency.addItem('taskgroups');
appDependency.addItem('color.picker');
appDependency.addItem(`swipeLine`);
