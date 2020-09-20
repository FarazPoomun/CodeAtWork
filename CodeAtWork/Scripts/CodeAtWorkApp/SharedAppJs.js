

function BookMarkVid(elem, vidId) {
    $(elem).toggleClass("bookmarkUnselected");
    $(elem).toggleClass("bookmarkSelected");



    var ajaxPost = $.post('/CodeAtWorkApp/BookMarkVideo', {
        videoId: vidId, isSelected: elem.classList.value.includes("bookmarkSelected")
    });
    Promise.all([ajaxPost]).then((results) => {

    });
}

$("#NavSearch").focusin(function () {
    $("#HomeMain").addClass("HomeMainOverlap");
    $("#SearchResult").slideDown("fast");
})

$("#NavSearch").focusout(function () {
    $("#HomeMain").removeClass("HomeMainOverlap");
    $("#SearchResult").slideUp("fast");

})

function UpdateSearchCriteria(elem) {
    if (!IsNullOrEmpty(elem.value)) {
        var ajaxPost = $.post('/CodeAtWorkApp/SearchVideo', { searchedTxt: elem.value });
        Promise.all([ajaxPost]).then((results) => {
            document.getElementById("searchResultsFromNav").innerHTML = results;
        });
    }
    else {
        document.getElementById("searchResultsFromNav").innerHTML = "";

    }

}

function IsNullOrEmpty(value) {
    return !value || value.length == 0;
}