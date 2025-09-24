export default class DropdownList {
    constructor(element, dropdownSelected, dropdown) {
        this._element = element;
        this.dropdownSelected = dropdownSelected;
        this.boundEventHandler = this.select.bind(this);
        this._bind();
    }

    select(e) {
        e.stopPropagation();
        let value = e.target.closest(`vanilla-dropdown-value`) || e.target.querySelector(`vanilla-dropdown-value`) || e.target;
        this.onChange(e, value.innerHTML.trim());
    }

    onChange() {}

    get element() {
        return this._element;
    }

    _bind() {
        if (this._element != null) {
            this._element.addEventListener(`click`, this.boundEventHandler);
        }
    }

    _unbind() {
        if (this._element) {
            this._element.removeEventListener(`click`, this.boundEventHandler);
        }
    }
}
