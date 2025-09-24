import ace from 'ace-builds/src-noconflict/ace.js';

import ext from 'ace-builds/src-noconflict/ext-language_tools.js';
import extSearch from 'ace-builds/src-noconflict/ext-searchbox.js';

ace.define('ace/ext/language_tools', null, ext);
ace.define('ace/ext/searchbox', null, extSearch);

import modeCss from 'ace-builds/src-noconflict/mode-css.js';
import modeHtml from 'ace-builds/src-noconflict/mode-html.js';
import modeJavascript from 'ace-builds/src-noconflict/mode-javascript.js';

ace.config.setModuleUrl('ace/mode/css', modeCss);
ace.config.setModuleUrl('ace/mode/html', modeHtml);
ace.config.setModuleUrl('ace/mode/javascript', modeJavascript);

import snippetText from 'ace-builds/src-noconflict/snippets/text.js';
import snippetCss from 'ace-builds/src-noconflict/snippets/css.js';
import snippetHtml from 'ace-builds/src-noconflict/snippets/html.js';
import snippetJavascript from 'ace-builds/src-noconflict/snippets/javascript.js';

ace.config.setModuleUrl('ace/snippets/text', snippetText);
ace.config.setModuleUrl('ace/snippets/css', snippetCss);
ace.config.setModuleUrl('ace/snippets/html', snippetHtml);
ace.config.setModuleUrl('ace/snippets/javascript', snippetJavascript);

ace.config.set('useWorker', false);

import '../Content/vendors/ace/ui-ace.js';
