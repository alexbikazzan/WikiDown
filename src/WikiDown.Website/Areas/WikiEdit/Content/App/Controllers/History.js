angular.module('WikiEdit').controller('HistoryController', [
    '$scope', 'articleRevisionsDataApi',
    function($scope, articleRevisionsDataApi) {
        'use strict';

        function previewActiveRevision() {
            var activeRevision;
            $scope.articleRevisions.some(function(x) {
                return x.isActive ? ((activeRevision = x), true) : false;
            });

            if (activeRevision && activeRevision.id) {
                $scope.$state.go('history.preview', { revisionDate: activeRevision.id }, { location: 'replace', notify: false });
            }
        }

        $scope.closeRevision = function() {
            $scope.revisionDiff = undefined;
            $scope.revisionPreview = undefined;
            $scope.$state.go('history');
        };

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

        $scope.revertArticleToDraft = function() {
            var params = { slug: $scope.articleSlug };
            articleRevisionsDataApi.revertArticleToDraft(params, {},
                function() {
                    $scope.articleRevisions.forEach(function(x) { x.isActive = false; });

                    $scope.$state.go('history');
                });
        };

        $scope.publishRevision = function(revision, $event) {
            $event.stopPropagation();

            var params = { slug: $scope.articleSlug, revisionDate: revision.id };
            articleRevisionsDataApi.publishRevision(params, {},
                function() {
                    $scope.articleRevisions.forEach(function(x) { x.isActive = false; });

                    revision.isActive = true;

                    $scope.$state.go('history.preview', { revisionDate: revision.id });
                });
        };

        $scope.getPreviewRevisionText = function() {
            var activeRevision;
            $scope.articleRevisions.some(function(x) {
                return x.id === revisionDateParam ? ((activeRevision = x), true) : false;
            });

            return activeRevision ? activeRevision.text : undefined;
        };

        $scope.getIsAnyRevisionActive = function() {
            return $scope.articleRevisions.some(function(x) { return x.isActive; });
        };

        $scope.getIsRevisionFirst = function(revision) {
            return $scope.articleRevisions.length && ($scope.articleRevisions[0] === revision);
        };

        $scope.getIsRevisionLast = function(revision) {
            return $scope.articleRevisions.length && ($scope.articleRevisions[$scope.articleRevisions.length - 1] === revision);
        };

        $scope.getDiffLatestRevisionUrl = function(revision) {
            if (!revision || !$scope.articleRevisions.length) {
                return;
            }

            var latest = $scope.articleRevisions[0];
            if (latest !== revision) {
                return $scope.$state.href('history.diff', {
                    oldRevisionDate: revision.id,
                    newRevisionDate: latest.id,
                });
            }
        }

        $scope.getDiffPreviousRevisionUrl = function(revision) {
            if (!revision || !$scope.articleRevisions.length) {
                return;
            }

            var revisionIndex = $scope.articleRevisions.indexOf(revision),
                previous = (revisionIndex >= 0) ? $scope.articleRevisions[revisionIndex + 1] : undefined;

            if (previous && previous !== revision) {
                return $scope.$state.href('history.diff', {
                    oldRevisionDate: previous.id,
                    newRevisionDate: revision.id,
                });
            }
        }

        $scope.articleRevisions = articleRevisionsDataApi.query(
            { slug: $scope.articleSlug },
            function() {
                if ($scope.$state.is('history')) {
                    previewActiveRevision();
                }
            });
    }
]);