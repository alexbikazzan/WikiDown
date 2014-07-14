angular
    .module('AccountAdmin', ['WikiDown'])
    .controller('AccountAdminController', [
        '$scope', 'accountsDataApi', 'metaDataApi',
        function($scope, accountsDataApi, metaDataApi) {
            'use strict';

            function resetAddAccount() {
                var accessLevel = $scope.addAccount ? $scope.addAccount.accessLevel : 1;
                $scope.addAccount = { accessLevel: accessLevel };
            }

            $scope.deleteAccount = function (account) {
                var params = { username: account.userName };
                accountsDataApi.delete(
                    params,
                    function() {
                        var index = $scope.accounts.indexOf(account);
                        if (index >= 0) {
                            $scope.accounts.splice(index, 1);
                        }
                    });
            };

            $scope.roleFilter = function(account) {
                return function(role) {
                    var isAdmin = account.accessLevel >= 30;
                    return !isAdmin || (role.id >= 30);
                };
            };

            $scope.saveAccount = function(account, isNew) {
                accountsDataApi.save(
                    account,
                    function(result) {
                        if (isNew) {
                            $scope.accounts.push(result);

                            $scope.accounts.sort(function(a, b) {
                                var aEmail = a.email,
                                    bEmail = b.email;
                                return (aEmail < bEmail) ? -1 : ((aEmail > bEmail) ? 1 : 0);
                            });

                            resetAddAccount();
                        } else {
                            angular.copy(result, account);
                        }
                    });
            };

            $scope.accounts = accountsDataApi.query();

            $scope.roleOptions = metaDataApi.getRoles();

            resetAddAccount();
        }
    ]);