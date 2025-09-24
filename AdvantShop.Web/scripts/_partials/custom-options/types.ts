import {IEvaluatedCustomOptions} from "../../catalog/types";
import {IHttpPromise, IHttpService} from "angular";
import {CartServiceType} from "../cart/types";
import {OverrideProperties} from "type-fest";

export type CustomOptionsServiceFnType = ($http: IHttpService, urlHelper: any) => CartServiceType;

export type CustomOptionsServiceType = {
    getData: (productId: number) => IHttpPromise<ICustomOptionBaseDto>;
    get: (productId: number, items: ICustomOption[], canceler: number | null) => IHttpPromise<ICustomOptionBaseDto>;
    getSelectedOptions: (items: ICustomOption[]) => ICustomOptionItem[];
    isValidOptions: (options: any) => { invalidOptions: Set<any>, isValidOptions: boolean };
    isValidAddOption: (nextValueOption: number, item: ICustomOption, option: ICustomOptionItem) => boolean;
    isEqualCustomOptions: (target: IEvaluatedCustomOptions[], other: IEvaluatedCustomOptions[]) => boolean;
    customOptionItemToEvaluatedCustomOptionsMapper: (options: ICustomOptionItem[]) => IEvaluatedCustomOptions[];
}


export enum ECustomOptionInputType {
    DropDownList = 0,
    RadioButton = 1,
    CheckBox = 2,
    TextBoxSingleLine = 3,
    TextBoxMultiLine = 4,
    ChoiceOfProduct = 5,
    MultiCheckBox = 6,
}

export enum EOptionPriceType {
    Fixed = 0,
    Percent = 1
}

interface ICustomOptionBaseDto {
    ID: number;
    CustomOptionsId: number;
    InputType: ECustomOptionInputType;
    Title: string;
    SortOrder: number;
    Description: string;
    IsRequired: boolean;
    MinQuantity: number | null;
    MaxQuantity: number | null;
    Options: ICustomOptionItem[];
    SelectedOptions: ICustomOptionItem[] | null;
}



interface ICustomOptionBase<IsNumberField extends boolean> extends ICustomOptionBaseDto {
    MinQuantity: IsNumberField extends true ? number : null;
    MaxQuantity: IsNumberField extends true ? number : null;
    Selected?: boolean
}

interface ICustomOptionDropDownList extends OverrideProperties<ICustomOptionBase<true>, {
    SelectedOptions: ICustomOptionItem | null;
}> {
    InputType: 0
}

interface ICustomOptionRadioButton extends OverrideProperties<ICustomOptionBase<true>, {
    SelectedOptions: ICustomOptionItem | null;
}> {
    InputType: 1
}

interface ICustomOptionCheckBox extends ICustomOptionBase<true> {
    InputType: 2
}

interface ICustomOptionTextBoxSingleLine extends OverrideProperties<ICustomOptionBase<false>, {
    SelectedOptions: ICustomOptionItem | null;
}> {
    InputType: 3
}

interface ICustomOptionTextBoxMultiLine extends OverrideProperties<ICustomOptionBase<false>, {
    SelectedOptions: ICustomOptionItem | null;
}> {
    InputType: 4
}

interface ICustomOptionChoiceOfProduct extends ICustomOptionBase<true> {
    InputType: 5
}

interface ICustomOptionMultiCheckBox extends ICustomOptionBase<true> {
    InputType: 6
}

export type ICustomOption =
    ICustomOptionDropDownList |
    ICustomOptionRadioButton |
    ICustomOptionCheckBox |
    ICustomOptionTextBoxSingleLine |
    ICustomOptionTextBoxMultiLine |
    ICustomOptionChoiceOfProduct |
    ICustomOptionMultiCheckBox;

export interface ICustomOptionItemDto {
    ID: number;
    OptionId: number;
    CustomOptionsId: number;
    Title: string;
    PriceType: EOptionPriceType;
    OptionText: string;
    ProductId: number | null;
    OfferId: number | null;
    MinQuantity: number | null;
    MaxQuantity: number | null;
    DefaultQuantity: number | null;
    PictureUrl: string;
    Description: string;
    PriceString: string;
}


export interface ICustomOptionItem extends ICustomOptionItemDto {
    Selected?: boolean
}

