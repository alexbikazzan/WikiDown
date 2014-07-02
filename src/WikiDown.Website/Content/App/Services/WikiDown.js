angular.module('WikiDown').service('wikiDown', [
    '$window',
    function($window) {
        var wikiDown = $window.WikiDown || {};

        return wikiDown;
    }
]);