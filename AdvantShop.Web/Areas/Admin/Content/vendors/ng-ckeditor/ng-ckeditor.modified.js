(function (angular) {
    var app = angular.module('ngCkeditor', ['oc.lazyLoad']);
    var $defer, loaded = false;

    app.directive('ckeditor', /* @ngInject */function ($timeout, $q, $parse, ngCkeditorOptions, $ocLazyLoad, urlHelper) {
        'use strict';

        return {
            restrict: 'AC',
            require: ['ngModel', '^?form'],
            scope: true,
            link: function (scope, element, attrs, ctrls) {
                var ngModel = ctrls[0];
                var form = ctrls[1] || null;
                var EMPTY_HTML = '',
                    isTextarea = element[0].tagName.toLowerCase() == 'textarea',
                    data = null,
                    isReady = false;
                const ckeditorAutocomplete = attrs.ckeditorAutocompletePlugin != null;
                let ckeditorAutocompleteData = null;
                if (ckeditorAutocomplete && attrs.ckeditorAutocompletePluginData?.length === 0) {
                    console.error('ngCkeditor: attribute "ckeditorAutocompletePluginData" is required for autocomplete')
                } else {
                    ckeditorAutocompleteData = $parse(attrs.ckeditorAutocompletePluginData);
                }


                if (!isTextarea) {
                    element.attr('contenteditable', true);
                }

                var onLoad = function () {
                    var options = angular.copy(ngCkeditorOptions);
                    var customOptions = $parse(attrs.ckeditor)(scope);
                    let deregWatchNgModel;

                    options = angular.extend(options, customOptions);

                    var instance = (isTextarea) ? CKEDITOR.replace(element[0], options) : CKEDITOR.inline(element[0], options);

                    element[0].ckEditorInstance = instance;

                    var setModelData = function (setPristine, data) {
                        if (data == '') {
                            data = null;
                        }

                        if (data == ngModel.$viewValue) {
                            return;
                        }

                        if (timerChange != null) {
                            clearTimeout(timerChange);
                        }

                        if (setPristine !== true || data !== ngModel.$viewValue) {
                            ngModel.$setViewValue(data);
                        }

                        if (setPristine === true && form) {
                            form.$setPristine()
                        }

                        scope.$digest();
                    };

                    const watchNgModel = () => {
                        deregWatchNgModel = scope.$watch(() => ngModel.$modelValue, (newVal, oldVal) => {
                            if (newVal !== oldVal && newVal !== instance.getData()) {
                                if (timerChange != null) {
                                    clearTimeout(timerChange);
                                }
                                setTimeout(() => {
                                    instance.setData({
                                        data: newVal || '',
                                        internal: true,
                                    })
                                }, 100)
                            }
                        })
                    }

                    const watchDestroy = () => {
                        if (deregWatchNgModel != null) {
                            deregWatchNgModel();
                        }
                        deregWatchNgModel = null;
                    }

                    watchNgModel();

                    element.on('$destroy', function () {
                        delete element[0].ckEditorInstance;

                        instance.destroy(
                            false //If the instance is replacing a DOM element, this parameter indicates whether or not to update the element with the instance contents.
                        );

                        watchDestroy();

                        scope.$destroy();
                    });

                    instance.on('instanceReady', function (event) {

                        if (ckeditorAutocomplete) {
                            var autocomplete = new CKEDITOR.plugins.autocomplete(event.editor, {
                                textTestCallback: textTestCallback,
                                dataCallback: (matchInfo, callback) => dataCallback(matchInfo, callback, ckeditorAutocompleteData(scope))
                            });
                        }

                        scope.$broadcast("ckeditor.ready");
                    });

                    let timerChange;
                    instance.on('change', function () {
                        if (timerChange != null) {
                            clearTimeout(timerChange);
                        }
                        timerChange = setTimeout(() => {
                                watchDestroy();
                                setModelData(false, instance.getData());
                                watchNgModel();
                            },
                            200);
                    });
                };

                $q.when(typeof (CKEDITOR) !== 'undefined' && CKEDITOR != null && CKEDITOR.env.isCompatible === true ? true : $ocLazyLoad.load(urlHelper.getAbsUrl('./vendors/ckeditor/ckeditor.js?v=4.22.1', true)))
                    .then(function () {

                        CKEDITOR.disableAutoInline = true;

                        if (CKEDITOR.stylesSet.get('adminStyles') == null) {
                            CKEDITOR.stylesSet.add('adminStyles', [
                                // Block-level styles
                                {name: 'Заголовок', element: 'h1', attributes: {'class': 'h1'}},
                                {name: 'Подзаголовок 2 уровня', element: 'h2', attributes: {'class': 'h2'}},
                                {name: 'Подзаголовок 3 уровня', element: 'h3', attributes: {'class': 'h3'}},
                                {name: 'Подзаголовок 4 уровня', element: 'h4', attributes: {'class': 'h4'}},

                                // Inline styles
                                {name: 'Крупный текст', element: 'span', attributes: {'class': 'h1'}},
                                {name: 'Большой текст', element: 'span', attributes: {'class': 'h2'}},
                                {name: 'Средний текст', element: 'span', attributes: {'class': 'h3'}},
                            ]);
                        }

                        for (name in CKEDITOR.instances) {
                            if (CKEDITOR.instances[name].elementMode === 3) {
                                CKEDITOR.instances[name].destroy(true);
                            }
                        }

                        onLoad();
                    });
            }
        };
    });

    app.constant('ngCkeditorOptions', {
        autoParagraph: false,
        forceSimpleAmpersand: true,
        height: '250px',
        uiColor: '#FAFAFA',
        toolbar: [
            {name: 'source', items: ['Ace', 'Templates']},
            {
                name: 'elements',
                items: ['NumberedList', 'BulletedList', 'Link', 'Unlink', '-', 'Image', 'VideoYTEmbed', 'Table', 'HorizontalRule'],
            },
            {name: 'styles', items: ['Styles', 'Font', 'FontSize', 'lineheight']},
            '/',
            {name: 'text', items: ['Bold', 'Italic', 'Underline', 'Strike', 'Subscript', 'RemoveFormat']},
            {name: 'text', items: ['TextColor', 'BGColor']},
            {name: 'align', items: ['Outdent', 'Indent', 'JustifyLeft', 'JustifyCenter', 'JustifyRight', 'JustifyBlock']},
            {name: 'document', items: ['Paste', 'PasteText', 'PasteFromWord', 'Undo', 'Redo']},
            {name: 'icons', items: ['Emojione', 'Maximize']},
        ],
        removePlugins: 'dragdrop, basket',
        extraPlugins: 'scriptencode,lineheight,bold,emojione,fontSizeExtend,videoytembed,autocomplete,textmatch,textwatcher,ace',
        stylesSet: 'adminStyles',
        customConfig: window.CKEDITOR_BASEPATH + 'config.js?p=' + (window.v || Math.random()),
    });

    function textTestCallback(range) {
        if (!range.collapsed) {
            return null;
        }

        return CKEDITOR.plugins.textMatch.match(range, matchCallback);
    }

    function matchCallback(text, offset) {
        const pattern = /#(\w*)*$/;
        const match = text.slice(0, offset)
            .match(pattern);

        if (!match) {
            return null;
        }

        return {
            start: match.index,
            end: offset
        };
    }

    function dataCallback(matchInfo, callback, data) {
        const dataFiltered = [];
        let counter = 0;
        for (const item of data) {
            if (item.toLowerCase().indexOf(matchInfo.query.toLowerCase().trim()) === 0) {
                dataFiltered.push({id: counter++, name: item});
            }
        }

        callback(dataFiltered);
    }

    return app;
})(angular);
