angular.module('WikiEdit').directive('tagsInput', [
    function() {
        'use strict';

        //http://stackoverflow.com/questions/11594842/any-open-source-tag-editors-just-like-sos
        //http://ivaynberg.github.io/select2/index.html#tags
        //http://xoxco.com/projects/code/tagsinput/example.html

        //https://www.google.se/?gws_rd=ssl#q=select2+remote+tags
        //http://stackoverflow.com/questions/14229768/tagging-with-ajax-in-select2
        //https://github.com/ivaynberg/select2/issues/87

        function splitArray(str) {
            if (!str) {
                return [];
            }

            return str.split(';')
                .map(function(x) { return x ? x.trim() : undefined; })
                .filter(function(x) { return !!x; });
            //.map(function (x) { return { text: x }; });
        }

        return {
            restrict: 'A',
            scope: {
                ngModel: '='
            },
            require: '^ngModel',
            link: function(scope, element) {
                console.log('tagsInput');

                /*var $select = element.select2({
                    tags: true,
                    //'simple_tags': true,
                    //tokenSeparators: [';'],
                    placeholder: 'Tags',
                    //multiple: true,
                    minimumInputLength: 2,
                    //ajax: {
                    //    url: '/api/articles/',
                    //    data: function(term, page) {
                    //        console.log('select2.ajax.data', arguments);
                    //        return {
                    //            search: term,
                    //            page_limit: 10
                    //        }
                    //    }
                    //},
                    result: function (data, page) {
                        console.log('select2.ajax.result', arguments);

                        return { results: [] };
                    }
                });*/

                scope.$watch('ngModel', function (val) {
                    if (!val) {
                        return;
                    }
                    var data = splitArray(scope.ngModel);
                    console.log('select2 -- data:', data, ', ngModel:', scope.ngModel);
                    element.select2({
                        tags: data,
                        multiple: true,
                    });


                    /*var data = (scope.ngModel && scope.ngModel.length) ? splitArray(scope.ngModel) : undefined;
                    console.log('select2 -- data:', data, ', ngModel:', scope.ngModel);
                    //var tags = scope.ngModel && scope.ngModel.length ? scope.ngModel : [];
                    if (data) {
                        //tags.slice().map(function (x) { return { text: x }; });

                        //element.select2({ data: data });
                        element.select2({
                            initSelection: function(el, callback) {
                                callback(data);
                            }
                        });

                        //element.select2({
                        //    tags: tags,
                        //    placeholder: 'Tags',
                        //    minimumInputLength: 2,
                        //    result: function (data, page) {
                        //        console.log('select2.ajax.result', arguments);

                        //        return { results: [] };
                        //    }
                        //});
                    }*/
                });

                scope.$on('$destroy', function() {
                    select.destroy();
                });
            }
        };
    }
]);