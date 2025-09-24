import type { IDirective, IParseService } from 'angular';
import { INgTourGuideService } from './ng-tour-guide.service';
import { TourGuideStep } from '@sjmc11/tourguidejs/src/types/TourGuideStep';

/* @ngInject */
export const ngTgTour = function ($parse: IParseService, ngTourGuideService: INgTourGuideService): IDirective {
    return {
        restrict: 'A',
        scope: true,
        link(scope, element, attrs) {
            const options: TourGuideStep = {
                title: typeof attrs.ngTgTitle !== 'undefined' ? $parse(attrs.ngTgTitle)(scope) : undefined,
                content: $parse(attrs.ngTgTour)(scope),
                target: element[0],
                dialogTarget: null,
                fixed: typeof attrs.ngTgFixed !== 'undefined' ? $parse(attrs.ngTgFixed)(scope) : undefined,
                order: typeof attrs.ngTgOrder !== 'undefined' ? $parse(attrs.ngTgOrder)(scope) : undefined,
                group: typeof attrs.ngTgGroup !== 'undefined' ? $parse(attrs.ngTgGroup)(scope) : undefined,
                propagateEvents: typeof attrs.ngTgPropagateEvents !== 'undefined' ? $parse(attrs.ngTgPropagateEvents)(scope) : undefined,
                beforeEnter:
                    typeof attrs.ngTgBeforeEnter !== 'undefined'
                        ? (currentStep: TourGuideStep, nextStep: TourGuideStep) => {
                              $parse(attrs.ngTgBeforeEnter)(scope, { currentStep, nextStep });
                          }
                        : undefined,
                afterEnter:
                    typeof attrs.ngTgAfterEnter !== 'undefined'
                        ? (currentStep: TourGuideStep, nextStep: TourGuideStep) => {
                              $parse(attrs.ngTgAfterEnter)(scope, { currentStep, nextStep });
                          }
                        : undefined,
                beforeLeave:
                    typeof attrs.ngTgBeforeLeave !== 'undefined'
                        ? (currentStep: TourGuideStep, nextStep: TourGuideStep) => {
                              $parse(attrs.ngTgBeforeLeave)(scope, { currentStep, nextStep });
                          }
                        : undefined,
                afterLeave:
                    typeof attrs.ngTgAfterLeave !== 'undefined'
                        ? (currentStep: TourGuideStep, nextStep: TourGuideStep) => {
                              $parse(attrs.ngTgAfterLeave)(scope, { currentStep, nextStep });
                          }
                        : undefined,
            };

            ngTourGuideService.addStepsItem(options);
        },
    };
};
