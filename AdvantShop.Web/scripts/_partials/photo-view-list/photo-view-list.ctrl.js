export default function PhotoViewListCtrl() {
    const ctrl = this;
    ctrl.updateActiveElements = function () {
        ctrl.activeNavIndex = 0;
        ctrl.activeItemIndex = 0;
    };
}
