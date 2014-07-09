(function (window, document, undefined) {
    'use strict';

    $('a[href="#"]').on('click', function(e) {
        e.preventDefault();
    });
}(window, document));