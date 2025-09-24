function ratingDirective() {
    return {
        require: ['rating', '?ngModel'],
        restrict: 'A',
        scope: true,
        controller: 'RatingCtrl',
        controllerAs: 'rating',
        bindToController: true,
        link: function (scope, element, attrs, ctrls) {
            const rating = ctrls[0];
            const ngModel = ctrls[1];
            const childs = element[0].children;

            rating.max = parseInt(attrs.max) || 5;
            rating.readonly = attrs.readonly != null ? attrs.readonly === 'true' : false;

            if (rating.readonly) {
                element[0].classList.add(`rating-readonly`);
            }

            rating.current = parseInt(attrs.current);
            rating.url = attrs.url;
            rating.objId = attrs.objId;
            rating.rateBinding = attrs.rateBinding;

            for (var i = 0; i <= childs.length - 1; i++) {
                childs[i].setAttribute(`data-index`, childs.length - i);
                rating.items[i] = {
                    isSelected: rating.current - 1 < i,
                };
            }

            if (ngModel != null) {
                if (isNaN(ngModel.$modelValue) && isNaN(rating.current) === false && rating.current > 0) {
                    ngModel.$setViewValue(rating.current);
                }
                ngModel.$render = () => {
                    if (isNaN(ngModel.$modelValue) === false) {
                        rating.current = ngModel.$modelValue;
                    }
                };
            }

            if (rating.readonly === false) {
                element[0].addEventListener('click', function (event) {
                    const item = event.target.closest('.rating-item');
                    if (item != null) {
                        if (ngModel != null) {
                            ngModel.$setViewValue(parseInt(item.getAttribute('data-index')));
                        }
                        const promise = rating.select(parseInt(item.getAttribute('data-index')));
                        if (promise) {
                            promise.then(() => {
                                element[0].classList.add(`rating-readonly`);
                            });
                        } else {
                            scope.$apply();
                        }
                    }
                });
            }
        },
    };
}

export { ratingDirective };
