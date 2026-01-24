// Add to cart functionality
document.addEventListener('DOMContentLoaded', function() {
    // Add to cart buttons
    const addToCartButtons = document.querySelectorAll('.btn-primary');
    addToCartButtons.forEach(button => {
        if (button.textContent.includes('Thêm Vào Giỏ')) {
            button.addEventListener('click', function(e) {
                e.preventDefault();
                const productCard = this.closest('.product-card') || this.closest('.card');
                const productName = productCard.querySelector('.card-title')?.textContent || 'Sản phẩm';
                
                // Show notification
                showNotification('Đã thêm "' + productName + '" vào giỏ hàng!', 'success');
                
                // Update cart count (if exists)
                updateCartCount();
            });
        }
    });

    // Quantity buttons
    const quantityInputs = document.querySelectorAll('input[type="number"]');
    quantityInputs.forEach(input => {
        const parent = input.closest('.input-group');
        if (parent) {
            const decreaseBtn = parent.querySelector('button:first-of-type');
            const increaseBtn = parent.querySelector('button:last-of-type');
            
            if (decreaseBtn) {
                decreaseBtn.addEventListener('click', function() {
                    if (parseInt(input.value) > 1) {
                        input.value = parseInt(input.value) - 1;
                    }
                });
            }
            
            if (increaseBtn) {
                increaseBtn.addEventListener('click', function() {
                    input.value = parseInt(input.value) + 1;
                });
            }
        }
    });

    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function(e) {
            const href = this.getAttribute('href');
            if (href !== '#' && href.length > 1) {
                e.preventDefault();
                const target = document.querySelector(href);
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            }
        });
    });

    // Form validation
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            if (!form.checkValidity()) {
                e.preventDefault();
                e.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });
});

// Show notification
function showNotification(message, type = 'info') {
    // Create notification element
    const notification = document.createElement('div');
    notification.className = `alert alert-${type === 'success' ? 'success' : 'info'} alert-dismissible fade show position-fixed`;
    notification.style.cssText = 'top: 20px; right: 20px; z-index: 9999; min-width: 300px;';
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;
    
    document.body.appendChild(notification);
    
    // Auto remove after 3 seconds
    setTimeout(() => {
        notification.remove();
    }, 3000);
}

// Update cart count
function updateCartCount() {
    const cartBadge = document.querySelector('.badge.bg-danger');
    if (cartBadge) {
        const currentCount = parseInt(cartBadge.textContent) || 0;
        cartBadge.textContent = currentCount + 1;
    }
}

// Image gallery for product details
function changeImage(src) {
    const mainImage = document.getElementById('mainImage');
    if (mainImage) {
        mainImage.src = src;
    }
}

// Search functionality
const searchForm = document.querySelector('form[role="search"]');
if (searchForm) {
    searchForm.addEventListener('submit', function(e) {
        e.preventDefault();
        const searchInput = this.querySelector('input[type="search"]');
        const searchTerm = searchInput.value.trim();
        if (searchTerm) {
            // Redirect to product page with search term
            window.location.href = `/Product?search=${encodeURIComponent(searchTerm)}`;
        }
    });
}
