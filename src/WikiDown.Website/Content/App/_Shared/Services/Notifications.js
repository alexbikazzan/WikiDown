angular.module('WikiDown').service('notifications', [
    '$window',
    function($window) {
        if (typeof $window.alertify === 'undefined') {
            throw new Error('alertify not loaded.');
        }

        return {
            error: function(message, wait) {
                if (typeof wait !== 'undefined') {
                    $window.alertify.error(message, wait);
                } else {
                    $window.alertify.error(message);
                }
            },
            log: function(message, type, wait) {
                if (typeof type !== 'undefined' && typeof wait !== 'undefined') {
                    $window.alertify.log("Notification", type, wait);
                } else if (typeof type !== 'undefined') {
                    $window.alertify.log("Notification", type);
                } else {
                    $window.alertify.log("Notification");
                }
            },
            success: function(message, wait) {
                if (typeof wait !== 'undefined') {
                    $window.alertify.success(message, wait);
                } else {
                    $window.alertify.success(message);
                }
            },
            alert: function(message) {
                $window.alertify.alert(message);
            },
            confirm: function(message, callbackFn) {
                $window.alertify.confirm(message, callbackFn);
            },
            prompt: function(message, callbackFn) {
                $window.alertify.prompt(message, callbackFn);
            },
        };
    }
]);