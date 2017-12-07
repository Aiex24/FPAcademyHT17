//TODO: funktionalitet för att ta bort rader och få rätt nummer och sånt.

$(document).ready(function () {
    //Körs bara en gång för att lagra den kod vi behöver och inte plocka med inmatad data i början.
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
        let thisRow = $(`tr[data-row = "${thisButtonRowNumber}"]`);
        let followingSiblings = thisRow.last().nextAll(".RowToSelect");
        //tar bort den aktuella raden (och valideringsraden)
        thisRow.remove();

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