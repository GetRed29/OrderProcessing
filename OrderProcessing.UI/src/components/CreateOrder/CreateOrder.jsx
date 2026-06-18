import { useState } from 'react';
import './CreateOrder.css';
import api from '../../services/api';

// Mock product list
const AVAILABLE_PRODUCTS = [
    { id: "p1", name: "Ceragon FibeAir Transceiver RF-Module", defaultPrice: 450.00, ageRestrict: false },
    { id: "p2", name: "XPAND-IP High-Capacity Antenna Link", defaultPrice: 1250.00, ageRestrict: false },
    { id: "p3", name: "Industrial Voltage Regulator 24V", defaultPrice: 89.99, ageRestrict: false },
    { id: "p4", name: "Restricted Dev-Kit Bypass Firmware License", defaultPrice: 299.99, ageRestrict: true }
];

export default function CreateOrder({ onSuccess, onCancel }) {
    const [customer, setCustomer] = useState({ name: '', email: '', age: 18, isTrusted: false });
    const [shippingAddress, setShippingAddress] = useState({ street: '', city: '', postalCode: '', country: '' });
    const [items, setItems] = useState([
        { productId: '', productName: '', quantity: 0, unitPrice: 0, currency: 'RON', hasAgeRestrict: false }
    ]);

    const handleProductSelectChange = (index, targetProductId) => {
        const selectedProd = AVAILABLE_PRODUCTS.find(p => p.id === targetProductId);
        const updatedItems = [...items];

        if (selectedProd) {
            updatedItems[index] = {
                ...updatedItems[index],
                productId: selectedProd.id,
                productName: selectedProd.name,
                unitPrice: selectedProd.defaultPrice,
                hasAgeRestrict: selectedProd.ageRestrict
            };
        } else {
            updatedItems[index] = { ...updatedItems[index], productId: '', productName: '', unitPrice: 0, hasAgeRestrict: false };
        }
        setItems(updatedItems);
    };

    const handleItemValueChange = (index, field, value) => {
        const updatedItems = [...items];
        updatedItems[index][field] = value;
        setItems(updatedItems);
    };

    const addItemRow = () => {
        setItems([...items, { productId: '', productName: '', quantity: 0, unitPrice: 0, currency: 'RON', hasAgeRestrict: false }]);
    };

    const removeItemRow = (index) => {
        if (items.length > 1) {
            setItems(items.filter((_, i) => i !== index));
        }
    };

    const calculatedTotal = items.reduce((sum, item) => sum + (Number(item.unitPrice || 0) * Number(item.quantity || 1)), 0);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (items.some(item => !item.productId)) {
            alert("Te rog selectează un produs valid pentru fiecare rând din listă.");
            return;
        }

        const payload = {
            customer: {
                id: null,
                name: customer.name,
                email: customer.email,
                age: parseInt(customer.age, 10),
                isTrusted: customer.isTrusted
            },
            shippingAddress,
            items: items.map(item => ({
                productId: null,
                productName: item.productName,
                quantity: parseInt(item.quantity, 10),
                unitPrice: parseFloat(item.unitPrice),
                currency: item.currency,
                hasAgeRestrict: item.hasAgeRestrict
            })),
            totalAmount: {
                amount: calculatedTotal,
                currency: items[0]?.currency || "RON"
            }
        };

        try {
            const response = await api.post('/orders', payload);

            console.log(" Order added successfully via API instance:", response.data);

            if (onSuccess) onSuccess();

        } catch (error) {
            console.error("Network communication failure trying to post:", error);

            if (error.response) {
                console.error("Status Code Received:", error.response.status);
                console.error("Server Validation Errors Data:", error.response.data);
            } else if (error.request) {
                console.error("No response received from server. Check if backend port 7297 is running.");
            } else {
                console.error("Runtime configuration error:", error.message);
            }
        }
    };

    return (
        <div className="create-order-scroll-container">
            <form onSubmit={handleSubmit} className="terminal-form-context">

                <div className="form-header-wrapper">
                    <h2 className="monospace-heading form-main-title">
                        ADD ORDER
                    </h2>
                </div>

                <div className="workflow-section-wrapper workflow-section-spacing">
                    <h3 className="workflow-section-title monospace-heading">CUSTOMER DATA</h3>
                    <div className="form-grid-half-half">
                        <div className="form-field-group">
                            <label className="form-label-text">Name</label>
                            <input type="text" className="terminal-input" required value={customer.name} onChange={(e) => setCustomer({ ...customer, name: e.target.value })} />
                        </div>
                        <div className="form-field-group">
                            <label className="form-label-text">Email</label>
                            <input type="email" className="terminal-input" required value={customer.email} onChange={(e) => setCustomer({ ...customer, email: e.target.value })} />
                        </div>
                        <div className="form-field-group">
                            <label className="form-label-text">Age</label>
                            <input type="number" className="terminal-input" min="1" required value={customer.age} onChange={(e) => setCustomer({ ...customer, age: e.target.value })} />
                        </div>
                        <div className="form-checkbox-alignment-row">
                            <label className="terminal-checkbox-wrapper">
                                <input type="checkbox" id="isTrusted" className="terminal-checkbox" checked={customer.isTrusted} onChange={(e) => setCustomer({ ...customer, isTrusted: e.target.checked })} />
                                <span className="form-label-text inline-label">Is Trusted Account</span>
                            </label>
                        </div>
                    </div>
                </div>

                <div className="workflow-section-wrapper workflow-section-spacing">
                    <h3 className="workflow-section-title monospace-heading">SHIPPING ADDRESS</h3>
                    <div className="form-grid-address">
                        <div className="form-grid-span-full form-field-group">
                            <label className="form-label-text">Street Address</label>
                            <input type="text" className="terminal-input" required value={shippingAddress.street} onChange={(e) => setShippingAddress({ ...shippingAddress, street: e.target.value })} />
                        </div>
                        <div className="form-field-group">
                            <label className="form-label-text">City</label>
                            <input type="text" className="terminal-input" required value={shippingAddress.city} onChange={(e) => setShippingAddress({ ...shippingAddress, city: e.target.value })} />
                        </div>
                        <div className="form-field-group">
                            <label className="form-label-text">Postal Code</label>
                            <input type="text" className="terminal-input" required value={shippingAddress.postalCode} onChange={(e) => setShippingAddress({ ...shippingAddress, postalCode: e.target.value })} />
                        </div>
                        <div className="form-grid-span-full form-field-group">
                            <label className="form-label-text">Country</label>
                            <input type="text" className="terminal-input" required value={shippingAddress.country} onChange={(e) => setShippingAddress({ ...shippingAddress, country: e.target.value })} />
                        </div>
                    </div>
                </div>

                <div className="workflow-section-wrapper workflow-section-spacing">
                    <h3 className="workflow-section-title monospace-heading">ORDERED ITEMS LIST</h3>
                    {items.map((item, index) => (
                        <div key={index} className="item-row-grid item-row-layout">
                            <div className="item-flex-grow-product form-field-group">
                                <label className="form-label-text">Product</label>
                                <select className="terminal-select" value={item.productId} onChange={(e) => handleProductSelectChange(index, e.target.value)} required>
                                    <option value="">-- Select Product --</option>
                                    {AVAILABLE_PRODUCTS.map(p => (
                                        <option key={p.id} value={p.id}>{p.name}</option>
                                    ))}
                                </select>
                            </div>
                            <div className="item-width-qty form-field-group">
                                <label className="form-label-text">Qty</label>
                                <input type="number" className="terminal-input text-center" min="0" value={item.quantity} onChange={(e) => handleItemValueChange(index, 'quantity', e.target.value)} required />
                            </div>
                            <div className="item-width-price form-field-group">
                                <label className="form-label-text">Price (RON)</label>
                                <input type="number" disabled step="0.01" className="terminal-input text-right" value={item.unitPrice} onChange={(e) => handleItemValueChange(index, 'unitPrice', e.target.value)} required />
                            </div>
                            <div className="item-column-restricted">
                                <span className="form-label-text sub-label-size">Restricted</span>
                                <input type="checkbox" checked={item.hasAgeRestrict} disabled className="terminal-checkbox item-checkbox-opacity" />
                            </div>
                            {items.length > 1 && (
                                <button type="button" className="item-btn-delete" onClick={() => removeItemRow(index)}>
                                    ✕
                                </button>
                            )}
                        </div>
                    ))}
                    <button type="button" className="btn-add-item-dashed" onClick={addItemRow}>
                        + Add Another Item
                    </button>
                </div>

                <div className="form-summary-action-panel">
                    <div>
                        <span className="total-label-mono">TOTAL AMOUNT:</span>
                        <div className="total-value-display">
                            {calculatedTotal.toFixed(2)} RON
                        </div>
                    </div>
                    <div className="form-actions-button-group">
                        <button type="button" className="btn-action-cancel" onClick={onCancel}>
                            Cancel
                        </button>
                        <button type="submit" className="btn-action-submit">
                            Add Order
                        </button>
                    </div>
                </div>

            </form>
        </div>
    );
}