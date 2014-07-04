(function(window, document, undefined) {
    'use strict';

    var $preview = $('#article-revision-preview'),
        $previewContent = $('#article-revision-preview-content'),
        cachedPreviews = {};

    function populatePreview(url) {
        var cachedPreview = cachedPreviews[url];
        if (cachedPreview) {
            $previewContent.html(cachedPreview);
            return;
        }

        $.ajax({
            url: url,
            success: function(result) {
                cachedPreviews[url] = result.htmlContent;
                $previewContent.html(result.htmlContent);
            }
        });
    }

    $('.article-revision-preview')
        .on('click', function() {
            var $this = $(this),
                apiUrl = $this.data('api-url');

            if (apiUrl) {
                populatePreview(apiUrl);
            }
        })
        .one('click', function() {
            $preview.show();
        });
}(window, document));