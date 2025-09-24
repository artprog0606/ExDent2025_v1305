import './styles/compare.scss';

import CompareCtrl from './controllers/compareController';
import CompareCountCtrl from './controllers/compareCountController';
import { compareControlDirective, compareCountDirective, compareRemoveAllDirective, compareRemoveDirective } from './directives/compareDirectives';

import { CompareService } from './services/compareService';
import type { Camelize } from '../../@types/generics';

export type OfferIdType = number;
export type CompareCountType = Camelize<CompareCountResponseType>;
export type CompareCountResponseType = {
    Count: number;
    isComplete: boolean;
};

export default angular
    .module('compare', [])
    .controller('CompareCtrl', CompareCtrl)
    .controller('CompareCountCtrl', CompareCountCtrl)
    .service('compareService', CompareService)
    .directive('compareControl', compareControlDirective)
    .directive('compareCount', compareCountDirective)
    .directive('compareRemoveAll', compareRemoveAllDirective)
    .directive('compareRemove', compareRemoveDirective).name;
