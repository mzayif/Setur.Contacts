// Write your JavaScript code.

// Toast notification fonksiyonu
function showToast(type, title, message) {
    // Toast container'ı oluştur (eğer yoksa)
    let toastContainer = document.getElementById('toast-container');
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.id = 'toast-container';
        toastContainer.style.cssText = `
            position: fixed;
            bottom: 20px;
            right: 20px;
            z-index: 9999;
            max-width: 450px;
        `;
        document.body.appendChild(toastContainer);
    }

    // Toast element'ini oluştur
    const toast = document.createElement('div');
    toast.style.cssText = `
        background: white;
        border-radius: 10px;
        box-shadow: 0 6px 16px rgba(0,0,0,0.2);
        margin-bottom: 12px;
        padding: 20px;
        border-left: 5px solid;
        animation: slideIn 0.3s ease-out;
        max-width: 450px;
        word-wrap: break-word;
        font-size: 15px;
    `;

    // Toast tipine göre renk ayarla
    const colors = {
        success: '#28a745',
        error: '#dc3545',
        warning: '#ffc107',
        info: '#17a2b8'
    };
    toast.style.borderLeftColor = colors[type] || colors.info;

    // Toast içeriğini oluştur
    toast.innerHTML = `
        <div style="display: flex; justify-content: space-between; align-items: flex-start;">
            <div style="flex: 1;">
                <div style="font-weight: bold; margin-bottom: 6px; color: #333; font-size: 16px;">${title}</div>
                <div style="color: #666; font-size: 15px; line-height: 1.4;">${message}</div>
            </div>
            <button onclick="this.parentElement.parentElement.remove()" 
                    style="background: none; border: none; color: #999; cursor: pointer; font-size: 20px; margin-left: 12px; padding: 0;">
                ×
            </button>
        </div>
    `;

    // Toast içindeki linklere tıklandığında toast'u kapat
    const links = toast.querySelectorAll('a');
    links.forEach(link => {
        link.addEventListener('click', function() {
            // Toast'u kapat
            if (toast.parentElement) {
                toast.remove();
            }
        });
    });

    // Toast'u container'a ekle
    toastContainer.appendChild(toast);

    // İnfo mesajları haricindekiler 5 saniye sonra otomatik kapanır.
    if (type !== 'info') {
        setTimeout(() => {
            if (toast.parentElement) {
                toast.style.animation = 'slideOut 0.3s ease-in';
                setTimeout(() => {
                    if (toast.parentElement) {
                        toast.remove();
                    }
                }, 300);
            }
        }, 5000);
    }
}

// CSS animasyonları
const style = document.createElement('style');
style.textContent = `
    @keyframes slideIn {
        from {
            transform: translateX(100%) translateY(20px);
            opacity: 0;
        }
        to {
            transform: translateX(0) translateY(0);
            opacity: 1;
        }
    }
    
    @keyframes slideOut {
        from {
            transform: translateX(0) translateY(0);
            opacity: 1;
        }
        to {
            transform: translateX(100%) translateY(20px);
            opacity: 0;
        }
    }
`;
document.head.appendChild(style);
