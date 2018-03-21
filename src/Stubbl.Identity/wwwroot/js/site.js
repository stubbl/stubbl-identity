;
(function($) {
    $(document).on("click",
        "a[data-append]",
        function() {
            var $this = $(this);
            var selector = $this.data("append");
            var key = selector.slice(1);
            var value = $(selector).val();

            if (!value) {
                return;
            }


            $this.attr("href",
                function() {
                    var uri = new URI(this.href);
                    uri.removeSearch(key)
                        .addSearch(key.charAt(0).toUpperCase() + key.slice(1), value);

                    return uri.href();
                });
        });
})(window.jQuery);