function search() {
    $('#search-form').on('submit', function (e) {
        e.preventDefault(); // prevent default form submission behavior

        // Send an AJAX request to the server to get the filtered data for the table
        $.ajax({
            url: '/Movies/index',
            type: 'GET',
            data: {
                searchString: $('#movie').val(),
                movieGenre: $('#genre').val()
            },
            success: function (data) {
                // Update the table with the new data
                $('#table').bootstrapTable('load', data);
            },
            error: function (xhr) {
                console.log(xhr);
            }
        });
    });
};