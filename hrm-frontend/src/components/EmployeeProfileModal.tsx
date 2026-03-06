import React, { useEffect, useState } from 'react';
import api from '../api/axiosConfig';

interface ContractDto { id: number; startDate: string; endDate: string; salary: number; status: string; }
interface DisciplineRewardDto { id: number; type: string; reason: string; amount: number; date: string; }
interface LeaveRequestDto { id: number; type: string; startDate: string; endDate: string; status: string; }
interface AttendanceRecordDto { id: number; date: string; status: string; }
interface PerformanceReviewDto { id: number; reviewDate: string; rating: number; status: string; }

interface EmployeeProfileDto {
    id: number;
    name: string;
    title: string;
    department: string;
    email: string;
    phone: string;
    status: string;
    avatar: string;
    joinDate: string;
    contracts: ContractDto[];
    disciplineRewards: DisciplineRewardDto[];
    leaveRequests: LeaveRequestDto[];
    attendanceRecords: AttendanceRecordDto[];
    performanceReviews: PerformanceReviewDto[];
}

interface Props {
    employeeId: number;
    onClose: () => void;
}

export default function EmployeeProfileModal({ employeeId, onClose }: Props) {
    const [profile, setProfile] = useState<EmployeeProfileDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [activeTab, setActiveTab] = useState('contracts');

    useEffect(() => {
        const fetchProfile = async () => {
            try {
                const res = await api.get(`/employees/${employeeId}/profile`);
                if (res.data.success) {
                    setProfile(res.data.data);
                }
            } catch (err) {
                console.error('Failed to fetch profile', err);
            } finally {
                setLoading(false);
            }
        };
        fetchProfile();
    }, [employeeId]);

    if (loading) return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={e => e.stopPropagation()} style={{ width: '80%', maxWidth: '900px', height: '80vh', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                <div className="spinner"></div>
            </div>
        </div>
    );

    if (!profile) return null;

    const renderTabContent = () => {
        switch (activeTab) {
            case 'contracts':
                return (
                    <table className="data-table">
                        <thead>
                            <tr><th>Mã Hợp đồng</th><th>Ngày bắt đầu</th><th>Ngày kết thúc</th><th>Lương Cơ bản</th><th>Trạng thái</th></tr>
                        </thead>
                        <tbody>
                            {profile.contracts.length > 0 ? profile.contracts.map(c => (
                                <tr key={c.id}>
                                    <td>#{c.id}</td>
                                    <td>{c.startDate.split('T')[0]}</td>
                                    <td>{c.endDate.split('T')[0]}</td>
                                    <td>{c.salary.toLocaleString()} VNĐ</td>
                                    <td><span className={`status-badge ${c.status.toLowerCase()}`}>{c.status}</span></td>
                                </tr>
                            )) : <tr><td colSpan={5} className="text-center text-gray-500 py-4">Chưa có hợp đồng nào.</td></tr>}
                        </tbody>
                    </table>
                );
            case 'leaves':
                return (
                    <table className="data-table">
                        <thead>
                            <tr><th>Loại nghỉ phép</th><th>Từ ngày</th><th>Đến ngày</th><th>Trạng thái</th></tr>
                        </thead>
                        <tbody>
                            {profile.leaveRequests.length > 0 ? profile.leaveRequests.map(l => (
                                <tr key={l.id}>
                                    <td>{l.type}</td>
                                    <td>{l.startDate.split('T')[0]}</td>
                                    <td>{l.endDate.split('T')[0]}</td>
                                    <td><span className={`status-badge ${l.status.toLowerCase()}`}>{l.status}</span></td>
                                </tr>
                            )) : <tr><td colSpan={4} className="text-center text-gray-500 py-4">Chưa có dữ liệu nghỉ phép.</td></tr>}
                        </tbody>
                    </table>
                );
            case 'perf':
                return (
                    <table className="data-table">
                        <thead>
                            <tr><th>Ngày đánh giá</th><th>Điểm rating</th><th>Trạng thái</th></tr>
                        </thead>
                        <tbody>
                            {profile.performanceReviews.length > 0 ? profile.performanceReviews.map(p => (
                                <tr key={p.id}>
                                    <td>{p.reviewDate.split('T')[0]}</td>
                                    <td>{p.rating} / 5</td>
                                    <td><span className={`status-badge ${p.status.toLowerCase()}`}>{p.status}</span></td>
                                </tr>
                            )) : <tr><td colSpan={3} className="text-center text-gray-500 py-4">Chưa có đánh giá nào.</td></tr>}
                        </tbody>
                    </table>
                );
            case 'attendance':
                return (
                    <table className="data-table">
                        <thead>
                            <tr><th>Ngày</th><th>Trạng thái</th></tr>
                        </thead>
                        <tbody>
                            {profile.attendanceRecords.length > 0 ? profile.attendanceRecords.slice(0, 10).map(a => (
                                <tr key={a.id}>
                                    <td>{a.date.split('T')[0]}</td>
                                    <td><span className={`status-badge ${a.status.toLowerCase()}`}>{a.status}</span></td>
                                </tr>
                            )) : <tr><td colSpan={2} className="text-center text-gray-500 py-4">Chưa có dữ liệu chấm công.</td></tr>}
                        </tbody>
                    </table>
                );
            default:
                return null;
        }
    };

    return (
        <div className="modal-overlay" onClick={onClose}>
            <div className="modal-content" onClick={e => e.stopPropagation()} style={{ width: '90%', maxWidth: '1000px', height: '85vh', display: 'flex', flexDirection: 'column', padding: 0 }}>

                {/* Header */}
                <div style={{ padding: '24px', borderBottom: '1px solid var(--border-color)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                    <h2 style={{ margin: 0, fontSize: '20px', fontWeight: 600 }}>Chi tiết Hồ sơ Nhân viên</h2>
                    <button onClick={onClose} style={{ background: 'var(--surface-container)', border: 'none', width: '32px', height: '32px', borderRadius: '50%', cursor: 'pointer' }}>✕</button>
                </div>

                {/* Layout */}
                <div style={{ display: 'flex', flex: 1, overflow: 'hidden' }}>

                    {/* Left Column - Profile Summary */}
                    <div style={{ width: '300px', padding: '24px', borderRight: '1px solid var(--border-color)', backgroundColor: 'var(--surface-color)', overflowY: 'auto' }}>
                        <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', marginBottom: '24px' }}>
                            {profile.avatar ? (
                                <img src={`http://localhost:5000${profile.avatar}`} alt="Avatar" style={{ width: 120, height: 120, borderRadius: '50%', objectFit: 'cover', marginBottom: '16px', border: '4px solid var(--surface-container)' }} />
                            ) : (
                                <div style={{ width: 120, height: 120, borderRadius: '50%', backgroundColor: 'var(--primary-color)', color: 'white', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '48px', marginBottom: '16px', fontWeight: 'bold' }}>
                                    {profile.name.charAt(0)}
                                </div>
                            )}
                            <h3 style={{ margin: 0, fontSize: '18px', fontWeight: 600, color: 'var(--text-primary)', textAlign: 'center' }}>{profile.name}</h3>
                            <p style={{ margin: '4px 0 0', color: 'var(--text-tertiary)', fontSize: '14px' }}>{profile.title}</p>
                            <span className={`status-badge ${(profile.status || 'Active').toLowerCase()}`} style={{ marginTop: '12px' }}>{profile.status || 'Active'}</span>
                        </div>

                        <div style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
                            <div>
                                <span style={{ fontSize: '12px', color: 'var(--text-tertiary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Phòng ban</span>
                                <div style={{ fontSize: '14px', fontWeight: 500, color: 'var(--text-primary)', marginTop: '4px' }}>{profile.department || 'Chưa xếp phòng'}</div>
                            </div>
                            <div>
                                <span style={{ fontSize: '12px', color: 'var(--text-tertiary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Email</span>
                                <div style={{ fontSize: '14px', fontWeight: 500, color: 'var(--text-primary)', marginTop: '4px', wordBreak: 'break-all' }}>{profile.email}</div>
                            </div>
                            <div>
                                <span style={{ fontSize: '12px', color: 'var(--text-tertiary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Số điện thoại</span>
                                <div style={{ fontSize: '14px', fontWeight: 500, color: 'var(--text-primary)', marginTop: '4px' }}>{profile.phone}</div>
                            </div>
                            <div>
                                <span style={{ fontSize: '12px', color: 'var(--text-tertiary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Ngày gia nhập</span>
                                <div style={{ fontSize: '14px', fontWeight: 500, color: 'var(--text-primary)', marginTop: '4px' }}>{new Date(profile.joinDate).toLocaleDateString()}</div>
                            </div>
                        </div>
                    </div>

                    {/* Right Column - Tabs and Content */}
                    <div style={{ flex: 1, display: 'flex', flexDirection: 'column', backgroundColor: 'var(--bg-color)', overflow: 'hidden' }}>

                        {/* Custom Simple Tabs */}
                        <div style={{ display: 'flex', borderBottom: '1px solid var(--border-color)', backgroundColor: 'var(--surface-color)', padding: '0 16px' }}>
                            <button
                                onClick={() => setActiveTab('contracts')}
                                style={{ background: 'none', border: 'none', padding: '16px', fontSize: '14px', fontWeight: 500, cursor: 'pointer', borderBottom: activeTab === 'contracts' ? '2px solid var(--primary-color)' : '2px solid transparent', color: activeTab === 'contracts' ? 'var(--primary-color)' : 'var(--text-secondary)' }}
                            >
                                Hợp đồng ({profile.contracts.length})
                            </button>
                            <button
                                onClick={() => setActiveTab('leaves')}
                                style={{ background: 'none', border: 'none', padding: '16px', fontSize: '14px', fontWeight: 500, cursor: 'pointer', borderBottom: activeTab === 'leaves' ? '2px solid var(--primary-color)' : '2px solid transparent', color: activeTab === 'leaves' ? 'var(--primary-color)' : 'var(--text-secondary)' }}
                            >
                                Nghỉ phép ({profile.leaveRequests.length})
                            </button>
                            <button
                                onClick={() => setActiveTab('perf')}
                                style={{ background: 'none', border: 'none', padding: '16px', fontSize: '14px', fontWeight: 500, cursor: 'pointer', borderBottom: activeTab === 'perf' ? '2px solid var(--primary-color)' : '2px solid transparent', color: activeTab === 'perf' ? 'var(--primary-color)' : 'var(--text-secondary)' }}
                            >
                                Đánh giá ({profile.performanceReviews.length})
                            </button>
                            <button
                                onClick={() => setActiveTab('attendance')}
                                style={{ background: 'none', border: 'none', padding: '16px', fontSize: '14px', fontWeight: 500, cursor: 'pointer', borderBottom: activeTab === 'attendance' ? '2px solid var(--primary-color)' : '2px solid transparent', color: activeTab === 'attendance' ? 'var(--primary-color)' : 'var(--text-secondary)' }}
                            >
                                Chấm công
                            </button>
                        </div>

                        {/* Tab content area */}
                        <div style={{ padding: '24px', overflowY: 'auto', flex: 1 }}>
                            {renderTabContent()}
                        </div>

                    </div>
                </div>

            </div>
        </div>
    );
}
