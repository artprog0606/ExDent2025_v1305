(function (ng) {
    'use strict';

    var helpTriggerService = function () {
        var service = this;
        var activeHelpTrigger;

        service.addActiveHelpTrigger = function (helpTrigger) {
            activeHelpTrigger = helpTrigger;
        };

        service.getActiveHelpTrigger = function () {
            return activeHelpTrigger;
        };

        service.clearActiveHelpTrigger = function (helpTrigger) {
            if (helpTrigger === activeHelpTrigger) {
                activeHelpTrigger = null;
            }
        };

        service.getContainerRect = function (container) {
            var rect = container.getBoundingClientRect();
            return {
                top: rect.top,
                right: rect.right,
                bottom: rect.bottom,
                left: rect.left,
                height: rect.height,
                width: rect.width,
                x: rect.x,
                y: rect.y,
            };
        };

        //https://www.geeksforgeeks.org/check-whether-a-given-point-lies-inside-a-triangle-or-not/
        service.checkInTriangle = function (triggerRect, containerRect, mouseLoc, options) {
            var point1 = { x: triggerRect.x - options.tolerance, y: triggerRect.y };
            var point2 = { x: containerRect.left + options.tolerance, y: containerRect.top - options.tolerance };
            var point3 = { x: containerRect.left + options.tolerance, y: containerRect.bottom + options.tolerance };
            return isTriangleInside(point1, point2, point3, mouseLoc);
        };

        function isTriangleInside(point1, point2, point3, currentPoint) {
            var a = triangleArea(point1, point2, point3);
            var a1 = triangleArea(currentPoint, point2, point3);
            var a2 = triangleArea(point1, currentPoint, point3);
            var a3 = triangleArea(point1, point2, currentPoint);
            return a === a1 + a2 + a3;
        }

        function triangleArea(point1, point2, point3) {
            return Math.abs((point1.x * (point2.y - point3.y) + point2.x * (point3.y - point1.y) + point3.x * (point1.y - point2.y)) / 2.0);
        }
    };

    ng.module('helpTrigger').service('helpTriggerService', helpTriggerService);
})(window.angular);
