import contentLoaderModule from '../../_shared/content-loader/content-loader.module.js';

import { MainMenu } from './mainMenu.ctrl.js';
import './main-menu.scss';

import '../../../../../../Content/src/_partials/top-panel-user/topPanelUser.js';
import '../../../../../../Content/src/settings/modal/addEditUser/ModalAddEditUserCtrl.js';
import '../../../../../../Content/src/_shared/modal/cropImage/ModalCropImageCtrl.js';
//
import '../../../../../../Content/src/_shared/modal/salesChannels/ModalSalesChannelsCtrl.js';
import '../../../../../../Content/src/_shared/modal/styles/simpleUiModal.scss';
import '../../../../../../Templates/AdminV3/Content/styles/partials/sales-channel/sales-channel.scss';
import '../../../../../../Templates/AdminV3/Content/styles/partials/sales-channel/card-channel.scss';
//
import '../../../styles/_partials/top-panel-user.scss';
import '../../../styles/_partials/top-panel-balance.scss';
import '../../../styles/_partials/message-balance.scss';
import '../../../styles/_shared/modal/simple-modal.scss';

import carousel from '../../../../../../../../scripts/_common/carousel/carousel.module.js';

const moduleName = `mainMenu`;

angular.module(moduleName, [contentLoaderModule, 'topPanelUser', 'modalSalesChannels', carousel]).controller(`mainMenuCtrl`, MainMenu);

export default moduleName;
