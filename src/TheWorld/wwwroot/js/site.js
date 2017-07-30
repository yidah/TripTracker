// site.js
(function () {
    //var ele = $("#username");
    //ele.text("Un senor");

    //var main = $("#main");
    
    //main.mouseenter(function () {
    //    main.css("background-color", "#888");
    //});
    //main.mouseleave(function () {
    //    main.css("background-color", "");
    //});

    var $sidebarAndWrapper = $("#sidebar,#wrapper");

    // In _Layout.cshtml we have the toggle button
    //<button id="sidebarToggle" class="btn btn-primary">

    // Access italic element and fa class
    //<i class="fa fa-angle-left"></i>
    var $icon = $("#sidebarToggle i.fa");


    $("#sidebarToggle").on("click", function () {
        $sidebarAndWrapper.toggleClass("hide-sidebar");
        if ($sidebarAndWrapper.hasClass("hide-sidebar")) {
            $icon.removeClass("fa-angle-left");
            $icon.addClass("fa-angle-right");
        } else {
            $icon.addClass("fa-angle-left");
            $icon.removeClass("fa-angle-right");
        }
    });


    //$("#sidebarToggle").on("click", function () {
    //    $sidebarAndWrapper.toggleClass("hide-sidebar");
    //    if ($sidebarAndWrapper.hasClass("hide-sidebar")) {
    //        $(this).text("Show Sidebar");
    //    } else {
    //        $(this).text("Hide Sidebar");
    //    }
    //});
    
})();