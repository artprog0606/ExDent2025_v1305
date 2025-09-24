/* @ngInject */
function orderService($http) {
    var service = this;

    service.getOrders = function () {
        return $http.get('/myaccount/GetCustomerOrderHistory', { params: { rnd: Math.random() } }).then(function (response) {
            return response.data;
        });
    };

    service.getOrderDetails = function (ordernumber) {
        return $http.get('/myaccount/GetOrderDetails', { params: { ordernumber: ordernumber, rnd: Math.random() } }).then(function (response) {
            return response.data;
        });
    };

    service.cancelOrder = function (ordernumber) {
        return $http.post('/myaccount/CancelOrder', { ordernumber: ordernumber, rnd: Math.random() }).then(function (response) {
            return response.data;
        });
    };

    service.changePaymentMethod = function (ordernumber, paymentId) {
        return $http
            .post('/myaccount/ChangePaymentMethod', {
                ordernumber: ordernumber,
                paymentId: paymentId,
                rnd: Math.random(),
            })
            .then(function (response) {
                return response.data;
            });
    };

    service.changeOrderComment = function (ordernumber, customercomment) {
        return $http
            .post('/myaccount/ChangeOrderComment', {
                ordernumber: ordernumber,
                customercomment: customercomment,
                rnd: Math.random(),
            })
            .then(function (response) {
                return response.data;
            });
    };

    service.getOrderReview = function (orderNumber) {
        return $http.get('/myaccount/GetOrderReview', { params: { orderNumber: orderNumber } }).then(function (response) {
            return response.data;
        });
    };

    service.addOrderReview = function (orderNumber, ratio, text) {
        return $http
            .post('/myaccount/AddOrderReview', {
                orderNumber: orderNumber,
                ratio: ratio,
                text: text,
                rnd: Math.random(),
            })
            .then(function (response) {
                return response.data;
            });
    };
}

export default orderService;
