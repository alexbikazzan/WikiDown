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
                $scope.$state.go('history.preview', { revisionDate: activeRevision.id }, { location: 'replace' });
            }
        }

        $scope.closeRevision = function() {
            $scope.revisionDiff = undefined;
            $scope.revisionPreview = undefined;
            $scope.$state.go('history', {}, { location: false });
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
        
        $scope.getIsAnyRevisionActive = function() {
            return $scope.articleRevisions.some(function(x) { return x.isActive; });
        };

        $scope.articleRevisions = articleRevisionsDataApi.query(
            { slug: $scope.articleSlug },
            function() {
                $scope.articleRevisionFirst = $scope.articleRevisions[0];

                for (var i = 0, len = $scope.articleRevisions.length; i < len; i++) {
                    var item = $scope.articleRevisions[i];
                    //item.previousItem = $scope.articleRevisions[i - 1];
                    item.nextItem = $scope.articleRevisions[i + 1];
                }

                if ($scope.$state.is('history')) {
                    previewActiveRevision();
                }
            });
    }
]);