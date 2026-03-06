import { NavLink, useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import {
    HomeIcon,
    UsersIcon,
    ClockIcon,
    CalendarDaysIcon,
    BanknotesIcon,
    ChartBarIcon,
    BriefcaseIcon,
    ChatBubbleLeftRightIcon,
    CubeIcon,
    DocumentChartBarIcon,
    ArrowRightStartOnRectangleIcon,
    Cog6ToothIcon,
    DocumentTextIcon,
    StarIcon,
    AcademicCapIcon
} from '@heroicons/react/24/outline';

const navItems = [
    { label: 'Bảng điều khiển', path: '/dashboard', icon: HomeIcon },
    { label: 'Sơ đồ tổ chức', path: '/orgchart', icon: UsersIcon, adminOnly: false },
    { label: 'Nhân sự', path: '/employees', icon: UsersIcon, adminOnly: true },
    { label: 'Hợp đồng', path: '/contracts', icon: DocumentTextIcon, adminOnly: true },
    { label: 'Khen thưởng', path: '/disciplinerewards', icon: StarIcon, adminOnly: true },
    { label: 'Đào tạo', path: '/trainingcourses', icon: AcademicCapIcon, adminOnly: true },
    { label: 'Chấm công', path: '/attendance', icon: ClockIcon },
    { label: 'Nghỉ phép', path: '/leave', icon: CalendarDaysIcon },
    { label: 'Phiếu lương', path: '/payroll', icon: BanknotesIcon },
    { label: 'Đánh giá chung', path: '/performance', icon: ChartBarIcon, adminOnly: true },
    { label: 'Tài sản', path: '/assets', icon: CubeIcon, adminOnly: true },
    { label: 'Tuyển dụng', path: '/recruitment', icon: BriefcaseIcon, adminOnly: true },
    { label: 'Phỏng vấn', path: '/interview', icon: ChatBubbleLeftRightIcon, adminOnly: true },
    { label: 'Hệ thống Tài khoản', path: '/accounts', icon: UsersIcon, adminOnly: true },
    { label: 'Báo cáo', path: '/reports', icon: DocumentChartBarIcon, adminOnly: true },
    { label: 'Cấu hình hệ thống', path: '/settings', icon: Cog6ToothIcon, adminOnly: true },
];

export default function Sidebar() {
    const { user, logout } = useAuthStore();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <aside className="sidebar" style={{ background: 'var(--surface)', borderRight: '1px solid var(--border-light)', color: 'var(--text-primary)' }}>
            <div className="sidebar-logo" style={{ borderBottomColor: 'var(--border-light)' }}>
                <div className="sidebar-logo-icon" style={{ background: 'var(--primary-light)', color: 'var(--primary)', borderRadius: '12px' }}>📋</div>
                <div className="sidebar-logo-text">
                    <h1 style={{ color: 'var(--text-primary)' }}>HRM Pro</h1>
                    <span style={{ color: 'var(--text-tertiary)' }}>Phiên bản Doanh nghiệp</span>
                </div>
            </div>

            <nav className="sidebar-nav">
                <div style={{ fontSize: '11px', fontWeight: 600, color: 'var(--text-tertiary)', padding: '12px 14px 4px', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Điều hướng chính</div>
                {navItems.filter(item => !item.adminOnly || user?.role === 'Admin').map((item) => (
                    <NavLink
                        key={item.path}
                        to={item.path}
                        className={({ isActive }) => `sidebar-item ${isActive ? 'active' : ''}`}
                        style={({ isActive }) => ({
                            color: isActive ? 'var(--primary)' : 'var(--text-secondary)',
                            background: isActive ? 'var(--primary-light)' : 'transparent',
                            borderRadius: '100px'
                        })}
                    >
                        {({ isActive }) => (
                            <>
                                <item.icon style={{ color: isActive ? 'var(--primary)' : 'var(--text-tertiary)' }} />
                                {item.label}
                            </>
                        )}
                    </NavLink>
                ))}
            </nav>

            <div className="sidebar-user" style={{ borderTopColor: 'var(--border-light)' }}>
                {user?.avatarUrl ? (
                    <img src={`http://localhost:5000${user.avatarUrl}`} alt="Avatar" className="sidebar-user-avatar" style={{ objectFit: 'cover', width: 32, height: 32, borderRadius: '50%', cursor: 'pointer' }} onClick={() => navigate('/profile')} />
                ) : (
                    <div className="sidebar-user-avatar" style={{ background: 'var(--primary-light)', color: 'var(--primary)', cursor: 'pointer' }} onClick={() => navigate('/profile')}>
                        {user?.name?.charAt(0) || 'U'}
                    </div>
                )}
                <div className="sidebar-user-info" style={{ flex: 1, color: 'var(--text-primary)', cursor: 'pointer' }} onClick={() => navigate('/profile')}>
                    <p style={{ margin: 0 }}>{user?.name || 'User'}</p>
                    <span style={{ color: 'var(--text-tertiary)' }}>{user?.role || 'User'}</span>
                </div>
                <button onClick={handleLogout} style={{ background: 'var(--surface-container)', border: 'none', cursor: 'pointer', color: 'var(--text-secondary)', width: '32px', height: '32px', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <ArrowRightStartOnRectangleIcon style={{ width: 16, height: 16 }} />
                </button>
            </div>
        </aside>
    );
}
