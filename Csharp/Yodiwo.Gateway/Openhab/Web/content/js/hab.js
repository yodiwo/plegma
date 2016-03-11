
toggleSwitch = function (e) {
    alert("toggleSwitch");
};
strReplace = function (x, a, b) {
    alert("toggleSwitch")
    var res = x.replace(a, b);
    return res;
};
pjaxnavigateTo = function (url) {

    //alert(url)
    $.pjax({ url: url, container: '.content' });

};

toggleSwitch = function (e) {
    alert('toggleSwitch');
};

startScan = function(purl) {

    //alert(url);
      $.ajax({
        type: "POST",
        url: purl,
        //data: { title: title },
        success: function(data) {
            alert("OK");
        },
        error: function() {
            alert("ERROR");
        }
    });
};
function UpdateGroup(cb) {
    var x = document.getElementById("groupids");
    var z = document.getElementById("groups");
    if (cb.checked) {
        x.value += cb.value + ' ';
        z.value += cb.name + ' ';
    }
    else {
        var y = x.value;
        var w = z.value;
        x.value = y.replace(cb.value + ' ', "");
        z.value = w.replace(cb.name + ' ', "");
    }
};

/* center modal */
function centerModals() {
    $('.modal').each(function (i) {
        var $clone = $(this).clone().css('display', 'block').appendTo('body');
        var top = Math.round(($clone.height() - $clone.find('.modal-content').height()) / 2);
        top = top > 0 ? top : 0;
        $clone.remove();
        $(this).find('.modal-content').css("margin-top", top);
    });
}

$('.modal').on('show.bs.modal', centerModals);
$(window).on('resize', centerModals);