(function(window, document, undefined) {
    'use strict';

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

    $('a[href="#"]').on('click', function(e) {
        e.preventDefault();
    });
}(window, document));