import { IScope, IParseService, IRootElementService, IAttributes, IDirective, IDirectiveController, IController } from 'angular';
import type { IGeoModeCtrl } from './geoMode.ctrl';
import GeoModeService, { IGeoModeService } from './geoMode.service';
import { ICustomerContact } from '../../myaccount/types';

/* @ngInject */
export default function locationModalDirective($parse: IParseService, addressService: any): IDirective<IScope, JQLite, IAttributes, IGeoModeCtrl> {
    return {
        scope: true,
        controller: 'geoModeCtrl',
        controllerAs: 'geoMode',
        bindToController: true,
        link(scope, element, attrs, ctrl) {
            if (ctrl !== undefined) {
                ctrl.hideShippingPointText();
                ctrl.currentAddress = $parse(attrs.currentAddress)(scope);
                ctrl.shippingsPoints = $parse(attrs.shippingsPoints)(scope);
                ctrl.isPointSelected = $parse(attrs.isPointSelected)(scope);
                ctrl.setShippingType(attrs.shippingType);
                ctrl.setShippingPoint().finally(() => {
                    ctrl.showShippingPointText();
                    scope.$apply();
                });
                if (ctrl.currentAddress) {
                    ctrl.addressString = addressService.addressStringify(ctrl.currentAddress, true);
                }
            }
        },
    };
}

/* @ngInject */
export function GeoModeChangeAddressTriggerDirective(
    $parse: IParseService,
    geoModeService: IGeoModeService,
): IDirective<IScope, JQLite, IAttributes, IGeoModeCtrl> {
    return {
        scope: true,
        controller: () => {},
        controllerAs: 'geoModeChangeAddressTrigger',
        bindToController: true,
        link(scope, element, attrs, ctrl) {
            if (ctrl) {
                let currentAddress = $parse(attrs.geoModeChangeAddressTrigger)(scope);
                ctrl.currentAddress = currentAddress;
                const currentCustomerContactFromState: ICustomerContact | null = geoModeService.currentCustomerContact;
                if (currentAddress && !currentCustomerContactFromState) {
                    geoModeService.initGeoModeState(currentAddress);
                }
                const removeCb = geoModeService.subscribeChangeAddress((customerContact: ICustomerContact) => {
                    ctrl.currentAddress = customerContact;
                    ctrl.currentCity = customerContact.City;
                    scope.$apply();
                });
                const changeAddressHandler = (e: Event) => {
                    const currentAddress = geoModeService.currentCustomerContact;
                    e.stopPropagation();
                    currentAddress != null ? geoModeService.openChangeAddressModal() : geoModeService.openAddNewAddressModal();
                    scope.$apply();
                };
                element[0].addEventListener('click', changeAddressHandler);

                scope.$on('$destroy', function () {
                    removeCb();
                    element[0].removeEventListener('click', changeAddressHandler);
                });
            }
        },
    };
}
