angular.module('WikiDown').controller('EditController', [
    '$scope', 'wikiDown', 'articleRevisionsDataApi',
    function($scope, wikiDown, articleRevisionsDataApi) {
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

        // https://code.google.com/p/pagedown/wiki/PageDown
        function registerConverterHooks(converterHooks) {
            registerConverterHook(converterHooks, 'preConversion');
            registerConverterHook(converterHooks, 'postConversion');
            //registerConverterHook(converterHooks, 'plainLinkText');
        }

        var converter = Markdown.getSanitizingConverter();
        var editor = new Markdown.Editor(converter);

        registerConverterHooks(converter.hooks);

        editor.hooks.chain('onPreviewRefresh', function() {
            $scope.$root.$broadcast('onWmdPreviewRefresh');
        });

        editor.run();
        editor.refreshPreview();

        function updateMarkdownContent(markdownContent) {
            //console.log('updateMarkdownContent -- markdownContent:', markdownContent);
            $scope.markdownContent = markdownContent;
            //converter.makeHtml(markdownContent);
            editor.refreshPreview();
        }

        $scope.$on('markdownInputChange', function(e, markdownContent) {
            updateMarkdownContent(markdownContent);
            //$wmdInput.val(markdownContent);
        });

        console.log('selectedRevision: ', $scope.selectedRevision);

        $scope.articleRevisionChange = function (selectedRevision) {
            console.log('articleRevisionChange -- selectedRevision: ', selectedRevision);

            $scope.articleRevision = articleRevisionsDataApi.get({ articleSlug: $scope.articleSlug, revisionDateTime: selectedRevision },
                function() {
                    updateMarkdownContent($scope.articleRevision.markdownContent);
                });
        };

        $scope.articleRevisions = articleRevisionsDataApi.query({ articleSlug: $scope.articleSlug });
    }
]);