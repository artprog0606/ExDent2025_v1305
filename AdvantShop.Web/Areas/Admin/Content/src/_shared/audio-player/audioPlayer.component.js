import audioPlayerTemplate from './templates/audio-player.html';
(function (ng) {
    'use strict';

    ng.module('audioPlayer').component('audioPlayer', {
        templateUrl: audioPlayerTemplate,
        controller: 'AudioPlayerCtrl',
        bindings: {
            src: '<',
            loading: '<',
            onLoadSrc: '&',
        },
    });
})(window.angular);
