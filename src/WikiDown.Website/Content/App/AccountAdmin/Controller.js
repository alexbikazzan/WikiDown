angular.module('AccountAdmin').controller('AccountAdminController', [
    '$scope', 'accountsDataApi',
    function($scope, accountsDataApi) {
        'use strict';

        $scope.addAccount = {};
        $scope.roleOptions = [
            { id: 'Editor', text: 'Editor' }, { id: 'SuperUser', text: 'SuperUser' }, { id: 'Admin', text: 'Admin' }
        ];

        function updateAccounts() {
            $scope.accounts = accountsDataApi.query();
        }

        $scope.saveAccount = function(account) {
            $scope.dataApiPromise = accountsDataApi.save(account, function () {
                updateAccounts();
            });
        };

        $scope.deleteAccount = function(account) {

            $scope.dataApiPromise = account.$delete(params,
                function() {
                    updateAccounts();
                });
        };

        updateAccounts();
    }
]);