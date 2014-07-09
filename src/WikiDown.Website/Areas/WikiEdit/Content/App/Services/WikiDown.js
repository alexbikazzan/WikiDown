angular.module('WikiEdit').service('wikiDown', [
    '$window',
    function($window) {
        var wikiDown = $window.WikiDown || {};

        return wikiDown;
    }
]);