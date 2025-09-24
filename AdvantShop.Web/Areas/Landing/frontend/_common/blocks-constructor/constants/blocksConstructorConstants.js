(function (ng) {
    'use strict';

    var coefficientPadding = 5,
        arrayPaddingTop = {},
        arrayPaddingBotton = {},
        temp;

    for (var i = 0; i <= 75; i++) {
        temp = i * coefficientPadding;

        arrayPaddingTop[temp] = 'block-padding-top--' + temp;

        arrayPaddingBotton[temp] = 'block-padding-bottom--' + temp;
    }

    ng.module('blocksConstructor')
        .constant('blocksConstructorPaddingTop', arrayPaddingTop)
        .constant('blocksConstructorPaddingBottom', arrayPaddingBotton)
        .constant('blocksConstructorBackgroundColors', [
            {
                Color: 'white',
                ColorId: 'white',
                ColorCode: 'rgb(255, 255, 255)',
                ColorName: 'Белая',
            },
            {
                Color: 'darkwhite',
                ColorId: 'darkwhite',
                ColorCode: 'rgb(239, 239, 239)',
                ColorName: 'Темно-белая',
            },
            {
                Color: 'silver',
                ColorId: 'silver',
                ColorCode: 'rgb(222, 222, 222)',
                ColorName: 'Серебрянная',
            },
            {
                Color: 'silver2',
                ColorId: 'silver2',
                ColorCode: 'rgb(214, 214, 214)',
                ColorName: 'Серебрянная 2',
            },
            {
                Color: 'lilac',
                ColorId: 'lilac',
                ColorCode: 'rgb(229, 148, 163)',
                ColorName: 'Сиреневая',
            },
            {
                Color: 'pale-red',
                ColorId: 'pale-red',
                ColorCode: 'rgb(243, 114, 138)',
                ColorName: 'Бледно-красный',
            },
            {
                Color: 'red',
                ColorId: 'red',
                ColorCode: 'rgb(228, 73, 55)',
                ColorName: 'Красная',
            },
            {
                Color: 'red2',
                ColorId: 'red2',
                ColorCode: 'rgb(255, 1, 41)',
                ColorName: 'Красная 2',
            },
            {
                Color: 'orange',
                ColorId: 'orange',
                ColorCode: 'rgb(254, 126, 0)',
                ColorName: 'Оранжевая',
            },
            {
                Color: 'orange2',
                ColorId: 'orange2',
                ColorCode: 'rgb(243, 149, 0)',
                ColorName: 'Оранжевая 2',
            },
            {
                Color: 'yellow',
                ColorId: 'yellow',
                ColorCode: 'rgb(248, 195, 18)',
                ColorName: 'Желтая',
            },
            {
                Color: 'brightYellow',
                ColorId: 'brightYellow',
                ColorCode: 'rgb(255, 221, 0)',
                ColorName: 'Ярко желтая',
            },
            {
                Color: 'lightgreen',
                ColorId: 'lightgreen',
                ColorCode: 'rgb(156, 207, 49)',
                ColorName: 'Салатовый',
            },
            {
                Color: 'lightgreen2',
                ColorId: 'lightgreen2',
                ColorCode: 'rgb(144, 191, 51)',
                ColorName: 'Светло-зеленая 2',
            },
            {
                Color: 'green',
                ColorId: 'green',
                ColorCode: 'rgb(103, 179, 83)',
                ColorName: 'Зеленая',
            },
            {
                Color: 'green2',
                ColorId: 'green2',
                ColorCode: 'rgb(63, 192, 1)',
                ColorName: 'Зеленая 2',
            },
            {
                Color: 'dark-green',
                ColorId: 'dark-green',
                ColorCode: 'rgb(50, 127, 15)',
                ColorName: 'Темно-зеленая',
            },
            {
                Color: 'turquoise',
                ColorId: 'turquoise',
                ColorCode: 'rgb(123, 200, 164)',
                ColorName: 'Бирюзовая',
            },
            {
                Color: 'turquoise2',
                ColorId: 'turquoise2',
                ColorCode: 'rgb(114, 166, 170)',
                ColorName: 'Бирюзовая 2',
            },
            {
                Color: 'seablue',
                ColorId: 'seablue',
                ColorCode: 'rgb(66, 189, 230)',
                ColorName: 'Морская',
            },
            {
                Color: 'azure',
                ColorId: 'azure',
                ColorCode: 'rgb(0, 149, 192)',
                ColorName: 'Лазурная',
            },
            {
                Color: 'blue2',
                ColorId: 'blue2',
                ColorCode: 'rgb(55, 146, 197)',
                ColorName: 'Голубая 2',
            },
            {
                Color: 'blue3',
                ColorId: 'blue3',
                ColorCode: 'rgb(4, 118, 214)',
                ColorName: 'Синяя 3',
            },
            {
                Color: '_none',
                ColorId: '_none',
                ColorCode: 'rgb(6, 98, 193)',
                ColorName: 'Синяя',
            },
            {
                Color: 'pink2',
                ColorId: 'pink2',
                ColorCode: 'rgb(255, 75, 154)',
                ColorName: 'Розовая 2',
            },
            {
                Color: 'pink',
                ColorId: 'pink',
                ColorCode: 'rgb(219, 68, 188)',
                ColorName: 'Розовая',
            },
            {
                Color: 'purple2',
                ColorId: 'purple2',
                ColorCode: 'rgb(158, 42, 139)',
                ColorName: 'Фиолетовая 2',
            },
            {
                Color: 'purple',
                ColorId: 'purple',
                ColorCode: 'rgb(147, 100, 141)',
                ColorName: 'Фиолетовая',
            },
            {
                Color: 'darkblue',
                ColorId: 'darkblue',
                ColorCode: 'rgb(60, 102, 142)',
                ColorName: 'Темно-синяя',
            },
            {
                Color: 'blue-black',
                ColorId: 'blue-black',
                ColorCode: 'rgb(6, 59, 111)',
                ColorName: 'Темно-синяя 2',
            },
            {
                Color: 'tomato',
                ColorId: 'tomato',
                ColorCode: 'rgb(173, 60, 16)',
                ColorName: 'Томатная',
            },
            {
                Color: 'brown',
                ColorId: 'brown',
                ColorCode: 'rgb(134, 71, 21)',
                ColorName: 'Коричневая',
            },
            {
                Color: 'darkgray',
                ColorId: 'darkgray',
                ColorCode: 'rgb(134, 134, 134)',
                ColorName: 'Темно-серая',
            },
            {
                Color: 'dark',
                ColorId: 'dark',
                ColorCode: 'rgb(75, 79, 88)',
                ColorName: 'Темная',
            },

            {
                Color: 'black',
                ColorId: 'black',
                ColorCode: 'rgb(67, 65, 66)',
                ColorName: 'Черная',
            },
            {
                Color: 'black2',
                ColorId: 'black2',
                ColorCode: 'rgb(0, 0, 0)',
                ColorName: 'Черная 2',
            },
        ]);
})(window.angular);
