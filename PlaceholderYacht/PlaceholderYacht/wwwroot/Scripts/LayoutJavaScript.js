$(document).ready(function () {
    $(window).bind("scroll", function () {
        if ($(window).scrollTop() > 1) {
            $(".Nav-Index").addClass("scrolled");
        }
        else {
            $(".Nav-Index").removeClass("scrolled");
        }
    });

    
    var path = window.location.pathname.split("/").pop();

    // Account for home page with empty path
    if (path === '') {
        path = 'index';
    }

    var target = $('.menu li a[href="/home/' + path + '"]');
    // Add active class to target link
    var s = $('.menu li[class="' + path + '"]');
    s.addClass('active');
});