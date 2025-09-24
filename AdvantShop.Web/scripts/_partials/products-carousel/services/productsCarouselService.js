/* @ngInject */
function productsCarouselService($http) {
    var service = this;

    service.getData = function (ids, title, type, visibleItems, carouselResponsive) {
        return $http
            .post('catalog/productsbyIds', {
                ids: ids,
                title: title,
                type: type,
                visibleItems: visibleItems,
                enabledCarousel: true,
                carouselResponsive: toMvcDictionary(carouselResponsive),
            })
            .then(function (response) {
                return response.data;
            });
    };

    function toMvcDictionary(carouselResponsive) {
        var result = [];
        if (carouselResponsive != null) {
            Object.keys(carouselResponsive).forEach(function (key) {
                result.push({
                    key: key,
                    value: carouselResponsive[key],
                });
            });
        }

        return result;
    }
}

export default productsCarouselService;
