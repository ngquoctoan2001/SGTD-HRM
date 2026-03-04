import { useLocation } from 'react-router-dom';
import {
    MagnifyingGlassIcon,
    BellIcon,
    Cog6ToothIcon,
} from '@heroicons/react/24/outline';

const pageTitles: Record<string, string> = {
    '/dashboard': 'Dashboard',
    '/employees': 'Employees',
    '/attendance': 'Attendance',
    '/leave': 'Leave Management',
    '/payroll': 'Payroll',
    '/performance': 'Performance',
    '/assets': 'Assets',
    '/recruitment': 'Recruitment',
    '/interview': 'Interviews',
    '/reports': 'Reports',
};

export default function Topbar() {
    const location = useLocation();
    const title = pageTitles[location.pathname] || 'Dashboard';

    return (
        <header className="topbar">
            <div className="topbar-search">
                <MagnifyingGlassIcon />
                <input type="text" placeholder="Search employees, payroll, reports..." />
            </div>

            <div className="topbar-right">
                <div className="topbar-icon">
                    <BellIcon />
                </div>
                <div className="topbar-icon">
                    <Cog6ToothIcon />
                </div>
                <div className="topbar-title">
                    <h2>{title}</h2>
                    <span>Overview</span>
                </div>
            </div>
        </header>
    );
}
