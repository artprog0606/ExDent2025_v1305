/* @ngInject */
function VideosCtrl($http, $sce, $timeout) {
    var ctrl = this;

    ctrl.$onInit = function () {
        ctrl.getVideos();
    };

    ctrl.getVideos = function () {
        return $http.get('productExt/getvideos', { params: { productId: ctrl.productId } }).then(function (response) {
            ctrl.videos = response.data;

            for (var i = 0; i < ctrl.videos.length; i++) {
                ctrl.videos[i].PlayerCode = $sce.trustAsHtml(ctrl.videos[i].PlayerCode);
            }

            if (ctrl.onReceive != null) {
                $timeout(function () {
                    ctrl.onReceive({});
                });
            }

            return response.data;
        });
    };
}

export default VideosCtrl;
