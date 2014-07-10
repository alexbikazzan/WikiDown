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

        $scope.saveArticleTags = function() {
            var tags = splitArray($scope.articleTags);

            articlesMetaDataApi.saveArticleTags(
                { slug: $scope.articleSlug },
                tags);
        };

        $scope.saveArticleRedirects = function() {
            var redirects = splitArray($scope.articleRedirects);

            articlesMetaDataApi.saveArticleRedirects(
                { slug: $scope.articleSlug },
                redirects);
        };

        $scope.saveArticleMeta = function() {
            articlesMetaDataApi.save(
                { slug: $scope.articleSlug },
                $scope.articleMeta);
        };

        articlesMetaDataApi.getArticleTags(
            { slug: $scope.articleSlug },
            function(result) {
                var resultText = joinArray(result);
                $scope.articleTags = resultText;
            });

        articlesMetaDataApi.getArticleRedirects(
            { slug: $scope.articleSlug },
            function(result) {
                var resultText = joinArray(result);
                $scope.articleRedirects = resultText;
            });

        $scope.articleMeta = articlesMetaDataApi.get({ slug: $scope.articleSlug });
    }
]);