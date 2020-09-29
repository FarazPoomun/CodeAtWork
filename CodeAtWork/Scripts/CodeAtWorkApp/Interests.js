var storedPillsPerCategory = []; // Load per category once

$(".learnCode").hover(function () {
    $(".learnCode").css("color", "grey");
}, function () {
    $(".learnCode").css("color", "white");
});

$(".learnData").hover(function () {
    $(".learnData").css("color", "grey");
}, function () {
    $(".learnData").css("color", "white");
});



function subscribeTopic(item) {
    $(item).toggleClass("isNotSubscribed isSubscribed");
}

function toggleCategory(item) {
    var elem = item;
    if (typeof item === 'string')
        elem = document.getElementsByClassName("Categorylabel " + item)[0];

    if (!elem.classList.contains('selectedTopic')) {
        var selectedCat = document.getElementsByClassName("selectedTopic")[0];
        $(selectedCat).removeClass("selectedTopic");
        $(elem).addClass("selectedTopic");

        if (!storedPillsPerCategory.map(t => { return t.CategoryName == elem.innerHTML.trim(" ") }).filter(item => item)[0]) {
            storedPillsPerCategory.map(z => {
                if (z.CategoryName == selectedCat.innerHTML.trim(" "))
                    return z;
            }).filter(item => item)[0].Pills = document.getElementById("TopicPills").innerHTML;

            var ajaxPost = $.post('/CodeAtWorkApp/GetTopicsByCategoryName', { CatergoryName: elem.innerHTML.trim(" ") });
            Promise.all([ajaxPost]).then((results) => {
                document.getElementById("TopicPills").innerHTML = results[0];
                storedPillsPerCategory.push({ CategoryName: elem.innerHTML.trim(" "), Pills: results[0] });
            });
        }
        else {
            storedPillsPerCategory.map(z => {
                if (z.CategoryName == selectedCat.innerHTML.trim(" "))
                    return z;
            }).filter(item => item)[0].Pills = document.getElementById("TopicPills").innerHTML;

            document.getElementById("TopicPills").innerHTML = storedPillsPerCategory.map(t => {
                if (t.CategoryName == elem.innerHTML.trim(" "))
                    return t.Pills;
            }).filter(item => item)[0];
        }
    }
}

function UpdateTopics() {
    var UpdatePills = [];

    var selectedCat = document.getElementsByClassName("selectedTopic")[0];

    storedPillsPerCategory.map(z => {
        if (z.CategoryName == selectedCat.innerHTML.trim(" "))
            return z;
    }).filter(item => item)[0].Pills = document.getElementById("TopicPills").innerHTML;

    storedPillsPerCategory.forEach(z => {
        var tempElem = document.createElement('div');
        tempElem.innerHTML = z.Pills;
        var allTopicsUnderCat = tempElem.getElementsByClassName("br-pill");

        for (t = 0; t < allTopicsUnderCat.length - 1; t++) {
            var x = allTopicsUnderCat[t];
            var interestCategoryTopicId = parseInt(x.id.split('_')[1]);
            var obj = { InterestCategoryName: z.CategoryName, InterestCategoryTopicId: interestCategoryTopicId, IsSelected: !!x.classList.contains('isSubscribed') };
            UpdatePills.push(obj);
        }
    });

    var ajaxPost = $.post('/CodeAtWorkApp/UpdateInterestTopics', { InterestTopics: UpdatePills });
    Promise.all([ajaxPost]).then((results) => { });
}

function UpdateSearchCriteria(searchBox) {
    var searchedVal = searchBox.value.toLowerCase();
    if (!IsNullOrEmpty(searchedVal)) {
        storedPillsPerCategory.forEach(z => {
            var tempElem = document.createElement('div');
            tempElem.innerHTML = z.Pills;
            var allTopicsUnderCat = tempElem.getElementsByClassName("br-pill");

            for (t = 0; t < allTopicsUnderCat.length; t++) {
                var x = allTopicsUnderCat[t];
                if (!x.text.toLowerCase().includes(searchedVal)) {
                    $(x).css("display", "none");
                }
                else {
                    $(x).css("display", "inline-flex");
                }
            }

            z.Pills = tempElem.innerHTML;
        });

        var selectedCat = document.getElementsByClassName("selectedTopic")[0];
        var pillsDiv = document.getElementById("TopicPills");

        pillsDiv.innerHTML = storedPillsPerCategory.map(z => {
            if (z.CategoryName == selectedCat.innerHTML.trim(" "))
                return z;
        }).filter(item => item)[0].Pills;
    }
    else {
        storedPillsPerCategory.forEach(z => {
            var tempElem = document.createElement('div');
            tempElem.innerHTML = z.Pills;
            var allTopicsUnderCat = tempElem.getElementsByClassName("br-pill");

            for (t = 0; t < allTopicsUnderCat.length - 1; t++) {
                var x = allTopicsUnderCat[t];
                $(x).css("display", "inline-flex");
            }

            z.Pills = tempElem.innerHTML;
        });

        var selectedCat = document.getElementsByClassName("selectedTopic")[0];
        var pillsDiv = document.getElementById("TopicPills");

        pillsDiv.innerHTML = storedPillsPerCategory.map(z => {
            if (z.CategoryName == selectedCat.innerHTML.trim(" "))
                return z;
        }).filter(item => item)[0].Pills;
    }
}

function IsNullOrEmpty(value) {
    return !value || value.length == 0;
}