$(document).ready(function () {

    function increaseTally(tallier) {
        var total = parseInt($(tallier).text());
        var increment = parseInt($("input[type='radio'][name='tally-increment']:checked").val());
        var positive = $("input[type='radio'][name='tally-direction']:checked").val();

        if (positive === "1") {
            $(tallier).text(total + increment);
        } else {
            $(tallier).text(total - increment);
        }
    }

    function postTally(tallier, id) {
        var notifier = $("#notifier");
        var total = parseInt(tallier.text());
        var increment = parseInt($("input[type='radio'][name='tally-increment']:checked").val());
        var positive = $("input[type='radio'][name='tally-direction']:checked").val();

        if (positive === "1") {
            total = total + increment;
        } else {
            total = total - increment;
        }

        var data = { 'total': total, 'id': id }

        $.post("/Home/UpdateTotal", data, function (result) {
            if (result.success) {
                increaseTally(tallier);
                notifier.attr("class", "alert alert-success");
                notifier.text('Successfully set ' + JSON.stringify(data));
            } else {
                notifier.attr("class", "alert alert-danger");
                notifier.text('Failed to set ' + JSON.stringify(data));
            }
        }).fail(function (e) {
            notifier.attr("class", "alert alert-danger");
            notifier.text('Failed to set ' + JSON.stringify(data) + ' due to ' + e.statusText);
        });
    }

    $(".tally-button").click(function (e) {
        if ($(e.target).attr('id') !== undefined) {
            var tallier = $($(e.target).children(".tally-total"));
            var id = $(e.target).attr('id');
            postTally(tallier, id);
        }
    });
    $(".tally-total").click(function (e) {
        var tallier = $(e.target);
        var id = $(e.target).parent().attr('id');
        postTally(tallier, id);
    });
})
