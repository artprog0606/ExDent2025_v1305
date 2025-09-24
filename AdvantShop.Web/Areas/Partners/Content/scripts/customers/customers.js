(function (ng) {
    'use strict';

    var CustomersCtrl = function ($http, toaster) {
        var ctrl = this;
    };

    CustomersCtrl.$inject = ['$http', 'toaster'];

    ng.module('customers', []).controller('CustomersCtrl', CustomersCtrl);
})(window.angular);
