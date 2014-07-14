angular.module('WikiEdit').controller('EditController', [
    '$scope', '$timeout', 'wikiDown', 'articleRevisionsDataApi',
    function($scope, $timeout, wikiDown, articleRevisionsDataApi) {
        'use strict';

        function onSaveRevisionSuccess(result) {
            var revisionDate = result ? result.dateId : undefined;

            $scope.$state.go('history.revision', { revisionDate: revisionDate });
        }

        function updateMarkdownContent(content) {
            $scope.$emit('markdownEditorContentChange', content);
        }

        $scope.saveArticleRevision = function(publish) {
            $scope.articleSaving = articleRevisionsDataApi.save(
                { slug: $scope.articleSlug, publish: publish },
                $scope.articleRevision,
                onSaveRevisionSuccess);
        };

        var revisionDateParam = $scope.$state.params.revisionDate;

        var dataApiFunction = revisionDateParam ? articleRevisionsDataApi.get : articleRevisionsDataApi.getLatest;

        $scope.articleRevision = dataApiFunction(
            { slug: $scope.articleSlug, revisionDate: revisionDateParam },
            function() {
                updateMarkdownContent($scope.articleRevision.markdownContent);
            });
    }
]);