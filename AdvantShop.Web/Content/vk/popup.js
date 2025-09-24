const toggleView = function (isAuthorize) {
    if (isAuthorize === true) {
        document.getElementById('login').setAttribute('hidden', 'hidden');
        document.getElementById('user').removeAttribute('hidden');
    } else {
        document.getElementById('login').removeAttribute('hidden');
        document.getElementById('user').setAttribute('hidden', 'hidden');
    }
};

const signInForm = document.getElementById('signInForm'),
    signOutForm = document.getElementById('signOutForm'),
    btnSignInForm = document.getElementById('btnSignInForm');

signInForm.addEventListener('submit', function (event) {
    event.preventDefault();

    if (btnSignInForm.getAttribute('disabled') == null) {
        btnSignInForm.setAttribute('disabled', 'disabled');

        chrome.runtime.sendMessage({ cmd: 'signIn', params: document.getElementById('input').value }, function (response) {
            if (response != null && response.result.status !== 'error') {
                toggleView(true);
            } else {
                alert('Ошибка при авторизации');
            }

            btnSignInForm.removeAttribute('disabled');
        });
    }
});

signOutForm.addEventListener('submit', function (event) {
    event.preventDefault();
    chrome.runtime.sendMessage({ cmd: 'signOut' }, function (response) {
        toggleView(false);
    });
});

chrome.runtime.sendMessage({ cmd: 'getAuthorizeState' }, function (response) {
    toggleView(response.result);
});
