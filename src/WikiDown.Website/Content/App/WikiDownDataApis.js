angular.module('WikiDown')
    .value('articlesApiBaseUrl', '/api/articles/')
    .factory('articlesDataApi', [
        '$resource', 'articlesApiBaseUrl',
        function($resource, articlesApiBaseUrl) {
            return $resource(articlesApiBaseUrl + ':articleId', {}, {});
        }
    ])
    .value('articleRevisionsApiBaseUrl', '/api/articlerevisions/:articleSlug/:revisionDateTime/')
    .factory('articleRevisionsDataApi', [
        '$resource', 'articleRevisionsApiBaseUrl',
        function($resource, articleRevisionsApiBaseUrl) {
            return $resource(articleRevisionsApiBaseUrl, {}, {});
        }
    ]);