import type {ICountry} from "../zone/types";
import {ICustomerContact} from "../../myaccount/types";

export type CustomerContactsFields = {
    IsShowCountry: boolean;
    IsRequiredCountry: boolean;
    IsShowState: boolean;
    IsRequiredState: boolean;
    IsShowCity: boolean;
    IsRequiredCity: boolean;
    IsShowDistrict: boolean;
    IsRequiredDistrict: boolean;
    IsShowAddress: boolean;
    IsRequiredAddress: boolean;
    IsShowZip: boolean;
    IsRequiredZip: boolean;
    IsShowFullAddress: boolean;
    UseAddressSuggestions: boolean;
    SuggestAddressUrl?: string;
    IsShowFirstName: boolean;
    CustomerFirstNameField: string;
    IsShowLastName: boolean;
    IsRequiredLastName: boolean;
    IsShowPatronymic: boolean;
    IsRequiredPatronymic: boolean;
}

export type AddressListConfig = {
    autocompleteAlt: boolean;
    themeAlt: boolean;
    compactMode: boolean;
    overrideFields: OverrideFields,
    requiredValidationEnabled: boolean;
}

export type OverrideFields = {
    visible: Array<keyof CustomerContactsFields>
};

export type ContactId = Form['contactId'];

export type Form = {
    house?: string | null;
    apartment?: string | null;
    structure?: string | null;
    entrance?: string | null;
    floor?: string | null;
    countries?: ICountry[];
    countryId?: number;
    country?: ICountry;
    region?: string;
    district?: string;
    zip?: string;
    byCity?: boolean;
    contactId?: string;
    fio?:string;
    firstName?:string;
    lastName?:string;
    patronymic?:string;
    city?:string;
    street?:string;
} | Record<PropertyKey, never>

export type OnAddEditAddressCallbackData = {
    formData: Form;
    contacts: ICustomerContact[],
    addressSelected: ICustomerContact;
}
