angular.module('WikiEdit').directive('wmdEditor', [
    '$rootScope', '$timeout', 'wikiDown',
    function ($rootScope, $timeout, wikiDown) {
        'use strict';
        
        function registerConverterHook(converterHooks, chainType) {
            var conversionsName = chainType + 's',
                conversions = wikiDown[conversionsName];

            if (!conversions || !conversions.length) {
                return;
            }

            for (var i = 0, len = conversions.length; i < len; i++) {
                var conversion = conversions[i];
                converterHooks.chain(chainType, conversion);
            }
        }

        function registerConverterHooks(converterHooks) {
            // https://code.google.com/p/pagedown/wiki/PageDown

            registerConverterHook(converterHooks, 'preConversion');
            registerConverterHook(converterHooks, 'postConversion');
            //registerConverterHook(converterHooks, 'plainLinkText');
        }

        return {
            restrict: 'A',
            scope: {
                ngModel: '='
            },
            require: '?ngModel',
            link: function (scope, element) {
                function updateMarkdownContent(markdownContent) {
                    scope.ngModel = markdownContent;

                    $timeout(function () {
                        editor.refreshPreview();
                    });
                }

                function onEditorPreviewRefresh() {
                    var elementValue = element.val();

                    if (scope.ngModel !== elementValue) {
                        scope.ngModel = elementValue;
                    }

                    $rootScope.$broadcast('wmdPreviewRefresh');
                }

                $rootScope.$on('markdownEditorContentChange', function (e, content) {
                    updateMarkdownContent(content);
                });

                var converter = Markdown.getSanitizingConverter();
                registerConverterHooks(converter.hooks);

                var editor = new Markdown.Editor(converter);
                editor.hooks.chain('onPreviewRefresh', onEditorPreviewRefresh);

                editor.run();
                //editor.refreshPreview();
            }
        };
    }
]);