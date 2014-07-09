angular.module('WikiEdit').controller('HistoryController', [
    '$scope', 'articleRevisionsDataApi',
    function($scope, articleRevisionsDataApi) {
        'use strict';

        var cachedRevisionPreviews = {};

        function populatePreview(revisionId) {
            var cachedPreview = cachedRevisionPreviews[revisionId];
            if (cachedPreview) {
                $scope.revisionPreview = cachedPreview;
                return;
            }

            var params = { slug: $scope.articleSlug, revisionDate: revisionId };
            $scope.revisionPreview = articleRevisionsDataApi.getPreview(params,
                function() {
                    cachedRevisionPreviews[revisionId] = $scope.revisionPreview;
                });
        }

        $scope.deleteRevision = function(revision, $event) {
            $event.stopPropagation();

            var params = { slug: $scope.articleSlug, revisionDate: revision.id };
            articleRevisionsDataApi.delete(params, {},
                function() {
                    var index = $scope.articleRevisions.indexOf(revision);
                    if (index >= 0) {
                        $scope.articleRevisions.splice(index, 1);
                    }
                });
        };

        $scope.editRevision = function(revision, $event) {
            $event.stopPropagation();

            $scope.$state.go('edit.revision', { revisionDate: revision.id });
        };

        $scope.previewRevision = function(revision) {
            populatePreview(revision.id);
        };

        $scope.setActiveRevision = function(revision, $event) {
            $event.stopPropagation();

            var params = { slug: $scope.articleSlug, revisionDate: revision.id };
            articleRevisionsDataApi.updateActive(params, {},
                function() {
                    $scope.articleRevisions.forEach(function(x) { x.isActive = false; });

                    revision.isActive = true;
                });
        };

        $scope.articleRevisions = articleRevisionsDataApi.query(
            { slug: $scope.articleSlug },
            function() {
                var activeRevision;
                $scope.articleRevisions.some(function(x) {
                    return x.isActive ? ((activeRevision = x), true) : false;
                });

                if (activeRevision) {
                    $scope.previewRevision(activeRevision);
                }
            });
    }
]);