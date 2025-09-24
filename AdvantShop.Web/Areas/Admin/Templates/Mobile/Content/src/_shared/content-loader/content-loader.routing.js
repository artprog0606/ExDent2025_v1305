const routesName = {
    home: `home`,
    catalog: `catalog`,
    orders: `orders`,
    dashboard: `dashboard`,
};

const routes = new Map();

routes.set(routesName.home, {
    urlPattern: /home/,
    fnModuleLoad: () => import(/* webpackChunkName: "homeLazyLoad" */ '../../../../bundle_config/home.source.js'),
});
routes.set(routesName.catalog, {
    urlPattern: /catalog/,
    fnModuleLoad: () => import(/* webpackChunkName: "catalogLazyLoad" */ '../../../../bundle_config/catalog.source.js'),
});
routes.set(routesName.orders, {
    urlPattern: /orders/,
    fnModuleLoad: () => import(/* webpackChunkName: "ordersLazyLoad" */ '../../../../bundle_config/orders.source.js'),
});
routes.set(routesName.dashboard, {
    urlPattern: /dashboard/,
    fnModuleLoad: () => import(/* webpackChunkName: "dashboardLazyLoad" */ '../../../../bundle_config/dashboard.source.js'),
});

class ContentRouteService {
    static findByUrl(url) {
        let result;

        for (const [name, data] of routes.entries()) {
            if (data.urlPattern.test(url) === true) {
                result = [name, data];
                break;
            }
        }

        return result;
    }
}

export { routesName, ContentRouteService };
