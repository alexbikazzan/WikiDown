angular.module('WikiDown').directive('scrollToOn', [
    function() {
        'use strict';
        return {
            restrict: 'A',
            scope: {
                onEvent: '@scrollToOn',
                duration: '@scrollToOnDuration'
            },
            link: function(scope, element) {
                if (!scope.onEvent) {
                    return;
                }

                scope.duration = parseInt(scope.duration, 10) || 500;

                scope.$on(scope.onEvent, function() {
                    var elementOffset = element.offset(),
                        elementTop = elementOffset ? elementOffset.top : undefined;

                    if (elementTop >= 0) {
                        $('html, body').animate({
                            scrollTop: elementTop
                        }, scope.duration);
                    }
                });
            }
        };
    }
]);