angular.module('WikiEdit')
    .value('articlesBaseUrl', '/api/articles/')
    .factory('articlesDataApi', [
        '$resource', 'articlesBaseUrl',
        function ($resource, articlesBaseUrl) {
            return $resource(articlesBaseUrl, {}, {});
        }
    ])
    .value('articleRevisionsBaseUrl', '/api/article-revisions/:slug/:revisionDate/')
    .factory('articleRevisionsDataApi', [
        '$resource', 'articleRevisionsBaseUrl',
        function($resource, articleRevisionsBaseUrl) {
            return $resource(articleRevisionsBaseUrl, {}, {
                getDiff: {
                    url: articleRevisionsBaseUrl + 'diff/:oldRevisionDate/:newRevisionDate/'
                },
                getLatest: {
                    url: articleRevisionsBaseUrl + 'latest/'
                },
                getPreview: {
                    url: articleRevisionsBaseUrl + 'preview/'
                },
                publishRevision: {
                    url: articleRevisionsBaseUrl + 'publish/',
                    method: 'POST'
                },
                revertArticleToDraft: {
                    url: articleRevisionsBaseUrl + 'revert-to-draft/',
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