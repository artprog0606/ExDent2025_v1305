import type { IHttpService, cookies, translate } from 'angular';
import type { ICoords, AddressValue, AddressData } from '../../../@types/location';
import type { Response } from '../../../@types/http';
import { IHttpPromise } from 'angular';
import { PubSub } from '../../../_common/PubSub/PubSub.js';
import type { ZoneEventName } from '../types';
import type { Guid } from '../../../@types/shared';
import type { ParentScopeModalOption, OptionsModal } from '../../../_common/modal/types';
import type { ICheckoutAddress } from '../../../checkout/types';
import type { IIpZone } from '../types';

export interface IZoneMapService {
    showLocationModal(parentScope: ParentScopeModalOption): void;
    showSuggestionModal(parentScope: ParentScopeModalOption, options?: OptionsModal): void;
    hideLocationModal(): void;
    hideSuggestionModal(): void;
    getAddressByCoords(coords: ICoords): IHttpPromise<Response<AddressValue>>;
    getCoordsByAddress(address: string): void;
    setNotShowGeoCookie(): void;
    getNotShowGeoCookie(): any;
    setCheckoutAddress(checkoutAddress: ICheckoutAddress): Promise<boolean>;
    setLocationZone(zoneData: AddressData): Promise<IIpZone>;
    getLocationCurrentZone(): Promise<IIpZone>;
    setMainContact(contactId: Guid): IHttpPromise<Response>;
    setGeoPositionCookie(coords: ICoords): void;
    getGeoPositionCookie(): ICoords | null;
    removeGeoPositionCookie(): void;
    subscribeEvent<T>(eventName: ZoneEventName, cb: (data: T) => void): void;
    publishEvent<T>(eventName: ZoneEventName, data: T): void;
}

export default class ZoneMapService implements IZoneMapService {
    private isRenderLocationModal: boolean = false;
    private isRenderAddressListModal: boolean = false;
    private locationModalId: string = 'locationModal';
    private addressListModalId: string = 'addressListModal';
    private suggestionModalId: string = 'suggestionModal';
    private geoPositionCookieName: string = 'advposition';
    /* @ngInject */
    constructor(
        private modalService: any,
        private $http: IHttpService,
        private $cookies: cookies.ICookiesService,
        private zoneService: any,
        private checkoutService: any,
        private apiMapService: any,
        private $translate: translate.ITranslateService,
    ) {}

    showLocationModal = (parentScope: ParentScopeModalOption) => {
        // if (isRenderLocationModal === false) {
        const mapApiKey = this.apiMapService.getMapApiKey();
        this.modalService.renderModal(
            this.locationModalId,
            'Укажите адрес',
            `<div data-zone-map="" data-center="center" data-map-api-key="${mapApiKey}" data-on-apply-address="onApplyAddress(address)" data-address="currentAddress"></div>`,
            null,
            {
                destroyOnClose: true,
                isOpen: true,
                modalClass: 'geo-location-modal',
            },
            parentScope,
        );
        this.isRenderLocationModal = true;
        // } else {
        //     modalService.open(locationModalId);
        // }
    };

    showSuggestionModal = (parentScope: ParentScopeModalOption, options?: OptionsModal) => {
        const modalOptions = Object.assign(
            {
                destroyOnClose: true,
                isOpen: true,
                modalClass: 'suggestion-modal',
            },
            options,
        );
        this.modalService.renderModal(
            this.suggestionModalId,
            null,
            `<div class="suggestion-modal__content"><div class="suggestion-modal__question">${this.$translate.instant('Js.Suggestion.Title')}</div><div class="suggestion-modal__address" data-ng-bind="address"></div></div>`,
            `<div><button type="button" class="btn btn-middle btn-action btn--xs" data-ng-click="disagree()">${this.$translate.instant('Js.Suggestion.Unconfirmed')}</button> <button type="button" data-ng-click="agree()" class="btn btn-middle btn-submit btn--xs">${this.$translate.instant('Js.Suggestion.Confirm')}</button></div>`,
            modalOptions,
            parentScope,
        );
    };

    hideLocationModal = () => {
        this.modalService.close(this.locationModalId);
    };

    hideSuggestionModal = () => {
        this.modalService.close(this.suggestionModalId);
    };

    getAddressByCoords = ({ Latitude, Longitude }: ICoords): IHttpPromise<Response<AddressValue>> => {
        const a = this.$http.post<Response<AddressValue>>('location/GetAddressByCoords', {
            latitude: Latitude,
            longitude: Longitude,
        });

        return this.$http.post<Response<AddressValue>>('location/GetAddressByCoords', {
            latitude: Latitude,
            longitude: Longitude,
        });
    };

    getCoordsByAddress = (address: string) => {
        const yaApiKey = this.apiMapService.getMapApiKey();
        if (!yaApiKey) {
            console.error('apikey required for map');
        }
        return this.$http.post('location/GetCoordsByAddress', {
            address,
            yaApiKey,
        });
    };

    setNotShowGeoCookie = () => {
        this.$cookies.putObject('advShowGeoSuggestionModal', {
            showLocationModal: false,
        });
    };

    getNotShowGeoCookie = (): any => {
        return this.$cookies.getObject('advShowGeoSuggestionModal');
    };

    setCheckoutAddress = async (checkoutAddress: ICheckoutAddress): Promise<boolean> => {
        return await this.checkoutService.saveContact(checkoutAddress);
    };

    setLocationZone = async (zoneData: AddressData): Promise<IIpZone> => {
        return await this.zoneService.setCurrentZone(
            zoneData.City,
            zoneData,
            zoneData.CountryId,
            zoneData.Region,
            zoneData.CountryName || zoneData.Country,
            zoneData.Zip,
            zoneData.District,
        );
    };

    getLocationCurrentZone = async (): Promise<IIpZone> => {
        return await this.zoneService.getCurrentZone();
    };

    setMainContact = (contactId: Guid): IHttpPromise<Response> => {
        return this.$http.post<Response>('checkout/setCourierDeliveryGeoMode', { contactId });
    };

    setGeoPositionCookie = (coords: ICoords) => {
        this.$cookies.putObject(this.geoPositionCookieName, {
            Longitude: coords?.Longitude,
            Latitude: coords?.Latitude,
        });
    };

    getGeoPositionCookie = (): any => {
        return this.$cookies.getObject(this.geoPositionCookieName);
    };

    removeGeoPositionCookie = (): void => {
        this.$cookies.remove(this.geoPositionCookieName);
    };

    subscribeEvent = <T>(eventName: ZoneEventName, cb: (data: T) => void) => {
        PubSub.subscribe(eventName, cb);
    };

    publishEvent = <T>(eventName: ZoneEventName, data?: T) => {
        PubSub.publish(eventName, data);
    };
}
