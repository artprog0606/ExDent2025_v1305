import '../../../node_modules/choices.js/public/assets/styles/choices.css';
import './choices.theme.scss';

import { ngChoices } from './choices.component.js';
import NgChoicesCtrl from './choices.ctrl.js';
import { choiceDefaultConfig as choiceDefaultConst } from './choices.constants.js';

const MODULE = 'ngChoices';

angular
    .module(MODULE, ['select'])
    .constant('choiceDefaultConfig', choiceDefaultConst)
    .controller('NgChoicesCtrl', NgChoicesCtrl)
    .component('ngChoices', ngChoices)
    .run(
        /* @ngInject */ function (choiceDefaultConfig, $translate) {
            choiceDefaultConfig.loadingText = $translate.instant('Js.Select.LoadingText');
            choiceDefaultConfig.noResultsText = $translate.instant('Js.Select.NoResultsText');
            choiceDefaultConfig.noChoicesText = $translate.instant('Js.Select.NoChoicesText');
            choiceDefaultConfig.itemSelectText = $translate.instant('Js.Select.ItemSelectText');
            choiceDefaultConfig.uniqueItemText = $translate.instant('Js.Select.UniqueItemText');
            choiceDefaultConfig.customAddItemText = $translate.instant('Js.Select.СustomAddItemText');
            choiceDefaultConfig.addItemText = (value) => $translate.instant('Js.Select.AddItemText', { value });
            choiceDefaultConfig.maxItemText = (maxItemCount) => $translate.instant('Js.Select.MaxItemText', { maxItemCount });
            choiceDefaultConfig.allowHTML = false;
        },
    );

export default MODULE;
