import { useEffect, useState } from 'react';
import api from '../api/axiosConfig';
import { PlusIcon, TrashIcon, PencilSquareIcon } from '@heroicons/react/24/outline';

interface Department {
    id: number;
    name: string;
    description: string;
}

export default function Settings() {
    const [activeTab, setActiveTab] = useState('configs');
    const [configs, setConfigs] = useState<any[]>([]);

    // Departments state
    const [departments, setDepartments] = useState<Department[]>([]);
    const [loading, setLoading] = useState(true);
    const [showDeptModal, setShowDeptModal] = useState(false);
    const [deptForm, setDeptForm] = useState({ name: '', description: '' });
    const [editingDeptId, setEditingDeptId] = useState<number | null>(null);

    // Configs state
    const [showConfigModal, setShowConfigModal] = useState(false);
    const [configForm, setConfigForm] = useState({ key: '', value: '', description: '', group: 'General' });
    const [editingConfigId, setEditingConfigId] = useState<number | null>(null);

    const fetchData = () => {
        setLoading(true);
        Promise.all([
            api.get('/departments?pageSize=50'),
            api.get('/systemconfigs')
        ]).then(([deptRes, configRes]) => {
            if (deptRes.data.success) setDepartments(deptRes.data.data.items);
            if (configRes.data.success) setConfigs(configRes.data.data.items);
        }).finally(() => setLoading(false));
    };

    useEffect(() => { fetchData(); }, []);

    // Department actions
    const handleDeptSubmit = async () => {
        try {
            if (editingDeptId) await api.put(`/departments/${editingDeptId}`, deptForm);
            else await api.post('/departments', deptForm);
            setShowDeptModal(false);
            setDeptForm({ name: '', description: '' });
            setEditingDeptId(null);
            fetchData();
        } catch { alert('Có lỗi xảy ra khi lưu phòng ban.'); }
    };
    const handleDeptEdit = (dept: Department) => {
        setDeptForm({ name: dept.name, description: dept.description || '' });
        setEditingDeptId(dept.id);
        setShowDeptModal(true);
    };
    const handleDeptDelete = async (id: number) => {
        if (!confirm('Bạn có chắc chắn muốn xoá phòng ban này không?')) return;
        try {
            await api.delete(`/departments/${id}`);
            fetchData();
        } catch { alert('Lỗi khi xoá phòng ban.'); }
    };

    // Config actions
    const handleConfigSubmit = async () => {
        try {
            if (editingConfigId) await api.put(`/systemconfigs/${editingConfigId}`, configForm);
            else await api.post('/systemconfigs', configForm);
            setShowConfigModal(false);
            setConfigForm({ key: '', value: '', description: '', group: 'General' });
            setEditingConfigId(null);
            fetchData();
        } catch (error: any) {
            alert(error.response?.data?.message || 'Có lỗi xảy ra khi lưu cấu hình.');
        }
    };
    const handleConfigEdit = (conf: any) => {
        setConfigForm({ key: conf.key, value: conf.value, description: conf.description || '', group: conf.group });
        setEditingConfigId(conf.id);
        setShowConfigModal(true);
    };
    const handleConfigDelete = async (id: number) => {
        if (!confirm('Bạn có chắc xoá cấu hình này không?')) return;
        try {
            await api.delete(`/systemconfigs/${id}`);
            fetchData();
        } catch { alert('Lỗi khi xoá.'); }
    };

    if (loading) return <div className="loading-spinner"><div className="spinner" /></div>;

    return (
        <div className="fade-in">
            <div className="page-header">
                <div>
                    <h1>Cài đặt Hệ thống</h1>
                    <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginTop: '4px' }}>Quản lý tham số hệ thống và danh mục phòng ban</p>
                </div>
            </div>

            <div className="tabs" style={{ display: 'flex', gap: '24px', borderBottom: '1px solid var(--border-light)', marginBottom: '24px' }}>
                <div
                    onClick={() => setActiveTab('configs')}
                    style={{ padding: '12px 0', cursor: 'pointer', fontWeight: 600, color: activeTab === 'configs' ? 'var(--primary)' : 'var(--text-secondary)', borderBottom: activeTab === 'configs' ? '2px solid var(--primary)' : '2px solid transparent' }}
                >
                    Tham số Hệ thống
                </div>
                <div
                    onClick={() => setActiveTab('departments')}
                    style={{ padding: '12px 0', cursor: 'pointer', fontWeight: 600, color: activeTab === 'departments' ? 'var(--primary)' : 'var(--text-secondary)', borderBottom: activeTab === 'departments' ? '2px solid var(--primary)' : '2px solid transparent' }}
                >
                    Quản lý Phòng ban
                </div>
            </div>

            {activeTab === 'departments' && (
                <div className="card" style={{ marginBottom: '24px' }}>
                    <div className="card-header" style={{ padding: '16px 24px', borderBottom: '1px solid var(--border-light)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                        <h3 style={{ margin: 0, fontSize: '16px', fontWeight: 600 }}>Tất cả phòng ban</h3>
                        <button className="btn btn-primary btn-sm" onClick={() => { setDeptForm({ name: '', description: '' }); setEditingDeptId(null); setShowDeptModal(true); }}>
                            <PlusIcon style={{ width: 16, height: 16 }} /> Thêm KQ
                        </button>
                    </div>
                    <div className="card-body" style={{ padding: 0 }}>
                        <div style={{ overflowX: 'auto' }}>
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Tên phòng ban</th>
                                        <th>Mô tả</th>
                                        <th style={{ width: '120px' }}>Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {departments.length === 0 ? (
                                        <tr><td colSpan={4} style={{ textAlign: 'center', padding: '40px', color: 'var(--text-tertiary)' }}>Chưa có.</td></tr>
                                    ) : departments.map(dept => (
                                        <tr key={dept.id}>
                                            <td style={{ fontWeight: 600 }}>#{dept.id}</td>
                                            <td style={{ fontWeight: 500, color: 'var(--text-primary)' }}>{dept.name}</td>
                                            <td>{dept.description || '-'}</td>
                                            <td>
                                                <button className="btn btn-secondary btn-sm" onClick={() => handleDeptEdit(dept)} style={{ marginRight: 8, padding: '4px 8px', borderRadius: '8px' }} title="Sửa"><PencilSquareIcon style={{ width: 16, height: 16 }} /></button>
                                                <button className="btn btn-danger btn-sm" onClick={() => handleDeptDelete(dept.id)} style={{ padding: '4px 8px', borderRadius: '8px' }} title="Xoá"><TrashIcon style={{ width: 16, height: 16 }} /></button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            )}

            {activeTab === 'configs' && (
                <div className="card" style={{ marginBottom: '24px' }}>
                    <div className="card-header" style={{ padding: '16px 24px', borderBottom: '1px solid var(--border-light)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                        <h3 style={{ margin: 0, fontSize: '16px', fontWeight: 600 }}>Tham số (Key-Value)</h3>
                        <button className="btn btn-primary btn-sm" onClick={() => { setConfigForm({ key: '', value: '', description: '', group: 'General' }); setEditingConfigId(null); setShowConfigModal(true); }}>
                            <PlusIcon style={{ width: 16, height: 16 }} /> Thêm Key mới
                        </button>
                    </div>
                    <div className="card-body" style={{ padding: 0 }}>
                        <div style={{ overflowX: 'auto' }}>
                            <table className="data-table">
                                <thead>
                                    <tr>
                                        <th>Nhóm</th>
                                        <th>Key (Mã)</th>
                                        <th>Value (Giá trị)</th>
                                        <th>Mô tả</th>
                                        <th style={{ width: '120px' }}>Thao tác</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {configs.length === 0 ? (
                                        <tr><td colSpan={5} style={{ textAlign: 'center', padding: '40px', color: 'var(--text-tertiary)' }}>Chưa có cấu hình nào.</td></tr>
                                    ) : configs.map(conf => (
                                        <tr key={conf.id}>
                                            <td><span className="status-badge active" style={{ backgroundColor: 'var(--surface-container)' }}>{conf.group}</span></td>
                                            <td style={{ fontWeight: 600, color: 'var(--primary)', fontFamily: 'monospace' }}>{conf.key}</td>
                                            <td style={{ fontWeight: 500 }}>{conf.value}</td>
                                            <td style={{ color: 'var(--text-secondary)' }}>{conf.description || '-'}</td>
                                            <td>
                                                <button className="btn btn-secondary btn-sm" onClick={() => handleConfigEdit(conf)} style={{ marginRight: 8, padding: '4px 8px', borderRadius: '8px' }} title="Sửa"><PencilSquareIcon style={{ width: 16, height: 16 }} /></button>
                                                <button className="btn btn-danger btn-sm" onClick={() => handleConfigDelete(conf.id)} style={{ padding: '4px 8px', borderRadius: '8px' }} title="Xoá"><TrashIcon style={{ width: 16, height: 16 }} /></button>
                                            </td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            )}

            {/* Modals */}
            {showDeptModal && (
                <div className="modal-overlay" onClick={() => setShowDeptModal(false)}>
                    <div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: '500px' }}>
                        <div className="modal-header">
                            <h3>{editingDeptId ? 'Chỉnh sửa' : 'Thêm mới'} Phòng ban</h3>
                            <button onClick={() => setShowDeptModal(false)} style={{ background: 'var(--surface-container)', color: 'var(--text-secondary)', border: 'none', width: '32px', height: '32px', borderRadius: '50%', cursor: 'pointer' }}>✕</button>
                        </div>
                        <div className="modal-body">
                            <div className="form-group">
                                <label>Tên phòng ban <span style={{ color: 'var(--danger)' }}>*</span></label>
                                <input className="form-control" value={deptForm.name} onChange={e => setDeptForm({ ...deptForm, name: e.target.value })} placeholder="VD: Khối Kỹ thuật" />
                            </div>
                            <div className="form-group">
                                <label>Mô tả chi tiết</label>
                                <textarea className="form-control" value={deptForm.description} onChange={e => setDeptForm({ ...deptForm, description: e.target.value })} rows={3}></textarea>
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button className="btn btn-secondary" onClick={() => setShowDeptModal(false)}>Hủy bỏ</button>
                            <button className="btn btn-primary" onClick={handleDeptSubmit} disabled={!deptForm.name.trim()}>{editingDeptId ? 'Cập nhật' : 'Lưu lại'}</button>
                        </div>
                    </div>
                </div>
            )}

            {showConfigModal && (
                <div className="modal-overlay" onClick={() => setShowConfigModal(false)}>
                    <div className="modal-content" onClick={e => e.stopPropagation()} style={{ maxWidth: '500px' }}>
                        <div className="modal-header">
                            <h3>{editingConfigId ? 'Chỉnh sửa' : 'Thêm mới'} Cấu hình</h3>
                            <button onClick={() => setShowConfigModal(false)} style={{ background: 'var(--surface-container)', color: 'var(--text-secondary)', border: 'none', width: '32px', height: '32px', borderRadius: '50%', cursor: 'pointer' }}>✕</button>
                        </div>
                        <div className="modal-body">
                            <div className="form-group">
                                <label>Key (Mã tham số) <span style={{ color: 'var(--danger)' }}>*</span></label>
                                <input className="form-control" value={configForm.key} onChange={e => setConfigForm({ ...configForm, key: e.target.value })} placeholder="VD: MAX_LEAVE_DAYS" style={{ fontFamily: 'monospace' }} />
                            </div>
                            <div className="form-group">
                                <label>Value (Giá trị) <span style={{ color: 'var(--danger)' }}>*</span></label>
                                <input className="form-control" value={configForm.value} onChange={e => setConfigForm({ ...configForm, value: e.target.value })} placeholder="VD: 12" />
                            </div>
                            <div className="form-group">
                                <label>Nhóm (Group)</label>
                                <select className="form-control" value={configForm.group} onChange={e => setConfigForm({ ...configForm, group: e.target.value })}>
                                    <option value="General">Chung (General)</option>
                                    <option value="Leave">Nghỉ phép (Leave)</option>
                                    <option value="Payroll">Tính lương (Payroll)</option>
                                </select>
                            </div>
                            <div className="form-group">
                                <label>Mô tả / Ghi chú</label>
                                <textarea className="form-control" value={configForm.description} onChange={e => setConfigForm({ ...configForm, description: e.target.value })} rows={2}></textarea>
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button className="btn btn-secondary" onClick={() => setShowConfigModal(false)}>Hủy bỏ</button>
                            <button className="btn btn-primary" onClick={handleConfigSubmit} disabled={!configForm.key.trim() || !configForm.value.trim()}>{editingConfigId ? 'Cập nhật' : 'Lưu lại'}</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
