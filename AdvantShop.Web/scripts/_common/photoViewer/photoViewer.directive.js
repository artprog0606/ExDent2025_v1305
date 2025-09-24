const imageExts = ['.jpg', '.jpeg', '.gif', '.png', '.webp'];

function create(target, opts, $ocLazyLoad, $injector) {
    let el;
    let type;

    if (target.nodeName === `A`) {
        let href = target.getAttribute('href');
        if (href.startsWith('#')) {
            type = 'html';
        } else if (imageExts.some((ext) => href.endsWith(ext))) {
            type = 'image';
        }
    } else {
        type = 'image';
    }

    if (type === 'image') {
        let withoutTagImg = false;

        if (target.nodeName !== `A`) {
            target.addEventListener(`click`, (e) => {
                if (e.target.closest('a')) {
                    e.preventDefault();
                }
            });
            el = target;
        } else {
            el = target.querySelector(`img`);
        }

        if (el == null) {
            withoutTagImg = true;
            let img = new Image();
            img.dataset.original = target.getAttribute('href');
            el = img;
        }

        return import(
            /* webpackChunkName: "viewerjs" */
            /* webpackMode: "lazy" */
            './viewerjs.module.js'
        ).then((module) => {
            let viewer = new module.default(
                el,
                Object.assign(
                    {
                        moveLimit: true,
                        toggleSizeToInitial: true,
                        url(image) {
                            return image.dataset.original || image.closest(`a`).href;
                        },
                        viewed: function () {
                            viewer.options.minZoomRatio = Math.min(viewer.imageData.width / viewer.imageData.naturalWidth, 1);
                        },
                        ready: function () {
                            if (viewer.images == null || viewer.images.length === 1) {
                                viewer.options.toolbar = false;
                                //viewer.update();
                                viewer.toolbar.remove();
                            }
                        },
                    },
                    opts,
                ),
            );

            if (target.nodeName === `A`) {
                target.addEventListener(`click`, (e) => {
                    e.preventDefault();
                    e.stopPropagation();

                    if (withoutTagImg) {
                        viewer.show();
                    }
                });
            }

            return viewer;
        });
    } else if (type === 'html' || opts.type === 'iframe') {
        target.addEventListener(`click`, (e) => {
            e.preventDefault();
            let href = target.getAttribute('href');

            return new Promise((resolve, reject) => {
                if ($ocLazyLoad.isLoaded('modal') === false) {
                    import(
                        /* webpackChunkName: "modal" */
                        /* webpackMode: "lazy" */
                        '../modal/modal.module.js'
                    )
                        .then(() => $ocLazyLoad.inject('modal'))
                        .then(() => resolve())
                        .catch((err) => reject(err));
                } else {
                    resolve();
                }
            })
                .then(() => $injector.get('modalService'))
                .then((modalService) => {
                    if (type === 'html') {
                        modalService.renderModal(href.slice(1), null, document.querySelector(href).innerHTML, null, {
                            isOpen: true,
                            destroyOnClose: true,
                        });
                    } else if (opts.type === 'iframe') {
                        return new Promise((resolve, reject) => {
                            if ($ocLazyLoad.isLoaded('iframeResponsive') === false) {
                                return import(
                                    /* webpackChunkName: "iframe-responsive" */
                                    /* webpackMode: "lazy" */
                                    '../iframe-responsive/iframeResponsive.module.js'
                                )
                                    .then(() => $ocLazyLoad.inject('iframeResponsive'))
                                    .then(() => resolve())
                                    .catch((err) => reject(err));
                            } else {
                                resolve();
                            }
                        })
                            .then(() => $ocLazyLoad.inject('iframeResponsive'))
                            .then(() => {
                                modalService.renderModal(
                                    href.replace(/[\W\D]/g, ''),
                                    null,
                                    `<iframe-responsive src="${href}" autoplay="true" in-modal="true" data-from-upload="false"></iframe-responsive>`,
                                    null,
                                    {
                                        isOpen: true,
                                        destroyOnClose: true,
                                        modalClass: 'photo-viewer-modal',
                                    },
                                );
                            });
                    }
                });
        });
    }
}

//https://stackoverflow.com/questions/3960843/how-to-find-the-nearest-common-ancestors-of-two-or-more-nodes
function getCommonAncestor(node1, node2) {
    var method = 'contains' in node1 ? 'contains' : 'compareDocumentPosition',
        test = method === 'contains' ? 1 : 0x0010;

    while ((node1 = node1.parentNode)) {
        if ((node1[method](node2) & test) == test) return node1;
    }

    return null;
}

//TODO: compatibility with old version
/*@ngInject*/
function pluginDirective(photoViewerDefaultOptions, $ocLazyLoad, $injector, $q, $parse) {
    return {
        link: function (scope, element, attrs) {
            if (attrs.plugin === 'fancybox') {
                const rel = element[0].getAttribute(`rel`);
                const isNotImage = false;
                let el;

                if (rel != null) {
                    const allItems = Array.from(document.querySelectorAll(`[data-plugin="fancybox"][rel="${rel}"]`));
                    if (allItems.length === 1) {
                        el = element[0];
                    } else {
                        if (allItems[0].nextElementSibling === allItems[1]) {
                            el = element[0].parentNode;
                        } else {
                            el = getCommonAncestor(allItems[0], allItems[1]);
                        }
                    }
                } else {
                    el = element[0];
                }

                $q.when(
                    create(
                        el,
                        angular.extend({}, photoViewerDefaultOptions, {
                            url: function (image) {
                                let src = image.closest('[data-plugin="fancybox"]')?.href;
                                return src || image.dataset.original;
                            },
                        }),
                        $ocLazyLoad,
                        $injector,
                    ),
                ).then((photoViewer) => {
                    scope.photoViewer = {
                        original: photoViewer,
                    };
                    if (attrs.photoViewerOnLoad != null) {
                        $parse(attrs.photoViewerOnLoad)(scope);
                    }
                });
            }
        },
    };
}

//TODO: compatibility with old version
/*@ngInject*/
function magnificPopupDirective($parse, photoViewerDefaultOptions, $ocLazyLoad, $injector, $q) {
    return {
        link: function (scope, element, attrs) {
            $q.when(
                create(element[0], angular.extend({}, photoViewerDefaultOptions, $parse(attrs.magnificPopupOptions)(scope)), $ocLazyLoad, $injector),
            ).then((photoViewer) => {
                scope.photoViewer = {
                    original: photoViewer,
                };

                scope.photoViewer.update = function () {
                    setTimeout(() => photoViewer.updateItemHTML(), 100);
                };

                if (attrs.photoViewerOnLoad != null) {
                    $parse(attrs.photoViewerOnLoad)(scope);
                }
            });
        },
    };
}

/*@ngInject*/
function photoViewerDirective($parse, photoViewerDefaultOptions, $ocLazyLoad, $injector, $q) {
    return {
        link: function (scope, element, attrs) {
            $q.when(
                create(
                    element[0],
                    angular.extend({}, photoViewerDefaultOptions, $parse(attrs.photoViewerDefaultOptions)(scope)),
                    $ocLazyLoad,
                    $injector,
                ),
            ).then((photoViewer) => {
                scope.photoViewer = {
                    original: photoViewer,
                };

                scope.photoViewer.update = function () {
                    setTimeout(() => photoViewer.update(), 100);
                };

                if (attrs.photoViewerOnLoad != null) {
                    $parse(attrs.photoViewerOnLoad)(scope);
                }
            });
        },
    };
}

export { pluginDirective, magnificPopupDirective, photoViewerDirective };
