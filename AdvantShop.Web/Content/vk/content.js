const userInfo = new UserInfo('.page_photo', '.im-right-menu');

chrome.runtime.onMessage.addListener(function (request, sender, sendResponse) {
    if (request.cmd == 'render') {
        const data = request.params.Data;

        userInfo.destroy();

        if (data != null) {
            userInfo.render(data).then(function () {
                userInfo.bind();
            });
        }
    } else if (request.cmd === 'signOut') {
        userInfo.destroy();
    }
});

//chrome.runtime.sendMessage({ cmd: 'checkAuthorize' });
