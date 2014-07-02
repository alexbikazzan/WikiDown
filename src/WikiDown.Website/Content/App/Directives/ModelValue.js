angular.module('WikiDown').directive('modelValue', [
    function() {
        'use strict';

        return {
            restrict: 'A',
            scope: {
                ngModel: '=',
                //asArray: '@'
            },
            require: '?ngModel',
            link: function(scope, element) {
                var value = element.val();

                if (value) {
                    scope.ngModel = value;
                }
            }
        };
    }
]);