export const evaluatedCustomOptionsToCustomOptionItemMapper = (options) => {
    return options.map((x) => {
        return {
            CustomOptionsId: x.CustomOptionId,
            OptionText: x.OptionTitle,
            DefaultQuantity: x.OptionAmount,
            OptionId: x.OptionId,
        };
    });
};
export const cartData = {
    CartProducts: [
        {
            Price: '406 руб.',
            ProductId: 1753,
            Amount: 1.0,
            Sku: '3648492',
            PhotoAlt: 'Товар1',
            Name: 'Товар1',
            Cost: '406 руб.',
            ShoppingCartItemId: 1496,
            AvailableAmount: 100.0,
            MinAmount: 1.0,
            MaxAmount: 100,
        },
        {
            Price: '4026 руб.',
            ProductId: 1750,
            Amount: 1.0,
            Sku: '3648489',
            PhotoAlt: 'Товар2',
            Name: 'Товар2',
            Cost: '4026 руб.',
            ShoppingCartItemId: 1497,
            AvailableAmount: 5.0,
            MinAmount: 1.0,
            MaxAmount: 100,
            Multiplicity: 1.0,
        },
    ],
    TotalItems: 2.0,
};

export const cartDataWithCustomOptions = {
    CartProducts: [
        {
            OfferId: 17570,
            Price: '872 руб.',
            ProductId: 6750,

            Amount: 1.0,
            Sku: '4710493',
            Name: 'Товар1',
            Cost: '872 руб.',
            ShoppingCartItemId: 1768,
            SelectedOptions: [
                {
                    CustomOptionId: 275,
                    OptionId: 1782,
                    CustomOptionTitle: 'Мультивыбор',
                    OptionTitle: 'Чтото2',
                    OptionPriceBc: 11.0,
                    OptionAmount: 1.0,
                    OptionPriceType: 0,
                },
                {
                    CustomOptionId: 275,
                    OptionId: 1783,
                    CustomOptionTitle: 'Мультивыбор',
                    OptionTitle: 'ч24',
                    OptionPriceBc: 222.0,
                    OptionAmount: 2.0,
                    OptionPriceType: 0,
                },
                {
                    CustomOptionId: 275,
                    OptionId: 1784,
                    CustomOptionTitle: 'Мультивыбор',
                    OptionTitle: 'фвап',
                    OptionPriceBc: 333.0,
                    OptionAmount: 1.0,
                    OptionPriceType: 0,
                },
                {
                    CustomOptionId: 276,
                    OptionId: 1785,
                    CustomOptionTitle: 'Список',
                    OptionTitle: 'ыыы',
                    OptionPriceBc: 34.0,
                    OptionAmount: 1.0,
                    OptionPriceType: 0,
                },
            ],
            ColorName: null,
            SizeName: null,
            Avalible: '',
            AvailableAmount: 45.0,
            MinAmount: 1.0,
            MaxAmount: 2.14748365e9,
            Multiplicity: 1.0,
            FrozenAmount: false,
            IsGift: false,
            Unit: 'шт',
            PriceRuleName: null,
        },
        {
            OfferId: 17570,
            Price: '117 руб.',
            ProductId: 6750,
            Amount: 1.0,
            Sku: '4710493',
            Name: 'Товар1',
            Cost: '117 руб.',
            ShoppingCartItemId: 1769,
            SelectedOptions: [
                {
                    CustomOptionId: 275,
                    OptionId: 1782,
                    CustomOptionTitle: 'Мультивыбор',
                    OptionTitle: 'Чтото2',
                    OptionPriceBc: 11.0,
                    OptionAmount: 3.0,
                    OptionPriceType: 0,
                },
                {
                    CustomOptionId: 276,
                    OptionId: 1785,
                    CustomOptionTitle: 'Список',
                    OptionTitle: 'ыыы',
                    OptionPriceBc: 34.0,
                    OptionAmount: 1.0,
                    OptionPriceType: 0,
                },
            ],
            ColorName: null,
            SizeName: null,
            Avalible: '',
            AvailableAmount: 45.0,
            MinAmount: 1.0,
            MaxAmount: 2.14748365e9,
            Multiplicity: 1.0,
            FrozenAmount: false,
            IsGift: false,
            Unit: 'шт',
            PriceRuleName: null,
        },

        {
            OfferId: 11111,
            Price: '117 руб.',
            ProductId: 6750,
            Amount: 1.0,
            Sku: '4710493',
            Name: 'Товар1 в чёрном',
            Cost: '117 руб.',
            ShoppingCartItemId: 1769,
            SelectedOptions: [
                {
                    CustomOptionId: 275,
                    OptionId: 1782,
                    CustomOptionTitle: 'Мультивыбор',
                    OptionTitle: 'Чтото2',
                    OptionPriceBc: 11.0,
                    OptionAmount: 3.0,
                    OptionPriceType: 0,
                },
                {
                    CustomOptionId: 276,
                    OptionId: 1785,
                    CustomOptionTitle: 'Список',
                    OptionTitle: 'ыыы',
                    OptionPriceBc: 34.0,
                    OptionAmount: 1.0,
                    OptionPriceType: 0,
                },
            ],
            ColorName: null,
            SizeName: null,
            Avalible: '',
            AvailableAmount: 45.0,
            MinAmount: 1.0,
            MaxAmount: 2.14748365e9,
            Multiplicity: 1.0,
            FrozenAmount: false,
            IsGift: false,
            Unit: 'шт',
            PriceRuleName: null,
        },
    ],
    ColorHeader: 'Цвет',
    SizeHeader: 'Размер',
    Count: '2 товара',
    TotalItems: 2.0,
    BonusPlus: '30 бонусов',
    Valid: '',
    CouponInputVisible: true,
    ShowConfirmButtons: true,
    ShowBuyInOneClick: true,
    BuyInOneClickText: 'Купить в один клик',
    TotalProductPrice: '989 руб.',
    TotalPrice: '989 руб.',
    DiscountPrice: null,
    Certificate: null,
    CertificateCode: null,
    Coupon: null,
    MobileIsFullCheckout: true,
    EnablePhoneMask: true,
    IsDefaultCustomerGroup: true,
    IsShowUnits: false,
};
export const addToCartRequest = {
    offerId: 0,
    productId: 1750,
    amount: 1,
    payment: null,
    offerIds: [],
};
export const updateCartRequestSingle = {
    items: [{ Key: 1495, Value: 2 }],
};
export const updateCartResponseSuccess = { TotalItems: 2.0, status: 'success' };
export const updateCartResponseFail = { status: 'fail' };
export const updateCartParams = {
    items: [
        { Key: 1495, Value: 2 },
        { Key: 111, Value: 0.65 },
    ],
};

