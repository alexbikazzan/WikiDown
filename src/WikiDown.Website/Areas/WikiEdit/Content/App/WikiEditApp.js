angular.module('WikiEdit', ['WikiDown', 'ngSanitize', 'ui.router'])
    .constant('wikiPartialsUrlBase', '/wikieditpartials/')
    .config([
        '$stateProvider', '$urlRouterProvider', 'wikiPartialsUrlBase',
        function($stateProvider, $urlRouterProvider, wikiPartialsUrlBase) {
            $urlRouterProvider.otherwise(function($injector) {
                var $state = $injector.get('$state');
                $state.go('edit');
            });

            $stateProvider
                .state('edit', {
                    url: '/edit/',
                    templateUrl: wikiPartialsUrlBase + 'edit/',
                    controller: 'EditController'
                })
                .state('edit.revision', {
                    url: '{revisionDate}/',
                    templateUrl: wikiPartialsUrlBase + 'edit/',
                    controller: 'EditController'
                })
                .state('history', {
                    url: '/history/',
                    templateUrl: wikiPartialsUrlBase + 'history/',
                    controller: 'HistoryController'
                })
                .state('history.preview', {
                    url: 'preview/{revisionDate}/',
                    templateUrl: wikiPartialsUrlBase + 'historypreview/',
                    controller: 'HistoryPreviewController'
                })
                .state('history.diff', {
                    url: 'diff/{oldRevisionDate}/{newRevisionDate}/',
                    templateUrl: wikiPartialsUrlBase + 'historydiff/',
                    controller: 'HistoryDiffController'
                })
                .state('meta', {
                    url: '/meta/',
                    templateUrl: wikiPartialsUrlBase + 'meta/',
                    controller: 'MetaController'
                })
                .state('admin', {
                    url: '/admin/',
                    templateUrl: wikiPartialsUrlBase + 'admin/',
                    controller: 'AdminController'
                });
        }
    ])
    .config([
        '$locationProvider',
        function($locationProvider) {
            $locationProvider.html5Mode(false);
        }
    ])
    .run([
        '$rootScope', '$state', '$stateParams',
        function($rootScope, $state, $stateParams) {
            $rootScope.$state = $state;
            $rootScope.$stateParams = $stateParams;
        }
    ]);