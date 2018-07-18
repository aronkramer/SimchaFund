$(() => {
    $('#new-contributor').on('click', function () {
        $('.edit-title').html('New Contributor');
        $('.new-contrib').modal();
    });

    $('.Edit').on('click', function () {
        const id = $(this).data('id');
        const first = $(this).data('first');
        const last = $(this).data('last');
        const cell = $(this).data('cell');
        const alwaysInclude = $(this).data('alwaysIncluded');
        const date = $(this).data('date');
        $('.edit-title').html(`EDIT: ${first} ${last}`);
        const form = $('.change-url');

        $('#firstname').val(first);
        $('#lastname').val(last);
        $('#phone').val(cell);
        $('#AlwaysInclude').prop('checked', alwaysInclude === "True");
        $('#dateCreated').val(date);

        $('#get-person-id').val(id);
        $('#initialDepositDiv').hide();
        form.attr('action', '/home/updateperson');
        $('.new-contrib').modal();
    });

    $('.clear-inputs').on('click', function () {
        $('#firstname').val('');
        $('#lastname').val('');
        $('#phone').val('');
        $('#dateCreated').val('');
        $('#initialDepositDiv').show();
    });

    $('.clear').on('click', function () {
        $('.search').val('');
        $('tr').show();
    });

    $('.deposit-more').on('click', function () {
        const id = $(this).data('person-id');
        const name = $(this).data('person-name');
        $('#cash-add').html(`Add Cash for ${name}`);
        $('#ContributorsId').val(id);
        $('.deposit-more-money').modal();
    });

    $('.search').on('keyup', function () {
        const text = $(this).val();
        $("table tr:gt(0)").each(function () {
            const tr = $(this);
            const name = tr.find('td:eq(1)').text();
            if (name.toLowerCase().indexOf(text.toLowerCase()) !== -1) {
                tr.show();
            } else {
                tr.hide();
            }
        });
    });
});