import { MOBILE_SIDEBAR_DEFAULT_OPTIONS } from '../../consts/consts.js';

const _sidebarsContainerService = new WeakMap();

class MainMenu {
    /*@ngInject*/
    constructor(sidebarsContainerService, $scope, $http, $compile, $rootScope, contentLoaderService, $timeout) {
        _sidebarsContainerService.set(this, sidebarsContainerService, $http);
        this.$scope = $scope;
        this.$http = $http;
        this.$compile = $compile;
        this.$rootScope = $rootScope;
        this.$timeout = $timeout;
        this.contentLoaderService = contentLoaderService;
        this.sidebarOption = {
            ...MOBILE_SIDEBAR_DEFAULT_OPTIONS,
            sidebarClass: `sidebar--main-menu`,
            contentId: `main-menu`,
            loader: this.shimmerContent.trim(),
            scope: this.$scope.$new(),
        };
        this.changeActiveMenu(``);
        this.handlePopState = (e) => this.closeMenuFromBrowserArrow(e);
    }

    get shimmerElement() {
        if (this.shimmerElement) {
            return this.shimmerElement;
        } else {
            const div = document.createElement('div');
            div.innerHTML = this.shimmerContent.trim();
            return div;
        }
    }

    get shimmerContent() {
        return `<div class="shimmer-wrapper">
            <div class=" main-menu-top-block">
                <div class="top-panel-user">
                    <div class="top-panel-user__photo">
                        <div class="shimmer-circle shimmer-circle-md shimmer-animate"></div>
                    </div>
                    <div class="top-panel-user__info">
                        <div class="top-panel-user__fio">
                            <div class="shimmer-line shimmer-line-br shimmer-line-60 shimmer-animate m-n"></div>
                        </div>
                        <div class="top-panel-user__edit">
                            <div class="shimmer-line shimmer-line-br shimmer-line-60 shimmer-animate m-t-md"></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="p-md p-t-md p-b-sm main-menu__category-name">
                <div class="shimmer-line shimmer-line-br shimmer-line-10 shimmer-animate m-n"></div>
            </div>
            <div class="p-l-md p-r-md p-b-sm main-menu__category-name">
                <div class="shimmer-line shimmer-line-br shimmer-line-20 shimmer-animate m-n"></div>
            </div>
            
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            
            <div class="p-l-md p-t-sm p-r-md p-b-sm">
                <div class="shimmer-line shimmer-line-br shimmer-line-30 shimmer-animate m-n"></div>
            </div>
            
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            
            <div class="p-l-md p-t-sm p-r-md p-b-sm">
                <div class="shimmer-line shimmer-line-br shimmer-line-30 shimmer-animate m-n"></div>
            </div>
            
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
           <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
            <div class="navigation-item navigation-item--horizontal p-l-md p-r-md p-t-sm p-b-sm flex">
                <div class="shimmer-line shimmer-line-br shimmer-line-full shimmer-animate m-n"></div>
            </div>
        </div>`;
    }
    getContent(url) {
        this.changeActiveMenu(url);

        this.contentLoaderService.getContent(url).then(() => {
            this.closeMenu();
        });
        this.$timeout(() => {}, 0);
    }

    openMenu() {
        const sidebarContainerService = _sidebarsContainerService.get(this);
        const stateSidebar = sidebarContainerService.getState();
        this.changeActiveMenu(``);
        if (stateSidebar == null) {
            sidebarContainerService.open(this.sidebarOption);
        }
    }

    toggleMenu() {
        const sidebarContainerService = _sidebarsContainerService.get(this);
        const stateSidebar = sidebarContainerService.getState();
        if (stateSidebar == null) {
            window.addEventListener('popstate', this.handlePopState);
            window.history.pushState({ menu: 'open' }, null, window.location.href);
            this.changeActiveMenu(`openSidebarMenu`);
        } else {
            this.changeActiveMenu(this.prevActiveMenu);
            window.removeEventListener('popstate', this.closeMenuFromBrowserArrow);
        }
        sidebarContainerService.toggle(this.sidebarOption);
    }

    changeActiveMenu(urlMenuLink = '') {
        this.prevActiveMenu = this.menuSelected;
        this.menuSelected = urlMenuLink;

        let conteinerBottomFixed = document.querySelector('.container-bottom-fixed');
        if (this.menuSelected === 'openSidebarMenu') {
            conteinerBottomFixed.classList.add('container-bottom-fixed--fixed');
        } else {
            conteinerBottomFixed.classList.remove('container-bottom-fixed--fixed');
        }
    }

    closeMenuFromBrowserArrow(event) {
        event.preventDefault();
        this.toggleMenu();
        window.history.replaceState({ menu: 'close' }, null, window.location.href);
        window.removeEventListener('popstate', this.handlePopState);
    }

    closeMenu(e) {
        _sidebarsContainerService.get(this).close();
    }
}

export { MainMenu };
