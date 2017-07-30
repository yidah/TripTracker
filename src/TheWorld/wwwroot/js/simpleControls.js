(function () {
    "use strict";

    angular.module("simpleControls", [])
    .directive("waitCursor", waitCursor);

    // In our cshtml document as Cursor is with upper case the directive will be 
    // <wait-cursor>
    function waitCursor() {
        return {
            // scope name is what is inside of the template(waitCursor.html) that is, show
            // displayWhen is used by the consumer in the Trips.cshtml
            scope: {
                show: "=displayWhen"
            },
            restrict: "E",
            templateUrl: "/views/waitCursor.html"
        };
    }

})();