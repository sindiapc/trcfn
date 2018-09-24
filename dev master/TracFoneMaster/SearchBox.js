var srchPos;
    function moveSearchBox() {
    //alert("Working");
    
        if ($("#SearchBox").length > 0) {           
            $("#SearchContainer").append($("#SearchBox"));
            clearInterval(srchPos);
        }
    }
    $(function () {
        
        if (window.location.href.indexOf("osssearchresults.aspx") <0) {
            srchPos = setInterval(moveSearchBox, 300);
        }
    });
