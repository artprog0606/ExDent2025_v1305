export interface ICoords {
    Longitude: number;
    Latitude: number;
}

export type Address = string;

export type IIpZone = {
    CountryId: number;
    CountryName: string;
    RegionId: number;
    Region: string;
    CityId: number;
    City: string;
    District: string;
    Phone: string;
    MobilePhone: string;
    Zip: string;
};

export type AddressData = {
    Country: string;
    CountryName: string;
    Region: string;
    District: string;
    City: string;
    Zip: string;
    Street: string;
    House: string;
    Structure: string;
    CountryId?: number;
    RegionId?: number;
};

export type AddressValue = {
    Value: Address;
    Coords?: ICoords;
    AddressData: Readonly<AddressData>;
};
