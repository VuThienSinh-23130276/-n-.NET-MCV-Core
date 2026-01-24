function openCreateModal() {
    $('#categoryId').val('');
    $('#categoryName').val('');
    $('#categoryDescription').val('');
    $('#categoryActive').prop('checked', true);
    $('#previewImage').hide();
    $('#categoryImage').val('');

    const modal = new bootstrap.Modal(
        document.getElementById('categoryModal')
    );
    modal.show();
}

function saveCategory() {

    const formData = new FormData();
    formData.append('Id', $('#categoryId').val());
    formData.append('Name', $('#categoryName').val());
    formData.append('Description', $('#categoryDescription').val());
    formData.append('IsActive', $('#categoryActive').is(':checked'));

    const image = $('#categoryImage')[0].files[0];
    if (image) {
        formData.append('imageFile', image);
    }

    $.ajax({
        url: $('#categoryId').val()
            ? '/Admin/Categories/EditAjax'
            : '/Admin/Categories/CreateAjax',
        type: 'POST',
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.success) {
                location.reload();
            } else {
                alert(res.message || 'Lỗi');
            }
        },
        error: function () {
            alert('Lỗi server');
        }
    });
}

// Preview ảnh khi chọn file
$('#categoryImage').on('change', function () {
    const file = this.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function (e) {
        $('#previewImage')
            .attr('src', e.target.result)
            .show();
    };
    reader.readAsDataURL(file);
});

function openEdit(id) {
    $.get('/Admin/Categories/Get/' + id, function (data) {
        $('#categoryId').val(data.id);
        $('#categoryName').val(data.name);
        $('#categoryDescription').val(data.description);
        $('#categoryActive').prop('checked', data.isActive);

        if (data.imageUrl) {
            $('#previewImage')
                .attr('src', data.imageUrl)
                .show();
        } else {
            $('#previewImage').hide();
        }

        $('#categoryImage').val('');

        const modal = new bootstrap.Modal(
            document.getElementById('categoryModal')
        );
        modal.show();
    });
}

function deleteCategory(id) {
    if (!confirm('Xóa danh mục?')) return;

    $.post('/Admin/Categories/DeleteAjax/' + id, function (res) {
        if (res.success) $('#row-' + id).remove();
    });
}
function toggleActive(id, checkbox) {
    $.post('/Admin/Categories/ToggleActive', { id }, function (res) {
        if (!res.success) {
            alert('Lỗi');
            checkbox.checked = !checkbox.checked;
        }
    });
}
