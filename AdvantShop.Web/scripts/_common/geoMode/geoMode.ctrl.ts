import type { cookies, IController, IHttpPromise, ILocationService, IScope, translate } from 'angular';
import { type AddressData } from '../../@types/location';
import { type Response } from '../../@types/http';
import { IAbstractShippingOption, IBaseShippingOption, IGeoModeDeliveries } from '../../_partials/shipping/types';
import type { ICustomerContact } from '../../myaccount/types';
import type { IGeoModeService, ShippingPointModalScopeType } from './geoMode.service';

type ShippingTypes = {
    readonly delivery: 'courier';
    readonly selfDelivery: 'pickpoint';
};

type ShippingTypesValue = ShippingTypes[keyof ShippingTypes];
export interface IGeoModeCtrl extends IController {
    currentAddress: ICustomerContact | undefined;
    addressString: string;
    currentShippingPoint: IAbstractShippingOption | null;
    isPointSelected: boolean;
    changeMode(type: ShippingTypesValue): void;
    updateAddress(addressData: ICustomerContact): void;
    addressStringify(address: any): string;
    selectShippingPoint(shippingPoint: IAbstractShippingOption, point: IBaseShippingOption): void;
    setShippingCheckout(shippingPoint: IAbstractShippingOption): Promise<boolean>;
    setAddressCheckout(customerContact: ICustomerContact): Promise<IHttpPromise<Response> | null>;
    initCurrentShippingPoint(shippingPoint: IAbstractShippingOption): void;
    subscribeToChangeAddress(): void;
    onChangeShippingPoint(shippingPoint: IAbstractShippingOption, point: IBaseShippingOption): void;
}

type ShippingType = ShippingTypesValue | undefined;

export default class GeoModeCtrl implements IGeoModeCtrl {
    isSetShippingPoint: boolean | null = null;
    currentAddress: ICustomerContact | undefined = undefined;
    private modeTypes: ShippingTypes = {
        delivery: 'courier',
        selfDelivery: 'pickpoint',
    };
    addressString = 'Введите ваш адрес';
    currentShippingPoint: IAbstractShippingOption | null = null;
    isPointSelected: boolean = false;
    private shippingType?: ShippingType;
    private readonly shippingTypeCookieName: string = 'advShippingType';
    private readonly shippingPointCookieName: string = 'geomode_point';
    private currentCity: string | null = null;

    /* @ngInject */
    constructor(
        private readonly $scope: IScope,
        private readonly addressService: any,
        private readonly checkoutService: any,
        private readonly $cookies: cookies.ICookiesService,
        private readonly warehousesService: any,
        private readonly $location: ILocationService,
        private readonly geoModeService: IGeoModeService,
    ) {}

    $postLink = async () => {
        const geoModeUrlParam = this.$location.search();
        const currentCustomerContactFromState: ICustomerContact | null = this.geoModeService.currentCustomerContact;
        this.subscribeToChangeAddress();
        if (this.currentAddress && !currentCustomerContactFromState) {
            this.geoModeService.initGeoModeState(this.currentAddress);
        }
        if (geoModeUrlParam[this.geoModeService.geoModeUrlParam]) {
            !this.currentAddress ? this.geoModeService.openAddNewAddressModal() : this.geoModeService.openChangeAddressModal();
        }
    };

    subscribeToChangeAddress = () => {
        const removeCb = this.geoModeService.subscribeChangeAddress(this.updateAddress);
        this.$scope.$on('$destroy', function () {
            removeCb();
        });
    };

    initCurrentShippingPoint = (shippingPoint: IAbstractShippingOption | undefined) => {
        let warehouseIds = this.getWarehouseIds(shippingPoint);
        this.warehousesService.setWarehouseCookie(warehouseIds);
    };

    setShippingType = (shippingType: ShippingType) => {
        this.shippingType = shippingType || this.modeTypes.delivery;
    };

    setShippingPoint = async () => {
        if (this.isPointSelected && this.shippingType == this.modeTypes.selfDelivery) {
            try {
                const response = await this.getShippingPoints();
                if (response) {
                    let id = this.$cookies.get(this.shippingPointCookieName);

                    this.currentShippingPoint =
                        response.selectedOption != null && (id == null || id == response.selectedOption.Id) ? response.selectedOption : null;

                    if (this.currentShippingPoint != null && id == null) {
                        this.$cookies.put(this.shippingPointCookieName, this.currentShippingPoint.Id);
                    }
                }
            } catch (e) {
                console.error(e);
            }
        }
    };

