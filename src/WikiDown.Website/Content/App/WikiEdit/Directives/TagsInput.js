angular.module('WikiEdit').directive('tagsInput', [
    function () {
        'use strict';

        return {
            restrict: 'A',
            scope: {
                ngModel: '='
            },
            require: '?ngModel',
            link: function (scope, element) {
                element.select2({
                    placeholder: 'Tags',
                    minimumInputLength: 3,
                    ajax: {
                        url: '/api/articles/',
                        data: function (term, page) {
                            console.log('select2.ajax.data', arguments);
                            return {
                                search: term,
                                page_limit: 10
                            }
                        }
                    },
                    result: function(data, page) {
                        console.log('select2.ajax.result', arguments);

                        return { results: [] };
                    }
                });
            }
        };
    }
]);