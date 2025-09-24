export default class Dropdown {
    constructor(element) {
        this._element = element;
        this.triggerElement =
            this._element.querySelector(`[data-vanilla-dropdown-trigger]`) || this._element.querySelector(`[vanilla-dropdown-trigger]`);
        this._bind();
        this.isOpen = false;
    }

    open(e) {
        e.stopPropagation();
        this.isOpen = true;
        this.preOpen();
        this._element.classList.add(`is-open`);
        this.onOpen();
    }

    close(e) {
        this.isOpen = false;
        this._element.classList.remove(`is-open`);
        this.onClose();
    }

    toggle(e) {
        this.isOpen ? this.close(e) : this.open(e);
    }

    preOpen() {}

    onOpen() {} //callback

    onClose() {} //callback

    _bind() {
        if (this.triggerElement != null) {
            this.triggerElement.addEventListener(`click`, (e) => this.toggle(e));
        }
    }
}
