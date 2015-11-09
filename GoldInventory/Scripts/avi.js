$(function() {

    var ajaxFormSubmit = function() {
        var $form = $(this);
        var options = {
            url: $form.attr("action"),
            type: $form.attr("method"),
            data: $form.serialize()
        }

        $.ajax(options).done(function(data) {
            var target = $form.attr("data-avi-target");
            $('div[id=' + target + ']').replaceWith(data);
            $('div[id=' + target + ']').effect('highlight');
        });

        return false;
    };

    var submitAutoCompleteForm = function(event, ui) {
        var $input = $(this);
        $input.val(ui.item.label);

        var $form = $input.parents("form:first");
        $form.submit();
    };

    var createAutoComplete = function() {
        var $input = $(this);

        var options = {
            source: $input.attr("data-avi-autocomplete"),
            select: submitAutoCompleteForm
        };

        $input.autocomplete(options);
    };

    var getPage = function() {
        var $a = $(this);

        var options = {
            url: $a.attr("href"),
            data: $("form").serialize(),
            type: "get"
        };

        $.ajax(options).done(function(data) {
            var target = $a.parents("div.pager").attr("data-avi-target");
            $(target).replaceWith(data);
        });

        return false;
    };

    $("form[data-avi-ajax='true']").submit(ajaxFormSubmit);
    $("input[data-avi-autocomplete]").each(createAutoComplete);
    $(".body-content").on("click", ".pager a", getPage);

});

function updateFileName(fileControl) {
    var dt = new Date();
    var time = dt.getHours() + ":" + dt.getMinutes() + ":" + dt.getSeconds();
    $("#fileNameSpan").text("Updated at " + time);
    $("#AddPhotosText").text("Update Photo");
}

