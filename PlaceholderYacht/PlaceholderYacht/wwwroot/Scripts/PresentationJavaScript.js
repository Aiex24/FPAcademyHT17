
$(document).ready(function () {
    $(".css-slider-wrapper").hide();
    $(".navigation").hide();
    $(".buttonstart").click(function () {
        $(".css-slider-wrapper").show();
        $(".navigation").show();
    });
    $(".buttonexit").click(function () {
        $(".css-slider-wrapper").hide();
        $(".navigation").hide();
    });
});

var TIMEOUT = 6000;
 
var interval = setInterval(handleNext, TIMEOUT);
 

function handleNext() {
 
  var $radios = $('input[class*="slide-radio"]');
  var $activeRadio = $('input[class*="slide-radio"]:checked');
 
  var currentIndex = $activeRadio.index();
  var radiosLength = $radios.length;
 
  $radios.attr('checked', false);
 
  if (currentIndex > radiosLength - 1) {
 
    $radios.first().attr('checked', true);
 
  } else {
 
    $activeRadio.next('input[class*="slide-radio"]').attr('checked', true);
 
  }
}