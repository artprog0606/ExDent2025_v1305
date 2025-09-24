import appDependency from '../../../../../scripts/appDependency.js';
import '../Content/styles/views/settings-menu.scss';
import '../../../Content/src/menus/menus.js';
import '../../../Content/src/menus/components/menu-treeview/menuTreeview.js';
import '../../../Content/src/menus/components/menu-treeview/menuTreeview.component.js';
import '../../../Content/src/menus/components/menu-item-actions/menuItemActions.js';
import '../../../Content/src/menus/components/menu-item-actions/styles/menu-item-actions.scss';
import '../../../Content/src/menus/modal/changeParentMenuItem/ModalChangeParentMenuItemCtrl.js';
import '../../../Content/src/menus/modal/addEditMenuItem/ModalAddEditMenuItemCtrl.js';
import 'ng-file-upload';
appDependency.addItem(`ngFileUpload`);
appDependency.addItem(`menus`);

import '../../../Content/src/_shared/icon-move/iconMove.js';
import '../../../Content/src/_shared/icon-move/styles/icon-move.css';
appDependency.addItem(`iconMove`);

import '../Content/styles/_shared/picture-uploader/picture-uploader.scss';

import '../../../Content/src/_shared/modal/selectStaticPage/ModalSelectStaticPageCtrl.js';
import '../Content/styles/_shared/card/card.scss';
import '../Content/styles/views/settings.scss';

import '../../../Content/src/_shared/modal/selectNews/ModalSelectNewsCtrl.js';
