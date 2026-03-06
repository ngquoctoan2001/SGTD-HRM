import { useLocation } from 'react-router-dom';
import {
    MagnifyingGlassIcon,
    BellIcon,
    Cog6ToothIcon,
} from '@heroicons/react/24/outline';
import NotificationBell from '../components/NotificationBell';

const pageTitles: Record<string, string> = {
    '/dashboard': 'Bảng điều khiển',
    '/employees': 'Quản lý Nhân sự',
    '/attendance': 'Chấm công',
    '/leave': 'Nghỉ phép',
    '/payroll': 'Phiếu lương',
    '/performance': 'Đánh giá năng lực',
    '/assets': 'Quản lý Tài sản',
    '/recruitment': 'Tuyển dụng',
    '/interview': 'Lịch Phỏng vấn',
    '/accounts': 'Hệ thống Tài khoản',
    '/reports': 'Báo cáo & Thống kê',
    '/profile': 'Hồ sơ Cá nhân',
    '/settings': 'Cài đặt Hệ thống',
};

export default function Topbar() {
    const location = useLocation();
    const title = pageTitles[location.pathname] || 'Bảng điều khiển';

    return (
        <header className="topbar">
            <div className="topbar-search">
                <MagnifyingGlassIcon />
                <input type="text" placeholder="Tìm kiếm nhân viên, phiếu lương..." />
            </div>

            <div className="topbar-right">
                <NotificationBell />
                <div className="topbar-icon" style={{ background: 'var(--surface-container)' }}>
                    <Cog6ToothIcon />
                </div>
                <div className="topbar-title">
                    <h2>{title}</h2>
                    <span>Tổng quan hệ thống</span>
                </div>
            </div>
        </header>
    );
}
