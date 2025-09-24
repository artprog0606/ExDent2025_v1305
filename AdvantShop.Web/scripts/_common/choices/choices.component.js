import { CUSTOM_EVENTS_MAP } from './choices.constants.js';

const eventsBindings = Object.keys(CUSTOM_EVENTS_MAP).reduce((prev, current) => Object.assign(prev, { [current]: '&' }), {});
export const ngChoices = {
    template: () => `<select></select>`,
    controller: 'NgChoicesCtrl',
    require: {
        ngModel: 'ngModel',
    },
    transclude: {
        ngChoicesItemTemplate: '?ngChoicesItemTemplate',
        ngChoicesChoiceTemplate: '?ngChoicesChoiceTemplate',
    },
    bindings: Object.assign(
        {
            choices: '<',
            //how mapping option
            valueKey: '<?',
            valueFn: '&',
            labelKey: '<?',
            labelFn: '&',
            disabledKey: '<?',
            disabledFn: '&',
            selectedKey: '<?',
            selectedFn: '&',
            //use for save non-primitive in ngModel
            customPropertiesKey: '<?',
            customPropertiesFn: '&',
            //
            callbackOnInit: '&',
            sorter: '&',
            addItemText: '&',
            maxItemText: '&',
            valueComparer: '&',
            options: '<?',
        },
        eventsBindings,
    ),
};
