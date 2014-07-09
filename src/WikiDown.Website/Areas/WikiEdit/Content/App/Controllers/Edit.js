angular.module('WikiEdit').controller('EditController', [
    '$scope', '$timeout', 'wikiDown', 'articleRevisionsDataApi',
    function($scope, $timeout, wikiDown, articleRevisionsDataApi) {
        'use strict';

        function getLatestRevisionId() {
            var revision = ($scope.articleRevisions && $scope.articleRevisions.length) ?
                $scope.articleRevisions[0] :
                undefined;
            return (revision) ? revision.id : undefined;
        }

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

        function updateMarkdownContent(markdownContent) {
            $scope.markdownContent = markdownContent;

            $timeout(function() {
                editor.refreshPreview();
            });
        }

        function updateArticleRevision(selectedRevisionId) {
            $scope.$state.go('edit.revision', { revisionDate: selectedRevisionId }, { notify: false, location: 'replace' });

            $scope.articleRevision = articleRevisionsDataApi.get(
                { slug: $scope.articleSlug, revisionDate: selectedRevisionId },
                function() {
                    updateMarkdownContent($scope.articleRevision.markdownContent);
                });
        }

        //$scope.$on('markdownInputChange', function(e, markdownContent) {
        //    updateMarkdownContent(markdownContent);
        //});

        var converter = Markdown.getSanitizingConverter();
        var editor = new Markdown.Editor(converter);

        registerConverterHooks(converter.hooks);

        editor.hooks.chain('onPreviewRefresh', function() {
            ($scope.$root || $scope).$broadcast('onWmdPreviewRefresh');
        });

        editor.run();
        //editor.refreshPreview();

        $scope.saveArticleRevision = function() {
            articleRevisionsDataApi.save(
                { slug: $scope.articleSlug },
                $scope.articleRevision,
                function(result) {
                    $scope.articleRevisions.unshift(result);

                    var revisionId = getLatestRevisionId();
                    if (revisionId) {
                        $scope.selectedRevisionId = revisionId;
                    }

                    $scope.$root.isEditMode = !!revisionId;
                });
        };

        $scope.articleRevisionChange = function(selectedRevisionId) {
            updateArticleRevision(selectedRevisionId);
        };

        var revisionDate = $scope.$state.params.revisionDate;

        $scope.articleRevisions = articleRevisionsDataApi.query(
            { slug: $scope.articleSlug },
            function() {
                var revisionId = revisionDate || getLatestRevisionId();
                if (revisionId) {
                    $scope.selectedRevisionId = revisionId;

                    updateArticleRevision(revisionId);
                }
            });
    }
]);