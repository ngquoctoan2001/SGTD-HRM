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
} from '@heroicons/react/24/outline';

const navItems = [
    { label: 'Dashboard', path: '/dashboard', icon: HomeIcon },
    { label: 'Employees', path: '/employees', icon: UsersIcon },
    { label: 'Attendance', path: '/attendance', icon: ClockIcon },
    { label: 'Leave', path: '/leave', icon: CalendarDaysIcon },
    { label: 'Payroll', path: '/payroll', icon: BanknotesIcon },
    { label: 'Performance', path: '/performance', icon: ChartBarIcon },
    { label: 'Assets', path: '/assets', icon: CubeIcon },
    { label: 'Recruitment', path: '/recruitment', icon: BriefcaseIcon },
    { label: 'Interview', path: '/interview', icon: ChatBubbleLeftRightIcon },
    { label: 'Reports', path: '/reports', icon: DocumentChartBarIcon },
];

export default function Sidebar() {
    const { user, logout } = useAuthStore();
    const navigate = useNavigate();

    const handleLogout = () => {
        logout();
        navigate('/login');
    };

    return (
        <aside className="sidebar">
            <div className="sidebar-logo">
                <div className="sidebar-logo-icon">📋</div>
                <div className="sidebar-logo-text">
                    <h1>HRM System</h1>
                    <span>Enterprise Edition</span>
                </div>
            </div>

            <nav className="sidebar-nav">
                {navItems.map((item) => (
                    <NavLink
                        key={item.path}
                        to={item.path}
                        className={({ isActive }) => `sidebar-item ${isActive ? 'active' : ''}`}
                    >
                        <item.icon />
                        {item.label}
                    </NavLink>
                ))}
            </nav>

            <div className="sidebar-user">
                <div className="sidebar-user-avatar">
                    {user?.name?.charAt(0) || 'U'}
                </div>
                <div className="sidebar-user-info" style={{ flex: 1 }}>
                    <p>{user?.name || 'User'}</p>
                    <span>{user?.role || 'User'}</span>
                </div>
                <button onClick={handleLogout} style={{ background: 'none', border: 'none', cursor: 'pointer', color: '#94a3b8' }}>
                    <ArrowRightStartOnRectangleIcon style={{ width: 20, height: 20 }} />
                </button>
            </div>
        </aside>
    );
}
