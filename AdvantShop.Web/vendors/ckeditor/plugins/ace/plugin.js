var aceMain = ['ace.js']

var aceFiles = ['ext-language_tools.js'];

// var aceFiles = [
//     'theme-textmate.js',
//     'ext-searchbox.js',
//     'mode-css.js', 'mode-html.js', 'mode-javascript.js',
//     'snippets/text.js', 'snippets/css.js', 'snippets/html.js', 'snippets/javascript.js'];

var commandName = 'ace';

/**
 * Plugin definition
 */
CKEDITOR.plugins.add('ace', {
    icons: 'ace,ace-rtl',
    hidpi: true,
    lang: ['en', 'ru'],
    init: function (editor) {
        // load CSS for the dialog
        editor.on('instanceReady', function () {
            CKEDITOR.document.appendStyleSheet(this.path + 'dialogs/style.css');
        }.bind(this));

        // add the button in the toolbar
        editor.ui.addButton('Ace', {
            label: editor.lang.ace.toolbar,
            command: commandName,
            toolbar: 'ace'
        });

        // link the button to the command
        editor.addCommand(commandName, new CKEDITOR.dialogCommand('aceDialog'));

        // disable the button while the required js files are not loaded
        editor.getCommand(commandName).disable();

        // add the plugin dialog element to the plugin
        CKEDITOR.dialog.add('aceDialog', this.path + 'dialogs/ace.js');

        // Load the required js files
        // enable the button when loaded
        var pluginRoot = this.path;
        CKEDITOR.scriptLoader.load(aceMain.map(function (item) {
            return resolveURI(pluginRoot, item)
        }), function () {
            editor.getCommand(commandName).enable();
            CKEDITOR.scriptLoader.load(aceFiles.map(function (item) {
                return resolveURI(pluginRoot, item)
            }));
        });
    }
});


function resolveURI(root, filePath) {
    return root + 'ace-builds/src-noconflict/' + filePath;
}

