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

function IsNullOrEmpty(value) {
    return !value || value.length == 0;
}