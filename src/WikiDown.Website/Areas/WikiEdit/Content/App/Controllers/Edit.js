angular.module('WikiEdit').controller('EditController', [
    '$scope', '$timeout', 'wikiDown', 'articleRevisionsDataApi',
    function($scope, $timeout, wikiDown, articleRevisionsDataApi) {
        'use strict';

        console.log('EditController');

        function getLatestRevisionId() {
            var revision = ($scope.articleRevisions && $scope.articleRevisions.length) ?
                $scope.articleRevisions[0] :
                undefined;
            return (revision) ? revision.id : undefined;
        }

        function onArticleRevisionsLoaded() {
            $scope.isCreateMode = (!$scope.articleRevisions || !$scope.articleRevisions.length);

            var revisionId = getLatestRevisionId();
            if (revisionId) {
                $scope.selectedRevisionId = revisionId;

                updateArticleRevision(revisionId);
            }
        }

        function onSaveRevisionSuccess(result) {
            $scope.$root.isCreateMode = false;

            var revisionDate = result ? result.dateId : undefined;
            $scope.$state.go('history', { revisionDate: revisionDate });
        }

        function updateArticleRevision(selectedRevisionId) {
            $scope.articleRevision = articleRevisionsDataApi.get(
                { slug: $scope.articleSlug, revisionDate: selectedRevisionId },
                function() {
                    updateMarkdownContent($scope.articleRevision.markdownContent);
                });
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
        if (revisionDateParam) {
            updateArticleRevision(revisionDateParam);
            return;
        }

        $scope.articleRevisions = articleRevisionsDataApi.query(
            { slug: $scope.articleSlug },
            onArticleRevisionsLoaded);
    }
]);