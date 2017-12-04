$(document).ready(function () {
    $(window).bind("scroll", function () {
        var distance = 1;
        if ($(window).scrollTop() > distance) {
            $(".Nav-Index").addClass("scrolled");
        }
        else {
            $(".Nav-Index").removeClass("scrolled");
        }
    });


    $(".menu li").click(function () {
        if (!$(this).hasClass("active")) {
            $(".menu li.active").removeClass("active");
            var pageurl = "/home/" + window.location.href.substr(window.location.href
                .lastIndexOf("/") + 1);
            console.log(pageurl)
            $("#nav ul li a").each(function () {
                if ($(this).attr("href") == pageurl || $(this).attr("href") == '')
                    $(this).addClass("active");
            })

        }
    });

    var path = window.location.pathname.split("/").pop();

    // Account for home page with empty path
    if (path == '') {
        path = 'index';
    }

    var target = $('#nav a[href="/home/' + path + '"]');
    // Add active class to target link
    var s = $('.menu li[class="' + path + '"]');
    s.addClass('active');
});