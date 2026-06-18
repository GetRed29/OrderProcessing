import './OrderList.css';

export default function OrderList({ orders, selectedOrderId, onSelectOrder, onAddOrderClick, loading }) {
    const safeOrders = Array.isArray(orders) ? orders : [];

    const getCleanId = (idField) => {
        if (!idField) return '';
        if (typeof idField === 'object') {
            return (idField.value ?? idField.id ?? idField.Id ?? '').toString();
        }
        return idField.toString();
    };

    return (
        <>
            <button className="btn-create-order" onClick={onAddOrderClick}>
                + Comandă nouă
            </button>

            <h2 className="orders-section-header">
                <span className="orders-title-text">ORDERS</span>
                <span className="orders-counter-dot">· {loading ? '...' : safeOrders.length}</span>
            </h2>

            <div className="orders-list-wrapper">
                {loading ? (
                    <div className="list-info-text">Se încarcă...</div>
                ) : safeOrders.length === 0 ? (
                    <div className="list-info-text">Nicio comandă găsită.</div>
                ) : (
                    safeOrders.map((order) => {
                        const rawId = order?.id ?? order?.Id;

                        const displayId = getCleanId(rawId);
                        const shortId = displayId.length > 8 ? displayId.substring(0, 7) : displayId;

                        const rawStatus = order?.status ?? order?.Status ?? 'Pending';
                        const statusLower = rawStatus.toLowerCase();

                        const isSelected = selectedOrderId !== null && displayId === getCleanId(selectedOrderId);

                        return (
                            <div
                                key={displayId}
                                onClick={() => onSelectOrder(rawId)}
                                className={`order-list-row row-state-${statusLower} ${isSelected ? 'row-is-selected' : ''}`}
                            >
                                <div className="order-row-identity">
                                    <span className="selection-arrow-pointer">▶</span>
                                    <span className="order-hash-id">#{shortId}</span>
                                </div>

                                <span className={`status-pill status-pill-${statusLower}`}>
                                    {rawStatus}
                                </span>
                            </div>
                        );
                    })
                )}
            </div>
        </>
    );
}