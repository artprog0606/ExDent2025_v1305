class SetCssCustomPropsService {
    static $inject = [];
    #root = document.querySelector(`:root`);

    getValuePropertyElement = (el, property) => {
        const computedStyleElement = getComputedStyle(el);
        return computedStyleElement[property];
    };

    getValueFromMapAndSetProperty = (el, objMap, computedStyleDocument) => {
        for (const [nameCssProperty, nameProperty] of Object.entries(objMap)) {
            this.setCustomProperty(el, nameCssProperty, nameProperty, computedStyleDocument);
        }
    };

    setCustomProperty = (el, name, value, computedStyleDocument) => {
        const nameCustomProperty = `--${name}`;
        const prevValueCssProperty = computedStyleDocument.getPropertyValue(nameCustomProperty);
        if (!prevValueCssProperty) {
            if (typeof value === `function`) {
                this.#root.style.setProperty(nameCustomProperty, `${value()}px`);
            } else {
                const valueProperty = this.getValuePropertyElement(el, value);
                if (valueProperty) {
                    this.#root.style.setProperty(nameCustomProperty, valueProperty);
                }
            }
        } else {
            console.error(`Custom css property with name ${nameCustomProperty} already in use`);
        }
    };

    sum = (el, args) => () => {
        return args.reduce((sum, it) => {
            const value = this.getValuePropertyElement(el, it);
            if (value) {
                return parseFloat(value) + sum;
            }
            return sum;
        }, 0);
    };
}

export default SetCssCustomPropsService;
