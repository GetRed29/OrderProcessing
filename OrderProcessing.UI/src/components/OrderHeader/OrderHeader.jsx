import './OrderHeader.css';

export default function OrderHeader() {
    return (
        <header className="order-workspace-header">
            <div className="header-brand-group">
                📦 <h1 className="brand-title">Order Processing</h1> 
            </div>
            <a href="http://localhost:5048/swagger" target="_blank" rel="noreferrer" className="btn-swagger-link">
                Swagger ↗
            </a>
        </header>
    );
}