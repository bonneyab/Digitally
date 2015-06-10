$(document).ready(function () {

    function getParameterByName(name) {
        name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
        var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
            results = regex.exec(location.search);
        return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
    }

    function postAndNotify(action, currentValue, value, notifier, messageObject, data) {
        $.post(action, data, function (result) {
            if (result.success) {
                currentValue.text(value);
                notifier.attr("class", "alert alert-success");
                notifier.text('Successfully set ' + JSON.stringify(messageObject));
            } else {
                notifier.attr("class", "alert alert-danger");
                notifier.text('Failed to set ' + JSON.stringify(messageObject));
            }
        }).fail(function (e) {
            notifier.attr("class", "alert alert-danger");
            notifier.text('Failed to set ' + JSON.stringify(messageObject) + ' due to ' + e.statusText);
        });
    }

    var currentDate = getParameterByName('date');
    $('.table tbody td .currentValue').click(function (e) {
        $(e.target).parent().find($('.currentValue')).hide();
        $(e.target).parent().find($('.tallier')).show();
    });

    $('.delete-group').click(function (e) {
        var groupId = $(e.target).attr('group-id');
        var date = $(e.target).attr('date');
        var url = "/home/deletegroup?date=" + date + "&groupId=" + groupId;
        window.location.href = url;
    });

    $('.saver').click(function (e) {
        var tallier = $(e.target).parent();
        var currentValue = tallier.parent().find($('.currentValue'));
        tallier.hide();

        var numberEntry = tallier.find($('.numberEntry'));
        currentValue.show();
        $(e.target).parent().hide();

        var td = tallier.parent();
        var messageObject = { 'time': td.attr('time'), 'age': td.attr('age'), 'total': numberEntry.val(), 'attendanceId': td.attr('id') }

        var notifier = $("#notifier");
        var isGroup = td.attr("isGroup");
        var data;
        if (isGroup === 'true') {
            data = { 'id': td.attr('id'), 'groupName': numberEntry.val() }
            postAndNotify("/Home/UpdateGroupName", currentValue, numberEntry.val(), notifier, messageObject, data);
        } else {
            data = { 'id': td.attr('id'), 'total': numberEntry.val() }
            postAndNotify("/Home/UpdateTotal", currentValue, numberEntry.val(), notifier, messageObject, data);
        }
    });

    $('.canceller').click(function (e) {
        var tallier = $(e.target).parent();
        tallier.hide();
        tallier.find($('.numberEntry')).val(tallier.parent().find($('.currentValue')).text());
        tallier.parent().find($('.currentValue')).show();
    });

    $('#datepicker').datepicker({
        onSelect: function (date) {
            var url = "/home/index?date=" + date;
            window.location.href = url;
        },
        defaultDate: currentDate
    });
})
