var modalOpened = false;
var videoOpened = false;
var userSidebarHidden = false;
var roomSiderbarHidden = false;

jQuery(window).ready(function ($) {

    if ($(document).width() < 768)
    {
        $("#UserSidebar").addClass("hide");
        userSidebarHidden = true;
    }
    if ($(document).width() < 618) {
        $("#RoomSidebar").addClass("hide");
        roomSiderbarHidden = true;
    }


    setInterval(function () {
        $('time').each(function (i, e) {
            var time = moment($(e).attr('datetime'));
            $(e).html(" " + time.fromNow());
        });
    }, 1000);
});

$(document).keyup(function (e) {
    if ($("#UserChatMessage:focus") && (e.keyCode === 13)) {
        SendMessage();
    }
    if ($("#UserVideoLink:focus") && (e.keyCode === 13)) {
        SendVideo();
    }
    if (e.keyCode == 27) {
        if(modalOpened)
        {
            modalOpened = false
            $("#ModalWindow").addClass("hidden");
        }

    }
});

function UserLeftTheChat(username)
{
}

function GetYoutubeVideoId(link)
{
    var i, r, rx = /^.*(?:(?:youtu\.be\/|v\/|vi\/|u\/\w\/|embed\/)|(?:(?:watch)?\?v(?:i)?=|\&v(?:i)?=))([^#\&\?]*).*/;
    r = link.match(rx);
    var id = r[1];
    if (id.indexOf(" ") > 0)
    {
        id = id.substring(0, id.indexof(" "));
    }
    return id;
}

$(window).resize(function () {

    if ($(document).width() < 618 && roomSiderbarHidden === false) {
        $("#RoomSidebar").addClass("hide");
        roomSiderbarHidden = true;
    }
    if ($(document).width() < 768 && userSidebarHidden === false) {
        $("#UserSidebar").addClass("hide");
        userSidebarHidden = true;
    }
    if ($(document).width() > 618 && roomSiderbarHidden === true) {
        $("#RoomSidebar").removeClass("hide");
        roomSiderbarHidden = false;
    }
    if ($(document).width() > 768 && userSidebarHidden === true) {
        $("#UserSidebar").removeClass("hide");
        userSidebarHidden = false;
    }
});



function AddMessageToDB(message, room)
{
    $.ajax({
        type: "POST",
        url: "/Home/AddMessage",
        data: { Message: message, Room : room },
        cache: false,
        dataType: "json"
    });
}

function ApproveConversation(conversation) {
    $.ajax({
        type: "POST",
        url: "/Private/ApproveConversation",
        data: { ConversationId: conversation},
        cache: false,
        dataType: "json"
    });
}

function AddPrivateMessageToDB(message, conversation) {
    $.ajax({
        type: "POST",
        url: "/Private/AddPrivateMessage",
        data: { Message: message, ConversationId : conversation },
        cache: false,
        dataType: "json"
    });
}
function UpvoteUser(displayname) {
    var rated = $("#RatingOverview").data('rated');
    if (rated == "No")
    {
        $.ajax({
            type: "POST",
            url: "/User/UpvoteUser",
            data: { DisplayName: displayname },
            cache: false,
            dataType: "json"
        });
        var score = parseInt($("#UserRatingCount").text(), 10);
        score++;
        $("#UserRatingCount").text(score);
    }
    var rated = $("#RatingOverview").data('rated',"Yes");
}

function DownvoteUser(displayname) {
    var rated = $("#RatingOverview").data('rated');
    if (rated == "No")
    {
        $.ajax({
            type: "POST",
            url: "/User/DownvoteUser",
            data: { DisplayName: displayname },
            cache: false,
            dataType: "json"
        });
        var score = parseInt($("#UserRatingCount").text(), 10);
        score -= 1;
        $("#UserRatingCount").text(score);
    }
    var rated = $("#RatingOverview").data('rated', "Yes");
}


$(document).on('click', '.panel-heading span.icon_minim', function (e) {
    var $this = $(this);
    if (!$this.hasClass('panel-collapsed')) {
        $this.parents('.panel').find('.panel-body').slideUp();
        $this.addClass('panel-collapsed');
        $this.removeClass('glyphicon-minus').addClass('glyphicon-plus');
    } else {
        $this.parents('.panel').find('.panel-body').slideDown();
        $this.removeClass('panel-collapsed');
        $this.removeClass('glyphicon-plus').addClass('glyphicon-minus');
    }
});
$(document).on('focus', '.panel-footer input.chat_input', function (e) {
    var $this = $(this);
    if ($('#minim_chat_window').hasClass('panel-collapsed')) {
        $this.parents('.panel').find('.panel-body').slideDown();
        $('#minim_chat_window').removeClass('panel-collapsed');
        $('#minim_chat_window').removeClass('glyphicon-plus').addClass('glyphicon-minus');
    }
});
$(document).on('click', '#new_chat', function (e) {
    var size = $(".chat-window:last-child").css("margin-left");
    size_total = parseInt(size) + 400;
    alert(size_total);
    var clone = $("#UsersInChat").clone().appendTo(".container");
    clone.css("margin-left", size_total);
});
$(document).on('click', '.icon_close', function (e) {
    $(this).parent().parent().parent().parent().remove();
});

function OpenModal(resource, username)
{
    modalOpened = true

    switch(resource)
    {
        case "Settings":
            $("#ModalContent").attr("src", "/Manage/Index");
            break;
        case "Profile":
            $("#ModalContent").attr("src", "/User/Profile/" + username);
            break;
    }
    $("#ModalWindow").removeClass("hidden");
}

function CloseModal()
{
    modalOpened = false
    $("#ModalContent").attr("src", "");
    $("#ModalWindow").addClass("hidden");
}

function OpenVideoDialog()
{
    if (videoOpened)
    {
        videoOpened = false
        $("#SendVideo").addClass("hidden");
    }
    else
    {
        videoOpened = true
        $("#SendVideo").removeClass("hidden");
    }
}

function CloseVideoDialog()
{

    videoOpened = false
    $("#SendVideo").addClass("hidden");
}


function OpenSidebar(sidebar)
{
    $(sidebar).removeClass("hide");
    switch(sidebar)
    {
        case "#UserSidebar":
            userSidebarHidden = false;
            break;
        case "#RoomSidebar":
            roomSiderbarHidden = false;
            break;
    }
}

function CloseSidebar(sidebar) {
    $(sidebar).addClass("hide");
    switch (sidebar) {
        case "#UserSidebar":
            userSidebarHidden = true;
            break;
        case "#RoomSidebar":
            roomSiderbarHidden = true;
            break;
    }
}