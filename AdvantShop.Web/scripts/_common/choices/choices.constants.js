export const CUSTOM_EVENTS_MAP = {
    onAddItem: 'addItem',
    onRemoveItem: 'removeItem',
    onHighlightItem: 'highlightItem',
    onUnhighlightItem: 'unhighlightItem',
    onChoice: 'choice',
    onChange: 'change',
    onSearch: 'search',
    onShowDropdown: 'showDropdown',
    onHideDropdown: 'hideDropdown',
    onHighlightChoice: 'highlightChoice',
};

export const choiceDefaultConfig = {
    searchEnabled: false,
    shouldSort: false,
    classNames: {
        containerOuter: ['choices'],
        containerInner: ['cs-br-1', 'cs-t-1', 'form-control', 'form-select', 'input-alt'],
    },
};
