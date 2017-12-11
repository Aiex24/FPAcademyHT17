$(document).ready(function () {

    //Körs bara en gång för att lagra den kod vi behöver och inte plocka med inmatad data i början.
    let rowId = $(".RowToSelect").length;

    //Plockar ut de element vi vill ha in i den nya raden
    let elementOfFirstRow = $(".RowToSelect:first").clone();

    //Plockar ut input elementen i första raden och sätter deras värde till en tom sträng
    $(elementOfFirstRow).find(".colUserInput").each((index, element) => {
        $(element).attr('value', '');
    });

    //Smackar in första radens HTML kod i en enda sträng!
    let pureHTMLRow = $(elementOfFirstRow).prop('outerHTML');

    $("#AddRowButton").click(
        () => {
            let htmlRow = pureHTMLRow.replace(/0/g, rowId);
            $("#ButtonRow").before(htmlRow);
            rowId++;
            validateBoatForm();
        }
    );
    //TODO: Putsa Regexen så den inte ändrar några tabellvärden.
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
        rowId--;
        validateBoatForm();
    });

    function validateBoatForm() {
        //Tar bort och lägger till unobtrusive validering på samtliga fält i formen för att alla element ska lira.
        var form = $("#boatForm");
        $(form).removeData('validator');
        $(form).removeData('unobtrusiveValidation');
        $.validator.unobtrusive.parse(form);
            //.removeData("validator") /* added by the raw jquery.validate plugin */
            //.removeData("unobtrusiveValidation");  /* added by the jquery unobtrusive plugin*/
        //$.validator.unobtrusive.parse(form);
    }
});