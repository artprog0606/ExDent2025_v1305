let instance = null;
export default class DropdownService {
    constructor() {
        if (!instance) {
            instance = this;
            this.dropdownList = new Set();
        }
        return instance;
    }

    getActiveDropdown() {
        return this.activeDropdown;
    }

    setActiveDropdown(activeDropdown) {
        this.activeDropdown = activeDropdown;
    }

    addDropdown(dropdown) {
        this.dropdownList.add(dropdown);
    }

    getDropdownList() {
        return this.dropdownList;
    }

    closeAllDropdowns() {
        for (let dropdown of this.dropdownList) {
            dropdown.close();
        }
    }
}
