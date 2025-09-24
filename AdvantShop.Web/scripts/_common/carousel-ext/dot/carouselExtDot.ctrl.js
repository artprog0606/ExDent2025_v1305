class carouselExtDotCtrl {
    /*@ngInject*/
    constructor($attrs, $element, $scope) {
        this.$attrs = $attrs;
        this.$scope = $scope;
        this.el = $element[0];
    }

    $postLink() {
        this.$attrs.$set('data-glide-el', 'controls[nav]');
        let massiveSlides = this.carouselExtRoot.massiveSlides;
        let nodeElemsDot = '';

        if (massiveSlides.length > 0) {
            for (let i = 0; i < this.carouselExtRoot.massiveSlides.length; i++) {
                nodeElemsDot += `<button class="glide__bullet carousel-ext__dot-item" data-glide-dir="=${i}"></button>`;
            }
            this.el.innerHTML = nodeElemsDot;
        }
    }
}

export default carouselExtDotCtrl;
