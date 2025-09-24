export type Guid = string;

export type Currency = {
    CurrencyId: number;
    Name: string;
    Symbol: string;
    Rate: number;
    Iso3: string;
    NumIso3: number;
    IsCodeBefore: boolean;
    RoundNumbers: number;
    EnablePriceRounding: boolean;
};

export enum TaxType {
    None = 0,
    VatWithout = 1,
    Vat0 = 2,
    Vat10 = 3,
    Vat18 = 4,
    Vat20 = 5,
    Other = 100,
}

export enum PhotoType {
    None,
    Product,
    Product360,
    CategoryBig,
    CategorySmall,
    CategoryIcon,
    News,
    StaticPage,
    Brand,
    Carousel,
    MenuIcon,
    Shipping,
    Payment,
    Color,
    Manager,
    Review,
    Logo,
    DarkThemeLogo,
    Favicon,
    LogoMobile,
    MobileApp,
    LandingMobileApp,
    LogoSvg,
    FaviconSvg,
    LogoBlog,
    CarouselApi,
    ShippingZones,
    CityAddressPoints,
    CustomOptions,
}

export interface IPhoto {
    PhotoId: number;
    ObjId: number;
    PhotoType: PhotoType;
    PhotoName: string;
    PhotoNameSize1: string;
    PhotoNameSize2: string;
    ModifiedDate: Date;
    Description: string;
    PhotoSortOrder: number;
    Main: boolean;
    OriginName: string;
    ColorID: number;
    PhotoCategoryId: number;
    Photo: {
        PhotoId: number;
        ObjId: number;
        Type: PhotoType;
    };
}

export interface IDiscount {
    Percent: number;
    Amount: number;
    Type: EDiscountType;
}

export enum EDiscountType {
    Percent = 0,
    Amount = 1
}
