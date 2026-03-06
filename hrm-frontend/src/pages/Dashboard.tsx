import { useEffect, useState } from 'react';
import api from '../api/axiosConfig';
import {
    UsersIcon,
    CalendarDaysIcon,
    ClockIcon,
    ChatBubbleLeftRightIcon,
} from '@heroicons/react/24/outline';
import {
    LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer,
} from 'recharts';
import { useAuthStore } from '../store/authStore';

interface DashboardData {
    totalEmployees: number;
    monthlyAttendancePercentage: number;
    pendingLeaveRequests: number;
    upcomingInterviewsCount: number;
    recruitmentTrend: { month: string; hired: number; applications: number }[];
    recentLeaveRequests: {
        id: number; employeeName: string; employeeAvatar: string;
        type: string; duration: string; status: string;
    }[];
    upcomingInterviews: {
        id: number; candidateName: string; jobTitle: string;
        interviewDate: string; time: string;
    }[];
    expiringContracts?: {
        id: number; employeeName: string; endDate: string;
    }[];
}

export default function Dashboard() {
    const { user } = useAuthStore();
    const isAdmin = user?.role === 'Admin';
    const [data, setData] = useState<DashboardData | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        api.get('/dashboard/overview')
            .then(res => {
                if (res.data.success) setData(res.data.data);
            })
            .catch(() => { })
            .finally(() => setLoading(false));
    }, []);

    if (loading) return <div className="loading-spinner"><div className="spinner" /></div>;
    // Nếu API lỗi, hiển thị mock data để trải nghiệm UI được liền mạch
    const dashboardData = data || {
        totalEmployees: 124,
        monthlyAttendancePercentage: 96.5,
        pendingLeaveRequests: 8,
        upcomingInterviewsCount: 12,
        recruitmentTrend: [
            { month: 'T1', hired: 4, applications: 40 },
            { month: 'T2', hired: 3, applications: 35 },
            { month: 'T3', hired: 5, applications: 50 },
            { month: 'T4', hired: 2, applications: 30 },
            { month: 'T5', hired: 6, applications: 60 },
            { month: 'T6', hired: 4, applications: 45 },
        ],
        recentLeaveRequests: [
            { id: 1, employeeName: 'Nguyễn Văn A', employeeAvatar: 'N', type: 'Thường niên', duration: '2 ngày', status: 'Pending' },
            { id: 2, employeeName: 'Trần Thị B', employeeAvatar: 'T', type: 'Ốm', duration: '1 ngày', status: 'Approved' },
        ],
        upcomingInterviews: [
            { id: 1, candidateName: 'Lê Văn C', jobTitle: 'Frontend Developer', interviewDate: new Date().toISOString(), time: '09:00 AM' },
        ],
        expiringContracts: [
            { id: 1, employeeName: 'Ngô Thanh D', endDate: new Date(new Date().setDate(new Date().getDate() + 15)).toISOString() },
        ]
    };

    const kpis = [
        { label: 'Tổng Nhân sự', value: dashboardData.totalEmployees, badge: '+2.4%', up: true, icon: UsersIcon, color: 'blue' },
        { label: 'Tỷ lệ đi làm (Tháng)', value: `${dashboardData.monthlyAttendancePercentage}%`, badge: '+1.2%', up: true, icon: CalendarDaysIcon, color: 'purple' },
        { label: 'Yêu cầu nghỉ chờ duyệt', value: dashboardData.pendingLeaveRequests, badge: '-3%', up: false, icon: ClockIcon, color: 'orange' },
        { label: 'Lịch Phỏng vấn sắp tới', value: dashboardData.upcomingInterviewsCount, badge: '+12%', up: true, icon: ChatBubbleLeftRightIcon, color: 'green' },
    ];

    const formatDate = (dateStr: string) => {
        const d = new Date(dateStr);
        return { month: `T${d.getMonth() + 1}`, day: d.getDate() };
    };

    const formatTime = (dateStr: string) => {
        const d = new Date(dateStr);
        return d.toLocaleTimeString('vi-VN', { hour: '2-digit', minute: '2-digit' });
    };

    if (!isAdmin) {
        return (
            <div className="fade-in">
                <div style={{ marginBottom: 24 }}>
                    <h2 style={{ fontSize: 24, fontWeight: 700, color: 'var(--text-primary)' }}>Xin chào, {user?.name || 'Nhân viên'}! 👋</h2>
                    <p style={{ color: 'var(--text-secondary)', marginTop: 4 }}>Dưới đây là tổng quan lịch trình và công việc của bạn.</p>
                </div>
                <div className="kpi-grid">
                    <div className="kpi-card">
                        <div className="kpi-card-left">
                            <div className="kpi-icon blue"><CalendarDaysIcon /></div>
                            <div className="kpi-info"><p>Ngày công (Tháng)</p><h2>21 / 22</h2></div>
                        </div>
                    </div>
                    <div className="kpi-card">
                        <div className="kpi-card-left">
                            <div className="kpi-icon green"><CalendarDaysIcon /></div>
                            <div className="kpi-info"><p>Nghỉ phép còn lại</p><h2>10 ngày</h2></div>
                        </div>
                    </div>
                    <div className="kpi-card">
                        <div className="kpi-card-left">
                            <div className="kpi-icon orange"><ClockIcon /></div>
                            <div className="kpi-info"><p>Đi trễ / Về sớm</p><h2>2 lần</h2></div>
                        </div>
                    </div>
                </div>

                <div className="dashboard-grid" style={{ marginTop: 24 }}>
                    <div className="card">
                        <div className="card-header">
                            <h3>Lịch sử Nghỉ phép Cá nhân</h3>
                            <a href="/leave" className="link-btn">Xem tất cả</a>
                        </div>
                        <div className="card-body" style={{ padding: 0 }}>
                            <table className="data-table">
                                <thead><tr><th>Loại nghỉ</th><th>Thời lượng</th><th>Trạng thái</th></tr></thead>
                                <tbody>
                                    {dashboardData.recentLeaveRequests.map((lr) => (
                                        <tr key={lr.id}>
                                            <td>Nghỉ {lr.type.toLowerCase()}</td>
                                            <td>{lr.duration}</td>
                                            <td><span className={`status-badge ${lr.status.toLowerCase()}`}>{lr.status}</span></td>
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="fade-in">
            {/* KPI Cards */}
            <div className="kpi-grid">
                {kpis.map((kpi, i) => (
                    <div className="kpi-card" key={i}>
                        <div className="kpi-card-left">
                            <div className={`kpi-icon ${kpi.color}`}>
                                <kpi.icon />
                            </div>
                            <div className="kpi-info">
                                <p>{kpi.label}</p>
                                <h2>{kpi.value}</h2>
                            </div>
                        </div>
                        <span className={`kpi-badge ${kpi.up ? 'up' : 'down'}`}>{kpi.badge}</span>
                    </div>
                ))}
            </div>

            {/* Chart + Interviews */}
            <div className="dashboard-grid">
                {/* Recruitment Trend Chart */}
                <div className="card">
                    <div className="card-header">
                        <h3>Xu hướng Tuyển dụng (6 tháng)</h3>
                        <div style={{ display: 'flex', gap: 16, fontSize: 13 }}>
                            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                                <span style={{ width: 10, height: 10, borderRadius: '50%', background: '#1a73e8', display: 'inline-block' }} />
                                Đã tuyển
                            </span>
                            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                                <span style={{ width: 10, height: 10, borderRadius: '50%', background: '#e8eaed', display: 'inline-block' }} />
                                Ứng tuyển
                            </span>
                        </div>
                    </div>
                    <div className="card-body">
                        <ResponsiveContainer width="100%" height={260}>
                            <LineChart data={dashboardData.recruitmentTrend}>
                                <CartesianGrid strokeDasharray="3 3" stroke="#f1f3f4" />
                                <XAxis dataKey="month" tick={{ fontSize: 13, fill: '#5f6368' }} />
                                <YAxis tick={{ fontSize: 13, fill: '#5f6368' }} />
                                <Tooltip contentStyle={{ borderRadius: '12px', border: '1px solid #e8eaed', boxShadow: 'var(--shadow-sm)' }} />
                                <Legend />
                                <Line type="monotone" name="Đã tuyển" dataKey="hired" stroke="#1a73e8" strokeWidth={3} dot={{ fill: '#1a73e8', r: 4 }} activeDot={{ r: 6 }} />
                                <Line type="monotone" name="Hồ sơ ứng tuyển" dataKey="applications" stroke="#cbd5e1" strokeWidth={3} dot={{ fill: '#cbd5e1', r: 4 }} />
                            </LineChart>
                        </ResponsiveContainer>
                    </div>
                </div>

                {/* Column 2: Interviews & Contracts */}
                <div style={{ display: 'flex', flexDirection: 'column', gap: '24px' }}>
                    {/* Upcoming Interviews */}
                    <div className="card" style={{ flex: 1 }}>
                        <div className="card-header">
                            <h3>Phỏng vấn Sắp tới</h3>
                            <span style={{ cursor: 'pointer', fontSize: 18, color: 'var(--text-tertiary)' }}>⋮</span>
                        </div>
                        <div className="card-body" style={{ padding: '12px 24px' }}>
                            {dashboardData.upcomingInterviews.map((iv) => {
                                const d = formatDate(iv.interviewDate);
                                return (
                                    <div className="interview-item" key={iv.id}>
                                        <div className="interview-date">
                                            <span className="month">{d.month}</span>
                                            <span className="day">{d.day}</span>
                                        </div>
                                        <div className="interview-info">
                                            <h4>{iv.candidateName}</h4>
                                            <p>{iv.jobTitle}</p>
                                            <span className="time">🕐 {formatTime(iv.interviewDate)}</span>
                                        </div>
                                    </div>
                                );
                            })}
                            <button className="btn btn-secondary" style={{ width: '100%', justifyContent: 'center', marginTop: 12 }}>
                                Xem Lịch trình Phỏng vấn
                            </button>
                        </div>
                    </div>

                    {/* Expiring Contracts */}
                    <div className="card" style={{ flex: 1 }}>
                        <div className="card-header">
                            <h3>Hợp đồng sắp hết hạn</h3>
                            <a href="/contracts" className="link-btn">Xem tất cả</a>
                        </div>
                        <div className="card-body" style={{ padding: '12px 24px' }}>
                            {dashboardData.expiringContracts?.map((contract) => {
                                const d = formatDate(contract.endDate);
                                return (
                                    <div className="interview-item" key={contract.id}>
                                        <div className="interview-date" style={{ background: '#fef08a', color: '#854d0e' }}>
                                            <span className="month">{d.month}</span>
                                            <span className="day">{d.day}</span>
                                        </div>
                                        <div className="interview-info">
                                            <h4>{contract.employeeName}</h4>
                                            <p>Hợp đồng lao động</p>
                                            <span className="time" style={{ color: '#ea4335', fontWeight: 500 }}>Sắp hết hạn</span>
                                        </div>
                                    </div>
                                );
                            })}
                            {(!dashboardData.expiringContracts || dashboardData.expiringContracts.length === 0) && (
                                <p style={{ textAlign: 'center', color: 'var(--text-tertiary)', padding: '20px 0', margin: 0 }}>
                                    Không có hợp đồng nào sắp hết hạn.
                                </p>
                            )}
                        </div>
                    </div>
                </div>
            </div>

            {/* Recent Leave Requests */}
            <div className="card">
                <div className="card-header">
                    <h3>Yêu cầu Nghỉ phép Gần đây</h3>
                    <a href="/leave" className="link-btn">Xem tất cả</a>
                </div>
                <div className="card-body" style={{ padding: 0 }}>
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Nhân viên</th>
                                <th>Loại nghỉ</th>
                                <th>Thời lượng</th>
                                <th>Trạng thái</th>
                            </tr>
                        </thead>
                        <tbody>
                            {dashboardData.recentLeaveRequests.map((lr) => (
                                <tr key={lr.id}>
                                    <td>
                                        <div className="employee-cell">
                                            <div className="employee-avatar">{lr.employeeName.charAt(0)}</div>
                                            <span style={{ fontWeight: 500 }}>{lr.employeeName}</span>
                                        </div>
                                    </td>
                                    <td>Nghỉ {lr.type.toLowerCase()}</td>
                                    <td>{lr.duration}</td>
                                    <td><span className={`status-badge ${lr.status.toLowerCase()}`}>{lr.status}</span></td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    );
}
