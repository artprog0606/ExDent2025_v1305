import './Achievements.scss';

(function (ng) {
    'use strict';

    var AchievementsCtrl = function (toaster, $http, $translate, $scope, urlHelper) {
        var ctrl = this;

        ctrl.$onInit = function () {
            ctrl.getData(true);
        };

        ctrl.getData = function (isFirstLoad) {
            return $http.get('home/getAchievementsData').then(function (response) {
                var data = response.data;
                var obj = data.obj;

                if (data.result === true) {
                    ctrl.data = obj;

                    ctrl.bonuses = obj.Bonuses;
                    ctrl.steps = obj.Groups;
                    ctrl.currentStepComplited = 0;
                    ctrl.allSteps = 0;

                    for (var j = 0; j < ctrl.steps.length; j++) {
                        ctrl.allSteps += ctrl.steps[j].Achievements.length;
                        for (var i = 0; i < ctrl.steps[j].Achievements.length; i++) {
                            if (ctrl.steps[j].Achievements[i].Complete) {
                                ctrl.currentStepComplited += 1;
                            }
                        }
                    }
                } else if (data.errors != null) {
                    data.errors.forEach(function (err) {
                        toaster.pop('error', '', err);
                    });
                }
            });
        };

        ctrl.openLink = function (link) {
            if (angular.isString(link)) {
                window.open(link, '_blank');
            } else {
                console.error('ActionButtonLink не содержит допустимую ссылку');
            }
        };

        ctrl.subscribeAndRedirect = function (link) {
            $http
                .get('achievementsEvents/subscribeToCompanySocialNetworks')
                .then(function (response) {
                    if (response) {
                        window.open(link, '_blank');
                    } else {
                        console.error('Ошибка при отправке запроса на подписку');
                    }
                })
                .catch(function (error) {
                    console.error('Ошибка при отправке запроса', error);
                });
        };

        ctrl.goSupportCenterAndRedirect = function (link) {
            $http
                .get('achievementsEvents/goToCompanySupportCenter')
                .then(function (response) {
                    if (response) {
                        window.open(link, '_blank');
                    } else {
                        console.error('Ошибка при отправке запроса на подписку');
                    }
                })
                .catch(function (error) {
                    console.error('Ошибка при отправке запроса', error);
                });
        };

        ctrl.trackEvent = function (trackEvent) {
            $http.post('home/trackCongratulationsDashboardEvents', { trackEvent: trackEvent });
        };
    };

    AchievementsCtrl.$inject = ['toaster', '$http', '$translate', '$scope', 'urlHelper'];

    ng.module('achievements', []).controller('AchievementsCtrl', AchievementsCtrl);
})(window.angular);
