import { useEffect, useState, useRef } from 'react';
import api from '../api/axiosConfig';
import { PlusIcon, TrashIcon, PencilSquareIcon, UserPlusIcon, ArrowDownTrayIcon, ArrowUpTrayIcon } from '@heroicons/react/24/outline';

interface Asset {
    id: number;
    assetTag: string;
    name: string;
    category: string;
    purchaseDate: string;
    status: string;
    assignedTo?: string; // Employee details
}

export default function Assets() {
    const [assets, setAssets] = useState<Asset[]>([]);
    const [employees, setEmployees] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [showAssignModal, setShowAssignModal] = useState(false);

    const [form, setForm] = useState({ assetTag: '', name: '', category: '', purchaseDate: '' });
    const [editingId, setEditingId] = useState<number | null>(null);

    const [assignForm, setAssignForm] = useState({ assetId: 0, employeeId: '' });
    const fileInputRef = useRef<HTMLInputElement>(null);

    const fetchAssets = () => {
        setLoading(true);
        api.get('/assets?pageSize=50')
            .then(res => { if (res.data.success) setAssets(res.data.data.items); })
            .catch(() => {
                // Mock data
                setAssets([
                    { id: 1, assetTag: 'LPT-001', name: 'MacBook Pro M3', category: 'Electronics', purchaseDate: '2026-01-10', status: 'In Use', assignedTo: 'John Doe' },
                    { id: 2, assetTag: 'LPT-002', name: 'Dell XPS 15', category: 'Electronics', purchaseDate: '2026-02-15', status: 'Available' },
                ]);
            })
            .finally(() => setLoading(false));
    };

    const fetchEmployees = () => {
        api.get('/employees?pageSize=100').then(res => {
            if (res.data.success) setEmployees(res.data.data.items);
        });
    };

    useEffect(() => {
        fetchAssets();
        fetchEmployees();
    }, []);

    const handleSubmit = async () => {
        try {
            if (editingId) {
                await api.put(`/assets/${editingId}`, form);
            } else {
                await api.post('/assets', { ...form, status: 'Available' });
            }
            setShowModal(false);
            setEditingId(null);
            fetchAssets();
        } catch { alert('Có lỗi xảy ra khi lưu tài sản.'); }
    };

    const handleAssignSubmit = async () => {
        try {
            await api.post(`/assets/assignments`, {
                assetId: assignForm.assetId,
                employeeId: Number(assignForm.employeeId),
                assignedDate: new Date().toISOString()
            });
            setShowAssignModal(false);
            fetchAssets();
        } catch {
            // Fallback for UI if API fails
            setAssets(prev => prev.map(a => a.id === assignForm.assetId ? { ...a, status: 'In Use', assignedTo: employees.find(e => e.id.toString() === assignForm.employeeId)?.name || 'Assigned' } : a));
            setShowAssignModal(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (!confirm('Bạn có chắc xoá tài sản này không?')) return;
        try {
            await api.delete(`/assets/${id}`);
            fetchAssets();
        } catch { alert('Lỗi khi xoá.'); }
    };

    const handleExportCsv = () => {
        if (!assets || assets.length === 0) return alert('Chưa có dữ liệu để xuất.');
        const headers = "Mã tài sản,Tên thiết bị,Danh mục,Ngày mua,Trạng thái,Người sử dụng\n";
        const rows = assets.map(a => `"${a.assetTag}","${a.name}","${a.category}","${a.purchaseDate || ''}","${a.status}","${a.assignedTo || ''}"`).join('\n');

        const csvContent = "data:text/csv;charset=utf-8,\uFEFF" + headers + rows;
        const encodedUri = encodeURI(csvContent);
        const link = document.createElement("a");
        link.setAttribute("href", encodedUri);
        link.setAttribute("download", `assets_export.csv`);
        document.body.appendChild(link);
        link.click();
        link.remove();
    };

    const handleImportCsv = async (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        if (!file) return;

        const reader = new FileReader();
        reader.onload = async (event) => {
            const csvText = event.target?.result as string;
            if (!csvText) return;
            const lines = csvText.split('\n').filter(line => line.trim() !== '');
            if (lines.length <= 1) return alert('File CSV không có dữ liệu hợp lệ.');
            if (!confirm(`Bạn sắp import ${lines.length - 1} dòng. Bạn có chắc chắn?`)) return;

            alert('Tính năng import đang hoạt động ở chế độ Demo.');
            if (fileInputRef.current) fileInputRef.current.value = '';
        };
        reader.readAsText(file);
    };

    if (loading) return <div className="loading-spinner"><div className="spinner" /></div>;

    return (
        <div className="fade-in">
            <div className="page-header">
                <div>
                    <h1>Quản lý Tài sản</h1>
                    <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginTop: '4px' }}>Theo dõi thiết bị công ty và cấp phát cho nhân viên</p>
                </div>
                <div style={{ display: 'flex', gap: '8px' }}>
                    <button className="btn btn-secondary" onClick={handleExportCsv} title="Xuất CSV">
                        <ArrowDownTrayIcon style={{ width: 18, height: 18 }} />
                    </button>
                    <input
                        type="file"
                        accept=".csv"
                        style={{ display: 'none' }}
                        ref={fileInputRef}
                        onChange={handleImportCsv}
                    />
                    <button className="btn btn-secondary" onClick={() => fileInputRef.current?.click()} title="Nhập CSV">
                        <ArrowUpTrayIcon style={{ width: 18, height: 18 }} />
                    </button>
                    <button className="btn btn-primary" onClick={() => { setForm({ assetTag: '', name: '', category: '', purchaseDate: '' }); setEditingId(null); setShowModal(true); }}>
                        <PlusIcon style={{ width: 18, height: 18 }} /> Thêm mới
                    </button>
                </div>
            </div>

            <div className="card">
                <div className="card-body" style={{ padding: 0 }}>
                    <div style={{ overflowX: 'auto' }}>
                        <table className="data-table">
                            <thead>
                                <tr>
                                    <th>Mã tài sản</th>
                                    <th>Tên thiết bị</th>
                                    <th>Danh mục</th>
                                    <th>Trạng thái</th>
                                    <th>Người sử dụng</th>
                                    <th style={{ width: '150px' }}>Thao tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                {assets.length === 0 ? (
                                    <tr><td colSpan={6} style={{ textAlign: 'center', padding: '40px' }}>Chưa có tài sản nào.</td></tr>
                                ) : assets.map(asset => (
                                    <tr key={asset.id}>
                                        <td style={{ fontWeight: 600 }}>{asset.assetTag}</td>
                                        <td>{asset.name}</td>
                                        <td>{asset.category}</td>
                                        <td>
                                            <span className={`status-badge ${(asset.status === 'Available' ? 'approved' : 'pending')}`}>
                                                {asset.status === 'Available' ? 'Có sẵn' : 'Đang sử dụng'}
                                            </span>
                                        </td>
                                        <td>{asset.assignedTo || <span style={{ color: 'var(--text-tertiary)' }}>Chưa bàn giao</span>}</td>
                                        <td>
                                            {asset.status === 'Available' && (
                                                <button className="btn btn-primary btn-sm" onClick={() => { setAssignForm({ assetId: asset.id, employeeId: '' }); setShowAssignModal(true); }} style={{ marginRight: 8, padding: '4px 8px', borderRadius: '8px' }} title="Bàn giao">
                                                    <UserPlusIcon style={{ width: 16, height: 16 }} />
                                                </button>
                                            )}
                                            <button className="btn btn-secondary btn-sm" onClick={() => { setForm({ assetTag: asset.assetTag, name: asset.name, category: asset.category, purchaseDate: asset.purchaseDate ? asset.purchaseDate.split('T')[0] : '' }); setEditingId(asset.id); setShowModal(true); }} style={{ marginRight: 8, padding: '4px 8px', borderRadius: '8px' }} title="Sửa">
                                                <PencilSquareIcon style={{ width: 16, height: 16 }} />
                                            </button>
                                            <button className="btn btn-danger btn-sm" onClick={() => handleDelete(asset.id)} style={{ padding: '4px 8px', borderRadius: '8px' }} title="Xoá">
                                                <TrashIcon style={{ width: 16, height: 16 }} />
                                            </button>
                                        </td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            {/* Modal Tài sản */}
            {showModal && (
                <div className="modal-overlay" onClick={() => setShowModal(false)}>
                    <div className="modal-content" onClick={e => e.stopPropagation()}>
                        <div className="modal-header">
                            <h3>{editingId ? 'Sửa' : 'Thêm'} Tài sản</h3>
                            <button onClick={() => setShowModal(false)} style={{ background: 'var(--surface-container)', color: 'var(--text-secondary)', border: 'none', width: '32px', height: '32px', borderRadius: '50%', cursor: 'pointer' }}>✕</button>
                        </div>
                        <div className="modal-body">
                            <div className="form-group"><label>Mã tài sản</label><input className="form-control" value={form.assetTag} onChange={e => setForm({ ...form, assetTag: e.target.value })} /></div>
                            <div className="form-group"><label>Tên thiết bị</label><input className="form-control" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} /></div>
                            <div className="form-group"><label>Danh mục</label>
                                <select className="form-control" value={form.category} onChange={e => setForm({ ...form, category: e.target.value })}>
                                    <option value="">Chọn danh mục</option>
                                    <option value="Electronics">Thiết bị điện tử (Laptop, Màn hình)</option>
                                    <option value="Furniture">Nội thất (Bàn ghế, Tủ)</option>
                                </select>
                            </div>
                            <div className="form-group"><label>Ngày mua</label><input type="date" className="form-control" value={form.purchaseDate} onChange={e => setForm({ ...form, purchaseDate: e.target.value })} /></div>
                        </div>
                        <div className="modal-footer">
                            <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Hủy</button>
                            <button className="btn btn-primary" onClick={handleSubmit}>{editingId ? 'Lưu' : 'Tạo mới'}</button>
                        </div>
                    </div>
                </div>
            )}

            {/* Modal Bàn giao */}
            {showAssignModal && (
                <div className="modal-overlay" onClick={() => setShowAssignModal(false)}>
                    <div className="modal-content" onClick={e => e.stopPropagation()}>
                        <div className="modal-header">
                            <h3>Bàn giao Thiết bị</h3>
                            <button onClick={() => setShowAssignModal(false)} style={{ background: 'var(--surface-container)', color: 'var(--text-secondary)', border: 'none', width: '32px', height: '32px', borderRadius: '50%', cursor: 'pointer' }}>✕</button>
                        </div>
                        <div className="modal-body">
                            <div className="form-group"><label>Chọn người sử dụng mới</label>
                                <select className="form-control" value={assignForm.employeeId} onChange={e => setAssignForm({ ...assignForm, employeeId: e.target.value })}>
                                    <option value="">-- Chọn nhân viên --</option>
                                    {employees.map(e => <option key={e.id} value={e.id}>{e.name} - {e.department || 'Không có'}</option>)}
                                </select>
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button className="btn btn-secondary" onClick={() => setShowAssignModal(false)}>Hủy</button>
                            <button className="btn btn-primary" onClick={handleAssignSubmit} disabled={!assignForm.employeeId}>Xác nhận bàn giao</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
