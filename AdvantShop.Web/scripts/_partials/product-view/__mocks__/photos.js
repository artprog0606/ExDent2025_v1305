import { faker } from '@faker-js/faker';

export const getPhotoObj = () => {
    return {
        PathXSmall: faker.image.url(),
        PathSmall: faker.image.url(),
        PathMiddle: faker.image.url(),
        PathBig: faker.image.url(),
        ColorID: faker.number.int({ min: 1, max: 1000 }),
        PhotoId: faker.number.int({ min: 1, max: 1000 }),
        Description: faker.lorem.lines(1),
        Main: faker.datatype.boolean(0.5),
        XSmallProductImageHeight: faker.number.int({ min: 1, max: 1000 }),
        XSmallProductImageWidth: faker.number.int({ min: 1, max: 1000 }),
        SmallProductImageHeight: faker.number.int({ min: 1, max: 1000 }),
        SmallProductImageWidth: faker.number.int({ min: 1, max: 1000 }),
        MiddleProductImageWidth: faker.number.int({ min: 1, max: 1000 }),
        MiddleProductImageHeight: faker.number.int({ min: 1, max: 1000 }),
        BigProductImageWidth: faker.number.int({ min: 1, max: 1000 }),
        BigProductImageHeight: faker.number.int({ min: 1, max: 1000 }),
        Alt: faker.lorem.lines(1),
        Title: faker.lorem.lines(1),
    };
};
export const getInitialPhotoStartJson = (obj) => {
    return {
        PathSmall: obj.PathSmall,
        PathMiddle: obj.PathMiddle,
        PathBig: obj.PathBig,
    };
};
