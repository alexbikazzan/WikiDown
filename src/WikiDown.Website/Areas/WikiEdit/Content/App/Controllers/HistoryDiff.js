angular.module('WikiEdit').controller('HistoryDiffController', [
    '$scope', 'articleRevisionsDataApi',
    function($scope, articleRevisionsDataApi) {
        'use strict';

        function diffRevisions(oldRevisionDate, newRevisionDate) {
            $scope.revisionPreview = undefined;

            var params = { slug: $scope.articleSlug, oldRevisionDate: oldRevisionDate, newRevisionDate: newRevisionDate };
            $scope.revisionDiff = articleRevisionsDataApi.getDiff(params);
        }

        var diffDatesParams = {
            oldRevisionDate: $scope.$state.params.oldRevisionDate,
            newRevisionDate: $scope.$state.params.newRevisionDate,
        };

        if (!diffDatesParams.oldRevisionDate || !diffDatesParams.newRevisionDate) {
            $scope.$state.go('history');
            return;
        }

        diffRevisions(diffDatesParams.oldRevisionDate, diffDatesParams.newRevisionDate);
    }
]);