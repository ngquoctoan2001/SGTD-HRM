import { useEffect, useState } from 'react';
import api from '../api/axiosConfig';
import { PencilSquareIcon, TrashIcon, UserGroupIcon, MagnifyingGlassIcon } from '@heroicons/react/24/outline';

export default function AccountManagement() {
    const [users, setUsers] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [searchTerm, setSearchTerm] = useState('');
    const [debouncedSearch, setDebouncedSearch] = useState('');
    const [showRoleModal, setShowRoleModal] = useState(false);
    const [activeUser, setActiveUser] = useState<any>(null);
    const [newRole, setNewRole] = useState('');

    useEffect(() => {
        const timer = setTimeout(() => setDebouncedSearch(searchTerm), 500);
        return () => clearTimeout(timer);
    }, [searchTerm]);

    useEffect(() => {
        fetchUsers();
    }, [debouncedSearch]);

    const fetchUsers = async () => {
        setLoading(true);
        try {
            const res = await api.get(`/users?pageSize=100&search=${encodeURIComponent(debouncedSearch)}`);
            if (res.data.success) {
                setUsers(res.data.data.items);
            }
        } catch (error) {
            console.error('Lỗi tải danh sách người dùng', error);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = async (id: number) => {
        if (!confirm('Bạn có chắc chắn muốn xoá tài khoản này không? Hành động này không thể hoàn tác.')) return;
        try {
            const res = await api.delete(`/users/${id}`);
            if (res.data.success) {
                alert('Đã xoá tài khoản thành công.');
                fetchUsers();
            }
        } catch {
            alert('Lỗi cập nhật. Vui lòng thử lại sau.');
        }
    };

    const handleChangeRole = (user: any) => {
        setActiveUser(user);
        setNewRole(user.role);
        setShowRoleModal(true);
    };

    const submitChangeRole = async () => {
        try {
            const res = await api.put(`/users/${activeUser.id}/role`, { role: newRole });
            if (res.data.success) {
                alert('Cập nhật phân quyền thành công.');
                setShowRoleModal(false);
                fetchUsers();
            }
        } catch {
            alert('Lỗi khi cập nhật quyền.');
        }
    };

    return (
        <div className="fade-in">
            <div className="page-header">
                <div>
                    <h1>Quản lý Hệ thống Tài khoản</h1>
                    <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginTop: '4px' }}>Giám sát và phân quyền truy cập cho nhân viên</p>
                </div>
            </div>

            <div className="toolbar" style={{ marginBottom: '24px' }}>
                <div className="search-box" style={{ position: 'relative', maxWidth: '400px' }}>
                    <MagnifyingGlassIcon style={{ position: 'absolute', left: '16px', top: '50%', transform: 'translateY(-50%)', width: '20px', color: 'var(--text-secondary)' }} />
                    <input
                        type="text"
                        placeholder="Tìm kiếm theo Tên, Username hoặc Email..."
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                        style={{ width: '100%', padding: '12px 16px 12px 44px', borderRadius: '12px', border: '1px solid var(--border-color)', outline: 'none', background: 'var(--surface-container)' }}
                    />
                </div>
            </div>

            <div className="card">
                <div className="card-header" style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                    <UserGroupIcon style={{ width: 24, height: 24, color: 'var(--primary)' }} />
                    <h3 style={{ margin: 0 }}>Danh sách Tài khoản</h3>
                </div>
                <div className="card-body" style={{ padding: 0 }}>
                    <div style={{ overflowX: 'auto' }}>
                        <table className="data-table">
                            <thead>
                                <tr>
                                    <th>Username</th>
                                    <th>Họ Tên</th>
                                    <th>Email</th>
                                    <th>Phân quyền</th>
                                    <th style={{ width: '120px' }}>Thao tác</th>
                                </tr>
                            </thead>
                            <tbody>
                                {loading ? (
                                    <tr><td colSpan={5} style={{ textAlign: 'center', padding: '40px' }}><div className="loading-spinner"><div className="spinner" /></div></td></tr>
                                ) : users.length === 0 ? (
                                    <tr><td colSpan={5} style={{ textAlign: 'center', padding: '60px 40px', color: 'var(--text-tertiary)' }}>Không tìm thấy người dùng nào.</td></tr>
                                ) : users.map(user => (
                                    <tr key={user.id}>
                                        <td style={{ fontWeight: 600 }}>{user.username}</td>
                                        <td>{user.name || '-'}</td>
                                        <td>{user.email || '-'}</td>
                                        <td>
                                            <span className="status-badge" style={{ background: user.role === 'Admin' ? '#e8eaf6' : 'var(--primary-light)', color: user.role === 'Admin' ? '#3f51b5' : 'var(--primary)' }}>
                                                {user.role}
                                            </span>
                                        </td>
                                        <td>
                                            <button className="btn btn-secondary btn-sm" onClick={() => handleChangeRole(user)} style={{ marginRight: 8, padding: '4px 8px', borderRadius: '8px' }} title="Đổi quyền">
                                                <PencilSquareIcon style={{ width: 16, height: 16 }} />
                                            </button>
                                            <button className="btn btn-danger btn-sm" onClick={() => handleDelete(user.id)} style={{ padding: '4px 8px', borderRadius: '8px' }} title="Xoá">
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

            {showRoleModal && activeUser && (
                <div className="modal-overlay" onClick={() => setShowRoleModal(false)}>
                    <div className="modal-content" onClick={e => e.stopPropagation()}>
                        <div className="modal-header">
                            <h3>Thiết lập Quyền truy cập</h3>
                            <button onClick={() => setShowRoleModal(false)} style={{ background: 'var(--surface-container)', color: 'var(--text-secondary)', border: 'none', width: '32px', height: '32px', borderRadius: '50%', cursor: 'pointer' }}>✕</button>
                        </div>
                        <div className="modal-body">
                            <p style={{ marginBottom: '16px', color: 'var(--text-secondary)' }}>Chọn quyền hạn mới cho tài khoản <strong>{activeUser.username}</strong> ({activeUser.name})</p>
                            <div className="form-group">
                                <label>Vai trò hệ thống</label>
                                <select className="form-control" value={newRole} onChange={e => setNewRole(e.target.value)}>
                                    <option value="Admin">Quản trị viên (Admin)</option>
                                    <option value="Manager">Quản lý (Manager)</option>
                                    <option value="User">Nhân viên (User)</option>
                                </select>
                            </div>
                        </div>
                        <div className="modal-footer">
                            <button className="btn btn-secondary" onClick={() => setShowRoleModal(false)}>Hủy bỏ</button>
                            <button className="btn btn-primary" onClick={submitChangeRole}>Lưu thay đổi</button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}
