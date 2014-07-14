angular.module('WikiDown').filter('localUtcTime', [
    '$window',
    function($window) {
        'use strict';

        if (typeof $window.moment === 'undefined') {
            throw new Error('moment.js not loaded.');
        }

        return function(input, format) {
            format = format || 'YYYY-MM-DD HH:mm:ss';

            var utcDate = input ? moment.utc(input) : undefined;
            var isValid = utcDate && utcDate.isValid && utcDate.isValid();
            var localDate = isValid ? utcDate.local().format(format) : undefined;

            return isValid ? localDate : input;
        };
    }
]);