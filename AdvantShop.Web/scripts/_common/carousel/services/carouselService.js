var carouselService = function ($q) {
    var service = this;

    var imageLoad = function (imageSrc) {
        var deferItem = $q.defer(),
            imageFake = new Image();

        if (imageSrc == null) return deferItem.promise;

        imageFake.addEventListener('load', function () {
            deferItem.resolve(true);
        });

        imageFake.addEventListener('error', function () {
            deferItem.resolve();
        });

        imageFake.src = imageSrc;

        return deferItem.promise;
    };

    var checkNeedLoad = function (image) {
        return !image.complete || typeof image.naturalWidth === 'undefined' || image.naturalWidth === 0;
    };

    service.waitLoadImages = function (images, carouselOptions) {
        var deferMain = $q.defer(),
            promises = [];

        var countLoadImageInit = carouselOptions.visibleMax;
        var countLoadImage;

        if (countLoadImageInit != null) {
            countLoadImage = images.length - 1 <= countLoadImageInit ? images.length - 1 : countLoadImageInit;
        } else {
            countLoadImage = images.length - 1;
        }

        for (var i = 0; i <= countLoadImage; i++) {
            if (checkNeedLoad(images[i]) === true) {
                promises.push(imageLoad(images[i].src || images[i].dataset.src));
            }

            if ((images[i].src == null || images[i].src.length === 0) && images[i].dataset.src != null && images[i].dataset.src.length > 0) {
                images[i].src = images[i].dataset.src;
            }
        }

        if (carouselOptions.auto === true && countLoadImageInit != null) {
            for (var k = images.length - 1; k >= images.length - countLoadImageInit; k--) {
                if (checkNeedLoad(images[k]) === true) {
                    promises.push(imageLoad(images[k].src || images[k].dataset.src));
                }

                if ((images[k].src == null || images[k].src.length === 0) && images[k].dataset.src != null && images[k].dataset.src.length > 0) {
                    images[k].src = images[k].dataset.src;
                }
            }
        }

        if (promises.length === 0) {
            promises.push(deferMain.promise);
            deferMain.resolve();
        }

        return $q.all(promises);
    };
};

carouselService.$inject = ['$q'];

export default carouselService;
