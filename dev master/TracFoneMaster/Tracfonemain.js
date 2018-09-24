/**
 * @method formatAMPM
 * @param {String} date
 * @returns hh:mm am/pm
 */

function formatAMPM(date) {
  var hours = date.getHours();
  var minutes = date.getMinutes();
  var ampm = hours >= 12 ? 'PM' : 'AM';
  hours = hours % 12;
  hours = hours ? hours : 12; // the hour '0' should be '12'
  minutes = minutes < 10 ? '0' + minutes : minutes;
  var strTime = hours + ':' + minutes + ' ' + ampm;
  return strTime;
}

/**
* @method dateRearranging
* @param {String} dateInString
* @param {Number} type
* @returns a date in string format with specified type
*/

$.dateRearranging = function (dateInString, type) {
  var actualDateTime = new Date(dateInString);
  if (actualDateTime === "Invalid Date" || isNaN(actualDateTime)) {
    //  add error component if needed
    console.log("Date Format is Invalid");
    return "";
  } else {
    var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    switch (type) {
      case 1:
        return actualDateTime.toLocaleDateString();
      case 2:
        return actualDateTime.toLocaleDateString() + " " + actualDateTime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
      case 3:
        return actualDateTime.getMonth() + 1 + "/" + actualDateTime.getDate() + "/" + actualDateTime.getFullYear();
      case 4:
        return actualDateTime.getMonth() + 1 + "/" + actualDateTime.getDate() + "/" + actualDateTime.getFullYear() + " " + formatAMPM(actualDateTime);
      case 5:
        return monthNames[actualDateTime.getMonth()] + " " + actualDateTime.getDate() + ", " + actualDateTime.getFullYear();
      default:
        return actualDateTime.toLocaleDateString() + actualDateTime.toLocaleTimeString();
    }
  }
};


/** 
 * @method fetchData
 * @param {String} url
 * @returns a object either resolved or rejected based on the type
 */

$.fetchData = function (url) {

  if (url) {
    return $.ajax({
      url: url,
      headers: {
        Accept: "application/json;odata=verbose"
      }
    })
  }

}

/**
 * @method errorHandler
 * @param {String} currentContainer class name for current container
 * @param {String} errorContainer class name for error
 * @param {String} msg text to be shown
 * @returns hide the loader and shows the error message in a container
 */

$.errorHandler = function (currentContainer, errorContainer, msg) {
  $("." + currentContainer).empty();
  $("." + currentContainer).append("<div class=" + errorContainer + ">" + msg + "</div>")
}

/** 
 * @method loaderHandler
 * @param {Boolean} loaderStatus
 * @param {String} loaderID id of the loader
 * @returns enables or disables the loader based on the loaderStatus
 */

$.loaderHandler = function (loaderStatus, loaderID) {
  if (loaderStatus) {
    return $("#" + loaderID).css("display", "block")
  }
  return $("#" + loaderID).css("display", "none");
}

/**
 * @method colorForHealth
 * @param {String} textOfColor  
 * @return Assigns the color for the div and returns the div
 */
$.colorForHealth = function (textOfColor) {
  var color;
  switch (textOfColor) {
    case "green":
      color = "#4b9f59"
      break;
    case "red":
      color = "#dd4337"
      break;
    case "yellow":
      color = "#f1b317"
      break;
    default:
      console.log("no color found");
      return;
  }
  return "<div class=\"health-box\" style=\"background:" + color + "\"></div>"
}

/**
 * @method checkResponse
 * @param {Object} accepts a object 
 * @return a boolean value
 * @description checks the object is array or object and checks the validations
 */

$.checkResponse = function(obj){
  switch (Object.prototype.toString.call(obj)) {
    case "[object Object]":
        return !$.isEmptyObject(obj)
      break;
    case "[object Array]":
       return typeof obj !== "undefined" && obj.length > 0
    default:
      break;
  }
}




