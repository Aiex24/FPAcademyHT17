//TODO: funktionalitet för att ta bort rader och få rätt nummer och sånt.

$(document).ready(function () {
    let rowId = 1;
    let firstHTML = "";
    let htmlOfFirst = document.querySelectorAll(".RowToSelect");
    htmlOfFirst.forEach((node) => firstHTML += node.outerHTML);
    console.log(htmlOfFirst);
    console.log(firstHTML);

    $("#AddRowButton").click(
        function () {
            let htmlRow = firstHTML.replace(/0/g, rowId);            rowId++;
            //console.log(htmlRow);
            $("#ButtonRow").before(htmlRow);
        }
    );
});