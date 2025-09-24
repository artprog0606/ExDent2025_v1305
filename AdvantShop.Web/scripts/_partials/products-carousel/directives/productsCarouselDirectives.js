function productsCarouselDirective() {
    return {
        restrict: 'A',
        scope: {
            ids: '@',
            title: '@',
            type: '@',
            visibleItems: '@',
            carouselResponsive: '<?',
        },
        controller: 'ProductsCarouselCtrl',
        controllerAs: 'productsCarousel',
        bindToController: true,
    };
}

export { productsCarouselDirective };
