import '@sjmc11/tourguidejs/src/scss/tour.scss'; // Styles
import { NgTourGuideService } from './ng-tour-guide.service';
import { ngTgTour } from './ng-tour-guide.directive';
import { ngTourGuideConstant } from './ng-tour-guide.constant';
import { TourGuideOptions } from '@sjmc11/tourguidejs/src/core/options';

const MODULE_NAME = 'ngTourGuide';
angular
    .module(MODULE_NAME, [])
    .service('ngTourGuideService', NgTourGuideService)
    .directive('ngTgTour', ngTgTour)
    .constant('ngTourGuideOptions', ngTourGuideConstant)
    .run(
        /* @ngInject */ (ngTourGuideOptions: TourGuideOptions, $translate) => {
            ngTourGuideOptions.prevLabel = $translate.instant('Admin.Js.Tour.Prev');
            ngTourGuideOptions.nextLabel = $translate.instant('Admin.Js.Tour.Next');
            ngTourGuideOptions.finishLabel = $translate.instant('Admin.Js.Tour.Finish');
        },
    );

export default MODULE_NAME;
