import { useState } from 'react';
import api from '../../services/api';

export default function OrderSummary({ displayId, currentStatus, customer, orderItems, address, onOrderDeleted }) {
    const [isDeleting, setIsDeleting] = useState(false);

    const handleDelete = async () => {
        if (!window.confirm("Sigur doriți să ștergeți această comandă?")) return;

        try {
            setIsDeleting(true);

            //const orderId = rawId?.value ?? rawId;

            if (!displayId) {
                console.error("Nu s-a găsit un ID valid pentru ștergere.");
                return;
            }

            await api.delete(`/orders/${displayId}`);
            console.log("🗑️ Comandă ștearsă cu succes.");

            await onOrderDeleted();
        } catch (error) {
            console.error("❌ Ștergerea a eșuat:", error.response?.data || error.message);
        } finally {
            setIsDeleting(false);
        }
    };

    return (
        <div className="order-metadata-summary-bar">
            <div className="order-summary-header">
                <h3 className="workflow-section-title" style={{ paddingLeft: 0, marginBottom: '16px' }}>
                Order Details
                </h3>

                <button
                    className="order-summary-delete-button"
                    onClick={handleDelete}
                    disabled={isDeleting}
                    style={{cursor: isDeleting ? 'not-allowed' : 'pointer'}}
                >
                    {isDeleting ? "Se șterge..." : "Șterge comandă"}
                </button>
            </div>

            <div className="meta-summary-row">
                <span className="meta-label">ID</span>
                <span className="meta-value-mono">#{displayId}</span>
            </div>

            <div className="meta-summary-row">
                <span className="meta-label">STATUS</span>
                <span className="meta-value-text">{currentStatus}</span>
            </div>

            <div className="meta-summary-row">
                <span className="meta-label">CUSTOMER</span>
                <span className="meta-value-text">{customer}</span>
            </div>

            <div className="meta-summary-row">
                <span className="meta-label">ITEMS</span>
                <span className="meta-value-text">{orderItems}</span>
            </div>

            <div className="meta-summary-row">
                <span className="meta-label">ADDRESS</span>
                <span className="meta-value-text">{address}</span>
            </div>
        </div>
    );
}