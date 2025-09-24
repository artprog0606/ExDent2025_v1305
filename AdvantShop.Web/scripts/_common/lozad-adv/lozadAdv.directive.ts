import {IAttributes, IDirective, IDirectiveFactory, IScope} from "angular";
import {ILozadAdvCtrl} from "./lozadAdv.ctrl";

interface LozadAdvDirective extends IDirectiveFactory<IScope, JQLite, IAttributes, ILozadAdvCtrl> {

}

const lozadAdv: LozadAdvDirective = function () {
    return {
        controller: 'LozadAdvCtrl',
        bindToController: true,
        controllerAs: 'lozadAdv',
        scope: true,
    };
}

export default lozadAdv
