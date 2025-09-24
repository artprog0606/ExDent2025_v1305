/* @ngInject */
export default function TimesOfWorkCtrl($element, $translate) {
    var ctrl = this;

    ctrl.init = function () {};

    ctrl.onChange = function () {
        if (ctrl.onChangeFn) {
            ctrl.onChangeFn({ times: ctrl.times });
        }
    };

    ctrl.changeTime = function () {
        ctrl.onChange();
    };

    ctrl.changeDay = function () {
        ctrl.onChange();
    };

    ctrl.addTime = function () {
        if (!ctrl.times) {
            ctrl.times = [];
        }

        ctrl.times.push({});
        ctrl.onChange();
    };

    ctrl.removeTime = function (index) {
        ctrl.times.splice(index, 1);
        ctrl.onChange();
    };
}
