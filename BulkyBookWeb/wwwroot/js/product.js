
var dataTable

// $ (document).ready is jQuery event that is fired when DOM is loaded, so it’s fired when the document structure is ready.
$(document).ready(function () {
    loadDataTable();
});

// #tblData is the id of the table in the index view page
// configuration includes an ajax request to load all the data with the API to the Product controller
// once the data is received from the ajax call, we then have to parse all the columns
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "category.name", "width": "15%" },
            {
                "data": "id",
                // data is nothing but the product id the user selects and we pass it with string interpolation
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/Admin/Product/Upsert?id=${data}" title="Edit" 
                            class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> </a>
                            <a onClick=Delete('/Admin/Product/Delete/${data}') title="Delete"
                            class="btn btn-danger mx-2"> <i class="bi bi-x-lg"></i> </a>
                        </div>
                    `
                }, 
                "width": "15%"
            },
        ]
    });
}

// delete the url endpoint that is invoked inside the product controller
function Delete(url) {
    swal({
        title: "Are you sure?",
        text: "Once deleted, you will not be able to recover this image!",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((result) => {
            if (result) {
                $.ajax({
                    url: url,
                    type: 'DELETE',
                    success: function (data) {
                        if (data.success) {
                            dataTable.ajax.reload();
                            toastr.success(data.message);
                        }
                        else {
                            toastr.error(data.message);
                        }
                    }
                })
            }
        });
}