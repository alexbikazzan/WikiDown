angular.module('WikiEdit').directive('wmdEditor', [
    '$rootScope', '$timeout', 'wikiDown',
    function($rootScope, $timeout, wikiDown) {
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

            //converterHooks.chain('preConversion', function(markdown) {
            //    console.log('preConversion', markdown);
            //    return markdown;
            //});

            //converterHooks.chain('postConversion', function (html) {
            //    console.log('postConversion', ﻿html);
            //    return html;
            //});

            //registerConverterHook(converterHooks, 'plainLinkText');
        }

        return {
            restrict: 'A',
            scope: {
                model: '=ngModel'
            },
            require: '^ngModel',
            link: function(scope, element, attrs) {
                function updateMarkdownContent() {
                    $timeout(function() {
                        editor.refreshPreview();
                    }, 100);
                }

                function onEditorPreviewRefresh() {
                    $timeout(function() {
                        var elementValue = element.val();

                        if (elementValue !== scope.model) {
                            scope.model = elementValue;
                        }
                    });
                }

                scope.$watch('model', function(val) {
                    updateMarkdownContent(val);
                });

                $rootScope.$on('markdownEditorContentChange', function(e, content) {
                    scope.model = content;
                });

                var converter = Markdown.getSanitizingConverter();
                registerConverterHooks(converter.hooks);

                var editor = new Markdown.Editor(converter);
                editor.hooks.chain('onPreviewRefresh', onEditorPreviewRefresh);

                editor.run();
            }
        };
    }
]);