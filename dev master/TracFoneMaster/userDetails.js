'use strict';
$(document).ready(function () {
    // fetching current user details
  var delayInMilliseconds = 500;
  setTimeout(function() {
    $.ajax({
        url: "http://dpvmspdev:10001/sites/intranet/_api/Web/CurrentUser",
        type: 'GET',
        headers: {
            accept: 'application/json;odata=verbose'
        },
        success: function success(res) {
  
            dataDisplay(res.d);
        },
        error: function error(err) {
            console.log(JSON.stringify(err));
        }
    });
    // appending the data to dom
    var dataDisplay = function dataDisplay(currentUser) {
        $("#login-user-name").append(currentUser.Title);
        if (currentUser.Email) {
            // $("#login img").attr('src', "https://outlook.office365.com/owa/service.svc/s/GetPersonaPhoto?email=" + currentUser.Email + "&UA=0&size=HR64x64&sc=1521518892021?" + new Date().getTime());
            $("#login-user-img").attr('src', "http://dpvmspdev:10001/sites/intranet/_layouts/15/userphoto.aspx?size=S&username=" + currentUser.Email);
        }
    };
    }, delayInMilliseconds);
});