export const removeFromCartRequest = { itemId: 1495 };
export const removeFromCartResponseZero = { TotalItems: 0.0, status: 'success', offerId: 12563 };
export const removeFromCartResponse = { TotalItems: 1.3, status: 'success', offerId: 12563 };

export const addToCartResultSucess = {
    status: 'success',
    url: '',
    cartId: 1495,
    TotalItems: 1.0,
    CartItem: {
        Amount: 1.0,
        OfferId: 12563,
        ArtNo: '3648489',
        IsGift: false,
        IsForbiddenChangeAmount: false,
        ModuleKey: null,
        FrozenAmount: false,
        Price: 406.0,
        CustomPrice: null,
        IsByCoupon: false,
    },
};
export const addItemServiceResultSucess = [addToCartResultSucess, cartData];

export const addToCartResultRedirect = {
    status: 'redirect',
    url: 'new-url',
    cartId: 0,
    TotalItems: 0.0,
    CartItem: null,
};

export const addItemServiceResultRedirect = [addToCartResultRedirect, cartData];

export const cartAddAttrsLp = {
    lpId: 11,
    lpUpId: 23,
    lpEntityId: 34,
    lpEntityType: 'type',
    lpBlockId: '4123',
    lpButtonName: 'lpButtonName',
};

export const cartAddAttrsSpinBox = {
    maxStepSpinbox: 25,
    minStepSpinbox: 0,
    stepSpinbox: 1,
};
export const cartAddAttrs = {
    cartAddValid: null,
    href: 'new-url',
    mode: '',
    hideShipping: '',
    source: '',
    modeFrom: '',
    offerId: 1495,
    productId: 1753,
    amount: 999,
    payment: '',
    forceHiddenPopup: false,
    attributesXml: {
        prop1: 'string',
        prop2: '2',
    },
    offerIds: [1495, 2, 4, 5, 6],
    cartAddType: 0, // 0 - classic, 1 -  withspinbox
};

export const cartAddNeedParamsKey = [
    'offerId',
    'productId',
    'amount',
    'attributesXml',
    'payment',
    'mode',
    'lpId',
    'lpUpId',
    'lpEntityId',
    'lpEntityType',
    'lpBlockId',
    'lpButtonName',
    'hideShipping',
    'offerIds',
    'modeFrom',
];

export const cartAddAttrsAll = {
    ...cartAddAttrs,
    ...cartAddAttrsLp,
    ...cartAddAttrsSpinBox,
};
export const cartAddNeedParams = Object.fromEntries(
    Object.entries(cartAddAttrsAll)
        .map(([key, value]) => {
            if (cartAddNeedParamsKey.includes(key)) {
                return [key, value];
            }
        })
        .filter((x) => x != undefined),
);
