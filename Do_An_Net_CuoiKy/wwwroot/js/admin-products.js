
let productModal;

$(document).ready(function () {
    productModal = new bootstrap.Modal(document.getElementById('productModal'));

    // Preview image
    $("#productImage").on("change", function () {
        const file = this.files[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = e => {
            $("#previewProductImage")
                .attr("src", e.target.result)
                .show();
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
    $("#productId").val("");
    $("#productName").val("");
    $("#productDescription").val("");
    $("#productPrice").val("");
    $("#productOldPrice").val("");
    $("#productStock").val("");
    $("#productActive").prop("checked", true);

    $("#productImage").val("");
    $("#previewProductImage").hide().attr("src", "");

    loadCategories(null, () => {
        productModal.show();
    });
}

function openEditProduct(id) {
    $.get("/Admin/Products/Get/" + id, function (p) {
        $("#productId").val(p.id);
        $("#productName").val(p.name);
        $("#productDescription").val(p.description);
        $("#productPrice").val(p.price);
        $("#productOldPrice").val(p.oldPrice);
        $("#productStock").val(p.stock);
        $("#productActive").prop("checked", p.isActive);

        loadCategories(p.categoryId, () => {
            productModal.show();
        });

        if (p.imageUrl) {
            $("#previewProductImage").attr("src", p.imageUrl).show();
        } else {
            $("#previewProductImage").hide();
        }
    });
}

function saveProduct() {

    // VALIDATE CLIENT
    if (!$("#productName").val()) {
        alert("Vui lòng nhập tên sản phẩm");
        return;
    }

    if (!$("#productCategory").val()) {
        alert("Vui lòng chọn danh mục");
        return;
    }

    if (!$("#productPrice").val()) {
        alert("Vui lòng nhập giá");
        return;
    }

    let formData = new FormData();
    const id = $("#productId").val();

    formData.append("Id", id);
    formData.append("Name", $("#productName").val());
    formData.append("Description", $("#productDescription").val());
    formData.append("Price", $("#productPrice").val());
    formData.append("OldPrice", $("#productOldPrice").val());
    formData.append("Stock", $("#productStock").val());
    formData.append("CategoryId", $("#productCategory").val());
    formData.append("IsActive", $("#productActive").is(":checked"));

    const image = $("#productImage")[0].files[0];
    if (image) {
        formData.append("imageFile", image);
    }

    const url = id
        ? "/Admin/Products/EditAjax"
        : "/Admin/Products/CreateAjax";

    $.ajax({
        url: url,
        type: "POST",
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
        error: function (err) {
            console.error(err);
            alert("Lỗi server (500)");
        }
    });
}

function deleteProduct(id) {
    if (!confirm("Xóa sản phẩm này?")) return;

    $.post("/Admin/Products/DeleteAjax/" + id, function (res) {
        if (res.success) {
            $("#row-" + id).remove();
        } else {
            alert("Không thể xóa");
        }
    });
}

function toggleProductActive(id) {
    $.post("/Admin/Products/ToggleActive/" + id);
}
