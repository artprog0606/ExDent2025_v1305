class carouselExtCtrl {
    /*@ngInject*/
    constructor($attrs, $element, $scope) {
        this.$attrs = $attrs;
        this.$scope = $scope;
        this.el = $element[0];
    }

    $onInit() {
        this.carouselExtRoot.addSlide(this);
    }
}

export default carouselExtCtrl;
