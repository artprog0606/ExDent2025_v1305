CKEDITOR.plugins.add('videoytembed', {
    icons: 'videoytembed',
    lang: 'ru',
    version: 1.1,
    init: function (editor) {
        editor.ui.addButton('VideoYTEmbed', {
            label: editor.lang.videoytembed.button,
            command: 'videoytembed',
            toolbar: 'insert'
        });

        editor.on('doubleclick', function(event) {
            var element = event.data.element;
            if(element.$.tagName === 'IMG' && element.$.closest('iframe-responsive')) {
                event.cancel();
            }
        });

        editor.addCommand('videoytembed', {
            exec: function(editor) {
                var selectedElement = editor.getSelection().getStartElement();
                var dialogData = {
                    maxHeight: '',
                    maxWidth: '',
                    src: ''
                };

                var elementData = '';
                if(selectedElement.$.tagName === 'IMG' && selectedElement.$.parentNode.tagName === 'IFRAME-RESPONSIVE') {
                    var iframeResponsive = selectedElement.$.parentNode;
                    var wrapper = iframeResponsive.closest('.ckeditor-yt-embed-wrap');
                    if(wrapper) {
                        dialogData.maxHeight = wrapper.style.maxHeight === 'none' ? '' : wrapper.style.maxHeight;
                        dialogData.maxWidth = wrapper.style.maxWidth === 'none' ? '' : wrapper.style.maxWidth;
                    }

                    dialogData.src = iframeResponsive.dataset.src || '';
                }
                editor.openDialog('videoytembedDialog', function(dialog) {
                    dialog.on('show', function() {
                        if (dialog.getName() === 'videoytembedDialog') {
                            dialog.setValueOf('tab-basic', 'max_height_video', dialogData.maxHeight);
                            dialog.setValueOf('tab-basic', 'max_width_video', dialogData.maxWidth);
                            dialog.setValueOf('tab-basic', 'url_video', dialogData.src);
                        }
                    })

                })
            }
        });


        // Dialog window
        CKEDITOR.dialog.add('videoytembedDialog', this.path + 'dialogs/videoytembedDialog.js');
    }
});
