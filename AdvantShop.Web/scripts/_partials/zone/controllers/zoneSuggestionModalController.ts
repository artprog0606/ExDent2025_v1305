import type { IController, IScope } from 'angular';
import type { IZoneMapService } from '../services/zoneMapService';
import type { ZoneEvents } from '../types';
import { type AddressValue, type ICoords } from '../../../@types/location';
import { isResponseError } from '../../../@types/http';

export interface IZoneSuggestionModalCtrl extends IController {
    currentAddress: CurrentAddress;
    tryDetermineAddress(position: GeolocationPosition): Promise<AddressValue | undefined>;
    getAddressByCoords(coords: ICoords): Promise<AddressValue | undefined | never>;
    showSuggestionModal(address: CurrentAddress): Promise<void>;
    showMap(): void;
    showLocationModal(showMap: boolean): void;
    setAddress(address: AddressValue): Promise<void>;
    showDialogZone(): void;
}

type CurrentAddress = AddressValue | null;
export default class ZoneSuggestionModalCtrl implements IZoneSuggestionModalCtrl {
    private mapZoom: number = 17;
    currentAddress: CurrentAddress = null;
    /* @ngInject */
    constructor(
        private readonly modalService: any,
        private readonly zoneService: any,
        private readonly zoneMapService: IZoneMapService,
        private readonly $scope: IScope,
        private readonly zoneEvents: ZoneEvents,
    ) {}

    $onInit = () => {
        this.currentAddress = null;

        const isShowSuggestion = this.zoneMapService.getNotShowGeoCookie();
        if (isShowSuggestion != null) return;
        if ('geolocation' in navigator) {
            navigator.geolocation.getCurrentPosition(
                async (position: GeolocationPosition) => {
                    const address = await this.tryDetermineAddress(position);
                    if (address != null) {
                        this.currentAddress = address;
                        await this.showSuggestionModal(this.currentAddress);
                        this.zoneMapService.setNotShowGeoCookie();
                    }
                },
                () => {
                    this.zoneMapService.removeGeoPositionCookie();
                    this.showDialogZone();
                },
                {
                    enableHighAccuracy: true,
                },
            );
        } else {
            this.showDialogZone();
        }
    };

    tryDetermineAddress = async (position: GeolocationPosition): Promise<AddressValue | undefined> => {
        const { latitude: Latitude, longitude: Longitude } = position.coords;
        try {
            return await this.getAddressByCoords({ Latitude, Longitude });
        } catch (e) {
            console.error(e.message);
        }
    };

    getAddressByCoords = async (coords: ICoords): Promise<AddressValue | undefined | never> => {
        try {
            const { data } = await this.zoneMapService.getAddressByCoords(coords);
            if (!isResponseError(data)) {
                return data.obj;
            }
        } catch (e) {
            console.error(e.message);
        }
    };

    showSuggestionModal = async (address: CurrentAddress) => {
        if (address) {
            const modalScope = {
                // callbackClose: () => this.setAddress(address),
                agree: () => this.setAddress(address),
                disagree: this.showMap,
                address: address.Value,
            };
            this.zoneMapService.showSuggestionModal(modalScope);
        }
    };

    showMap = () => {
        this.zoneMapService.hideSuggestionModal();
        this.zoneMapService.setNotShowGeoCookie();
        this.showLocationModal();
    };

    showLocationModal = (showMap = true): void => {
        const modalScope = {
            center: showMap && [this.currentAddress?.Coords?.Longitude, this.currentAddress?.Coords?.Latitude],
            currentAddress: this.currentAddress,
            showMap,
            mapZoom: this.mapZoom,
        };
        this.zoneMapService.showLocationModal(modalScope);
    };

    setAddress = async (address: AddressValue) => {
        if (address.Coords != null) {
            this.zoneMapService.setGeoPositionCookie(address.Coords);
            this.zoneMapService.setNotShowGeoCookie();
            try {
                const isSetCheckoutAddress = await this.zoneMapService.setCheckoutAddress(address.AddressData);
                const locationZone = await this.zoneMapService.setLocationZone(address.AddressData);
                if (isSetCheckoutAddress && locationZone != null) {
                    this.zoneMapService.publishEvent(this.zoneEvents.changeAddress, address);
                    this.zoneMapService.hideSuggestionModal();
                    this.$scope.$apply();
                }
            } catch (e) {
                console.error(e.message);
            }
        }
    };

    showDialogZone = () => {
        this.zoneService.addCallback(this.zoneEvents.changeCity, this.zoneMapService.setNotShowGeoCookie);
        this.zoneService.zoneDialogOpen({ showImmediately: true });
    };
}
