/*@ngInject*/
var ShowMoreCtrl = function ($location, $http, $timeout, $element) {
    var ctrl = this,
        initHtmlContainer,
        html;

    ctrl.$onInit = function () {
        ctrl.page = $location.search() != null && $location.search().page != null ? parseInt($location.search().page, 10) : 1;
        ctrl.products = null;
        ctrl.isShowSpinner = false;
    };

    ctrl.getItems = function () {
        return $http.get(ctrl.requestUrl, { params: ctrl.dataParams }).then(function (response) {
            return response.data;
        });
    };

    ctrl.getMore = function () {
        ctrl.page += 1;

        ctrl.dataParams.page = ctrl.page;
        ctrl.dataParams.rnd = Math.random();
        ctrl.isShowSpinner = true;

        ctrl.dataParams.priceFrom === 0 ? null : ctrl.dataParams.priceFrom;

        if (ctrl.requestUrl != null && ctrl.requestUrl.length > 0) {
            ctrl.getItems().then(function (response) {
                $location.search(ctrl.urlParameter, ctrl.page);
                ctrl.pageYOffset = window.pageYOffset;
                ctrl.isShowSpinner = false;
                if (initHtmlContainer) {
                    initHtmlContainer.remove();
                }
                html = html + response;
                ctrl.products = html;
                ctrl.setScrollAfterShowMore();
            });
        }
    };

    ctrl.setScrollAfterShowMore = function () {
        $timeout(function () {
            window.scroll(window.pageXOffset, ctrl.pageYOffset);
        }, 0);
    };

    ctrl.addInitHtmlContainer = function (container, initHtml) {
        if (container != null) {
            initHtmlContainer = container;
            html = initHtml;
        }
    };
};

export default ShowMoreCtrl;
