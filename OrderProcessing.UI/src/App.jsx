import { useState, useEffect, useCallback } from 'react';
import api from './services/api';

import OrderHeader from './components/OrderHeader/OrderHeader';
import OrderList from './components/OrderList/OrderList';
import OrderDetails from './components/OrderDetails/OrderDetails';
import OrderFooter from './components/OrderFooter/OrderFooter';
import CreateOrder from './components/CreateOrder/CreateOrder';

export default function App() {
    const [orders, setOrders] = useState([]);
    const [selectedOrderId, setSelectedOrderId] = useState(null);
    const [loading, setLoading] = useState(true);
    const [toast, setToast] = useState(null);
    const [viewMode, setViewMode] = useState('details');

    const handleSelectOrder = (selectedOrder) => {
        setSelectedOrderId(selectedOrder);
        setViewMode('details');
    }; 

    const handleAddOrderClick = () => {
        setViewMode('create');
    };

    const handleOrderCreatedOrCancelled = () => {
        setViewMode('details');
        setSelectedOrderId(null);
    }

    const showToast = (message, type = 'success') => {
        setToast({ message, type });
    };

    const fetchOrders = useCallback(async () => {
        setTimeout(async () => {
            try {
                setLoading(true);

                const response = await api.get('/orders');

                let incomingData = response.data;
                if (incomingData && typeof incomingData === 'object' && !Array.isArray(incomingData)) {
                    incomingData = incomingData.value || incomingData.data || incomingData.orders || [];
                }

                setOrders(Array.isArray(incomingData) ? [...incomingData] : []);
            } catch (err) {
                if (typeof showToast === 'function') {
                    showToast(err.message || 'Error fetching orders', 'error');
                }
                setOrders([]);
            } finally {
                setLoading(false);
            }
        }, 0);
    }, []);

    useEffect(() => {
        let isMounted = true;

        if (isMounted) {
            fetchOrders();
        }

        return () => {
            isMounted = false;
        };
    }, [fetchOrders]);

    const selectedOrder = Array.isArray(orders) && selectedOrderId !== null
        ? orders.find(o => {
            let orderId = o?.id ?? o?.Id;
            if (orderId && typeof orderId === 'object') {
                orderId = orderId.value ?? orderId.id ?? orderId.Id;
            }

            let clickedId = selectedOrderId;
            if (clickedId && typeof clickedId === 'object') {
                clickedId = clickedId.value ?? clickedId.id ?? clickedId.Id;
            }

            return orderId !== undefined && clickedId !== undefined && orderId.toString().toLowerCase().trim() === clickedId.toString().toLowerCase().trim();
        })
        : null;

    return (
        <div className="app-workspace-card">
            <OrderHeader />

            <div className="main-dashboard-grid">

                <div className="sidebar-wrapper-column">
                    <OrderList
                        orders={orders}
                        selectedOrderId={selectedOrderId}
                        onSelectOrder={handleSelectOrder}
                        onAddOrderClick={() => { showToast('Pentru a adăuga o comandă, completează detaliile', 'success'); handleAddOrderClick() }}
                        loading={loading}
                    />
                </div>

                <div className="details-workspace-column">
                    {viewMode === 'create' ? (
                        <CreateOrder
                            onSuccess={async () => { await fetchOrders(); handleOrderCreatedOrCancelled() }}
                            onCancel={handleOrderCreatedOrCancelled}
                        />
                    ) : (
                        <OrderDetails
                            selectedOrder={selectedOrder}
                            onOrderMutated={async () => { await fetchOrders(); handleOrderCreatedOrCancelled }}
                            showToast={showToast}
                        />
                    )}
                </div>

            </div>
            <OrderFooter
                toast={toast}
                onClearToast={() => setToast(null)}
            />
        </div>
    );
}