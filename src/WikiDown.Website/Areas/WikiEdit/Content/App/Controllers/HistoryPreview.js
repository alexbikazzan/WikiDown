angular.module('WikiEdit').controller('HistoryPreviewController', [
    '$scope', 'articleRevisionsDataApi',
    function($scope, articleRevisionsDataApi) {
        'use strict';

        var cachedRevisionPreviews = {};

        function previewRevision(revisionDate) {
            $scope.revisionDiff = undefined;

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

        var revisionDateParam = $scope.$state.params.revisionDate;
        if (!revisionDateParam) {
            $scope.$state.go('history');
            return;
        }

        previewRevision(revisionDateParam);
    }
]);