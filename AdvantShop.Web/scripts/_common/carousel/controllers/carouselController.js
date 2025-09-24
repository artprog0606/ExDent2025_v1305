var CarouselCtrl = function ($element, $scope, $q, carouselService, $timeout) {
    var ctrl = this;
    var carouselImgList = {};
    var deferList = [];

    ctrl.init = function () {
        var element = $element[0];
        return carouselService.waitLoadImages(element.querySelectorAll('img'), ctrl.carouselOptions).then(function () {
            setTimeout(function () {
                var carouselEl = element;

                if (ctrl.initilazeTo != null) {
                    carouselEl = carouselEl.querySelector(ctrl.initilazeTo);
                }
                /* eslint-disable no-undef */
                ctrl.carouselNative = new Carousel(carouselEl, ctrl.carouselOptions).init();

                if (deferList.length > 0) {
                    deferList.forEach(function (item) {
                        item.resolve(ctrl);
                    });
                }

                $scope.$digest();
            }, 0);
        });
    };

    ctrl.addCarouselImg = function (carouselImg) {
        var id = ctrl.generateCarouselImgId();
        carouselImgList[id] = carouselImg;
        return id;
    };

    ctrl.callFnFromCarouselImg = function (img, carouselItem) {
        var id = img.dataset.carouselImgId;
        if (carouselImgList[id] != null) {
            carouselImgList[id].callback();
        }
    };

    ctrl.generateCarouselImgId = function () {
        return 'carouselImgId_' + Math.random();
    };

    ctrl.whenCarouselInit = function () {
        var defer = $q.defer();
        if (ctrl.carouselNative == null) {
            deferList.push(defer);
        } else {
            defer.resolve(ctrl);
        }

        return defer.promise;
    };
};

CarouselCtrl.$inject = ['$element', '$scope', '$q', 'carouselService', '$timeout'];

export default CarouselCtrl;
