import { useEffect, useState } from 'react';
import api from '../api/axiosConfig';
import { PlusIcon, TrashIcon, PencilSquareIcon } from '@heroicons/react/24/outline';

interface Department {
    id: number;
    name: string;
    description: string;
}

export default function Settings() {
    const [departments, setDepartments] = useState<Department[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [form, setForm] = useState({ name: '', description: '' });
    const [editingId, setEditingId] = useState<number | null>(null);

    const fetchDepartments = () => {
        setLoading(true);
        api.get('/departments?pageSize=50')
            .then(res => { if (res.data.success) setDepartments(res.data.data.items); })
            .finally(() => setLoading(false));
    };

    useEffect(() => { fetchDepartments(); }, []);

    const handleSubmit = async () => {
        try {
            if (editingId) {
                await api.put(`/departments/${editingId}`, form);
            } else {
                await api.post('/departments', form);
            }
            setShowModal(false);
            setForm({ name: '', description: '' });
            setEditingId(null);
            fetchDepartments();
        } catch { alert('Có lỗi xảy ra khi lưu phòng ban.'); }
    };

    const handleEdit = (dept: Department) => {
        setForm({ name: dept.name, description: dept.description || '' });
        setEditingId(dept.id);
        setShowModal(true);
    };

    const handleDelete = async (id: number) => {
        if (!confirm('Bạn có chắc chắn muốn xoá phòng ban này không?')) return;
        try {
            await api.delete(`/departments/${id}`);
            fetchDepartments();
        } catch { alert('Lỗi khi xoá phòng ban.'); }
    };

    if (loading) return <div className="loading-spinner"><div className="spinner" /></div>;

    return (
        <div className="fade-in">
            <div className="page-header">
                <div>
                    <h1>Cấu hình hệ thống</h1>
                    <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginTop: '4px' }}>Quản lý danh mục, phòng ban và cài đặt chung</p>
                </div>
            </div>

            <div className="card" style={{ marginBottom: '24px' }}>
                <div className="card-header" style={{ padding: '16px 24px', borderBottom: '1px solid var(--border-light)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <h3 style={{ margin: 0, fontSize: '16px', fontWeight: 600 }}>Quản lý Phòng ban</h3>
                    <button className="btn btn-primary btn-sm" onClick={() => { setForm({ name: '', description: '' }); setEditingId(null); setShowModal(true); }}>
                        <PlusIcon style={{ width: 16, height: 16 }} /> Thêm mới
                    </button>
                </div>
                <div className="card-body" style={{ padding: 0 }}>
                    <div style={{ overflowX: 'auto' }}>
                        <table className="data-table">
                            <thead>
                                <tr>
                                    <th>Mã Nhận Diện (ID)</th>
                                    <th>Tên phòng ban</th>
                                    <th>Mô tả</th>
                                    <th style={{ width: '120px' }}>Thao tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                {departments.length === 0 ? (
                                    <tr><td colSpan={4} style={{ textAlign: 'center', padding: '40px', color: 'var(--text-tertiary)' }}>Chưa có phòng ban nào.</td></tr>
                                ) : departments.map(dept => (
                                    <tr key={dept.id}>
                                        <td style={{ fontWeight: 600 }}>#{dept.id}</td>
                                        <td style={{ fontWeight: 500, color: 'var(--text-primary)' }}>{dept.name}</td>
                                        <td>{dept.description || '-'}</td>
                                        <td>
                                            <button className="btn btn-secondary btn-sm" onClick={() => handleEdit(dept)} style={{ marginRight: 8, padding: '4px 8px', borderRadius: '8px' }} title="Sửa">
                                                <PencilSquareIcon style={{ width: 16, height: 16 }} />
                                            </button>
                                            <button className="btn btn-danger btn-sm" onClick={() => handleDelete(dept.id)} style={{ padding: '4px 8px', borderRadius: '8px' }} title="Xoá">
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

            {/* Modal */}
            {showModal && (
                <div className="modal-overlay" onClick={() => setShowModal(false)}>
                    <div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: '500px' }}>
                        <div className="modal-header">
                            <h3>{editingId ? 'Chỉnh sửa' : 'Thêm mới'} Phòng ban</h3>
                            <button onClick={() => setShowModal(false)} style={{ background: 'var(--surface-container)', color: 'var(--text-secondary)', border: 'none', width: '32px', height: '32px', borderRadius: '50%', cursor: 'pointer' }}>✕</button>
                        </div>
                        <div className="modal-body">
                            <div className="form-group">
                                <label>Tên phòng ban <span style={{ color: 'var(--danger)' }}>*</span></label>
                                <input className="form-control" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} placeholder="VD: Khối Kỹ thuật (Engineering)" />
                            </div>
                            <div className="form-group">
                                <label>Mô tả chi tiết</label>
                                <textarea className="form-control" value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} placeholder="Nhập mô tả về chức năng của phòng ban..." rows={3}></textarea>
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Hủy bỏ</button>
                            <button className="btn btn-primary" onClick={handleSubmit} disabled={!form.name.trim()}>{editingId ? 'Cập nhật' : 'Tạo mới'}</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
