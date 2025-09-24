import AimControl from '../../../_common/yandexMaps/aimControl.js';
import type { IController, IScope, ITimeoutService } from 'angular';
import { AddressValue } from '../../../@types/location';
import { isResponseError } from '../../../@types/http';
import type { Map } from 'yandex-maps';
import type { IZoneMapService } from '../services/zoneMapService';
import type { ZoneEvents } from '../types';

export interface IZoneMapController extends IController {
    address: AddressValue | undefined;
    set disabledSetAddressBtn(active: boolean);
    initAddress(address: AddressValue | undefined): void;
    setAddress(): Promise<void>;
    onInitMap(target: Map): Promise<void>;
    onActionEndMap(): Promise<void>;
    onApplySuggestionAddress(obj: any, event: any): Promise<void>; //TODO
    setAddressFromMap(): Promise<void>;
    clearGeoLocationSearch(): void;
}

export default class ZoneMapController implements IZoneMapController {
    skipAction: boolean = false;
    mapControls: string = 'zoomControl';
    mapZoom: number = 17;
    isLoadedMap: boolean = false;
    private _disabledSetAddressBtn: boolean = true;
    private map: Map | undefined;
    address: AddressValue | undefined = undefined;
    /* @ngInject */

    constructor(
        private readonly $timeout: ITimeoutService,
        private readonly zoneMapService: IZoneMapService,
        private readonly addressService: any,
        private readonly $scope: IScope,
        private readonly zoneEvents: ZoneEvents,
    ) {}
    $onInit(): void {
        this.skipAction = false;
        this.mapControls = 'zoomControl';
        this.mapZoom = 17;
        this.isLoadedMap = false;
        this.disabledSetAddressBtn = !Boolean(this.address);
    }

    set disabledSetAddressBtn(active: boolean) {
        this._disabledSetAddressBtn = active;
    }

    initAddress = (address: AddressValue | undefined): void => {
        this.address = address;
    };
    setAddress = async (): Promise<void> => {
        if (this.address) {
            // if (this.address?.Coords) {
            //     this.zoneMapService.setGeoPositionCookie(this.address?.Coords);
            // }
            // this.zoneMapService.setNotShowGeoCookie();
            try {
                const isSetCheckoutAddress = await this.zoneMapService.setCheckoutAddress(this.address.AddressData);
                const locationData = await this.zoneMapService.setLocationZone(this.address.AddressData);
                if (isSetCheckoutAddress && locationData != null) {
                    this.zoneMapService.publishEvent(this.zoneEvents.changeAddress, this.address);
                    this.zoneMapService.hideLocationModal();
                    this.$scope.$apply();
                }
            } catch (e) {
                console.error(e.message);
            }
        }
    };

    onInitMap = async (target: Map): Promise<void> => {
        this.map = target;
        // @ts-ignore
        this.map.controls.add(new AimControl());
        this.isLoadedMap = true;
        if (this.address?.Value == null) {
            await this.setAddressFromMap();
        }
    };
    onActionEndMap = async (): Promise<void> => {
        if (!this.skipAction) {
            await this.setAddressFromMap();
        }
        this.skipAction = false;
    };

    onApplySuggestionAddress = async (obj, event): Promise<void> => {
        // this.$timeout(async () => {
        //     if (this.address != null) {
        //         this.address.Value = obj != null ? this.addressService.addressStringify(obj, true) : null;
        //         this.disabledSetAddressBtn = !Boolean(this.address.Value);
        //         try {
        //             const { data } = await this.zoneMapService.getCoordsByAddress(this.address.Value);
        //             if (data.result) {
        //                 this.address.CheckoutAddress = obj;
        //                 this.address.Coords = data.obj?.Coords;
        //                 if (event.type === 'click') {
        //                     this.skipAction = true;
        //                     this.map?.setCenter([this.address.Coords.Longitude, this.address.Coords.Latitude], this.mapZoom);
        //                 }
        //             }
        //         } catch (e) {
        //             console.error(e.message);
        //         }
        //     }
        // });
    };

    setAddressFromMap = async (): Promise<void> => {
        const coords = this.map?.getCenter();
        if (coords != undefined) {
            const [lon, lat] = coords;

            const { data } = await this.zoneMapService.getAddressByCoords({ Latitude: lat, Longitude: lon });
            if (!isResponseError(data)) {
                this.$timeout(() => {
                    this.address = data.obj;
                    this.disabledSetAddressBtn = !Boolean(this.address);
                });
            } else {
            }
        }
    };

    clearGeoLocationSearch = (): void => {
        if (this.address !== undefined && this.address.Value) {
            this.address.Value = '';
            this.disabledSetAddressBtn = !Boolean(this.address?.Value);
        }
    };
}
