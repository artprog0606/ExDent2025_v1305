import './dropdown.scss';
import Dropdown from './dropdown.js';
import ListenerDocumentClick from './listenerDocumentClick.js';
import DropdownList from './dropdownList.js';
import DropdownSelected from './dropdownSelected.js';
import dropdownService from './dropdownService.js';

export default class VanillaDropdown {
    constructor(element, options) {
        this.element = element;
        this.options = options;
        this.listenerDocumentClick = new ListenerDocumentClick();
        this.dropdown = new Dropdown(this.element);
        this.dropdownService = new dropdownService();
        this.dropdownService.addDropdown(this.dropdown);
        this.init();
        return this;
    }

    init() {
        this.dropdown.preOpen = () => {
            const activeDropdown = this.dropdownService.getActiveDropdown();
            if (activeDropdown != null && activeDropdown != this.dropdown) {
                activeDropdown.close();
            }
            this.dropdownService.setActiveDropdown(this.dropdown);
        };

        this.dropdown.onOpen = () => {
            this.listenerDocumentClick._bind();
            this.listenerDocumentClick.onEmmit = () => {
                this.dropdown.close();
            };
        };

        const dropdownListElement = this.element.querySelector(`vanilla-dropdown-list`);
        const dropdownSelectedElement = this.element.querySelector(`vanilla-dropdown-selected`);

        const dropdownSelectedInstance = new DropdownSelected(dropdownSelectedElement);
        const dropdownListInstance = new DropdownList(dropdownListElement);

        dropdownListInstance.onChange = (e, value) => {
            dropdownSelectedInstance.update(e, value);
            this.listenerDocumentClick.emmit(e);
        };
    }
}
