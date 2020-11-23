
$(function () {
    var placeholderElement = $('#modal-placeholder');
    $(document).on('click', 'button[data-toggle="ajax-modal"]', function (event) {
        var url = $(this).data('url');
        $.get(url).done(function (data) {
            placeholderElement.html(data);
            placeholderElement.find('.modal').modal('show');
        });
    });

    placeholderElement.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();

        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var dataToSend = form.serialize();

        $.post(actionUrl, dataToSend).done(function (data) {
            var newBody = $('.modal-body', data);
            placeholderElement.find('.modal-body').replaceWith(newBody);

            // find IsValid input field 
            var isValid = newBody.find('[name="IsValid"]').val() == 'True';
            // check it's value equals true
            if (isValid) {
                var notificationsPlaceholder = $('#notification');
                var notificationsUrl = notificationsPlaceholder.data('url');

                $.get(notificationsUrl).done(function (notifications) {
                    notificationsPlaceholder.html(notifications);
                });

                var tableContacts = $('#contacts');
                var tableUrl = tableContacts.data('url');
                $.get(tableUrl).done(function (table) {
                    tableContacts.replaceWith(table);
                });

                // if it's valid then hide modal window
                placeholderElement.find('.modal').modal('hide');
            }
        });
    });
});
