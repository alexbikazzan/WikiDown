(function(window, document, undefined) {
    'use strict';

    function initAutocomplete() {
        if (typeof $.fn.autocomplete === 'undefined') {
            return;
        }

        function capitalise(str) {
            return (str) ? (str.charAt(0).toUpperCase() + str.slice(1)) : str;
        }

        function autoresizeSuggestionsContainer($this, $parent, $suggestionsContainer) {
            var left = parseInt($suggestionsContainer.css('left'), 10) || 0,
                width = parseInt($suggestionsContainer.css('width'), 10) || 0;

            var measuredWidth = getHiddenWidth($suggestionsContainer);
            if (measuredWidth <= width) {
                resetSuggestionsContainer($parent, $suggestionsContainer);
                return;
            }

            var bodyWidth = $('body').width();
            var autoWidth = Math.min(measuredWidth, bodyWidth - 4);

            var autoLeft = left - (autoWidth - width);
            autoLeft = Math.max(autoLeft, 0);

            $suggestionsContainer.css('left', autoLeft);
            $suggestionsContainer.width(autoWidth);
        }

        function formatResult(item, query) {
            var highlightStartRegex = new RegExp('^(' + query + ')(.*)$', 'i');

            var $result = $('<a/>').attr('href', item.data);

            if (item.redirect) {
                $result.text(item.value);
                $('<br/>').appendTo($result);

                var redirectTitleHtml = item.redirect.replace(highlightStartRegex, '<strong>$1</strong>$2');

                $('<em class="small" />').html('from: ' + redirectTitleHtml).appendTo($result);
            } else {
                var titleHtml = item.value.replace(highlightStartRegex, '<strong>$1</strong>$2');
                $result.html(titleHtml);
            }

            return $('<div/>').append($result).html();
        }

        function getHiddenWidth($el) {
            var $clone = $el.clone().css('visibility', 'hidden').appendTo($('body'));
            var width = $clone.outerWidth(true);

            $clone.remove();
            return width;
        }

        function resetSuggestionsContainer($parent, $suggestionsContainer) {
            var offset = $parent.offset(),
                width = $parent.outerWidth(true);

            $suggestionsContainer.css('left', offset.left);
            $suggestionsContainer.css('width', width);
        }

        $('.autocomplete').each(function() {
            var $this = $(this),
                $parent = $this.parentsUntil('form').last(),
                $autocomplete;

            // http://www.devbridge.com/sourcery/components/jquery-autocomplete/
            $this.autocomplete({
                serviceUrl: '/api/wiki/autocomplete/',
                minChar: 2,
                deferRequestBy: 200,
                paramName: 'q',
                tabDisabled: false,
                formatResult: formatResult,
                onSearchStart: function() {
                    if ($containsSuggestion) {
                        $suggestionsContainer.wrap('<div class="hidden"/>');

                        resetSuggestionsContainer($parent, $suggestionsContainer);
                    }
                },
                transformResult: function(data) {
                    var result = typeof data === 'string' ? $.parseJSON(data) : data,
                        hasResult = (result && result.suggestions && result.suggestions.length);
                    if (!hasResult) {
                        $suggestionsContainer.find('.autocomplete-suggestion:not(.contains-query)').remove();
                    }

                    return result;
                },
                onSearchComplete: function(query) {
                    if ($containsSuggestion) {
                        $containsSuggestion.appendTo($suggestionsContainer);

                        var capitalizedQuery = capitalise(query);
                        $queryLink.attr('href', '/search/?q' + query);
                        $queryText.text(capitalizedQuery);

                        setTimeout(function() {
                            autoresizeSuggestionsContainer($this, $parent, $suggestionsContainer);

                            $suggestionsContainer.unwrap().show();
                        }, 250);
                    }
                }
            });

            $autocomplete = $this.autocomplete();

            var $suggestionsContainer = $($autocomplete.suggestionsContainer),
                $containsSuggestion,
                $queryLink,
                $queryText;

            if (!$this.hasClass('header-nav-autocomplete')) {
                return;
            }

            $containsSuggestion = $('<div class="autocomplete-suggestion contains-query" />').append('');

            $queryLink = $('<a/>').appendTo($containsSuggestion);
            $queryText = $('<span/>').appendTo($queryLink);
            $('<small class="text-info">containing...</small><br/>').prependTo($queryLink);

            $suggestionsContainer.append($containsSuggestion);
        });
    }

    function initLocalTime() {
        if (typeof moment === 'undefined') {
            return;
        }

        var momentFormat = 'YYYY-MM-DD HH:mm:ss';

        function getLocalTime($el) {
            var dateValue = ($el.text() || '').trim() || '';

            var utcDate = dateValue ? moment.utc(dateValue) : undefined;
            var isValid = utcDate && utcDate.isValid && utcDate.isValid();
            var localDate = isValid ? utcDate.local().format(momentFormat) : undefined;

            return isValid ? localDate : undefined;
        }

        $('.utc-time').each(function() {
            var $this = $(this),
                localTime = getLocalTime($this);

            if (localTime) {
                $this.text(localTime);
            }
        });
    }

    $(function() {
        $('a[href="#"]').on('click', function(e) {
            e.preventDefault();
        });

        initAutocomplete();

        initLocalTime();
    });
}(window, document));