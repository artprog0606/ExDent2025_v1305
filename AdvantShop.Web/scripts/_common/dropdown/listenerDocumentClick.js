let instance = null;
export default class ListenerDocumentClick {
    constructor() {
        if (!instance) {
            instance = this;
        }
        return instance;
    }

    emmit(e) {
        //e.preventDefault();
        this._unbind();
        if (this.onEmmit != null) {
            this.onEmmit();
        }
    }

    onEmmit() {}

    _bind() {
        document.addEventListener(`click`, this);
    }

    _unbind() {
        document.removeEventListener(`click`, this);
    }

    handleEvent(e) {
        this.emmit(e);
    }
}
