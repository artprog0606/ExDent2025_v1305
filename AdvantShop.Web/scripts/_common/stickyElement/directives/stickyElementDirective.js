const classNameActive = 'is-pinned';
const classNameInitialized = 'sticky-element--initialized';
/* @ngInject */
export default function StickyElement(stickyElementService) {
    return {
        restrict: 'A',
        /*attributes
            top: string - example: 10px , bottom-self
            offset: number
        */
        scope: true,
        link: function (scope, element, attrs) {
            if (attrs.top != null && attrs.bottom != null) {
                throw new Error('StickyElement: "top" and "bottom" mutually exclusive');
            }
            scope.stickyElement = { isPinned: false };

            const init = () => {
                const dereg = stickyElementService.addElementToObserver(element[0], (isPinned) => {
                    scope.stickyElement.isPinned = isPinned;
                    element[0].classList[isPinned ? 'add' : 'remove'](classNameActive);
                    scope.$digest();
                });

                element.on('$destroy', () => {
                    dereg();
                    element[0].classList.remove(classNameInitialized);
                });

                element[0].classList.add(classNameInitialized);
            };

            if (document.readyState != 'complete') {
                window.addEventListener('load', () => init());
            } else {
                init();
            }
        },
    };
}
