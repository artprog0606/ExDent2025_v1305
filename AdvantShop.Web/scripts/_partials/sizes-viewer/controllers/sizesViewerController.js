function SizesViewerCtrl() {
    var ctrl = this;

    ctrl.$onInit = function () {
        if (ctrl.startSelectedSizes != null && ctrl.startSelectedSizes.length > 0) {
            for (var s = 0, lenS = ctrl.startSelectedSizes.length; s < lenS; s++) {
                for (var c = 0, lenC = ctrl.sizes.length; c < lenC; c++) {
                    if (ctrl.sizes[c].SizeId === ctrl.startSelectedSizes[s]) {
                        ctrl.sizeSelected = ctrl.sizes[c];
                        break;
                    }
                }
            }
        }

        ctrl.initSizes({ sizesViewer: ctrl });
    };
}

export default SizesViewerCtrl;
