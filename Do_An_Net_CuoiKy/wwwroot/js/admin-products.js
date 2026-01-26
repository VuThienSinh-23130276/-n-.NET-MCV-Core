let productModal;

function getToken() {
    return $('input[name="__RequestVerificationToken"]').val();
}

$(document).ready(function () {
    productModal = new bootstrap.Modal(document.getElementById('productModal'));

    $(document).on("change", "#productImage", function () {
        const file = this.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = e => {
            $("#previewProductImage").attr("src", e.target.result).show();
        };
        reader.readAsDataURL(file);
    });
});

function loadCategories(selectedId = null, callback = null) {
    $.get("/Admin/Products/GetCategories", function (data) {
        let html = `<option value="">-- Chọn danh mục --</option>`;
        data.forEach(c => {
            html += `<option value="${c.id}" ${selectedId == c.id ? "selected" : ""}>${c.name}</option>`;
        });
        $("#productCategory").html(html);
        if (callback) callback();
    });
}

function openCreateProduct() {
    $("#productModalTitle").text("Thêm sản phẩm");

    $("#productForm")[0].reset();
    $("#previewProductImage").hide();

    $("#productId").removeAttr("name").val("");

    loadCategories(null, () => productModal.show());
}

function openEditProduct(id) {
    $.get("/Admin/Products/Get/" + id, function (p) {

        $("#productModalTitle").text("Chỉnh sửa sản phẩm");

        $("#productId").attr("name", "Id").val(p.id);

        $("#productName").val(p.name);
        $("#productDescription").val(p.description);
        $("#productPrice").val(p.price);
        $("#productOldPrice").val(p.oldPrice);
        $("#productStock").val(p.stock);
        $("#productActive").prop("checked", p.isActive);

        if (p.imageUrl) {
            $("#previewProductImage").attr("src", p.imageUrl).show();
        } else {
            $("#previewProductImage").hide();
        }

        loadCategories(p.categoryId, () => productModal.show());
    });
}

$(document).on("submit", "#productForm", function (e) {
    e.preventDefault();

    let formData = new FormData(this);
    let hasId = $("#productId").attr("name") === "Id";
    let url = hasId ? "/Admin/Products/EditAjax" : "/Admin/Products/CreateAjax";

    $.ajax({
        url: url,
        type: "POST",
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
                alert(res.message || "Lỗi khi lưu sản phẩm");
            }
        },
        error: function () {
            alert("Lỗi server");
        }
    });
});

function deleteProduct(id) {
    if (!confirm("Xóa sản phẩm này?")) return;

    $.ajax({
        url: "/Admin/Products/DeleteAjax",
        type: "POST",
        headers: {
            'RequestVerificationToken': getToken()
        },
        data: { id: id },
        success: function (res) {
            if (res.success) {
                $("#row-" + id).fadeOut(300, function () {
                    $(this).remove();
                    updateSTT();
                });
            } else {
                alert(res.message || "Không thể xóa");
            }
        }
    });
}

function toggleProductActive(id, checkbox) {
    let oldValue = !checkbox.checked;

    $.ajax({
        url: "/Admin/Products/ToggleActive",
        type: "POST",
        headers: {
            'RequestVerificationToken': getToken()
        },
        data: { id: id },
        success: function (res) {
            if (res.success) {

                let countEl = $("#activeCount");
                let current = parseInt(countEl.text());

                if (checkbox.checked) {
                    countEl.text(current + 1);
                } else {
                    countEl.text(current - 1);
                }

            } else {
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

function updateSTT() {
    $("#productTable tbody tr").each(function (index) {
        $(this).find(".stt").text(index + 1);
    });
}
