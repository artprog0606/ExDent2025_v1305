import timesOfWorkTemplate from './templates/times-of-work.html';

/* @ngInject */
const timesOfWorkComponent = {
    templateUrl: timesOfWorkTemplate,
    controller: 'TimesOfWorkCtrl',
    bindings: {
        times: '<',
        onChangeFn: '&',
    },
};

export { timesOfWorkComponent };
