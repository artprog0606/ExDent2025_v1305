import TimesOfWorkCtrl from './times-of-work.controller.js';
import { timesOfWorkComponent } from './times-of-work.component.js';

const MODULE_NAME = 'timesOfWork';

angular.module(MODULE_NAME, []).controller('TimesOfWorkCtrl', TimesOfWorkCtrl).component('timesOfWork', timesOfWorkComponent);

export default MODULE_NAME;
