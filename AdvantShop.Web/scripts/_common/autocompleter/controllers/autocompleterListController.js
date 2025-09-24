function AutocompleterListCtrl() {
    var ctrl = this;

    ctrl.listHover = false;

    ctrl.mouseenter = function () {
        ctrl.listHover = true;
    };

    ctrl.mouseleave = function () {
        ctrl.listHover = false;
    };

    ctrl.getStateHover = function () {
        return ctrl.listHover;
    };
}

export default AutocompleterListCtrl;
