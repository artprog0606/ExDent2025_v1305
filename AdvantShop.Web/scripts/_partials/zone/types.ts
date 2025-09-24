export interface IIpZone {
    CountryId?: number;
    CountryName: string;
    RegionId: number;
    Region: string;
    CityId: number;
    City: string;
    District: string;
    Phone: string;
    MobilePhone: string;
    Zip: string;
    ReloadPage: boolean;
    ReloadUrl: string;
}

export interface ICountry {
    CountryId: number;
    Name: string;
    Iso2: string;
    Iso3: string;
    DisplayInPopup: boolean;
    SortOrder: number;
    DialCode?: number;
}

type Events = 'changeCity' | 'changeAddress';

export type ZoneEvents = Record<Events, string>;

export type ZoneEventName = ZoneEvents[keyof ZoneEvents];
