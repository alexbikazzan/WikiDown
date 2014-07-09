angular
    .module('AccountAdmin', ['WikiDown'])
    .controller('AccountAdminController', [
        '$scope', 'accountsDataApi', 'metaDataApi',
        function($scope, accountsDataApi, metaDataApi) {
            'use strict';

            $scope.addAccount = {};

            function updateAccounts() {
                $scope.accounts = accountsDataApi.query();
            }

            $scope.saveAccount = function(account, isNew) {
                accountsDataApi.save(account, function(result) {
                    if (isNew) {
                        updateAccounts();
                    } else {
                        angular.copy(result, account);
                    }

                    $scope.addAccount = {};
                });
            };

            $scope.deleteAccount = function(account) {
                account.$delete(params,
                    function() {
                        updateAccounts();
                    });
            };

            $scope.roleOptions = metaDataApi.getRoles();

            updateAccounts();
        }
    ]);