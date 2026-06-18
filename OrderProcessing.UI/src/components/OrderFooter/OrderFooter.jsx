import { useEffect } from 'react';
import './OrderFooter.css';

export default function OrderFooter({ toast, onClearToast }) {
    useEffect(() => {
        if (toast) {
            const timer = setTimeout(() => {
                onClearToast();
            }, 5000);
            return () => clearTimeout(timer);
        }
    }, [toast, onClearToast]);

    return (
        <footer className="order-workspace-footer">
            <div className="footer-status-zone">
                {toast ? (
                    <div className={`footer-toast-message type-${toast.type}`}>
                        <span className="toast-symbol">{toast.type === 'error' ? '❌' : '✓'}</span>
                        <span className="toast-text">{toast.message}</span>
                    </div>
                ) : (
                    <div className="footer-toast-message type-idle">
                        <span className="toast-symbol">⚙️</span>
                        <span className="toast-text">Sistem pregătit. Nicio acțiune recentă.</span>
                    </div>
                )}
            </div>

            {toast && (
                <button onClick={onClearToast} className="footer-close-btn">
                    dismiss ✕
                </button>
            )}
        </footer>
    );
}