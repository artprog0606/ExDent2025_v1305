import { PubSub } from '../PubSub/PubSub';
import { ICustomerContact } from '../../myaccount/types';
import { IHttpPromise, ILocationService, translate } from 'angular';
import { GeoModeEventsObjType } from './geoMode.module';
import { isResponseError, type Response } from '../../@types/http';
import type { AddressData } from '../../@types/location';
import type { IZoneMapService } from '../../_partials/zone/services/zoneMapService';
import type { Form } from '../../_partials/address/types';
import type { IAbstractShippingOption, IBaseShippingOption } from '../../_partials/shipping/types';

export type GeoModeUrlParamNameType = 'geoMode';

export type ShippingPointModalScopeType = {
    currentShippingPoint: IAbstractShippingOption | null;
    onChangeShippingPoint: (shippingPoint: IAbstractShippingOption, point: IBaseShippingOption) => void;
};

export interface IGeoModeService {
    subscribeChangeAddress(fn: (customerContact: ICustomerContact) => any): () => void;
    publishChangeAddress(customerContact: ICustomerContact): void;
    get geoModeUrlParam(): GeoModeUrlParamNameType;
    openAddNewAddressModal(): void;
    openChangeAddressModal(): void;
    setAddressCheckout(customerContact: ICustomerContact): Promise<IHttpPromise<Response> | null>;
    getAddressData(customerContact: ICustomerContact): AddressData;
    initGeoModeState(customerContact: ICustomerContact): void;
    get currentCustomerContact(): ICustomerContact | null;
    openChangeShippingPointModal(parentScope: ShippingPointModalScopeType): void;
    closeChangeShippingPointModal(): void;
}

type GeoModeState = {
    customerContact: ICustomerContact | null;
};

export default class GeoModeService implements IGeoModeService {
    _geoModeState: GeoModeState = {
        customerContact: null,
    };
    private isRenderShippingPointsModal: boolean = false;
    private _shippingPointModalId: string = 'shippingsPoints';

    /* @ngInject */
    constructor(
        private readonly $location: ILocationService,
        private readonly addressService: any,
        private readonly geoModeEvents: GeoModeEventsObjType,
        private readonly zoneMapService: IZoneMapService,
        private readonly modalService: any,
        private readonly $translate: translate.ITranslateService,
    ) {}

    initGeoModeState = async (customerContact: ICustomerContact) => {
        this._geoModeState = {
            ...this._geoModeState,
            customerContact,
        };
        await this.publishChangeAddress(customerContact);
    };

    get currentCustomerContact(): ICustomerContact | null {
        return this._geoModeState.customerContact;
    }
    get geoModeUrlParam() {
        return 'geoMode' as GeoModeUrlParamNameType;
    }
    subscribeChangeAddress = (fn: (customerContact: ICustomerContact) => unknown): (() => void) => {
        return PubSub.subscribe(this.geoModeEvents.CHANGE_ADDRESS, fn);
    };

    publishChangeAddress = async (customerContact: ICustomerContact) => {
        if (this._geoModeState.customerContact?.ContactId !== customerContact.ContactId) {
            await this.updateAddress(customerContact);
            this._geoModeState.customerContact = customerContact;
            PubSub.publish(this.geoModeEvents.CHANGE_ADDRESS, customerContact);
        }
    };

    setAddressCheckout = async (customerContact: ICustomerContact): Promise<IHttpPromise<Response> | null> => {
        try {
            return this.zoneMapService.setMainContact(customerContact.ContactId);
        } catch (e) {
            console.error(e);
            return null;
        }
    };

    updateAddress = async (customerContact: ICustomerContact): Promise<void> => {
        try {
            const res = await this.setAddressCheckout(customerContact);
            if (res && !isResponseError(res.data)) {
                const addressData: AddressData = this.getAddressData(customerContact);
                if (this._geoModeState.customerContact?.City != customerContact.City) {
                    const zone = await this.zoneMapService.setLocationZone(addressData);
                    zone?.ReloadPage && location.reload();
                }
            }
        } catch (e) {
            console.error(e.message || e);
        }
    };

    clearGeoModeUrlParam = () => {
        this.$location.search(this.geoModeUrlParam, null);
    };

    openAddNewAddressModal = () => {
        const modalData = {
            type: 'change',
            themeAlt: false,
            clearGeoModeUrlParam: this.clearGeoModeUrlParam,
            onAddEditAddress: (formData: Form, contacts: ICustomerContact[], addressSelected: ICustomerContact) =>
                this.publishChangeAddress(addressSelected),
        };

        this.addressService.dialogRender('addEditAddress.clearGeoModeUrlParam()', modalData);
    };

    openChangeAddressModal = () => {
        const modalData = {
            onApply: this.publishChangeAddress,
            showFormAddressEmpty: true,
            clearGeoModeUrlParam: this.clearGeoModeUrlParam,
        };
        const options = {
            callbackClose: 'addressList.clearGeoModeUrlParam()',
        };

        this.addressService.showAddressListModal(modalData, options);
    };

    getAddressData = (customerContact: ICustomerContact): AddressData => {
        return {
            Country: customerContact.Country,
            CountryName: customerContact.Country,
            Region: customerContact.Region,
            District: customerContact.District,
            City: customerContact.City,
            Zip: customerContact.Zip,
            Street: customerContact.Street,
            House: customerContact.House,
            Structure: customerContact.Structure,
            CountryId: customerContact.CountryId,
            RegionId: customerContact.RegionId,
        };
    };

    openChangeShippingPointModal = (parentScope: ShippingPointModalScopeType) => {
        const modalId = this._shippingPointModalId;
        const header = this.$translate.instant('Js.GeoMode.ShippingPoint');
        const content = `<shipping-points-list data-selected-shipping-point="currentShippingPoint"
                                                                           data-on-change="onChangeShippingPoint(shippingPoint, point)">
                                                    </shipping-points-list>`;
        const footer = null;
        const options = {
            destroyOnClose: true,
            isOpen: true,
            modalClass: 'shippings-points-modal',
        };

        this.modalService.renderModal(modalId, header, content, footer, options, parentScope);
    };

    closeChangeShippingPointModal = () => {
        this.modalService.close(this._shippingPointModalId);
    };
}
