export default function OrderHistoryLog({ historyLogs }) {
    const formatTime = (dateString) => {
        try {
            const date = new Date(dateString);
            return date.toTimeString().split(' ')[0];
        } catch {
            return "00:00:00";
        }
    };


    return (
        <div className="history-log-box">
            <h3 className="workflow-section-title">History</h3>
            <div className="history-logs-container">
                {historyLogs.length === 0 ? (
                    <div className="history-empty-text">No logged transitions found for this sequence.</div>
                ) : (
                    historyLogs.map((log, index) => {
                        const isCancelled = log.toState === 'Cancelled';
                        return (
                            <div key={index} className="history-log-row">
                                <div className="history-log-left">
                                    <span className={`status-indicator-dot ${isCancelled ? 'dot-cancelled' : 'dot-success'}`}></span>
                                    <span className="history-transition-text">{log.fromState} → {log.toState}</span>
                                </div>

                                <span className="history-timestamp">
                                    {formatTime(log.at)}
                                </span>
                            </div>
                        );
                    }))
                }
            </div>
        </div>
    );
}