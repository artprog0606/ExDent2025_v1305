/* @ngInject */
export const scrollSpyService = function ($rootScope, $document) {
    const service = this;
    const data = new Map();
    let activeTarget;
    let observer;
    let isStopWorking = false;

    const processItems = (entries) => {
        const dataItems = Array.from(data.entries());
        let result = null;
        for (let [currentTarget, currentItemData] of dataItems) {
            if (currentItemData.some((x) => x.scope.entry.isIntersecting === true)) {
                result = [currentTarget, currentItemData];
            }
        }

        if (result != null) {
            service.setActive(result[0]);
        }

        $rootScope.$apply();
    };

    service.setActive = (target) => {
        if (activeTarget != null) {
            data.get(activeTarget).forEach((x) => (x.scope.isActive = false));
        }

        activeTarget = target;

        data.get(target).forEach((current) => {
            current.scope.isActive = true;
            if (current.scope.options.alignHorizontal === true) {
                alignHorizontal(current.elementSpy);
            }
            if ($document[0].activeElement.dataset.scrollSpy != null && $document[0].activeElement !== current.elementSpy) {
                current.elementSpy.focus();
            }
        });
    };

    const alignHorizontal = (element) => {
        const parent = element.parentElement;
        const elWidth = element.offsetWidth;
        const parentCenter = parent.offsetWidth / 2;
        parent.scroll({ behavior: `smooth`, left: element.offsetLeft - parentCenter + elWidth / 2 });
    };

    service.addSpy = (elementSpy, elementTarget, spyCtrl) => {
        let timerId;
        if (observer == null) {
            observer = new IntersectionObserver((entries, observer) => {
                if (isStopWorking === true) {
                    return;
                }

                for (let entry of entries) {
                    data.get(entry.target).forEach((x) => {
                        x.scope.entry = entry;
                    });
                }

                if (timerId != null) {
                    clearTimeout(timerId);
                }
                timerId = setTimeout(() => processItems(entries), 300);
            }, spyCtrl.options.observe);
        }

        if (data.has(elementTarget)) {
            data.get(elementTarget).push({ elementSpy, scope: spyCtrl });
        } else {
            data.set(elementTarget, [{ elementSpy, scope: spyCtrl }]);
        }

        observer.observe(elementTarget);

        return () => observer.unobserve(elementTarget);
    };

    service.activate = () => (isStopWorking = false);
    service.deactivate = () => (isStopWorking = true);
};
