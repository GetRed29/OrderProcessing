import React from 'react';

export default function OrderState({ orderLifecycleStages, currentStatus, activeIndex }) {
    return (
        <div className="workflow-timeline-box">
            <h3 className="workflow-section-title">State diagram</h3>
            <div className="timeline-terminal-container">
                {orderLifecycleStages.map((stage, idx) => {
                    const isCompleted = idx < activeIndex || currentStatus.toLowerCase() === 'delivered';
                    const isActive = idx === activeIndex && currentStatus.toLowerCase() !== 'delivered';

                    let stateClass = 'state-future';
                    if (isCompleted) stateClass = 'state-completed';
                    else if (isActive) stateClass = `state-active active-${stage.toLowerCase().trim()}`;

                    return (
                        <React.Fragment key={stage}>
                            <div className={`timeline-step-node ${stateClass}`}>
                                <div className="node-stage-text">
                                    {stage}
                                </div>
                            </div>
                            {idx < orderLifecycleStages.length - 1 && (
                                <span className="timeline-dotted-connector">➔</span>
                            )}
                        </React.Fragment>
                    );
                })}
            </div>
        </div>
    );
}