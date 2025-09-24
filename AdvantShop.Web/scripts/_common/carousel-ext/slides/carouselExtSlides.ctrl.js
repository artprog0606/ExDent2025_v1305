class carouselExtSlidesCtrl {
    /*@ngInject*/
    constructor($attrs, $element, $scope) {
        this.$attrs = $attrs;
        this.$scope = $scope;
        this.el = $element[0];
    }

    $postLink() {
        if (this.generationChildren) {
            let listChilrenElem = this.el.children;
            let str = ``;
            for (let i = 0; i < listChilrenElem.length; i++) {
                str += `<carousel-ext-item class="glide__slide carousel-ext__slide">${listChilrenElem[i].outerHTML}</carousel-ext-item>`;
            }
            this.el.innerHTML = '';
            this.el.innerHTML = str;
        }
    }
}

export default carouselExtSlidesCtrl;
