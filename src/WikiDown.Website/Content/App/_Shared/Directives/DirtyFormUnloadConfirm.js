angular.module('WikiDown').directive('dirtyFormUnloadConfirm', [
    '$timeout', '$window',
    function($timeout, $window) {
        'use strict';

        var confirmText = 'You have unsaved changes.\n\nAre you sure you want to leave the editor?';

        return {
            restrict: 'A',
            require: 'form',
            scope: {
                dirtyFormUnloadConfirm: '&dirtyFormUnloadConfirm',
                dirtyFormResetOn: '@dirtyFormResetOn'
            },
            link: function(scope, element, attrs, form) {
                var originalWindowOnbeforeunload = $window.onbeforeunload,
                    cancelStateChangeStartListener;

                function attachListeners() {
                    detachListeners();

                    $window.onbeforeunload = function() {
                        if (typeof originalWindowOnbeforeunload === 'function') {
                            originalWindowOnbeforeunload.apply(this, arguments);
                        }

                        handleWindowBeforeUnload();
                    };

                    cancelStateChangeStartListener = scope.$on('$stateChangeStart', handleStateChangeStart);
                }

                function detachListeners() {
                    $window.onbeforeunload = originalWindowOnbeforeunload;
                    (cancelStateChangeStartListener || angular.noop)();
                }

                function formDirtyChangeListener(value) {
                    if (value) {
                        attachListeners();
                    } else {
                        detachListeners();
                    }
                }

                function handleStateChangeStart(e) {
                    if (!form.$dirty) {
                        return;
                    }

                    var result = $window.confirm(confirmText);

                    if (!result) {
                        e.preventDefault();
                    } else {
                        $timeout(handleUnload);
                    }
                }

                function handleWindowBeforeUnload() {
                    $timeout(handleUnload, 500);

                    return confirmText;
                }

                function handleUnload() {
                    form.$setPristine();

                    (scope.dirtyFormUnloadConfirm || angular.noop)();
                }

                scope.$watch(function() { return form.$dirty; }, formDirtyChangeListener);

                if (scope.dirtyFormResetOn) {
                    scope.$on(scope.dirtyFormResetOn, function() {
                        form.$setPristine();
                    });
                }
            }
        };
    }
]);