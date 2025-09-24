import searchPanelTemplate from '../templates/searchPanel.html';
function searchIconDirective() {
    return {
        restrict: 'A',
        //replace: true,
        template:
            "<div class='searchBtn inked ink-light' ng-class='{\"cs-bg-13\": spCtrl.active}' ng-mouseleave='spCtrl.hidePanel()'><a href='javascript:void(0);' class='search-link icon-search-before icon-margin-drop cs-t-8' ng-click='spCtrl.togglePanel()'></a></div>",
        controller: 'searchPanelCtrl',
        controllerAs: 'spCtrl',
        bindToController: true,
    };
}

function searchPanelDirective() {
    return {
        restrict: 'A',
        replace: true,
        controller: 'searchPanelCtrl',
        controllerAs: 'spCtrl',
        bindToController: true,
        templateUrl: searchPanelTemplate,
    };
}

export { searchIconDirective, searchPanelDirective };
