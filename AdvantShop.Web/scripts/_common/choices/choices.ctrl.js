import Choices from 'choices.js';

import { CUSTOM_EVENTS_MAP } from './choices.constants.js';

export default /* @ngInject */ function ($attrs, $element, $parse, $scope, choiceDefaultConfig, $transclude, $document) {
    const ctrl = this;
    let options,
        choicesObj,
        choicesOldVal,
        emptyOption,
        isInitialized = false;

    ctrl.$onInit = () => {
        validationOptionParams($attrs, 'label');
        validationOptionParams($attrs, 'value');
        ctrl.choiceItems = [];
        options = Object.assign({}, choiceDefaultConfig, ctrl.options || {});

        if ($attrs.callbackOnInit != null) {
            options.callbackOnInit = () => ctrl.callbackOnInit({ choices: choicesObj });
        }

        if ($attrs.callbackOnCreateTemplates != null) {
            options.callbackOnCreateTemplates = (template) => ctrl.callbackOnCreateTemplates({ template });
        }

        if ($attrs.sorter != null) {
            options.sorter = (a, b) => ctrl.sorter({ a, b });
        }

        if ($attrs.addItemText != null) {
            options.addItemText = (value) => ctrl.addItemText({ value });
        }

        if ($attrs.maxItemText != null) {
            options.maxItemText = (maxItemCount) => ctrl.maxItemText({ maxItemCount });
        }

        if ($attrs.valueComparer != null) {
            options.valueComparer = (value1, value2) => ctrl.valueComparer({ value1, value2 });
        }

        if ($transclude.isSlotFilled('ngChoicesItemTemplate') || $transclude.isSlotFilled('ngChoicesChoiceTemplate')) {
            options.allowHTML = true;
            const templateConfig = {};

            if ($transclude.isSlotFilled('ngChoicesItemTemplate')) {
                templateConfig.item = (...args) => {
                    const base = Choices.defaults.templates.item(...args);

                    return createTemplate($scope, $transclude, 'ngChoicesItemTemplate', base, args);
                };
            }

            if ($transclude.isSlotFilled('ngChoicesChoiceTemplate')) {
                templateConfig.choice = (...args) => {
                    const base = Choices.defaults.templates.choice(...args);
                    return createTemplate($scope, $transclude, 'ngChoicesChoiceTemplate', base, args);
                };
            }

            options.callbackOnCreateTemplates = () => {
                return templateConfig;
            };
        }
    };

    ctrl.$postLink = () => {
        const select = $element[0].querySelector('select');
        const modelValueSetter = $parse($attrs.ngModel).assign;
        const choiceCallback = (event) => {
            if (event.detail.disabled) {
                return;
            }
            modelValueSetter($scope.$parent, event.detail.customProperties || event.detail.value);
            $scope.$apply();
        };

        select.addEventListener('choice', choiceCallback);

        const classAddRemove = () => $document[0].documentElement.classList.toggle(options.classNamesCustom.rootOpenState);

        for (const eventKey in CUSTOM_EVENTS_MAP) {
            if ($attrs[eventKey] != null) {
                select.addEventListener(CUSTOM_EVENTS_MAP[eventKey], (...args) => {
                    ctrl[eventKey]({ ...args });
                });
            }
        }

        choicesObj = new Choices(select, options);

        $scope.$watch(
            () => ctrl.ngModel.$modelValue,
            (newVal, oldValue) => {
                if (ctrl.ngModel.$modelValue == null) {
                    emptyOption = emptyOption || {
                        label: 'Не выбрано',
                        value: '',
                        selected: true,
                    };
                    choicesObj.setChoices([emptyOption, ...ctrl.choiceItems], 'value', 'label', true);
                    return;
                }
                const itemSelected = ctrl.choiceItems.find((x) => x.customProperties === ctrl.ngModel.$modelValue);
                if (itemSelected != null) {
                    choicesObj.setChoiceByValue(itemSelected.value);
                }
            },
        );

        if (ctrl.choices != null) {
            ctrl.setChoices(ctrl.choices);
        }

        isInitialized = true;

        $element.on('$destroy', () => {
            choicesObj.destroy();
            select.removeEventListener('choice', choiceCallback);
        });
    };

    ctrl.$doCheck = () => {
        if (isInitialized && angular.equals(choicesOldVal, ctrl.choices) === false) {
            ctrl.setChoices(ctrl.choices);
            choicesOldVal = angular.copy(ctrl.choices);
        }
    };

    ctrl.setChoices = (data) => {
        ctrl.choiceItems.length = 0;
        let valueTemp, customPropertiesTemp, locals;
        for (const choicesDataItem of data) {
            locals = { choice: { customProperties: choicesDataItem } };
            valueTemp = ctrl.valueKey != null ? choicesDataItem[ctrl.valueKey] : ctrl.valueFn(locals);
            customPropertiesTemp = ctrl.customPropertiesKey != null ? choicesDataItem[ctrl.customPropertiesKey] : ctrl.customPropertiesFn(locals);
            ctrl.choiceItems.push({
                value: valueTemp,
                label: ctrl.labelKey != null ? choicesDataItem[ctrl.labelKey] : ctrl.labelFn(locals),
                disabled: ctrl.disabledKey != null ? choicesDataItem[ctrl.disabledKey] : ctrl.disabledFn(locals),
                customProperties: customPropertiesTemp,
                selected: angular.equals(ctrl.ngModel.$modelValue, customPropertiesTemp || valueTemp),
            });
        }
        choicesObj.setChoices(ctrl.choiceItems, 'value', 'label', true);
    };
}

const validationPostfix = ['Key', 'Fn'];
const validationOptionParams = ($attrs, baseProp) => {
    if (validationPostfix.every((x) => $attrs[baseProp + x] == null)) {
        throw new Error('Missing one of the options: ' + validationPostfix.map((x) => baseProp + x).join(' or '));
    }
};

const createTemplate = ($scope, $transclude, slotName, base, [config, choice, ...specificArgs]) => {
    const scopeChild = $scope.$parent.$new();
    scopeChild.config = config;
    scopeChild.choice = choice;
    const el = $transclude(
        scopeChild,
        (clone, scope) => {
            return clone;
        },
        null,
        slotName,
    );
    base.textContent = '';
    base.appendChild(el[0]);
    return base;
};
