export default function OrderAction({ currentStatus, transitioning, onTransition }) {
    const status = currentStatus?.toLowerCase() || 'pending';

    const actions = [
        { id: 'pay', label: 'Pay', targetState: 'Confirmed', allowed: status === 'pending', class: 'action-pay' },
        { id: 'process', label: 'Process', targetState: 'Processing', allowed: status === 'confirmed', class: 'action-process' },
        { id: 'ship', label: 'Ship', targetState: 'Shipped', allowed: status === 'processing', class: 'action-ship' },
        { id: 'deliver', label: 'Deliver', targetState: 'Delivered', allowed: status === 'shipped', class: 'action-deliver' },
        { id: 'cancel', label: 'Cancel', targetState: 'Cancelled', allowed: ['pending', 'confirmed', 'processing'].includes(status), class: 'action-cancel' }
    ];
    return (
        <div className="dispatched-operations-section workflow-section-wrapper">

            <div className="operations-header-row">
                <h3 className="workflow-section-title monospace-heading">ACTIONS</h3>
                <span className="operations-subtitle">TRANZIȚII PERMISE DIN STARE CURENTĂ</span>
            </div>

            <div className="operations-terminal-box">
                {actions.map((action) => {
                    const isClickable = action.allowed && !transitioning;
                    const dynamicStateClass = isClickable ? `state-allowed ${action.class}` : 'state-disabled';

                    return (
                        <button
                            key={action.id}
                            disabled={!isClickable}
                            onClick={() => onTransition(action.id, action.targetState)}
                            className={`btn-terminal-trigger ${dynamicStateClass}`}
                        >
                            {action.label}
                        </button>
                    );
                })}
            </div>
        </div>
    );
}