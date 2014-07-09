angular.module('WikiEdit').controller('MetaController', [
    '$scope', 'articlesMetaDataApi',
    function($scope, articlesMetaDataApi) {
        'use strict';

        //http://stackoverflow.com/questions/11594842/any-open-source-tag-editors-just-like-sos
        //http://ivaynberg.github.io/select2/index.html#tags
        //http://xoxco.com/projects/code/tagsinput/example.html

        //https://www.google.se/?gws_rd=ssl#q=select2+remote+tags
        //http://stackoverflow.com/questions/14229768/tagging-with-ajax-in-select2
        //https://github.com/ivaynberg/select2/issues/87

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
            console.log('saveArticleTags -- tags:', tags);

            articlesMetaDataApi.saveArticleTags(
                { slug: $scope.articleSlug },
                tags);
        };

        $scope.saveArticleRedirects = function() {
            var redirects = splitArray($scope.articleRedirects);
            console.log('saveArticleRedirects -- redirects:', redirects);

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