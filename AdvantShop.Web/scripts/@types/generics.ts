export type Camelize<T> = {
    [K in keyof T as CamelizeString<K>]: T[K];
};

export type CamelizeString<T extends PropertyKey> = T extends string ? Uncapitalize<T> : T;
