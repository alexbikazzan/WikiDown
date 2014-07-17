angular.module('WikiEdit').controller('AdminController', [
    '$scope', '$timeout', 'articlesAdminDataApi', 'metaDataApi',
    function($scope, $timeout, articlesAdminDataApi, metaDataApi) {
        'use strict';

        $scope.deleteArticle = function() {
            $scope.articleAccessDeleting = articlesAdminDataApi.delete({ slug: $scope.articleSlug },
                function() {
                    $scope.$state.go('edit');
                });
        };

        $scope.articleAccessChange = function() {
            $timeout(function() {
                if ($scope.articleAccess.canEdit < $scope.articleAccess.canRead) {
                    $scope.articleAccess.canEdit = $scope.articleAccess.canRead;
                }
                if ($scope.articleAccess.canAdmin < $scope.articleAccess.canRead) {
                    $scope.articleAccess.canAdmin = $scope.articleAccess.canRead;
                }
            });
        };

        $scope.saveArticleAccess = function() {
            $scope.articleAccessSaving = articlesAdminDataApi.save(
                { slug: $scope.articleSlug },
                $scope.articleAccess);
        };

        $scope.setDefaultArticleAccess = function() {
            $scope.articleAccess.canRead = 0;
            $scope.articleAccess.canEdit = 10;
            $scope.articleAccess.canAdmin = 30;
        };

        $scope.roleOptions = metaDataApi.getAllRoles();

        $scope.articleAccess = articlesAdminDataApi.get({ slug: $scope.articleSlug });
    }
]);