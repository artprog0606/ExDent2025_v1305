import {ICustomOptionItem, CustomOptionsServiceType} from '../custom-options/types';
import {IEvaluatedCustomOptions} from "../../catalog/types";
import {
    IAttributes,
    IDocumentService,
    IHttpService,
    IParseService,
    IPromise,
    IQService,
    IScope, ITimeoutService,
    IWindowService,
    translate
} from "angular";
import {IDiscount} from "../../@types/shared";
import {CartConfig} from "./cartConfigDefault";

export type CartServiceFnType = ($document: IDocumentService,
                                 $q: IQService,
                                 $http: IHttpService,
                                 $translate: translate.ITranslateService,
                                 $window: IWindowService,
                                 cartConfig: CartConfig,
                                 domService: any,
                                 SweetAlert: any,
                                 customOptionsService: CustomOptionsServiceType) => CartServiceType;

export type CartAddControllerFnType = ($document: IDocumentService,
                                       $scope: IScope,
                                       $attrs: IAttributes,
                                       $parse: IParseService,
                                       $q: IQService,
                                       $timeout: ITimeoutService,
                                       $window: IWindowService,
                                       cartConfig: CartConfig,
                                       cartService: CartServiceType,
                                       moduleService: any,
                                       popoverService: any,
                                       SweetAlert: any,
                                       $translate: translate.ITranslateService,
                                       domService: any,
                                       toaster: any,
                                       customOptionsService: CustomOptionsServiceType) => CartAddController;
export type CartAddController = {
    data: ICartAddDirectiveBindings
}

export type CartServiceType = {
    getData: (cache: boolean, queryParams: Record<PropertyKey, any>) => IPromise<ICart>;
    updateAmount: (items: { Key: number, Value: number }[], queryParams: Record<PropertyKey, any>) => IPromise<ICart>;
    removeItem: (shoppingCartItemId: number, queryParams: Record<PropertyKey, any>) => IPromise<ICart>;
    addItem: (data: IAddItemProps) => IPromise<ICart> | undefined;
    addItems: (items: Omit<IAddItemProps, "queryParams">[], queryParams: Record<PropertyKey, any>) => IPromise<ICart> | undefined;
    clear: (queryParams: Record<PropertyKey, any>) => IPromise<ICart>;
    addCallback: (name: CallbackNames, func: ICartCallback, targetName: string) => IPromise<ICart>;
    processCallback: (name: CallbackNames, params: any, targetName: string) => IPromise<ICart>;
    removeCallback: (name: CallbackNames, targetName: string, cb: ICartCallback) => IPromise<ICart>;
    setStateInfo: (needShow: boolean) => void;
    showInfoWithDebounce: (data: any) => void;
    showInfo: () => void;
    clickout: () => void;
    findInCart: (productId: number, offerId?: number | null, customOptions?: ICustomOptionItem[]) => ICartItem;
}

export type ICartAddDirectiveBindings = IAddItemProps & {
    cartAddValid?: (scope: IScope) => boolean;
    cartAddType: CartConfig["cartAddType"][keyof CartConfig["cartAddType"]];
    maxStepSpinbox: number;
    minStepSpinbox: number;
    stepSpinbox: number;
    href: number;
    source: "mobile" | null
}

export type CallbackNames = "get" | "update" | "remove" | "add" | "clear" | "open";
export type ICartCallback = (cart: ICart, params: any) => void


export interface IAddItemProps {
    offerId: number;
    productId: number;
    amount: number;
    attributesXml: string;
    payment: number;
    mode: string;
    lpId?: number;
    lpUpId?: number;
    lpEntityId?: number;
    lpEntityType?: string;
    lpBlockId?: number;
    lpButtonName?: string;
    hideShipping?: boolean;
    offerIds: number[];
    modeFrom: string;
    forceHiddenPopup: boolean;
    queryParams: Record<PropertyKey, any>;
}

export interface ICartCoupon {
    Price: string;
    Percent: string;
    Code: string;
    NotApplied: boolean;
}


export interface ICart {
    CartProducts: ICartItem[];
    ColorHeader: string;
    SizeHeader: string;
    Count: string;
    TotalItems: number;
    BonusPlus: string;
    Valid: string;
    CouponInputVisible: boolean;
    ShowConfirmButtons: boolean;
    ShowBuyInOneClick: boolean;
    BuyInOneClickText: string;
    TotalProductPrice: string;
    TotalPrice: string;
    DiscountPrice: string;
    Certificate: string;
    CertificateCode: string;
    Coupon: ICartCoupon;
    MobileIsFullCheckout: boolean;
    EnablePhoneMask: boolean;
    IsDefaultCustomerGroup: boolean;
    IsShowUnits: boolean;
}


interface ICartItem {
    OfferId: number;
    Price: string;
    ProductId: number | null;
    PriceWithDiscount: string;
    Discount: IDiscount;
    DiscountText: string;
    Amount: number;
    Sku: string;
    PhotoPath: string;
    PhotoSmallPath: string;
    PhotoMiddlePath: string;
    PhotoAlt: string;
    Name: string;
    Link: string;
    Cost: string;
    ShoppingCartItemId: number;
    SelectedOptions: IEvaluatedCustomOptions[];
    ColorName: string;
    SizeName: string;
    Avalible: string;
    AvailableAmount: number;
    MinAmount: number;
    MaxAmount: number;
    Multiplicity: number;
    FrozenAmount: boolean;
    IsGift: boolean;
    Unit: string;
    PriceRuleName: string;
}

