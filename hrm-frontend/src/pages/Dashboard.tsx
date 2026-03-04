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
}

export default function Dashboard() {
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
    if (!data) return <p>Failed to load dashboard data.</p>;

    const kpis = [
        { label: 'Total Employees', value: data.totalEmployees, badge: '+2.4%', up: true, icon: UsersIcon, color: 'blue' },
        { label: 'Monthly Attendance', value: `${data.monthlyAttendancePercentage}%`, badge: '+1.2%', up: true, icon: CalendarDaysIcon, color: 'purple' },
        { label: 'Pending Leave Requests', value: data.pendingLeaveRequests, badge: '-3%', up: false, icon: ClockIcon, color: 'orange' },
        { label: 'Upcoming Interviews', value: data.upcomingInterviewsCount, badge: '+12%', up: true, icon: ChatBubbleLeftRightIcon, color: 'green' },
    ];

    const formatDate = (dateStr: string) => {
        const d = new Date(dateStr);
        return { month: d.toLocaleString('en', { month: 'short' }).toUpperCase(), day: d.getDate() };
    };

    const formatTime = (dateStr: string) => {
        const d = new Date(dateStr);
        return d.toLocaleTimeString('en', { hour: '2-digit', minute: '2-digit', hour12: true });
    };

    return (
        <>
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
                        <h3>Monthly Recruitment Trends</h3>
                        <div style={{ display: 'flex', gap: 16, fontSize: 13 }}>
                            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                                <span style={{ width: 10, height: 10, borderRadius: '50%', background: '#4361ee', display: 'inline-block' }} />
                                Hired
                            </span>
                            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                                <span style={{ width: 10, height: 10, borderRadius: '50%', background: '#e2e8f0', display: 'inline-block' }} />
                                Applications
                            </span>
                        </div>
                    </div>
                    <div className="card-body">
                        <ResponsiveContainer width="100%" height={260}>
                            <LineChart data={data.recruitmentTrend}>
                                <CartesianGrid strokeDasharray="3 3" stroke="#f1f5f9" />
                                <XAxis dataKey="month" tick={{ fontSize: 13, fill: '#94a3b8' }} />
                                <YAxis tick={{ fontSize: 13, fill: '#94a3b8' }} />
                                <Tooltip />
                                <Legend />
                                <Line type="monotone" dataKey="hired" stroke="#4361ee" strokeWidth={2} dot={{ fill: '#4361ee' }} />
                                <Line type="monotone" dataKey="applications" stroke="#cbd5e1" strokeWidth={2} dot={{ fill: '#cbd5e1' }} />
                            </LineChart>
                        </ResponsiveContainer>
                    </div>
                </div>

                {/* Upcoming Interviews */}
                <div className="card">
                    <div className="card-header">
                        <h3>Upcoming Interviews</h3>
                        <span style={{ cursor: 'pointer', fontSize: 18, color: '#94a3b8' }}>⋮</span>
                    </div>
                    <div className="card-body" style={{ padding: '12px 24px' }}>
                        {data.upcomingInterviews.map((iv) => {
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
                            View Interview Calendar
                        </button>
                    </div>
                </div>
            </div>

            {/* Recent Leave Requests */}
            <div className="card">
                <div className="card-header">
                    <h3>Recent Leave Requests</h3>
                    <a href="/leave" className="link-btn">View All</a>
                </div>
                <div className="card-body" style={{ padding: 0 }}>
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Employee</th>
                                <th>Type</th>
                                <th>Duration</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
                            {data.recentLeaveRequests.map((lr) => (
                                <tr key={lr.id}>
                                    <td>
                                        <div className="employee-cell">
                                            <div className="employee-avatar">{lr.employeeName.charAt(0)}</div>
                                            {lr.employeeName}
                                        </div>
                                    </td>
                                    <td>{lr.type} Leave</td>
                                    <td>{lr.duration}</td>
                                    <td><span className={`status-badge ${lr.status.toLowerCase()}`}>{lr.status}</span></td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </>
    );
}
