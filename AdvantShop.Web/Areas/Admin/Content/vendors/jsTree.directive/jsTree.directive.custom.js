/*
 * jstree.directive [http://www.jstree.com]
 * http://arvindr21.github.io/jsTree-Angular-Directive
 *
 * Copyright (c) 2014 Arvind Ravulavaru
 * Licensed under the MIT license.
 */

var ngJSTree = angular.module('jsTree.directive', []);

ngJSTree.constant('jsTree.config', {
    core: {
        themes: {
            //name: 'advantshop',
            //dots: false,
            //icons: false,
            //url: false

            name: 'advantshop',
            dots: false,
            icons: true,
            url: false,
        },
        data: {
            type: 'POST',
            data: function (node) {
                var self = this,
                    params = {
                        id: node.id,
                        state: node.state,
                        stateOriginal:
                            node.original != null ? node.original.state : null,
                        checkbox: self.settings.checkbox,
                    };

                return params;
            },
        },
    },
    search: {
        show_only_matches: true,
        search_callback: function (str, node) {
            return node.original.name.toLowerCase().includes(str.toLowerCase());
        },
    },
});

ngJSTree.directive('jsTree', [
    '$compile',
    '$http',
    '$parse',
    'jsTree.config',
    '$ocLazyLoad',
    'urlHelper',
    '$timeout',
    '$q',
    '$translate',
    function (
        $compile,
        $http,
        $parse,
        jsTreeConfig,
        $ocLazyLoad,
        urlHelper,
        $timeout,
        $q,
        $translate,
    ) {
        var treeDir = {
            restrict: 'EA',
            scope: true,
            fetchResource: function (url, cb) {
                return $http.get(url).then(function (data) {
                    if (cb) cb(data.data);
                });
            },

            managePlugins: function (s, e, a, config) {
                if (a.treePlugins) {
                    config.plugins = a.treePlugins.split(',');
                    config.core = config.core || {};
                    config.core.check_callback =
                        config.core.check_callback || true;

                    if (config.plugins.indexOf('state') >= 0) {
                        config.state = config.state || {};
                        config.state.key = a.treeStateKey;
                    }

                    if (config.plugins.indexOf('search') >= 0) {
                        config.search = Object.assign(
                            {},
                            jsTreeConfig.search,
                            $parse(a.treeSearch)(s),
                        );
                        var to;
                        if (e.prev().attr('class') !== 'ng-tree-search') {
                            let notFound = $(
                                '<div class="jstree-not-found">' +
                                $translate.instant(
                                    'Admin.Js.Search.NothingFound',
                                ) +
                                '</div>',
                            );
                            e.before(
                                '<div class="form-group">' +
                                '<div class="input-group full-width">' +
                                '<div class="input-group-btn jstree-search__btn-wrap">' +
                                '<button class="btn btn-success btn-sm btn--slim jstree-search-btn" type="button">' +
                                '<span class="jstree-search__btn-text">Поиск</span>' +
                                '<svg class="jstree-search__btn-icon" width="18" height="18" viewBox="0 0 18 18" fill="none" xmlns="http://www.w3.org/2000/svg" hidden>' +
                                '<path d="M12.5 11h-.79l-.28-.27A6.471 6.471 0 0013 6.5 6.5 6.5 0 106.5 13c1.61 0 3.09-.59 4.23-1.57l.27.28v.79l5 4.99L17.49 16l-4.99-5zm-6 0C4.01 11 2 8.99 2 6.5S4.01 2 6.5 2 11 4.01 11 6.5 8.99 11 6.5 11z" fill="currentColor" />' +
                                '</svg>' +
                                '</button>' +
                                '</div>' +
                                '<input type="text" placeholder="Поиск по категориям" class="ng-tree-search form-control jstree-search__input input-sm m-b input-alt"/>' +
                                '</div>' +
                                '</div>',
                            )
                                .before(notFound)
                                .prev()
                                .prev()
                                .on('keyup', function (ev) {
                                    if (
                                        ev.target.value.length > 0 &&
                                        ev.target.value.length < 2
                                    ) {
                                        return;
                                    }
                                    if (to) {
                                        clearTimeout(to);
                                    }
                                    to = setTimeout(function () {
                                        e.addClass('jstree-search-loading').css(
                                            'height',
                                            e.outerHeight(),
                                        );
                                        $q.when(
                                            treeDir.tree
                                                .jstree(true)
                                                .search(ev.target.value),
                                        ).then(function (data) {
                                            //data == null, когда удалили из инпута текст
                                            e[
                                                data == null || data.length > 0
                                                    ? 'removeClass'
                                                    : 'addClass'
                                                ]('jstree-hidden');
                                            notFound[
                                                data == null || data.length > 0
                                                    ? 'removeClass'
                                                    : 'addClass'
                                                ]('jstree-not-found--visible');
                                            e.removeClass(
                                                'jstree-search-loading',
                                            ).css('height', 'auto');
                                        });
                                    }, 500);
                                });
                        }
                    }

                    if (config.plugins.indexOf('checkbox') >= 0) {
                        config.checkbox = angular.extend(
                            {},
                            config.checkbox,
                            $parse(a.treeCheckbox)(s),
                        );
                        config.checkbox.keep_selected_style = false;
                        //to enabled tree node working only cascad down
                        //config.checkbox.cascade = "down";
                        //config.checkbox.three_state = false;
                    }

                    if (config.plugins.indexOf('contextmenu') >= 0) {
                        if (a.treeContextmenu) {
                            config.contextmenu = $parse(a.treeContextmenu)(s);
                        }
                    }

                    if (config.plugins.indexOf('types') >= 0) {
                        if (a.treeTypes) {
                            config.types = $parse(a.treeTypes)(s);
                        }
                    }

                    if (config.plugins.indexOf('dnd') >= 0) {
                        if (a.treeDnd) {
                            config.dnd = $parse(a.treeDnd)(s);
                        }
                    }
                }
                return config;
            },
            manageEvents: function (s, e, a) {
                if (a.treeEvents) {
                    var evMap = a.treeEvents.split(';');
                    for (var i = 0; i < evMap.length; i++) {
                        if (evMap[i].length > 0) {
                            // plugins could have events with suffixes other than '.jstree'
                            var evt = evMap[i].split(':')[0];
                            if (evt.indexOf('.') < 0) {
                                evt = evt + '.jstree';
                            }

                            var cb = evMap[i].split(':')[1],
                                cbParsed = $parse(cb)(s);

                            bind(evt, cbParsed);
                        }
                    }
                }

                function bind(evt, fn) {
                    treeDir.tree.on(evt, function (event, data) {
                        fn(event, data);
                        //$timeout(function () {
                        s.$apply();
                        //}, 0);
                    });
                }
            },
            link: function (s, e, a) {
                // scope, element, attribute \O/
                //$(function () {
                s.isDraging = false;

                import(
                    /* webpackChunkName: "jstreeChunk" */
                    /* webpackMode: "lazy" */
                    'jstree')
                    .then(function (module) {
                        var config = Object.assign({}, jsTreeConfig);

                        if (a.treeCore) {
                            config.core = $.extend(
                                config.core,
                                $parse(a.treeCore)(s),
                            );
                        }

                        // clean Case
                        a.treeData = a.treeData ? a.treeData.toLowerCase() : '';
                        a.treeSrc = a.treeSrc ? a.treeSrc.toLowerCase() : '';

                        if (a.treeData == 'html') {
                            treeDir.fetchResource(a.treeSrc, function (data) {
                                e.html(data);
                                treeDir.init(s, e, a, config);
                            });
                        } else if (a.treeData == 'json') {
                            treeDir.fetchResource(a.treeSrc, function (data) {
                                config.core.data = data;
                                treeDir.init(s, e, a, config);
                            });
                        } else if (a.treeData == 'scope') {
                            s.$watch(
                                a.treeModel,
                                function (n, o) {
                                    if (n) {
                                        config.core.data = s[a.treeModel];
                                        $(e).jstree('destroy');
                                        treeDir.init(s, e, a, config);
                                    }
                                },
                                true,
                            );
                            // Trigger it initally
                            // Fix issue #13
                            config.core.data = s[a.treeModel];
                            treeDir.init(s, e, a, config);
                        } else if (a.treeAjax) {
                            var dataFnOld = angular.copy(config.core.data.data);
                            //config.core.data = angular.extend({}, config.core.data, a.treeAjaxData != null ? $parse(a.treeAjaxData)(s) : {});

                            var showRootIsProcessed = false;
                            config.core.data.data = function (node) {
                                var data = angular.extend(
                                    {},
                                    dataFnOld.call(this, node),
                                    a.treeAjaxData != null
                                        ? $parse(a.treeAjaxData)(s)
                                        : {},
                                );

                                var showRoot = e.attr('data-show-root');
                                if (
                                    showRoot != null &&
                                    showRootIsProcessed === false
                                ) {
                                    data.showRoot = true;
                                    e.removeAttr('data-show-root');
                                    showRootIsProcessed = true;
                                }

                                return data;
                            };

                            config.core.data.url = urlHelper.getAbsUrl(
                                a.treeAjax,
                            );
                            treeDir.init(s, e, a, config);
                        }
                    });
            },
            init: function (s, e, a, config) {
                treeDir.managePlugins(s, e, a, config);

                var treeElement = (this.tree = jQuery(e).jstree(config));

                treeElement.on('select_node.jstree', function (evt, data) {
                    if (data.event != null) {
                        $(data.event.target)
                            .closest('.jstree-node')
                            .addClass('jstree-node-selected');
                    }
                });

                treeElement.on('deselect_node.jstree', function (evt, data) {
                    if (data.event != null) {
                        $(data.event.target)
                            .closest('.jstree-node')
                            .removeClass('jstree-node-selected');
                    }
                });

                treeElement.on('open_node.jstree', function (evt, data) {
                    var jsTreeObj = treeElement.jstree();
                    $compile(jsTreeObj.get_node(data.node.id, true))(s);
                });

                treeElement.on('redraw.jstree', function (evt, data) {
                    if (data.nodes.length >= 1) {
                        $compile(treeElement.contents())(s);
                    }
                    //else {
                    //    $compile(treeElement.jstree().get_node(data.nodes[0].id || data.nodes[0], true))(s);
                    //}
                });

                treeElement.on('load_node.jstree', function (evt, data) {
                    var jsTreeObj = treeElement.jstree();
                    var itemTemp;
                    if (
                        config.checkbox != null &&
                        config.checkbox.cascade === 'down' &&
                        data.node.children.length > 0
                    ) {
                        for (
                            var i = 0, len = data.node.children.length;
                            i < len;
                            i++
                        ) {
                            itemTemp = jsTreeObj.get_node(
                                data.node.children[i],
                            );

                            if (
                                itemTemp.state.selected !==
                                itemTemp.original.state.selected
                            ) {
                                if (itemTemp.original.state.selected === true) {
                                    jsTreeObj.select_node(
                                        itemTemp,
                                        true,
                                        itemTemp.original.state.opened,
                                        {parentEvent: 'load_node'},
                                    );
                                } else {
                                    jsTreeObj.deselect_node(itemTemp, true, {
                                        parentEvent: 'load_node',
                                    });
                                }
                            }
                        }
                    }
                });

                treeElement.on('ready.jstree', function () {
                    var jsTreeObj = treeElement.jstree();
                    // Если есть элементы  в дереве. В data всегда есть элемент "#" поэтому 2
                    if (Object.keys(jsTreeObj._model.data).length < 2) {
                        e[0].insertAdjacentHTML(
                            'beforeend',
                            '<div class="text-center bold p-b">' +
                                $translate.instant(
                                    'Admin.Js.Partials.CategoryBlock.NoCategoryFound',
                                ) +
                            '</div>',
                        );
                    }
                });

                $(document).on(
                    'dnd_start.vakata.jstree',
                    function (event, data) {
                        $timeout(() => {
                            s.isDraging = true;
                        }, 0);

                        document.documentElement.classList.add(
                            'jstree-draging',
                        );
                    },
                );

                $(document).on(
                    'dnd_stop.vakata.jstree',
                    function (event, data) {
                        $timeout(() => {
                            s.isDraging = false;
                        }, 0);
                        document.documentElement.classList.remove(
                            'jstree-draging',
                        );
                    },
                );

                treeDir.manageEvents(s, e, a);

                if (a.treeOnInit != null) {
                    $parse(a.treeOnInit)(s, {jstree: treeElement.jstree()});
                }
            },
        };

        return treeDir;
    },
]);
