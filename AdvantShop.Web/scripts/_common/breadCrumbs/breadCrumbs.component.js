function breadCrumbsDirective() {
    return {
        scope: true,
        controller: `BreadCrumbsCtrl`,
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            const breadCrumbsItems = element[0].children;
            const breadCrumbsItemsArr = Array.prototype.slice.call(breadCrumbsItems);

            const getIndentItem = (el) => {
                const styles = window.getComputedStyle(el, null);
                return {
                    marginLeft: parseFloat(styles.getPropertyValue(`margin-left`)),
                    marginRight: parseFloat(styles.getPropertyValue(`margin-right`)),
                };
            };

            const getSummOfIndent = (numbersArr) => {
                return numbersArr.reduce((summ, curr) => {
                    return summ + curr;
                }, 0);
            };

            if (breadCrumbsItemsArr != null && breadCrumbsItemsArr.length > 1) {
                const lastChild = breadCrumbsItemsArr[breadCrumbsItemsArr.length - 1];
                const penulItem = breadCrumbsItemsArr[breadCrumbsItemsArr.length - 2];

                if (penulItem != null) {
                    const lastChildCoord = lastChild ? lastChild.getBoundingClientRect() : 0;
                    const penulItemCoord = penulItem ? penulItem.getBoundingClientRect() : 0;
                    const penulChildIndent = getIndentItem(penulItem);
                    const lastChildIndent = getIndentItem(lastChild);
                    const commonIndentArr = [].concat(Object.values(penulChildIndent), Object.values(lastChildIndent));
                    const summOfIndent = getSummOfIndent(commonIndentArr);
                    element[0].scrollTo(element[0].scrollWidth - (lastChildCoord.width + penulItemCoord.width + summOfIndent), 0);
                } else {
                    element[0].scrollTo(element[0].scrollWidth, 0);
                }
            }
        },
    };
}

export { breadCrumbsDirective };
