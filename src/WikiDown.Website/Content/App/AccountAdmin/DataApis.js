angular.module('AccountAdmin')
    .value('accountsApiBaseUrl', '/api/accounts/')
    .factory('accountsDataApi', [
        '$resource', 'accountsApiBaseUrl',
        function($resource, accountsApiBaseUrl) {
            return $resource(accountsApiBaseUrl + ':username/', {}, {});
        }
    ]);