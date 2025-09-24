/**
 *  * @description directive for sweet alert
 * @author Tushar Borole
 * @createDate 18/04/2015
 * @version 1.0.3
 * @lastmodifiedDate 06/18/2015
 */

(function () {
    'use strict';

    // Check we have sweet alert js included
    if (angular.isUndefined(window.Sweetalert2)) {
        throw 'Please inlcude sweet alert js and css from http://t4t5.github.io/sweetalert/';
    }

    var Swal = Swal || window.Sweetalert2;

    angular
        .module('ng-sweet-alert', [])
        .directive('sweetalert', sweetalert)
        .constant('sweetalertDefaultOptions', {
            cancelButtonText: 'Отмена',
            confirmButtonText: 'ОK',
            allowOutsideClick: false,
            buttonsStyling: false,
        })
        .factory('SweetAlert', sweetalert_service);

    sweetalert.$inject = ['$parse', 'sweetalertDefaultOptions'];

    /* @ngInject */
    function sweetalert($parse, sweetalertDefaultOptions) {
        // Usage:
        //
        // Creates:
        //
        var directive = {
            link: link,
        };
        return directive;

        function link(scope, element, attrs, controller) {
            var sweetElement = angular.element(element);
            sweetElement.click(function () {
                var sweetOptions = scope.$eval(attrs.sweetOptions);
                var sweetConfirmOption = scope.$eval(attrs.sweetConfirmOption);
                var sweetCancelOption = scope.$eval(attrs.sweetCancelOption);

                Swal.fire(angular.extend({}, sweetalertDefaultOptions, sweetOptions)).then(function (isConfirm) {
                    if (isConfirm != null && (isConfirm.isConfirmed || isConfirm.value)) {
                        if (sweetConfirmOption) Swal.fire(sweetConfirmOption);
                        if (attrs.sweetOnConfirm)
                            scope.$evalAsync(attrs.sweetOnConfirm);
                    } else {
                        if (sweetCancelOption) Swal.fire(sweetCancelOption);
                        if (attrs.sweetOnCancel)
                            scope.$evalAsync(attrs.sweetOnCancel);
                    }
                });
            });
        }
    }

    // Use SweetAlert as service
    //
    // swal() gets two arguments;
    // first argument is parameters Objects (with default values).
    // second argument is Callback function when clicking on "OK"/"Cancel", which is a promise.
    // register to the promise (using 'then') and handle the resolve / reject according to your business logic.
    //
    // Add 'SweetAlert' to your directive / controller / ect)
    // Use SweetAlert.confirm(msg, options) / SweetAlert.alert(msg, options) / SweetAlert.info(msg, options) / SweetAlert.success(msg, options)
    // pass arguments:
    // msg; String - The message to be displayed in the alert / confirm box (mandatory).
    // options; Object (optinal):
    //   title: String - the title of the box.
    //   icon: String - "warning" / "info" / "error" / "success" / "" (empty string will not display a graphic icon).
    //   showCancelButton: Boolean - shows the "cancel" button (true will behave like confirm dialog, false will behave like alert dialog).
    // Use returned promise;
    //
    // SweetAlert.confirm("Are you sure?", {title : "Careful now!"})
    //           .then(function(p) { do something on success },
    //                 function(p) { do something on fail }
    //           );
    //
    // SweetAlert.success("You have successfully completed our poll!", {title: "Good job!"});

    sweetalert_service.$inject = ['$q', '$timeout', 'sweetalertDefaultOptions'];

    function sweetalert_service($q, $timeout, sweetalertDefaultOptions) {
        function swal_alert(message, options) {
            return swal_confirm(
                message,
                angular.extend(
                    {},
                    sweetalertDefaultOptions,
                    {
                        title: 'Alert',
                        html: message,
                        icon: 'warning',
                        showCancelButton: false,
                    },
                    options,
                ),
            );
        }

        function swal_info(message, options) {
            return swal_alert(
                message,
                angular.extend
                (
                    {},
                    sweetalertDefaultOptions,
                    {
                        icon: 'info',
                    },
                    options,
                ),
            );
        }

        function swal_success(message, options) {
            return swal_alert(
                message,
                angular.extend(
                    {},
                    sweetalertDefaultOptions,
                    {
                        icon: 'success',
                    },
                    options,
                ),
            );
        }

        function swal_error(message, options) {
            return swal_alert(
                message,
                angular.extend(
                    {},
                    sweetalertDefaultOptions,
                    {
                        icon: 'error'
                    },
                    options,
                ),
            );
        }

        function swal_confirm(message, options) {
            var defered = $q.defer();
            var optionsNew = angular.extend(
                {},
                sweetalertDefaultOptions,
                {
                    title: 'Alert',
                    html: message,
                    icon: 'warning',
                    showCancelButton: true,
                },
                options,
            );

            Swal.fire(optionsNew).then(
                function (r) {
                    $timeout(() => defered.resolve(r));
                },
                function (e) {
                    $timeout(() => defered.reject(e));
                },
            ).catch(function(message){
                console.error(message)
            });

            return defered.promise;
        }

        return {
            alert: swal_alert,
            confirm: swal_confirm,
            info: swal_info,
            success: swal_success,
            error: swal_error,
            close: Swal.close
        };
    }
})();
