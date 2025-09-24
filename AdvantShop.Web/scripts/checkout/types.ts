import { PaymentMethodType, PaymentSubjectType } from '../_partials/payment/types';
import { Currency } from '../@types/shared';

export type TypeOfDelivery = 'courier' | 'self-delivery';

export interface ICheckoutAddress {
    Country?: string;
    City?: string;
    District?: string;
    Region?: string;
    Zip?: string;
    CustomField1?: string;
    CustomField2?: string;
    CustomField3?: string;
    Street?: string;
    House?: string;
    Apartment?: string;
    Structure?: string;
    Entrance?: string;
    Floor?: string;
    ContactId?: number;
}
