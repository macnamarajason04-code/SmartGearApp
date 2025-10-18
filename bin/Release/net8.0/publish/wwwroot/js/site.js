$(document).ready(function () {
    $(".btn-delete").on("click", function (e) {
        if (!confirm("Are you sure you want to delete this product?")) {
            e.preventDefault();
        }
    });

    const scrollBtn = $('<button id="scrollTop" class="btn btn-smartgear">↑</button>').appendTo("body");
    scrollBtn.css({
        position: "fixed",
        bottom: "20px",
        right: "20px",
        display: "none",
        "z-index": 1000
    });

    $(window).scroll(function () {
        if ($(this).scrollTop() > 100) {
            scrollBtn.fadeIn();
        } else {
            scrollBtn.fadeOut();
        }
    });

    scrollBtn.on("click", function () {
        $("html, body").animate({ scrollTop: 0 }, 400);
    });
});
