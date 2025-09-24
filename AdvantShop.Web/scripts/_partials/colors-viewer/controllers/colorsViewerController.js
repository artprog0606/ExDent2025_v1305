/* @ngInject */
function ColorsViewerCtrl($element, $scope, $transclude) {
    var ctrl = this,
        stopLoop = false,
        isFindedByStart = false,
        findedStartColorById,
        findedStartColorByIdAndMain;

    ctrl.$onInit = function () {
        ctrl.dirty = false;

        ctrl.multiselect = ctrl.multiselect === true;

        if (ctrl.multiselect === true) {
            ctrl.colorSelected = [];

            if (ctrl.colors != null && ctrl.colors.length > 0) {
                for (var i = 0, len = ctrl.colors.length; i < len; i++) {
                    if (ctrl.colors[i].Selected === true) {
                        ctrl.colorSelected.push(ctrl.colors[i]);
                    }
                }
            }

            ctrl.colorSelectedNamesList = ctrl.colorSelected.join(',');
        } else {
            if (ctrl.startSelectedColors != null && ctrl.startSelectedColors.length > 0) {
                for (var s = 0, lenS = ctrl.startSelectedColors.length; s < lenS; s++) {
                    for (var c = 0, lenC = ctrl.colors.length; c < lenC; c++) {
                        if (ctrl.colors[c].ColorId === ctrl.startSelectedColors[s]) {
                            if (ctrl.colors[c].Main === 1) {
                                findedStartColorByIdAndMain = ctrl.colors[c];
                                stopLoop = true;
                            } else if (findedStartColorById == null) {
                                findedStartColorById = ctrl.colors[c];
                            }

                            isFindedByStart = true;
                        }
                    }

                    if (stopLoop === true) {
                        stopLoop = false;
                        break;
                    }
                }
            }

            if (isFindedByStart === false) {
                for (var m = 0, l = ctrl.colors.length; m < l; m++) {
                    if (ctrl.colors[m].Main == true) {
                        ctrl.colorSelected = ctrl.colors[m];
                        break;
                    }
                }
                if (ctrl.colorSelected == null) {
                    ctrl.colorSelected = ctrl.colors[0];
                }
            } else {
                ctrl.colorSelected = findedStartColorByIdAndMain || findedStartColorById;
            }
        }

        if (ctrl.initColors != null) {
            ctrl.initColors({ colorsViewer: ctrl });
        }
    };

    ctrl.$postLink = function () {
        if (ctrl.carousel != null) {
            var colorSelectedFirst = ctrl.multiselect ? ctrl.colorSelected[0] : ctrl.colorSelected;
            var colorSelectedFirstIndex = ctrl.colors.indexOf(colorSelectedFirst);
            ctrl.carousel.load = true;
            ctrl.carousel.whenCarouselInit().then(function (carousel) {
                carousel.carouselNative.setItemSelect(colorSelectedFirstIndex);
                carousel.carouselNative.goto(colorSelectedFirstIndex, false);
            });
        }
    };

    ctrl.getImagePath = function (photoName) {
        return photoName.indexOf('://') != -1 ? photoName : 'pictures/color/' + ctrl.imageType.toLowerCase() + '/' + photoName;
    };

    ctrl.selectColor = function (event, color) {
        var indexInSelectedArray;

        if (ctrl.multiselect === true) {
            indexInSelectedArray = ctrl.colorSelected.indexOf(color);

            if (indexInSelectedArray > -1) {
                ctrl.colorSelected.splice(indexInSelectedArray, 1);
                //свойство используется в фильтрах
                color.Selected = false;
            } else {
                ctrl.colorSelected.push(color);
                //свойство используется в фильтрах
                color.Selected = true;
            }

            ctrl.colorSelectedNamesList = ctrl.colorSelected.join(',');
        } else {
            ctrl.colorSelected = color;
        }

        ctrl.dirty = true;

        ctrl.changeColor({ event: event, color: color });
    };

    ctrl.selectColorById = function (colorId) {
        var color = ctrl.colors.filter(function (x) {
            return x.ColorId == colorId;
        });
        if (color != null && color.length > 0) {
            ctrl.selectColor(null, color[0]);
        }
    };

    ctrl.getDirtyState = function () {
        return ctrl.dirty;
    };
}

export default ColorsViewerCtrl;
