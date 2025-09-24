(function (ng) {
    'use strict';

    var OkMainCtrl = function () {
        var ctrl = this;

        ctrl.initOkChannel = function (okChannel) {
            ctrl.okChannel = okChannel;
        };

        ctrl.refreshChild = function (childName) {
            if (ctrl.okChannel.childs[childName] != null && ctrl.okChannel.childs[childName].refresh != null) {
                ctrl.okChannel.childs[childName].refresh();
            }
        };
    };

    ng.module('okMain', []).controller('OkMainCtrl', OkMainCtrl);
})(window.angular);
