import { LoadingOverlayCssClass } from '../loading/loading.module.js';

class ContentLoaderPlaceCtrl {
    /*@ngInject*/
    constructor($compile, $element, $scope, contentLoaderService) {
        this.$compile = $compile;
        this.$element = $element;
        this.$scope = $scope;
        this.contentLoaderService = contentLoaderService;
    }

    $onInit = () => this.contentLoaderService.addPlace(this);

    showLoading() {
        this.$element[0].classList.add(LoadingOverlayCssClass);
    }

    hideLoading() {
        this.$element[0].classList.remove(LoadingOverlayCssClass);
    }

    setContent(content) {
        this.$element.html(content);
        this.$compile(this.$element[0].children)(this.$scope);
    }
}

export { ContentLoaderPlaceCtrl };
