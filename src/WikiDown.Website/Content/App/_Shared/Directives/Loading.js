angular.module('WikiDown').directive('loading', [
    function() {
        'use strict';

        return {
            restrict: 'A',
            scope: {
                loadingItem: '=loading'
            },
            template: '<i ng-show="isLoading" class="fa fa-spinner fa-spin"></i>',
            link: function(scope) {
                scope.$watch('loadingItem', function(val) {
                    if (typeof scope.loadingItem === 'undefined' || typeof val === 'undefined') {
                        return;
                    }
                    if (typeof val === 'boolean') {
                        scope.isLoading = val;
                    } else if (typeof val === 'string') {
                        var lowerVal = (val || '').toLowerCase();
                        if (lowerVal === 'true') {
                            scope.isLoading = true;
                        } else if (lowerVal === 'false') {
                            scope.isLoading = false;
                        }
                    } else {
                        watchPromise(val);
                    }
                });

                function watchPromise(promise) {
                    if (!promise || typeof promise['finally'] === 'undefined' || typeof promise.then === 'undefined') {
                        return;
                    }

                    scope.isLoading = true;
                    promise.then(function() {
                        scope.isLoading = false;
                    });
                }

                scope.$watch('loadingItem.$promise', function(val) {
                    if (typeof scope.loadingItem === 'undefined' || !val ||
                    (scope.loadingItem && typeof scope.loadingItem.$resolved !== 'undefined')) {
                        return;
                    }

                    watchPromise(val);
                });

                scope.$watch('loadingItem.$resolved', function(val) {
                    if (typeof scope.loadingItem === 'undefined' || typeof val !== 'boolean') {
                        return;
                    }

                    scope.isLoading = !val;
                });
            }
        };
    }
]);