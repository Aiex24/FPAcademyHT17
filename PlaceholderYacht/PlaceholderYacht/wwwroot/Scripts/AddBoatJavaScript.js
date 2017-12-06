//TODO: funktionalitet för att ta bort rader och få rätt nummer och sånt.

$(document).ready(function () {
        var rowId = 1;
        var firstHTML = $("#AngleTwsKnotInputFirst");
        var htmlOfFirst = '<div>' + $("#AngleTwsKnotInputFirst").html() + '</div>';

        $("#AddRowButton").click(
            function () {
                var htmlRow = htmlOfFirst.replace(/0/g, rowId);                rowId++;
                console.log(htmlRow);
                $("#VppInputDiv").append(htmlRow);
            }
        );
});