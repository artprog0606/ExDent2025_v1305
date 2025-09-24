export const cartAddConfigDefault = {
    callbackNames: {
        get: 'get',
        update: 'update',
        remove: 'remove',
        add: 'add',
        clear: 'clear',
        open: 'open',
    },
    cartAddType: {
        Classic: "Classic",
        WithSpinbox: "WithSpinbox",
    },
    cartStateButton: {
        add: "add",
        loading: "loading",
        update: "update"
    },
    cartMini: {
        delayHide: 3000,
    },
} as const

export type CartConfig = typeof cartAddConfigDefault;

