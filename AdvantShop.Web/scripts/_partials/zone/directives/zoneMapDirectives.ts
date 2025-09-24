import zoneMap from '../templates/zoneMap.html';
import type { IZoneMapController } from '../controllers/zoneMapController';
import { IAttributes, IDirective, IScope } from 'angular';
import { IZoneSuggestionModalCtrl } from '../controllers/zoneSuggestionModalController';
export function zoneSuggestionModalDirective(): IDirective<IScope, JQLite, IAttributes, IZoneSuggestionModalCtrl> {
    return {
        scope: true,
        controller: 'zoneSuggestionModalCtrl',
        controllerAs: 'zoneSuggestionModal',
        bindToController: true,
        link(scope, element, attrs, ctrl) {
            if (ctrl) {
                ctrl.mapApiKey = attrs.mapApiKey;
            }
        },
    };
}

export function zoneMapDirective(): IDirective<IScope, JQLite, IAttributes, IZoneMapController> {
    return {
        scope: {
            center: '<',
            address: '<',
            mapApiKey: '@',
            onApplyAddress: '&',
        },
        controller: 'zoneMapCtrl',
        controllerAs: 'zoneMap',
        bindToController: true,
        templateUrl: zoneMap,
        link(scope, element, attrs, ctrl) {
            if (ctrl) {
                ctrl.initAddress(ctrl.address);
                ctrl.mapApiKey = attrs.mapApiKey;
            }
        },
    };
}
