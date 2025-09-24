import type { ICompareService } from '../services/compareService';
import { IAttributes, IParseService, IScope, IDirectiveFactory, IDirective } from 'angular';
import type { ICompareCtrl } from '../controllers/compareController';
import type { ICompareCountCtrl } from '../controllers/compareCountController';

/* @ngInject */
function compareControlDirective(compareService: ICompareService, $parse: IParseService): IDirective<IScope, JQLite, IAttributes, ICompareCtrl> {
    return {
        restrict: 'A',
        scope: true,
        controller: 'CompareCtrl',
        controllerAs: 'compare',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            if (ctrl) {
                ctrl.dirty = true;
                const offerId: number = $parse(attrs.compareControl)(scope);
                ctrl.offerId = offerId;
                if (ctrl.offerId != null) {
                    compareService.addCompareScope(offerId, ctrl);
                }
                scope.$on('$destroy', function () {
                    compareService.removeCompareScope(offerId, ctrl);
                });
            }
        },
    };
}

function compareCountDirective(): IDirective<IScope, JQLite, IAttributes, ICompareCountCtrl> {
    return {
        restrict: 'A',
        scope: true,
        controller: 'CompareCountCtrl',
        controllerAs: 'compareCount',
        bindToController: true,
        link: function (scope, element, attrs, ctrl) {
            if (ctrl) {
                ctrl.countObj.count = parseInt(attrs.startCount, 10);
            }
        },
    };
}

/* @ngInject */
function compareRemoveAllDirective(compareService: ICompareService): IDirective<IScope, JQLite, IAttributes> {
    return {
        restrict: 'A',
        scope: true,
        link: function (scope, element, attrs) {
            element.on('click', async function (event) {
                event.preventDefault();
                await compareService.removeAll();
            });
        },
    };
}

/* @ngInject */
function compareRemoveDirective(compareService: ICompareService): IDirective<IScope, JQLite, IAttributes> {
    return {
        restrict: 'A',
        scope: true,
        link: function (scope: IScope, element: JQLite, attrs: IAttributes) {
            element.on('click', async function (event: JQueryEventObject) {
                event.preventDefault();
                await compareService.remove(attrs.compareRemove, false);
            });
        },
    };
}

export { compareControlDirective, compareCountDirective, compareRemoveAllDirective, compareRemoveDirective };
