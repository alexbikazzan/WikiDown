angular.module('WikiEdit', ['WikiDown', 'ui.router'])
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
                    views: {
                        '': {
                            templateUrl: wikiPartialsUrlBase + 'edit/',
                            controller: 'EditController'
                        }
                    }
                })
                .state('meta', {
                    url: '/meta/',
                    views: {
                        '': {
                            templateUrl: wikiPartialsUrlBase + 'meta/',
                            controller: 'MetaController'
                        }
                    }
                })
                .state('admin', {
                    url: '/admin/',
                    views: {
                        '': {
                            templateUrl: wikiPartialsUrlBase + 'admin/',
                            controller: 'AdminController'
                        }
                    }
                });
        }
    ]);