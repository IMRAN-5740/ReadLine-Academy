var dataTable;
$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    dataTable = $('#myTable').DataTable({
        "ajax": {
            "url": "/Admin/Product/GetAll"
        },
        "columns": [
            { "data": "title", "width": "15%" },
            { "data": "isbn", "width": "15%" },
            { "data": "authorName", "width": "15%" },
            { "data": "price", "width": "15%" },
            { "data": "category.name", "width": "15%" },
            { "data": "coverType.name", "width": "15%" },
            {
                "data": "imageURL", "render": function (data) {
                    return `<img src="${data}" width="100%" height="auto">`;
                }, "width": "12%"
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="btn-group ">
                        
                            <a  href="/Admin/Product/Upsert?id=${data}" class="btn btn-primary" > <i class="far fa-edit"></i></a>

                            <a  href="/Admin/Product/Details?id=${data}"   class="btn btn-success"><i class="fa-solid fa-list"></i></a>

                            <a onClick=Delete('/Admin/Product/Delete/+${data}') class="btn btn-danger"><i class="fas fa-trash-alt"></i></a>
                         </div>
                    `
                },
                "width": "15%"
            },
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        dataTable.ajax.reload();
                        toastr.error(data.message);
                    }
                    else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}