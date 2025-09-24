// в админке есть такой же файл
/*@ngInject*/
function iframeResponsiveCtrl($sce, iframeResponsiveService, $scope, $timeout) {
    var ctrl = this,
        stateChangeFlag = true;

    ctrl.showContent = function () {
        ctrl.isShowContent = true;
        if (ctrl.deviceMobile && ctrl.asBackground) {
            if (ctrl.useVimeo) {
                var vimeoVideoId = iframeResponsiveService.getVideoIdFromVimeo(ctrl.src);
                iframeResponsiveService.getVimeoCover(vimeoVideoId).then(function (response) {
                    if (response.data != null) {
                        ctrl.coverVideoPath = response.data.thumbnail_url;
                    }
                });
            } else if (ctrl.useYouTube) {
                var YTVideoId = iframeResponsiveService.getVideoIdFromYouTube(ctrl.src);
                ctrl.coverVideoPath = iframeResponsiveService.getYTCover(YTVideoId);
            }
        } else {
            if (ctrl.isPlayerCode) {
                ctrl.playerCode = ctrl.src;
            } else {
                ctrl.src = iframeResponsiveService.getSrc(ctrl.src);
            }
            ctrl.pasteVideo(ctrl.src, ctrl.autoplay, ctrl.loop);
            $scope.$digest();
        }
    };

    ctrl.pasteVideoForModal = function (src) {
        // ctrl.stopOthersVideo();
        ctrl.showVideo();
        ctrl.hideCover();

        if (ctrl.useYouTube) {
            src = iframeResponsiveService.getYouTubeCode(src, true);
        }
        if (ctrl.useVimeo) {
            src = iframeResponsiveService.getVimeoCode(src, true);
        }
        if (ctrl.useRutube) {
            src = iframeResponsiveService.getRutubeCode(src, true);
        }

        ctrl.iframeSrc = $sce.trustAsResourceUrl(src);
    };

    ctrl.onPlayerReady = function (event) {
        if (ctrl.autoplay) {
            ctrl.player.mute();
            ctrl.player.playVideo();
        }

        if (stateChangeFlag) {
            stateChangeFlag = false;
        }
    };

    ctrl.onPlayerStateChange = function (event) {
        ctrl.videoLoaded = true;
        if (event.data === -1) {
            ctrl.muteOn = true; // autoplay

            $timeout(function () {
                ctrl.hideCover();
            }, 100);
        } else if (event.data === 1) {
            if (!ctrl.disabledStop) {
                iframeResponsiveService.run(ctrl, 'youtube');
            }
        }
    };

    ctrl.showVideo = function () {
        ctrl.visibleVideo = true;
    };

    ctrl.hideVideo = function () {
        ctrl.visibleVideo = false;
    };

    ctrl.showCover = function () {
        ctrl.visibleCover = true;
    };

    ctrl.hideCover = function () {
        ctrl.visibleCover = false;
    };

    ctrl.stopOthersVideo = function () {
        if (!ctrl.disabledStop) {
            iframeResponsiveService.run(ctrl, 'vimeo');
            iframeResponsiveService.run(ctrl, 'youtube');
        }
    };

    ctrl.pasteYTIframeSrc = function (src, playerId, autoplay, loop) {
        var YTVideoId = iframeResponsiveService.getVideoIdFromYouTube(src);
        ctrl.coverVideoPath = iframeResponsiveService.getYTCover(YTVideoId);
        $timeout(function () {
            if (!iframeResponsiveService.checkInitYouTubeIframeAPI()) {
                iframeResponsiveService
                    .addOnYouTubeIframeAPIReady()
                    .then(function () {
                        ctrl.player = iframeResponsiveService.getYTPlayerAPI(
                            playerId,
                            YTVideoId,
                            { onReady: ctrl.onPlayerReady, onStateChange: ctrl.onPlayerStateChange },
                            autoplay,
                            loop,
                        );
                    })
                    .catch(function (error) {
                        console.error(error);
                    });
            } else {
                ctrl.player = iframeResponsiveService.getYTPlayerAPI(
                    ctrl.playerId,
                    YTVideoId,
                    { onReady: ctrl.onPlayerReady, onStateChange: ctrl.onPlayerStateChange },
                    autoplay,
                    loop,
                );
            }
        });
        var YTCode = iframeResponsiveService.getYouTubeCode(src, autoplay, YTVideoId, loop);
        ctrl.iframeSrc = $sce.trustAsResourceUrl(YTCode);
    };

    ctrl.pasteVimeoIframeSrc = function (src, playerId, autoplay, loop) {
        var vimeoVideoId = iframeResponsiveService.getVideoIdFromVimeo(src);

        iframeResponsiveService.getVimeoCover(vimeoVideoId).then(function (response) {
            if (response.data != null) {
                ctrl.coverVideoPath = response.data.thumbnail_url;
            }
        });
        $timeout(function () {
            if (!iframeResponsiveService.checkInitVimeoIframeAPI()) {
                iframeResponsiveService
                    .addVimeoIframeAPI()
                    .then(function () {
                        ctrl.player = iframeResponsiveService.getVimeoPlayerAPI(playerId, vimeoVideoId, autoplay, loop);
                        ctrl.player.on('play', function () {
                            // ctrl.stopOthersVideo();
                            iframeResponsiveService.run(ctrl, 'vimeo');
                            ctrl.hideCover();
                            $scope.$digest();
                        });
                    })
                    .catch(function (error) {
                        console.error(error);
                    });
            } else {
                ctrl.player = iframeResponsiveService.getVimeoPlayerAPI(playerId, vimeoVideoId, autoplay, loop);
            }
        });
    };

    ctrl.pasteRutubeIframeSrc = function (link, playerId, autoplay, loop) {
        let videoId = iframeResponsiveService.getVideoIdFromRutube(link);

        $timeout(function () {
            let player = document.getElementById(playerId);
            if (player != null && (ctrl.player == null || ctrl.player.getId() !== playerId)) {
                ctrl.player = iframeResponsiveService.getRutubePlayer(playerId, {
                    events: {
                        onReady: ctrl.onPlayerReadyRutube,
                        onStateChange: ctrl.onPlayerStateChangeRutube,
                        onComplete: ctrl.onCompleteRutube,
                    },
                });
            }

            // if (ctrl.player != null) {
            //     ctrl.stopOthersVideo();
            // }
        });
        let code = iframeResponsiveService.getRutubeCode(link, autoplay, videoId, loop);
        ctrl.iframeSrc = $sce.trustAsResourceUrl(code);
    };

    ctrl.pasteVkIframeSrc = function (link, playerId, autoplay, loop) {
        const [oid, id] = iframeResponsiveService.getVideoIdsFromVk(link);
        if (oid && id) {
            iframeResponsiveService
                .addVkAPI()
                .then(() => {
                    const src = `https://vk.com/video_ext.php?oid=${oid}&id=${id}&hd=2&autoplay=${autoplay ? '1' : '0'}&repeat=${loop ? '1' : '0'}&js_api=1`;
                    ctrl.iframeSrc = $sce.trustAsResourceUrl(src);
                    let iframe = document.getElementById(playerId);
                    iframe.src = src;
                    if (iframe) {
                        ctrl.player = iframeResponsiveService.getVkPlayer(iframe);
                        ctrl.player?.on('started', ctrl.onPlayerStateChangeVk);
                    }
                })
                .catch((e) => {
                    throw new Error(e);
                });
        }
    };

    ctrl.pasteDzenIframeSrc = function (link, playerId, autoplay, loop) {
        ctrl.iframeSrc = $sce.trustAsResourceUrl(link + `?autoplay=${autoplay ? '1' : '0'}&loop=${loop ? '1' : '0'}`);
    };

    ctrl.onPlayerReadyRutube = function () {
        if (ctrl.autoplay) {
            try {
                ctrl.player.mute();
                ctrl.player.play();
                $timeout(function () {
                    ctrl.hideCover();
                }, 100);
            } catch {
                /* tslint:disable:no-empty */
            }
        }
    };

    ctrl.onPlayerStateChangeVk = function (event) {
        if (event.state === 'playing') {
            ctrl.hideCover();
            $scope.$digest();
        }
    };

    ctrl.onPlayerStateChangeRutube = function (event) {
        if (event.playerState.PLAYING || event.playerState.PREROLL) {
            $timeout(function () {
                ctrl.hideCover();
            }, 100);
        }
    };

    ctrl.onCompleteRutube = function () {
        if (ctrl.loop) {
            try {
                ctrl.player.seekTo({ time: 0 });
                ctrl.player.play();
            } catch {
                /* tslint:disable:no-empty */
            }
        }
    };

    ctrl.pasteVideo = function (src, autoplay, loop) {
        ctrl.playerId = iframeResponsiveService.getPlayerId();
        if (ctrl.useYouTube) {
            ctrl.pasteYTIframeSrc(src, ctrl.playerId, autoplay, loop);
        }
        if (ctrl.useVimeo) {
            ctrl.pasteVimeoIframeSrc(src, ctrl.playerId, autoplay, loop);
        }
        if (ctrl.useRutube) {
            ctrl.pasteRutubeIframeSrc(src, ctrl.playerId, autoplay, loop);
        }
        if (ctrl.useVk) {
            ctrl.pasteVkIframeSrc(src, ctrl.playerId, autoplay, loop);
        }
        if (ctrl.useDzen) {
            ctrl.hideCover();
            ctrl.pasteDzenIframeSrc(src, ctrl.playerId, autoplay, loop);
        }
    };
}

export default iframeResponsiveCtrl;
