$(window).on("load", function() {
  dropdownHeightFixing();

})

function dropdownHeightFixing() {
  if (($("#vendor-staffing").offset().top + $("#vendor-staffing").height()) <= $(window).height()) {
	  var a = $("#vendor-staffing").outerHeight() 
	  $("#vendor-staffing").css("max-height", a + 180)
	  $("#vendor-staffing").css("overflow-y", "scroll")
  } 
  if (($("#CustomOpen").offset().top + $("#CustomOpen").height()) >= $(window).height()) {
	  var a = $("#CustomOpen").outerHeight() 
	  $("#CustomOpen").css("max-height", a - 100)
	  $("#CustomOpen").css("overflow-y", "scroll")
  } 

}
 
