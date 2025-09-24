import { Guid } from '../@types/shared';

export interface ICustomerContact {
    ContactId: Guid;
    CustomerGuid: Guid;
    CityId?: number;
    Name: string;
    CountryId: number;
    Country: string;
    City: string;
    District: string;
    RegionId?: number;
    Region: string;
    Zip: string;
    Street: string;
    House: string;
    Apartment: string;
    Structure: string;
    Entrance: string;
    Floor: string;
    DadataJson: string;
    IsMain: boolean;
}
