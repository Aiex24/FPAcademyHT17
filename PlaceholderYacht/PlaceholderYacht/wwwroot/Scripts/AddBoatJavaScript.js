//TODO: funktionalitet för att ta bort rader och få rätt nummer och sånt.

$(document).ready(function () {
    let rowId = 1;
    let pureHTMLRow = "";
    //Plockar ut de element vi vill ha in i den nya raden
    let htmlOfFirst = document.querySelectorAll(".RowToSelect");
    //smackar in hela kalaset i en enda sträng!
    htmlOfFirst.forEach((node) => pureHTMLRow += node.outerHTML);

    $("#AddRowButton").click(
        () => {
            let htmlRow = pureHTMLRow.replace(/0/g, rowId);            rowId++;
            $("#ButtonRow").before(htmlRow);
        }
    );

    $("#VppInputTable").on("click", ".RemoveRowButton", (e) => {
        let thisButton = e.target;
        let row = thisButton.dataset.row;
        console.log(row);

        let followingSiblings = $(thisButton).parent().parent().nextAll(".RowToSelect");
        console.log(followingSiblings);
        //console.log(e.target.dataset.row);

    });
});