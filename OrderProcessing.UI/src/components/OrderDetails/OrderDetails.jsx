import { useState } from 'react';
import api from '../../services/api';
import OrderSummary from './OrderSummary';
import OrderState from './OrderState';
import OrderAction from './OrderAction';
import OrderHistory from './OrderHistory';
import './OrderDetails.css';

export default function OrderDetails({ selectedOrder, onOrderMutated, showToast }) {
    const [transitioning, setTransitioning] = useState(false);

    if (!selectedOrder) {
        return (
            <div className="panel-empty-text">
                Selectează o comandă din listă pentru a vizualiza detaliile.
            </div>
        );
    }

    const getCleanId = (idField) => {
        if (!idField) return 'N/A';
        if (typeof idField === 'object') {
            return (idField.value ?? idField.id ?? idField.Id ?? '').toString();
        }
        return idField.toString();
    };

    const orderData = selectedOrder.value ? selectedOrder.value : selectedOrder;

    const displayId = getCleanId(orderData?.id ?? orderData?.Id);

    const rawStatus = orderData?.status ?? 'Created';
    const currentStatus = typeof rawStatus === 'string' ? rawStatus.trim() : 'Created';

    const customerName = orderData?.customer?.name;
    const customerAge = orderData?.customer?.age;
    const customer = customerName
        ? `${customerName}${customerAge ? ` (${customerAge} ani)` : 'Unknown'}`
        : 'Anonymous';

    const itemsArray = orderData?.items ?? [];
    const itemsCount = itemsArray.reduce((accumulator, item) => {
        const itemQuantity = item?.quantity ?? item?.Quantity ?? 1;
        return accumulator + itemQuantity;
    }, 0);

    const orderAmount = orderData?.totalAmount.amount;
    const orderCurrency = orderData?.totalAmount.currency;
    const orderPrice = itemsCount === 1 ? `${itemsCount} produs· ${orderAmount} ${orderCurrency}` : `${itemsCount} produse · ${orderAmount} ${orderCurrency}`;

    const shippingStreet = orderData?.shippingAddress.street;
    const shippingCity = orderData?.shippingAddress.city;
    const address = `${shippingStreet}, ${shippingCity}`

    const historyLogs = orderData?.history ?? [];
    const orderLifecycleStages = ['Pending', 'Confirmed', 'Processing', 'Shipped', 'Delivered'];
    const activeIndex = orderLifecycleStages.findIndex(s => s.toLowerCase() === currentStatus.toLowerCase());

    const handleTransition = async (endpoint, label) => {
        try {
            setTransitioning(true);
            await api.post(`/orders/${displayId}/${endpoint}`);
            if (showToast) showToast(`Comanda a trecut în starea ${label}!`, 'success');
            if (onOrderMutated) onOrderMutated();
        } catch (err) {
            if (showToast) {
                showToast(err.response?.data?.message || err.message || 'Tranziție de stare nepermisă.', 'error');
            }
        } finally {
            setTransitioning(false);
        }
    };

    return (
        <div className="details-workspace-view">
            <OrderSummary
                displayId={displayId}
                currentStatus={currentStatus}
                customer={customer}
                orderItems={orderPrice}
                address={address}
                onOrderDeleted={onOrderMutated}
            />

            <OrderState
                orderLifecycleStages={orderLifecycleStages}
                currentStatus={currentStatus}
                activeIndex={activeIndex}
            />

            <OrderAction
                currentStatus={currentStatus}
                transitioning={transitioning}
                onTransition={handleTransition}
            />

            <OrderHistory
                historyLogs={historyLogs}
            />
        </div>
    );
}