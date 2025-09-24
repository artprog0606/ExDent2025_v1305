import { TaxType, Currency, IPhoto } from '../../@types/shared';
import { PaymentMethodType, PaymentSubjectType } from '../payment/types';
import { TypeOfDelivery } from '../../checkout/types';

type Params = {
    ShowDeliveryInterval: string;
    DeliveryIntervals: string;
    TimezoneId: string;
    CountVisibleDeliveryDay: string;
    CountHiddenDeliveryDay: string;
    MinDeliveryTime: string;
    ShowSoonest: string;
};

type TimeWork = {
    Label: string;
    From: string;
    To: string;
};

export interface IBaseShippingOption {
    Id: string;
    Code: string;
    Name: string;
    Address: string;
    Description: string;
    Latitude?: number;
    Longitude?: number;
    AvailableCashOnDelivery?: boolean;
    AvailableCardOnDelivery?: boolean;
    Phones: string[];
    TimeWorkStr: string;
    TimeWork: TimeWork[];
    StoragePeriodInDay?: number;
    DeliveryPeriodInDay?: number;
    MaxWeightInGrams?: number;
    DimensionSumInMillimeters?: number;
    DimensionVolumeInCentimeters?: number;
    MaxHeightInMillimeters?: number;
    MaxWidthInMillimeters?: number;
    MaxLengthInMillimeters?: number;
    MaxCost?: number;
    WarehouseId?: number;
}

export interface IAbstractShippingOption {
    Id: string;
    DeliveryId: number;
    MethodId: number;
    Name: string;
    Hint: string;
    Desc: string;
    DisplayCustomFields: boolean;
    DisplayIndex: boolean;
    IconName: string;

    ShowInDetails: boolean;
    ZeroPriceMessage: string;
    TaxId?: number;
    PaymentMethodType: PaymentMethodType;
    PaymentSubjectType: PaymentSubjectType;
    ShippingType: string;

    NameRate: string;
    HideAddressBlock: boolean;
    Rate: number;
    ManualRate: number;
    UseExtracharge: boolean;
    ExtrachargeInNumbers: number;
    ExtrachargeInPercents: number;
    ExtrachargeFromOrder: boolean;
    PreCost: number;
    ShippingCurrency: Currency;
    ShippingPoints: IBaseShippingOption[];
    CurrentCurrency: Currency;
    FinalRate: number;
    FormatRate: string;
    DeliveryTime: string;
    ExtraDeliveryTime: number;
    ModelType: string;
    UseDeliveryInterval: boolean;
    ShowSoonest: boolean;
    TypeOfDelivery?: TypeOfDelivery;
    DeliveryIntervalsStr: string;
    DateOfDeliveryStr: string;
    MinDate: string;
    MaxDate: string;
    StartDateTime: Date;
    TimeZoneOffset?: number;
    Warehouses: number[];
    Template: string;
    TemplateName: string;
    ErrorMessage: string;
    ApplyPay: boolean;
    IsAvailablePaymentCashOnDelivery: boolean;
    IsAvailablePaymentPickPoint: boolean;
    SelectedPoint?: IBaseShippingOption;
    DateOfDelivery: Date;
    TimeOfDelivery: string;
    Asap?: boolean;
    AvailablePayment: boolean;
}

export interface IGeoModeDeliveries {
    options: IAbstractShippingOption[];
    selectedOption: IAbstractShippingOption;
}
