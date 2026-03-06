import { useEffect, useState, useRef } from 'react';
import api from '../api/axiosConfig';
import { MagnifyingGlassIcon, FunnelIcon, PlusIcon, TrashIcon, PencilSquareIcon, ArrowDownTrayIcon, ArrowUpTrayIcon } from '@heroicons/react/24/outline';

interface Employee {
    id: number; name: string; title: string; department: string; departmentId?: number;
    email: string; phone: string; status: string; avatar: string; joinDate: string;
}

interface Department {
    id: number;
    name: string;
}

export default function Employees() {
    const [employees, setEmployees] = useState<Employee[]>([]);
    const [departments, setDepartments] = useState<Department[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [form, setForm] = useState({ name: '', title: '', departmentId: '', email: '', phone: '', avatar: '' });
    const [editingId, setEditingId] = useState<number | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    // Filters
    const [search, setSearch] = useState('');
    const [statusFilter, setStatusFilter] = useState('');
    const [departmentFilter, setDepartmentFilter] = useState('');

    // Debounce search
    const [debouncedSearch, setDebouncedSearch] = useState('');
    useEffect(() => {
        const handler = setTimeout(() => { setDebouncedSearch(search); }, 500);
        return () => clearTimeout(handler);
    }, [search]);

    const fetchEmployees = () => {
        setLoading(true);
        const queryParams = new URLSearchParams({ pageSize: '50' });
        if (debouncedSearch) queryParams.append('search', debouncedSearch);
        if (statusFilter) queryParams.append('status', statusFilter);
        if (departmentFilter) queryParams.append('departmentId', departmentFilter);

        api.get(`/employees?${queryParams.toString()}`)
            .then(res => { if (res.data.success) setEmployees(res.data.data.items); })
            .finally(() => setLoading(false));
    };

    const fetchDepartments = () => {
        api.get('/departments?pageSize=50')
            .then(res => { if (res.data.success) setDepartments(res.data.data.items); });
    };

    useEffect(() => {
        fetchEmployees();
        fetchDepartments();
    }, [debouncedSearch, statusFilter, departmentFilter]);

    const handleSubmit = async () => {
        try {
            if (editingId) {
                await api.put(`/employees/${editingId}`, { ...form });
            } else {
                await api.post('/employees', { ...form, joinDate: new Date().toISOString() });
            }
            setShowModal(false);
            setForm({ name: '', title: '', departmentId: '', email: '', phone: '', avatar: '' });
            setEditingId(null);
            fetchEmployees();
        } catch { }
    };

    const handleEdit = (emp: Employee) => {
        const dept = departments.find(d => d.name === emp.department);
        setForm({ name: emp.name, title: emp.title, departmentId: dept ? dept.id.toString() : '', email: emp.email, phone: emp.phone, avatar: emp.avatar });
        setEditingId(emp.id);
        setShowModal(true);
    };

    const handleDelete = async (id: number) => {
        if (!confirm('Bạn có chắc chắn muốn xoá nhân viên này không?')) return;
        await api.delete(`/employees/${id}`);
        fetchEmployees();
    };

    const handleExportCsv = () => {
        if (!employees || employees.length === 0) return alert('Chưa có dữ liệu để xuất.');
        const headers = "ID,Tên nhân viên,Chức danh,Phòng ban,Email,Số điện thoại,Trạng thái,Ngày tham gia\n";
        const rows = employees.map(e => `"${e.id}","${e.name}","${e.title}","${e.department}","${e.email}","${e.phone}","${e.status}","${e.joinDate ? e.joinDate.split('T')[0] : ''}"`).join('\n');

        const csvContent = "data:text/csv;charset=utf-8,\uFEFF" + headers + rows;
        const encodedUri = encodeURI(csvContent);
        const link = document.createElement("a");
        link.setAttribute("href", encodedUri);
        link.setAttribute("download", `employees_export.csv`);
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
                    <h1>Danh sách Nhân sự</h1>
                    <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginTop: '4px' }}>Quản lý thông tin và tài khoản nhân viên</p>
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
                    <button className="btn btn-primary" onClick={() => { setForm({ name: '', title: '', departmentId: '', email: '', phone: '', avatar: '' }); setEditingId(null); setShowModal(true); }}>
                        <PlusIcon style={{ width: 18, height: 18 }} /> Thêm nhân viên
                    </button>
                </div>
            </div>

            <div className="card" style={{ marginBottom: '24px' }}>
                <div className="card-body" style={{ display: 'flex', gap: '16px', alignItems: 'center' }}>
                    <div className="form-group" style={{ flex: 1, marginBottom: 0 }}>
                        <div style={{ position: 'relative' }}>
                            <MagnifyingGlassIcon style={{ position: 'absolute', left: 12, top: 10, width: 20, height: 20, color: 'var(--text-tertiary)' }} />
                            <input
                                className="form-control"
                                style={{ paddingLeft: 40 }}
                                placeholder="Tìm kiếm theo tên hoặc mã nhân viên..."
                                value={search}
                                onChange={(e) => setSearch(e.target.value)}
                            />
                        </div>
                    </div>
                    <div className="form-group" style={{ width: '200px', marginBottom: 0, display: 'flex', alignItems: 'center', gap: 8 }}>
                        <FunnelIcon style={{ width: 20, height: 20, color: 'var(--text-tertiary)' }} />
                        <select className="form-control" value={departmentFilter} onChange={e => setDepartmentFilter(e.target.value)}>
                            <option value="">Tất cả phòng ban</option>
                            {departments.map(d => (
                                <option key={d.id} value={d.id}>{d.name}</option>
                            ))}
                        </select>
                    </div>
                    <div className="form-group" style={{ width: '200px', marginBottom: 0 }}>
                        <select className="form-control" value={statusFilter} onChange={e => setStatusFilter(e.target.value)}>
                            <option value="">Tất cả trạng thái</option>
                            <option value="Active">Đang làm việc</option>
                            <option value="OnLeave">Nghỉ phép</option>
                            <option value="Resigned">Đã nghỉ việc</option>
                        </select>
                    </div>
                </div>
            </div>

            <div className="card">
                <div className="card-body" style={{ padding: 0 }}>
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Nhân viên</th>
                                <th>Chức danh</th>
                                <th>Phòng ban</th>
                                <th>Email</th>
                                <th>Trạng thái</th>
                                <th>Thao tác</th>
                            </tr>
                        </thead>
                        <tbody>
                            {employees.map(emp => (
                                <tr key={emp.id}>
                                    <td>
                                        <div className="employee-cell">
                                            {emp.avatar ? (
                                                <img src={`http://localhost:5000${emp.avatar}`} alt="Avatar" className="employee-avatar" style={{ objectFit: 'cover' }} />
                                            ) : (
                                                <div className="employee-avatar">{emp.name.charAt(0)}</div>
                                            )}
                                            <div>
                                                <div style={{ fontWeight: 600, color: 'var(--text-primary)' }}>{emp.name}</div>
                                                <div style={{ fontSize: '13px', color: 'var(--text-tertiary)' }}>ID: #{emp.id}</div>
                                            </div>
                                        </div>
                                    </td>
                                    <td>{emp.title}</td>
                                    <td>{emp.department}</td>
                                    <td>{emp.email}</td>
                                    <td><span className={`status-badge ${(emp.status || 'Active').toLowerCase()}`}>{emp.status || 'Active'}</span></td>
                                    <td>
                                        <button className="btn btn-secondary btn-sm" onClick={() => handleEdit(emp)} style={{ marginRight: 8, padding: '4px 8px', borderRadius: '8px' }} title="Sửa">
                                            <PencilSquareIcon style={{ width: 16, height: 16 }} />
                                        </button>
                                        <button className="btn btn-danger btn-sm" onClick={() => handleDelete(emp.id)} style={{ padding: '4px 8px', borderRadius: '8px' }}>
                                            <TrashIcon style={{ width: 16, height: 16 }} />
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {showModal && (
                <div className="modal-overlay" onClick={() => setShowModal(false)}>
                    <div className="modal-content" onClick={e => e.stopPropagation()}>
                        <div className="modal-header">
                            <h3>{editingId ? 'Chỉnh sửa' : 'Thêm'} nhân viên</h3>
                            <button onClick={() => setShowModal(false)} style={{ background: 'var(--surface-container)', color: 'var(--text-secondary)', border: 'none', width: '32px', height: '32px', borderRadius: '50%', cursor: 'pointer' }}>✕</button>
                        </div>
                        <div className="modal-body">
                            <div className="form-group">
                                <label>Họ và tên</label>
                                <input className="form-control" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} placeholder="Nguyễn Văn A" />
                            </div>
                            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                                <div className="form-group">
                                    <label>Chức danh</label>
                                    <input className="form-control" value={form.title} onChange={e => setForm({ ...form, title: e.target.value })} placeholder="Developer" />
                                </div>
                                <div className="form-group">
                                    <label>Phòng ban</label>
                                    <select className="form-control" value={form.departmentId} onChange={e => setForm({ ...form, departmentId: e.target.value })}>
                                        <option value="" disabled>-- Chọn phòng ban --</option>
                                        {departments.map(d => (
                                            <option key={d.id} value={d.id}>{d.name}</option>
                                        ))}
                                    </select>
                                </div>
                            </div>
                            <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '16px' }}>
                                <div className="form-group">
                                    <label>Email</label>
                                    <input type="email" className="form-control" value={form.email} onChange={e => setForm({ ...form, email: e.target.value })} placeholder="email@congty.com" />
                                </div>
                                <div className="form-group">
                                    <label>Số điện thoại</label>
                                    <input className="form-control" value={form.phone} onChange={e => setForm({ ...form, phone: e.target.value })} placeholder="09xxxx" />
                                </div>
                            </div>
                            <div className="form-group">
                                <label>Ảnh đại diện (Avatar)</label>
                                <div style={{ display: 'flex', gap: '12px', alignItems: 'center' }}>
                                    {form.avatar && (
                                        <img src={`http://localhost:5000${form.avatar}`} alt="Avatar" style={{ width: 40, height: 40, borderRadius: '50%', objectFit: 'cover', background: 'var(--surface-container)' }} />
                                    )}
                                    <input type="file" className="form-control" accept="image/*" onChange={async (e) => {
                                        if (e.target.files && e.target.files[0]) {
                                            const file = e.target.files[0];
                                            const formData = new FormData();
                                            formData.append('file', file);
                                            try {
                                                const uploadRes = await api.post('/files/upload', formData, {
                                                    headers: { 'Content-Type': 'multipart/form-data' }
                                                });
                                                if (uploadRes.data.success) setForm({ ...form, avatar: uploadRes.data.data });
                                            } catch (err) { alert('Lỗi tải ảnh. Dung lượng quá lớn hoặc sai định dạng.'); }
                                        }
                                    }} style={{ flex: 1 }} />
                                </div>
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Hủy bỏ</button>
                            <button className="btn btn-primary" onClick={handleSubmit}>{editingId ? 'Cập nhật' : 'Tạo mới'}</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
