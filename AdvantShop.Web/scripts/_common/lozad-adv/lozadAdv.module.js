import LozadAdvCtrl from './lozadAdv.ctrl.ts';
import lozadAdvConstants from './lozadAdv.constants.ts';
import LozadAdv from './lozadAdv.directive.ts';
import LozadAdvService from './lozadAdv.service.ts';

const moduleName = `lozadAdv`;

angular
    .module(moduleName, [])
    .constant(`lozadAdvDefault`, lozadAdvConstants)
    .controller(`LozadAdvCtrl`, LozadAdvCtrl)
    .service('lozadAdvService', LozadAdvService)
    .directive(`lozadAdv`, LozadAdv);

export default moduleName;
