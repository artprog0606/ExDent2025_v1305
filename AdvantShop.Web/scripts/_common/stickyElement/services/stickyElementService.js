const storage = new Map();

const listener = (event) => {
    const scrollY = window.scrollY;
    let isPinned = false;
    let diffHeight = 0;

    for (let [point, { items, prevState }] of storage) {
        //если на предыдущем шаге отловили момент, когда елемент после смены состояния изменил высоту
        if (diffHeight !== 0) {
            items.forEach(({ element }) => {
                element.style.top = diffHeight + Number(element.style.top.replace('px', '')) + 'px';
            });
        }

        isPinned = scrollY > point;
        if (prevState !== isPinned) {
            let elementOldHeight,
                elementNewHeight,
                diffHeightCurrentItem = 0;
            items.forEach(({ element, callback }) => {
                elementOldHeight = element.offsetHeight;
                callback(isPinned);
                elementNewHeight = element.offsetHeight;
                //проверка елемента после смены состояния на изменение высоты
                if (elementOldHeight !== elementNewHeight) {
                    diffHeightCurrentItem = elementNewHeight - elementOldHeight;
                }
            });
            prevState = isPinned;
            storage.get(point).prevState = prevState;
            diffHeight += diffHeightCurrentItem;
        }
    }
};

export default class StickyElementService {
    constructor() {
        this.isInit = false;
    }
    createListener() {
        if (this.isInit === false) {
            window.addEventListener('scroll', listener, { passive: true });
        }
    }
    parseValue(elementHeight, value) {
        let val = null;
        if (value === 'bottom-self') {
            val = window.innerHeight > elementHeight ? 0 : window.innerHeight - elementHeight;
        } else if (value != null) {
            value = val;
        }
        return val;
    }
    calcValue(element, sumTopSticky) {
        let propName = 'top';
        let val = null;
        if (element.dataset.top != null) {
            val = this.parseValue(
                element.offsetHeight + sumTopSticky + (element.dataset.offset ? parseFloat(element.dataset.offset) : 0),
                element.dataset.top,
            );
        }

        return [propName, val == null || val === 0 ? sumTopSticky : val];
    }
    addElementToObserver(element, callback) {
        if (this.isInit === false) {
            this.createListener();
        }
        const point = element.getBoundingClientRect().top + window.scrollY;

        this.setPointValue(point, element);

        this.setState(point, {
            items: this.hasItem(point) ? this.getState(point).items.concat([{ element, callback }]) : [{ element, callback }],
        });
        return () => {
            const items = this.getState(point).items;
            const index = items.findIndex((x) => x.element === element);
            if (index !== -1) {
                items.splice(index, 1);
            }
        };
    }

    setPointValue(point, element) {
        const sumTopSticky = this.getSumTopSticky(point);
        const propData = this.calcValue(element, sumTopSticky);

        if (propData != null) {
            element.style[propData[0]] = propData[1] + 'px';
        }
    }
    getSumTopSticky(pointLimit) {
        let sum = 0;

        for (const [point, { items }] of storage) {
            if (point <= pointLimit) {
                sum += items.reduce((acc, cur) => (acc += cur.element.offsetHeight), 0);
            } else {
                break;
            }
        }

        return sum;
    }
    setState(key, value) {
        storage.set(key, value);
    }
    getState(key) {
        return storage.get(key);
    }
    hasItem(key) {
        return storage.get(key);
    }
}
