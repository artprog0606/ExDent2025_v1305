/* @ngInject */
function clientCodeService($http, urlHelper, $sce, $q) {
    var service = this;

    service.getClientCode = function () {
        return $http.get(urlHelper.getAbsUrl('user/getclientcode', true)).then(function (response) {
            if (response.data.result === true) {
                return $q.resolve(response.data.obj);
            } else {
                return $q.reject(response.data.errors);
            }
        });
    };

    service.trustClientCode = function (clientCode) {
        if (clientCode.Code != null && typeof clientCode.Code === 'string') {
            clientCode.Code = $sce.trustAsHtml(clientCode.Code);
        }

        return clientCode;
    };
}

export default clientCodeService;
