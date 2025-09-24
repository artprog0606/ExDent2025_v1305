import setCssCustomProps from './setCssCustomProps.directive.js';
import setCssCustomPropsService from './setCssCustomProps.service.js';

const MODULE_NAME = `setCssCustomProps`;
angular.module(MODULE_NAME, []).service(`setCssCustomPropsService`, setCssCustomPropsService).directive(`setCssCustomProps`, setCssCustomProps);

export default MODULE_NAME;
