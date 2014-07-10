angular.module('WikiEdit').controller('HistoryController', [
    '$scope', 'articleRevisionsDataApi',
    function($scope, articleRevisionsDataApi) {
        'use strict';
        
        var cachedRevisionPreviews = {};

        function previewRevision(revisionDate) {
            var cachedPreview = cachedRevisionPreviews[revisionDate];
            if (cachedPreview) {
                $scope.revisionPreview = cachedPreview;
                return;
            }

            var params = { slug: $scope.articleSlug, revisionDate: revisionDate };
            $scope.revisionPreview = articleRevisionsDataApi.getPreview(params,
                function() {
                    cachedRevisionPreviews[revisionDate] = $scope.revisionPreview;
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
            previewRevision(revision.id);
        };

        $scope.revertArticleToDraft = function() {
            var params = { slug: $scope.articleSlug };
            articleRevisionsDataApi.revertArticleToDraft(params, {},
                function() {
                    $scope.articleRevisions.forEach(function(x) { x.isActive = false; });
                });
        };

        $scope.publishRevision = function(revision, $event) {
            $event.stopPropagation();

            var params = { slug: $scope.articleSlug, revisionDate: revision.id };
            articleRevisionsDataApi.publishRevision(params, {},
                function() {
                    $scope.articleRevisions.forEach(function(x) { x.isActive = false; });

                    revision.isActive = true;
                });
        };

        function previewActiveRevision() {
            var activeRevision;
            $scope.articleRevisions.some(function(x) {
                return x.isActive ? ((activeRevision = x), true) : false;
            });

            if (activeRevision && activeRevision.id) {
                previewRevision(activeRevision.id);

                $scope.$state.go('history.revision', { revisionDate: activeRevision.id }, { location: 'replace', notify: false });
            }
        }

        $scope.getIsAnyRevisionActive = function() {
            return $scope.articleRevisions.some(function(x) { return x.isActive; });
        };

        var revisionDateParam = $scope.$state.params.revisionDate;

        $scope.articleRevisions = articleRevisionsDataApi.query(
            { slug: $scope.articleSlug },
            function() {
                if (!revisionDateParam) {
                    previewActiveRevision();
                } else {
                    previewRevision(revisionDateParam);
                }
            });

        $scope.articleRevisions.$promise.then(function() {
            $scope.$on('$stateChangeSuccess', function(e, toState, toParams) {
                var revisionDate = toParams.revisionDate;
                if (revisionDate) {
                    previewRevision(revisionDate);
                }
            });
        });
    }
]);