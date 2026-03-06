import { useEffect, useState } from 'react';
import api from '../api/axiosConfig';
import { useAuthStore } from '../store/authStore';
import { UserCircleIcon, KeyIcon, PencilSquareIcon, CheckIcon, XMarkIcon } from '@heroicons/react/24/outline';

export default function Profile() {
    const { user, login } = useAuthStore();
    const [profile, setProfile] = useState<any>(null);
    const [loading, setLoading] = useState(true);

    const [editMode, setEditMode] = useState(false);
    const [form, setForm] = useState({ name: '', phone: '', bio: '' });

    const [pwdMode, setPwdMode] = useState(false);
    const [pwdForm, setPwdForm] = useState({ currentPassword: '', newPassword: '', confirmPassword: '' });

    useEffect(() => {
        fetchProfile();
    }, []);

    const fetchProfile = async () => {
        try {
            const res = await api.get('/users/me');
            if (res.data.success) {
                setProfile(res.data.data);
                setForm({
                    name: res.data.data.name || '',
                    phone: res.data.data.phone || '',
                    bio: res.data.data.bio || ''
                });
            }
        } catch (error) {
            console.error('Lỗi khi tải profile', error);
        } finally {
            setLoading(false);
        }
    };

    const handleUpdateProfile = async () => {
        try {
            const res = await api.put('/users/me', form);
            if (res.data.success) {
                setProfile(res.data.data);
                setEditMode(false);
                alert('Cập nhật thông tin thành công!');
            }
        } catch (error) {
            alert('Lỗi cập nhật');
        }
    };

    const handleChangePassword = async () => {
        if (pwdForm.newPassword !== pwdForm.confirmPassword) {
            alert('Mật khẩu xác nhận không khớp!');
            return;
        }
        try {
            const res = await api.put('/users/me/change-password', {
                currentPassword: pwdForm.currentPassword,
                newPassword: pwdForm.newPassword
            });
            if (res.data.success) {
                alert('Đổi mật khẩu thành công!');
                setPwdMode(false);
                setPwdForm({ currentPassword: '', newPassword: '', confirmPassword: '' });
            }
        } catch (error: any) {
            alert(error.response?.data?.message || 'Lỗi đổi mật khẩu');
        }
    };

    if (loading) return <div style={{ padding: 40, textAlign: 'center' }}><div className="loading-spinner"><div className="spinner" /></div></div>;

    return (
        <div className="fade-in" style={{ maxWidth: '800px', margin: '0 auto' }}>
            <div className="page-header" style={{ marginBottom: '24px' }}>
                <div>
                    <h1>Hồ sơ Cá nhân</h1>
                    <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginTop: '4px' }}>Quản lý thông tin và bảo mật tài khoản</p>
                </div>
            </div>

            <div className="card" style={{ marginBottom: '24px' }}>
                <div className="card-header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <h3 style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <UserCircleIcon style={{ width: 24, height: 24, color: 'var(--primary)' }} /> Thông tin chung
                    </h3>
                    {!editMode ? (
                        <button className="btn btn-secondary btn-sm" onClick={() => setEditMode(true)}>
                            <PencilSquareIcon style={{ width: 16, height: 16 }} /> Chỉnh sửa
                        </button>
                    ) : (
                        <div style={{ display: 'flex', gap: '8px' }}>
                            <button className="btn btn-secondary btn-sm" onClick={() => {
                                setEditMode(false);
                                setForm({ name: profile?.name || '', phone: profile?.phone || '', bio: profile?.bio || '' });
                            }}>
                                <XMarkIcon style={{ width: 16, height: 16 }} /> Hủy
                            </button>
                            <button className="btn btn-primary btn-sm" onClick={handleUpdateProfile}>
                                <CheckIcon style={{ width: 16, height: 16 }} /> Lưu lại
                            </button>
                        </div>
                    )}
                </div>
                <div className="card-body">
                    <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px' }}>
                        <div>
                            <label style={{ display: 'block', fontSize: '13px', color: 'var(--text-tertiary)', marginBottom: '4px' }}>Tên đăng nhập / Email</label>
                            <div style={{ fontWeight: 500 }}>{profile?.username} - {profile?.email}</div>
                        </div>
                        <div>
                            <label style={{ display: 'block', fontSize: '13px', color: 'var(--text-tertiary)', marginBottom: '4px' }}>Họ và Tên</label>
                            {editMode ? (
                                <input className="form-control" value={form.name} onChange={e => setForm({ ...form, name: e.target.value })} />
                            ) : (
                                <div style={{ fontWeight: 500 }}>{profile?.name || '-'}</div>
                            )}
                        </div>
                        <div>
                            <label style={{ display: 'block', fontSize: '13px', color: 'var(--text-tertiary)', marginBottom: '4px' }}>Phân quyền</label>
                            <div style={{ fontWeight: 500 }}>
                                <span className={`status-badge`} style={{ background: 'var(--primary-light)', color: 'var(--primary)' }}>{profile?.role}</span>
                            </div>
                        </div>
                        <div>
                            <label style={{ display: 'block', fontSize: '13px', color: 'var(--text-tertiary)', marginBottom: '4px' }}>Số điện thoại</label>
                            {editMode ? (
                                <input className="form-control" value={form.phone} onChange={e => setForm({ ...form, phone: e.target.value })} />
                            ) : (
                                <div style={{ fontWeight: 500 }}>{profile?.phone || '-'}</div>
                            )}
                        </div>
                        <div style={{ gridColumn: '1 / -1' }}>
                            <label style={{ display: 'block', fontSize: '13px', color: 'var(--text-tertiary)', marginBottom: '4px' }}>Giới thiệu ngắn (Bio)</label>
                            {editMode ? (
                                <textarea className="form-control" value={form.bio} onChange={e => setForm({ ...form, bio: e.target.value })} rows={3} />
                            ) : (
                                <div style={{ fontWeight: 500, color: profile?.bio ? 'inherit' : 'var(--text-tertiary)' }}>{profile?.bio || 'Chưa cập nhật'}</div>
                            )}
                        </div>
                    </div>
                </div>
            </div>

            <div className="card">
                <div className="card-header" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <h3 style={{ display: 'flex', alignItems: 'center', gap: '8px' }}>
                        <KeyIcon style={{ width: 24, height: 24, color: '#f59e0b' }} /> Cài đặt mật khẩu
                    </h3>
                    {!pwdMode && (
                        <button className="btn btn-secondary btn-sm" onClick={() => setPwdMode(true)}>
                            <PencilSquareIcon style={{ width: 16, height: 16 }} /> Đổi mật khẩu
                        </button>
                    )}
                </div>
                {pwdMode && (
                    <div className="card-body" style={{ background: 'var(--surface-container)', borderRadius: '0 0 16px 16px' }}>
                        <div style={{ display: 'flex', flexDirection: 'column', gap: '16px', maxWidth: '400px' }}>
                            <div className="form-group">
                                <label>Mật khẩu hiện tại</label>
                                <input type="password" className="form-control" value={pwdForm.currentPassword} onChange={e => setPwdForm({ ...pwdForm, currentPassword: e.target.value })} />
                            </div>
                            <div className="form-group">
                                <label>Mật khẩu mới</label>
                                <input type="password" className="form-control" value={pwdForm.newPassword} onChange={e => setPwdForm({ ...pwdForm, newPassword: e.target.value })} />
                            </div>
                            <div className="form-group">
                                <label>Xác nhận mật khẩu mới</label>
                                <input type="password" className="form-control" value={pwdForm.confirmPassword} onChange={e => setPwdForm({ ...pwdForm, confirmPassword: e.target.value })} />
                            </div>
                            <div style={{ display: 'flex', gap: '12px', marginTop: '8px' }}>
                                <button className="btn btn-primary" onClick={handleChangePassword}>Cập nhật mật khẩu</button>
                                <button className="btn btn-secondary" onClick={() => {
                                    setPwdMode(false);
                                    setPwdForm({ currentPassword: '', newPassword: '', confirmPassword: '' });
                                }}>Hủy</button>
                            </div>
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}
