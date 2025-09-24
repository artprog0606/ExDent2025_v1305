/* @ngInject */
function checkOrderService($http) {
    var service = this;

    service.getStatus = function (orderNumber) {
        return $http.post('checkout/checkorder', { orderNumber: orderNumber, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };
}

export default checkOrderService;
