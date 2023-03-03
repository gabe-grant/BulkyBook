
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
            "url":"/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "author", "width": "15%" },
            { "data": "category.name", "width": "15%" },
        ]
    });
}