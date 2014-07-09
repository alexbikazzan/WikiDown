angular.module('WikiEdit')
    .value('articleRevisionsBaseUrl', '/api/article-revisions/:slug/:revisionDate/')
    .factory('articleRevisionsDataApi', [
        '$resource', 'articleRevisionsBaseUrl',
        function($resource, articleRevisionsBaseUrl) {
            return $resource(articleRevisionsBaseUrl, {}, {
                getPreview: {
                    url: articleRevisionsBaseUrl + 'preview/'
                },
                updateActive: {
                    url: articleRevisionsBaseUrl + 'update-active/',
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
                    method: 'POST'
                },
                saveArticleTags: {
                    url: articlesMetaBaseUrl + 'tags/',
                    method: 'POST'
                }
            });
        }
    ]);