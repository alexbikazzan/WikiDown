angular.module('WikiEdit').directive('wmdPreview', [
    'articlesDataApi',
    function (articlesDataApi) {
        'use strict';

        var wikiDownLinkPrefix = '/wiki/',
            linkExistsCache = {};

        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                function markLink($link, slug) {
                    var cachedExists = linkExistsCache[slug];

                    if (typeof cachedExists !== 'undefined') {
                        $link.toggleClass('no-article', !cachedExists);
                        return;
                    }

                    articlesDataApi.getExists({ slug: slug },
                        function(result) {
                            var exists = result && result.exists;
                            console.log(slug, exists);

                            linkExistsCache[slug] = exists;
                            $link.toggleClass('no-article', !exists);
                        });
                }

                function onWmdPreviewRefresh() {
                    element.find('a').each(function() {
                        var $this = $(this),
                            href = $this.attr('href');

                        if (href && (href.indexOf(wikiDownLinkPrefix) === 0)) {
                            var slug = href.slice(wikiDownLinkPrefix.length);

                            markLink($this, slug);
                        }
                    });
                }

                scope.$on('onWmdPreviewRefresh', onWmdPreviewRefresh);
            }
        };
    }
]);