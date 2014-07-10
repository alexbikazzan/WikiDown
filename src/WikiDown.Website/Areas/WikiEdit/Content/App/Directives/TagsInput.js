angular.module('WikiEdit').directive('tagsInput', [
    function () {
        'use strict';
        
        //http://stackoverflow.com/questions/11594842/any-open-source-tag-editors-just-like-sos
        //http://ivaynberg.github.io/select2/index.html#tags
        //http://xoxco.com/projects/code/tagsinput/example.html

        //https://www.google.se/?gws_rd=ssl#q=select2+remote+tags
        //http://stackoverflow.com/questions/14229768/tagging-with-ajax-in-select2
        //https://github.com/ivaynberg/select2/issues/87

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