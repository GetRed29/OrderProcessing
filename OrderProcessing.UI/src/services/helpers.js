export const getStatusColors = (status) => {
    const normalize = status?.toLowerCase() || '';
    switch (normalize) {
        case 'pending':
            return { text: 'text-blue-400', bg: 'bg-blue-500/10', border: 'border-blue-500/30', badge: 'bg-blue-600' };
        case 'processing':
            return { text: 'text-orange-400', bg: 'bg-orange-500/10', border: 'border-orange-500/30', badge: 'bg-orange-600' };
        case 'delivered':
            return { text: 'text-green-400', bg: 'bg-green-500/10', border: 'border-green-500/30', badge: 'bg-green-600' };
        case 'cancelled':
            return { text: 'text-red-400', bg: 'bg-red-500/10', border: 'border-red-500/30', badge: 'bg-red-600' };
        default:
            return { text: 'text-gray-400', bg: 'bg-gray-500/10', border: 'border-gray-500/30', badge: 'bg-gray-600' };
    }
};

export const formatCurrency = (amount, currency = 'RON') => {
    return `${Number(amount).toLocaleString('ro - RO')} ${currency}`;
};

export const formatTime = (dateString) => {
    if (!dateString) return '';
    const date = new Date(dateString);
    return date.tolocaleString('ro-RO', { hour: '2-digit', minute: '2-digit', second: '2-digit' });
}