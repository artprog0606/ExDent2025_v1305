export default function iframeResponsiveService($q, $window, $http) {
    var service = this,
        initializedYTList = [],
        initializedVimeoList = [],
        playerId = 0,
        regExpIsVkClip = /clip-\d+_\d+/,
        regExpVkId = /(?:video|clip)(-?\d+)_(\d+)/,
        regExpIdVideo = /(youtu.*be.*)\/(watch\?v=|embed\/|v|shorts|)(.*?((?=[&#?])|$))/,
        regExpIdRutubeVideo = /(?:video|shorts|embed)\/([a-zA-Z0-9]+)/,
        regExpIframe = new RegExp('(?:<iframe[^>])'),
        regExpGetUrlFromSrc = new RegExp('(?:src=").*?(?=[?"])'),
        urlRegex = /(http[s]?:)?(\/\/)?(www\.)?[a-zA-Z0-9]+\.[^\s]{2,}/,
        loadedYouTubeIframeAPI = false,
        loadedVimeoIframeAPI = false,
        activeItem;

    service.checkInitYouTubeIframeAPI = function () {
        return loadedYouTubeIframeAPI;
    };

    service.checkInitVimeoIframeAPI = function () {
        return loadedVimeoIframeAPI;
    };

    service.checkInitVkAPI = function () {
        return loadedVimeoIframeAPI;
    };

    service.addYouTubeIframeAPI = function () {
        var tag = document.createElement('script');
        tag.src = 'https://www.youtube.com/iframe_api';
        var firstScriptTag = document.getElementsByTagName('script')[0];
        firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
    };

    service.addVimeoIframeAPI = function () {
        var defer = $q.defer();
        initializedVimeoList.push(defer);
        var tag = document.createElement('script');
        tag.src = 'https://www.youtube.com/iframe_api';
        tag.onload = function () {
            initializedVimeoList.forEach(function (defer) {
                defer.resolve();
            });
            loadedVimeoIframeAPI = true;
        };
        var firstScriptTag = document.getElementsByTagName('script')[0];
        firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
        return defer.promise;
    };

    service.addOnYouTubeIframeAPIReady = function () {
        window.onYouTubeIframeAPIReady = function () {
            initializedYTList.forEach(function (defer) {
                defer.resolve();
            });
            loadedYouTubeIframeAPI = true;
        };
        var defer = $q.defer();
        initializedYTList.push(defer);
        if (!service.checkInitYouTubeIframeAPI()) {
            service.addYouTubeIframeAPI();
        }
        return defer.promise;
    };

    service.getPlayerId = function () {
        return 'player' + (playerId += 1);
    };

    service.getVideoIdFromYouTube = function (url) {
        return url.match(regExpIdVideo)[3];
    };

    service.getVideoIdFromVimeo = function (url) {
        var regexp =
            /(?:www\.|player\.)?vimeo.com\/(?:channels\/(?:\w+\/)?|groups\/(?:[^\/]*)\/videos\/|album\/(?:\d+)\/video\/|video\/|)(\d+)(?:[a-zA-Z0-9_\-]+)?/i;
        return url.match(regexp)[1];
    };

    service.getYTPlayerAPI = function (elId, videoId, callbacks, autoplay, loop) {
        /* eslint-disable no-undef */
        return new YT.Player(elId, {
            videoId: videoId,
            playerVars: {
                enablejsapi: 1,
                html5: 1,
                fmt: 1,
                showinfo: 0,
            },
            events: callbacks,
        });
    };

    service.getVimeoPlayerAPI = function (elId, videoId, autoplay, loop) {
        /* eslint-disable no-undef */
        return new Vimeo.Player(elId, {
            id: videoId,
            autoplay: autoplay != null ? autoplay : false,
            muted: autoplay != null ? autoplay : false,
            loop: loop === true,
        });
    };

    service.run = function (obj, type) {
        if (activeItem != null && activeItem.obj !== obj && activeItem.obj.player != null) {
            if (activeItem.type === 'youtube') {
                activeItem.obj.player.pauseVideo();
            } else if (activeItem.type === 'vimeo') {
                activeItem.obj.player.pause();
            }
            if (activeItem.type === 'rutube') {
                activeItem.obj.player.pause();
            }
        }

        activeItem = {
            obj: obj,
            type: type,
        };
    };

    service.checkUrlFromIframe = function (url) {
        return url.match(regExpIframe);
    };

    service.getSrc = function (url) {
        if (service.checkUrlFromIframe(url)) {
            return url.match(regExpGetUrlFromSrc)[0].match(urlRegex)[0];
        }
        return url;
    };

    service.isPlayerCode = function (url) {
        return url.match(urlRegex) == null;
    };

    service.getYouTubeCode = function (link, autoplay, videoId, loop) {
        link = link.indexOf('https://') === -1 ? 'https://' + link : link;
        link = link.indexOf('www') === -1 ? 'https://www.' + link.split('https://')[1] : link;
        return (
            link.replace('youtu.be', 'youtube.com/embed/').replace('watch?v=', 'embed/').replace('shorts', 'embed').split('&')[0] +
            '?rel=0&amp;enablejsapi=1&amp;showinfo=0' +
            (autoplay || loop ? '&amp;autoplay=1&amp;mute=1&amp;' : '') +
            (loop ? 'loop=1&controls=0&wmode=transparent&playlist=' + videoId + '' : '')
        );
    };

    service.getVimeoCode = function (link, autoplay, loop) {
        return (
            'https://player.vimeo.com/video' +
            link.split('vimeo.com')[link.split('vimeo.com').length - 1] +
            '?title=0&amp;byline=0&amp;portrait=0' +
            (autoplay ? '&autoplay=1&muted=1' : '') +
            (loop ? '&loop=1' : '')
        );
    };

    service.getYTCover = function (YTVideoId) {
        return 'https://i.ytimg.com/vi/' + YTVideoId + '/maxresdefault.jpg';
    };

    service.getVimeoCover = function (vimeoId) {
        return $http
            .get('https://vimeo.com/api/oembed.json?url=https%3A//vimeo.com/' + vimeoId, {
                format: 'json',
                width: '1280',
            })
            .then(function (response) {
                return response;
            })
            .catch(function (error) {
                console.error(error);
            });
    };

    /// #region Rutube
    // https://github.com/rutube/RutubePlayerJSAPI
    // https://github.com/evikza/rutube-player/blob/main/rt.js

    service.getVideoIdFromRutube = function (url) {
        const match = url.match(regExpIdRutubeVideo);
        return match && match[1];
    };

    service.getRutubeCode = function (link, autoplay, loop) {
        const videoId = service.getVideoIdFromRutube(link);
        return `https://rutube.ru/play/embed/${videoId}?autoplay=${autoplay ? '1' : '0'}`;
    };

    const Rutube = function () {
        this.Player = function (id, config) {
            this.selector = id;
            this.config = config;
        };

        this.getId = function () {
            return this.selector;
        };

        this.triggerEventObserver = function (env, args = null) {
            if (!this.config.events || !this.config.events[env]) return;

            return this.config.events[env](args);
        };

        this.setPlayerState = function (status) {
            const playerState = {
                playerState: { PLAYING: 0, PAUSED: 0, STOPPED: 0, PREROLL: 0 },
            };

            for (let state in playerState.playerState) {
                if (state.toLowerCase() === status.toLowerCase()) {
                    playerState.playerState[state] = 1;

                    break;
                }
            }

            return playerState;
        };

        for (let [iterator, type] of Object.entries({
            play: 'play',
            pause: 'pause',
            stop: 'stop',
            seekTo: 'setCurrentTime',
            changeVideo: 'changeVideo',
            mute: 'mute',
            unMute: 'unMute',
            setVolume: 'setVolume',
        })) {
            this[iterator] = function (data = {}) {
                document.getElementById(this.selector).contentWindow.postMessage(
                    JSON.stringify({
                        type: 'player:' + type,
                        data: data,
                    }),
                    '*',
                );
            };
        }

        this.playerEvent = function (receivedMessage) {
            switch (receivedMessage.type) {
                case 'player:ready':
                    this.triggerEventObserver('onReady');
                    break;

                case 'player:changeState':
                    this.triggerEventObserver('onStateChange', this.setPlayerState(receivedMessage.data.state));
                    break;

                case 'player:rollState': // реклама
                    this.triggerEventObserver('onStateChange', this.setPlayerState('preroll'));
                    break;

                case 'player:playComplete':
                    this.triggerEventObserver('onComplete');
                    break;
            }
        };

        window.addEventListener(
            'message',
            function (event) {
                const receivedMessage = JSON.parse(event.data);
                this.playerEvent(receivedMessage);
            }.bind(this),
            0,
        );
    };

    service.getRutubePlayer = function (playerId, events) {
        let rt = new Rutube();
        rt.Player(playerId, events);
        return rt;
    };

    // #endregion

    service.getVideoIdsFromVk = function (url) {
        const ids = url.match(regExpVkId);
        if (ids.length) {
            return ids.slice(1);
        }
    };
    service.checkIsVkClip = function (url) {
        return regExpIsVkClip.test(url);
    };

    service.addVkAPI = function () {
        return new Promise((resolve) => {
            if (!service.checkInitVkAPI()) {
                const tag = document.createElement('script');
                tag.src = 'https://vk.com/js/api/videoplayer.js';
                const firstScriptTag = document.getElementsByTagName('script')[0];
                firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
                tag.onload = () => {
                    resolve();
                };
            } else {
                resolve();
            }
        });
    };

    service.getVkPlayer = function (iframe) {
        return VK?.VideoPlayer(iframe);
    };
}

iframeResponsiveService.$inject = ['$q', '$window', '$http'];
