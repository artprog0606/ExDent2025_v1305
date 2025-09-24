const isTouchDevice = 'ontouchstart' in document.documentElement;

class RootMenuCtrl {
    /* @ngInject */
    constructor($element, submenuService) {
        this.$element = $element;
        this.submenuService = submenuService;
    }

    $postLink() {
        if (isTouchDevice === true) {
            this.$element[0].addEventListener(`click`, (event) => {
                if (this.$element.classList.contains(`active`) === false) {
                    event.preventDefault();
                }

                this.$element[0].classList.add(`active`);
            });

            this.$element[0].addEventListener(`mouseleave`, () => {
                this.$element[0].classList.remove(`active`);
            });
        }

        this.$element[0].addEventListener(`mouseenter`, () => {
            var otherActive = this.submenuService.closeAnotherMenu();

            if (otherActive != null) {
                otherActive.getBlockOrientation().style.zIndex = 0;
            }
        });
    }
}

export default RootMenuCtrl;
