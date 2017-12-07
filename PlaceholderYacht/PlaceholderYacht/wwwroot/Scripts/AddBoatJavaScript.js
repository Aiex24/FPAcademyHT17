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
            let htmlRow = pureHTMLRow.replace(/0/g, rowId);
            rowId++;
            $("#ButtonRow").before(htmlRow);
        }
    );

    $("#VppInputTable").on("click", ".RemoveRowButton", (e) => {
        let thisButton = e.target;
        let thisButtonRowNumber = thisButton.dataset.row;
        //TODO: Generell selektor istället för den här
        let followingSiblings = $(thisButton).parent().parent().next().nextAll(".RowToSelect");
        //tar bort den aktuella raden (och valideringsraden)
        $(`tr[data-row = "${thisButtonRowNumber}"]`).remove();


        followingSiblings.each((index, trElement) => {
            let row = trElement.dataset.row;
            let rowReplacing = row - 1;
            let regExVar = new RegExp(row, "g");
            trElement.outerHTML = trElement.outerHTML.replace(regExVar, rowReplacing);
        });
        //console.log(e.target.dataset.row);
        rowId--;

    });
});