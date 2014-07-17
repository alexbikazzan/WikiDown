angular.module('WikiEdit').controller('MetaController', [
    '$scope', 'articlesMetaDataApi',
    function($scope, articlesMetaDataApi) {
        'use strict';

        function splitArray(str) {
            if (!str) {
                return [];
            }

            return str.split(';')
                .map(function(x) { return x ? x.trim() : undefined; })
                .filter(function(x) { return !!x; });
        }

        function joinArray(arr) {
            return (arr && arr.join) ? arr.join('; ') : undefined;
        }

        function populateArticleRedirects(redirects) {
            $scope.articleRedirects = joinArray(redirects);
        }

        function populateArticleTags(tags) {
            $scope.articleTags = joinArray(tags);
        }

        $scope.saveArticleRedirects = function() {
            var redirects = splitArray($scope.articleRedirects);

            $scope.articleRedirectsSaving = articlesMetaDataApi.saveArticleRedirects(
                { slug: $scope.articleSlug },
                redirects,
                populateArticleRedirects);
        };

        $scope.saveArticleTags = function() {
            var tags = splitArray($scope.articleTags);

            $scope.articleTagsSaving = articlesMetaDataApi.saveArticleTags(
                { slug: $scope.articleSlug },
                tags,
                populateArticleTags);
        };

        $scope.articleRedirectsLoading = articlesMetaDataApi.getArticleRedirects(
            { slug: $scope.articleSlug },
            populateArticleRedirects);

        $scope.articleTagsLoading = articlesMetaDataApi.getArticleTags(
            { slug: $scope.articleSlug },
            populateArticleTags);
    }
]);