    async getShippingPoints(): Promise<IGeoModeDeliveries> {
        return await this.checkoutService.getGeoModeDeliveries('self-delivery');
    }

    changeMode = async (type: ShippingTypesValue) => {
        if (this.shippingType === type) return;
        this.$cookies.put(this.shippingTypeCookieName, type);
        this.shippingType = type;
        if (this.shippingType === this.modeTypes.delivery) {
            if (this.currentAddress) {
                await this.setAddressCheckout(this.currentAddress);
            }
        } else {
            if (this.currentShippingPoint) {
                await this.setShippingCheckout(this.currentShippingPoint);
            } else {
                this.hideShippingPointText();
                await this.setShippingPoint();
            }
            this.showShippingPointText();
            this.$scope.$apply();
        }
    };

    showShippingPointText = () => {
        this.isSetShippingPoint = true;
    };

    hideShippingPointText = () => {
        this.isSetShippingPoint = false;
    };

    updateAddress = async (customerContact: ICustomerContact): Promise<void> => {
        try {
            const addressData = this.geoModeService.getAddressData(customerContact);
            this.addressString = this.addressStringify(addressData);
            this.currentAddress = customerContact;
            this.$scope.$digest();
        } catch (e) {
            console.error(e.message || e);
        }
    };

    addressStringify = (addressData: AddressData) => {
        return this.addressService.addressStringify(addressData, true);
    };

    selectShippingPoint = (shippingPoint: IAbstractShippingOption, point: IBaseShippingOption) => {
        this.currentShippingPoint = shippingPoint;

        if (this.currentShippingPoint) {
            this.$cookies.put(this.shippingPointCookieName, this.currentShippingPoint.Id);
        } else {
            this.$cookies.remove(this.shippingPointCookieName);
        }

        let warehouseIds = this.getWarehouseIds(shippingPoint);

        this.warehousesService.setWarehouseCookie(warehouseIds).then(() => {
            location.reload();
        });
    };

    setAddressCheckout = async (customerContact: ICustomerContact): Promise<IHttpPromise<Response> | null> => {
        return this.geoModeService.setAddressCheckout(customerContact);
    };

    setShippingCheckout = async (shippingPoint: IAbstractShippingOption): Promise<boolean> => {
        try {
            const response = await this.checkoutService.saveShipping(shippingPoint, null);
            if (response == null) {
                console.error('Failed to set delivery method');
                return false;
            }
            return true;
        } catch (e) {
            console.error(e.message);
            return false;
        }
    };

    onChangeShippingPoint = (shippingPoint: IAbstractShippingOption, point: IBaseShippingOption) => {
        if (
            this.currentShippingPoint?.Id === shippingPoint.Id &&
            (!shippingPoint.SelectedPoint || shippingPoint.SelectedPoint?.Id === this.currentShippingPoint?.SelectedPoint?.Id)
        ) {
            this.geoModeService.closeChangeShippingPointModal();
            return;
        }

        this.setShippingCheckout(shippingPoint)
            .then((isSetShippingPoint: boolean) => {
                if (isSetShippingPoint) {
                    this.selectShippingPoint(shippingPoint, point);
                    this.geoModeService.closeChangeShippingPointModal();
                    this.$scope.$apply();
                }
            })
            .catch((reason) => {
                console.error(reason);
            });
    };

    changeShippingPoint = () => {
        const modalScope: ShippingPointModalScopeType = {
            currentShippingPoint: this.currentShippingPoint,
            onChangeShippingPoint: this.onChangeShippingPoint,
        };
        this.geoModeService.openChangeShippingPointModal(modalScope);
    };

    getWarehouseIds = (shippingPoint: IAbstractShippingOption | undefined): number[] | undefined => {
        return shippingPoint != null && shippingPoint.SelectedPoint != null && shippingPoint.SelectedPoint.WarehouseId != null
            ? [shippingPoint.SelectedPoint.WarehouseId]
            : shippingPoint?.Warehouses;
    };
}
