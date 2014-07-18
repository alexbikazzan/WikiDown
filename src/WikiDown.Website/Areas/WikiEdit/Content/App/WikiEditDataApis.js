angular.module('WikiEdit')
    .value('articlesBaseUrl', '/api/articles/:slug/')
    .factory('articlesDataApi', [
        '$resource', 'articlesBaseUrl',
        function ($resource, articlesBaseUrl) {
            return $resource(articlesBaseUrl, {}, {
                getExists: {
                    url: articlesBaseUrl + 'exists/'
                }
            });
        }
    ])
    .value('articleRevisionsBaseUrl', '/api/article-revisions/:slug/')
    .factory('articleRevisionsDataApi', [
        '$resource', 'articleRevisionsBaseUrl',
        function($resource, articleRevisionsBaseUrl) {
            return $resource(articleRevisionsBaseUrl + ':revisionDate/', {}, {
                getDiff: {
                    url: articleRevisionsBaseUrl + 'diff/:oldRevisionDate/:newRevisionDate/'
                },
                getLatest: {
                    url: articleRevisionsBaseUrl + ':revisionDate/latest/'
                },
                getPreview: {
                    url: articleRevisionsBaseUrl + ':revisionDate/preview/'
                },
                publishRevision: {
                    url: articleRevisionsBaseUrl + ':revisionDate/publish/',
                    method: 'POST'
                },
                revertArticleToDraft: {
                    url: articleRevisionsBaseUrl + ':revisionDate/revert-to-draft/',
                    method: 'POST'
                }
            });
        }
    ])
    .value('articlesAdminBaseUrl', '/api/articles-admin/:slug/')
    .factory('articlesAdminDataApi', [
        '$resource', 'articlesAdminBaseUrl',
        function($resource, articlesAdminBaseUrl) {
            return $resource(articlesAdminBaseUrl, {}, {});
        }
    ])
    .value('articlesMetaBaseUrl', '/api/articles-meta/:slug/')
    .factory('articlesMetaDataApi', [
        '$resource', 'articlesMetaBaseUrl',
        function($resource, articlesMetaBaseUrl) {
            return $resource(articlesMetaBaseUrl, {}, {
                getArticleRedirects: {
                    url: articlesMetaBaseUrl + 'redirects/',
                    isArray: true
                },
                getArticleTags: {
                    url: articlesMetaBaseUrl + 'tags/',
                    isArray: true
                },
                saveArticleRedirects: {
                    url: articlesMetaBaseUrl + 'redirects/',
                    method: 'POST',
                    isArray: true
                },
                saveArticleTags: {
                    url: articlesMetaBaseUrl + 'tags/',
                    method: 'POST',
                    isArray: true
                }
            });
        }
    ]);