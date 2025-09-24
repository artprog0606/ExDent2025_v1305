class carouselExtTrackCtrl {
    /*@ngInject*/
    constructor($attrs, $element, $scope) {
        this.$attrs = $attrs;
        this.$scope = $scope;
        this.el = $element[0];
    }

    $postLink() {
        this.$attrs.$set('data-glide-el', 'track');
    }
}

export default carouselExtTrackCtrl;
