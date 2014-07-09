angular.module('AccountAdmin')
    .value('accountsApiBaseUrl', '/api/admin/')
    .factory('accountsDataApi', [
        '$resource', 'accountsApiBaseUrl',
        function($resource, accountsApiBaseUrl) {
            return $resource(accountsApiBaseUrl + ':username/', {}, {});
        }
    ]);