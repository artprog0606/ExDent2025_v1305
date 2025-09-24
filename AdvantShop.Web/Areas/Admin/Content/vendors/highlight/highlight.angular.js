import hljs from 'highlight.js/lib/core';
import 'highlight.js/styles/vs.css';
import csharp from 'highlight.js/lib/languages/csharp';

hljs.registerLanguage('csharp', csharp);

(function (ng) {
    'use strict';

    ng.module('highlight', ['oc.lazyLoad'])
        .directive('highlight', ['$ocLazyLoad', 'urlHelper', function ($ocLazyLoad, urlHelper) {
            return {
                link: function (scope, element, attrs) {
                    setTimeout(()=> {
                        const code = element[0].textContent;
                        const result = hljs.highlight(code, {
                            language: 'csharp',
                            ignoreIllegals: true
                        }).value;

                        element.html(result);
                    }, 100);
                }
            };
        }]);
})(window.angular);
