class carouselExtNavCtrl {
    /*@ngInject*/
    constructor($attrs, $element, $scope) {
        this.$attrs = $attrs;
        this.$scope = $scope;
        this.el = $element[0];
    }

    $postLink() {
        this.$attrs.$set('data-glide-el', 'controls');
    }
}

export default carouselExtNavCtrl;
