angular.module('WikiEdit').controller('HistoryPreviewController', [
    '$scope', 'articleRevisionsDataApi',
    function($scope, articleRevisionsDataApi) {
        'use strict';

        var cachedRevisionPreviews = {};

        function previewRevision(revisionDate) {
            $scope.revisionDiff = undefined;

            var cachedPreview = cachedRevisionPreviews[revisionDate];
            if (cachedPreview) {
                showPreview(cachedPreview);
                return;
            }

            var params = { slug: $scope.articleSlug, revisionDate: revisionDate };
            $scope.revisionPreview = articleRevisionsDataApi.getPreview(
                params,
                function() {
                    cachedRevisionPreviews[revisionDate] = $scope.revisionPreview;
                    showPreview($scope.revisionPreview);
                });
        }

        function showPreview(preview) {
            $scope.revisionPreview = preview;
            $scope.$broadcast('historyElementDetailShow');
        }

        var revisionDateParam = $scope.$state.params.revisionDate;
        if (!revisionDateParam) {
            $scope.$state.go('history');
            return;
        }

        previewRevision(revisionDateParam);
    }
]);