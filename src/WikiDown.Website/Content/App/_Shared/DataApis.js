angular.module('WikiDown')
    .value('metaBaseUrl', '/api/meta/')
    .factory('metaDataApi', [
        '$resource', 'metaBaseUrl',
        function($resource, metaBaseUrl) {
            return $resource(metaBaseUrl, {}, {
                getRoles: {
                    url: metaBaseUrl + 'roles/',
                    isArray: true
                },
                getAllRoles: {
                    url: metaBaseUrl + 'roles-all/',
                    isArray: true
                }
            });
        }
    ]);