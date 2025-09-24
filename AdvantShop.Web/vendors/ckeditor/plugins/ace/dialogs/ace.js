CKEDITOR.dialog.add('aceDialog', function (editor) {
    // CKEditor variables
    var dialog;
    // ACE variables
    var aceEditor, aceSession, whitespace;

    // EDITOR panel
    var editorPanel = {
        id: 'editor',
        label: editor.lang.ace.editor,
        elements: [
            {
                type: 'html',
                html: '<div><div style="position: absolute; top: 0px; left: 0px; right: 0px; bottom: 0px;" id="ckeditor-ace-container"></div></div>',
                id: 'code-textarea',
                commit: function (element) {
                    element.setText(aceEditor.getValue());
                }
            }
        ]
    };

    var sizeDialog = CKEDITOR.document.getWindow().getViewPaneSize(),
        minWidth = Math.min(sizeDialog.width - 70, 800),
        minHeight = sizeDialog.height / 1.5;

    // dialog code
    return {
        // Basic properties of the dialog window: title, minimum size.
        title: editor.lang.ace.title,
        minWidth: minWidth,
        minHeight: minHeight,
        // Dialog window contents definition.
        contents: [
            editorPanel
        ],
        onLoad: function () {
            dialog = this;
        },
        onShow: function () {
            dialog.getElement().removeClass('cke_reset_all').addClass('cke_container_dialog_ace');
            var aceHtmlElement = dialog.getContentElement('editor', 'code-textarea').getElement().find('#ckeditor-ace-container').getItem(0).$;
            ace.require("ace/ext/language_tools");
            ace.require("ace/ext/searchbox");
            // we load the ACE plugin to our div
            aceEditor = ace.edit(aceHtmlElement);
            // save the aceEditor into the editor object for the resize event
            editor.aceEditor = aceEditor;
            // set default settings
            aceEditor.setShowPrintMargin(false);

            aceSession = aceEditor.getSession();
            aceSession.setMode('ace/mode/html');
            aceSession.setUseSoftTabs(true);
            aceEditor.setOptions({
                enableSnippets: true,
                enableBasicAutocompletion: true,
                enableLiveAutocompletion: true,
            });
            setTimeout(function () {
                aceEditor.focus();
                aceSession.setValue(editor.getData(), -1);
            })
        },
        // This method is invoked once a user clicks the OK button, confirming the dialog.
        onOk: function () {
            editor.focus();
            editor.setData(aceEditor.getValue());
        }
    };
});

/*
 * Resize the ACE Editor
 */
CKEDITOR.dialog.on('resize', function (evt) {
    var AceEditor = evt.editor.aceEditor;
    if (AceEditor !== undefined) {
        AceEditor.resize();
    }
});

