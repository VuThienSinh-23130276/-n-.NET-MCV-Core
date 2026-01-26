// =======================
// LẤY CSRF TOKEN
// =======================
function getToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

// =======================
// MỞ MODAL TẠO MỚI
// =======================
function openCreateModal() {
    $('#modalTitle').text("Thêm danh mục");
    $('#categoryForm')[0].reset();
    $('#previewImage').hide();

    // Không gửi Id khi tạo mới
    $('#categoryId').removeAttr('name').val('');

    const modal = new bootstrap.Modal(document.getElementById('categoryModal'));
    modal.show();
}

// =======================
// MỞ MODAL CHỈNH SỬA
// =======================
function openEdit(id) {
    $.get('/Admin/Categories/Get/' + id, function (data) {
        $('#modalTitle').text("Chỉnh sửa danh mục");

        // Gửi Id khi edit
        $('#categoryId').attr('name', 'Id').val(data.id);

        $('#categoryName').val(data.name);
        $('#categoryDescription').val(data.description);
        $('#categoryActive').prop('checked', data.isActive);

        if (data.imageUrl) {
            $('#previewImage').attr('src', data.imageUrl).show();
        } else {
            $('#previewImage').hide();
        }

        $('#categoryImage').val('');
        const modal = new bootstrap.Modal(document.getElementById('categoryModal'));
        modal.show();
    });
}

// =======================
// SUBMIT FORM CREATE / EDIT
// =======================
$(document).on('submit', '#categoryForm', function (e) {
    e.preventDefault();

    let formData = new FormData(this);
    let hasId = $('#categoryId').attr('name') === 'Id';
    let url = hasId ? '/Admin/Categories/EditAjax' : '/Admin/Categories/CreateAjax';

    $.ajax({
        url: url,
        type: 'POST',
        headers: {
            'RequestVerificationToken': getToken()
        },
        data: formData,
        processData: false,
        contentType: false,
        success: function (res) {
            if (res.success) {
                location.reload();
            } else {
                alert(res.message || "Có lỗi xảy ra");
            }
        },
        error: function () {
            alert("Lỗi server");
        }
    });
});

// =======================
// PREVIEW ẢNH
// =======================
$(document).on('change', '#categoryImage', function () {
    const file = this.files[0];
    if (!file) return;

    const reader = new FileReader();
    reader.onload = function (e) {
        $('#previewImage').attr('src', e.target.result).show();
    };
    reader.readAsDataURL(file);
});

// =======================
// XÓA DANH MỤC
// =======================
function deleteCategory(id) {
    if (!confirm("Bạn có chắc muốn xóa danh mục này?")) return;

    $.ajax({
        url: '/Admin/Categories/DeleteAjax',
        type: 'POST',
        headers: {
            'RequestVerificationToken': getToken()
        },
        data: { id: id },
        success: function (res) {
            if (res.success) {
                $('#row-' + id).fadeOut(300, function () {
                    $(this).remove();
                    updateCategorySTT(); // cập nhật lại STT
                });
            } else {
                alert(res.message || "Không thể xóa");
            }
        },
        error: function () {
            alert("Lỗi server");
        }
    });
}

// =======================
// BẬT/TẮT TRẠNG THÁI
// =======================
function toggleActive(id, checkbox) {
    let oldValue = !checkbox.checked;

    $.ajax({
        url: '/Admin/Categories/ToggleActive',
        type: 'POST',
        headers: {
            'RequestVerificationToken': getToken()
        },
        data: { id: id },
        success: function (res) {
            if (!res.success) {
                checkbox.checked = oldValue;
                alert("Không thể cập nhật trạng thái");
            }
        },
        error: function () {
            checkbox.checked = oldValue;
            alert("Lỗi server");
        }
    });
}

// =======================
// CẬP NHẬT LẠI STT
// =======================
function updateCategorySTT() {
    $("#categoryTable tbody tr").each(function (index) {
        $(this).find(".stt").text(index + 1);
    });
}
