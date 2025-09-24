export default class DropdownSelected {
    constructor(element) {
        this._element = element;
    }

    update(e, value) {
        this._element.innerHTML = ``;
        this._element.innerHTML = value;
    }

    onUpdate() {}
}
