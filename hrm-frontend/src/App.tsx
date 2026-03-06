import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { useAuthStore } from './store/authStore';
import MainLayout from './layouts/MainLayout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import Employees from './pages/Employees';
import { Attendance, Payroll, Recruitment, Interview, Performance, Reports } from './pages/CrudPages';
import Settings from './pages/Settings';
import Assets from './pages/Assets';
import Leave from './pages/Leave';
import Profile from './pages/Profile';
import AccountManagement from './pages/AccountManagement';
import Contracts from './pages/Contracts';
import DisciplineRewards from './pages/DisciplineRewards';
import TrainingCourses from './pages/TrainingCourses';
import OrgChart from './pages/OrgChart';

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated } = useAuthStore();
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" replace />;
}

function RoleRoute({ children, allowedRoles }: { children: React.ReactNode, allowedRoles: string[] }) {
  const { user } = useAuthStore();
  return user && allowedRoles.includes(user.role) ? <>{children}</> : <Navigate to="/dashboard" replace />;
}

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/" element={
          <ProtectedRoute>
            <MainLayout />
          </ProtectedRoute>
        }>
          <Route index element={<Navigate to="/dashboard" replace />} />
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="attendance" element={<Attendance />} />
          <Route path="leave" element={<Leave />} />
          <Route path="payroll" element={<Payroll />} />
          <Route path="profile" element={<Profile />} />
          <Route path="orgchart" element={<RoleRoute allowedRoles={['Admin', 'HR', 'Employee']}><OrgChart /></RoleRoute>} />

          {/* Admin Only Routes */}
          <Route path="employees" element={<RoleRoute allowedRoles={['Admin']}><Employees /></RoleRoute>} />
          <Route path="contracts" element={<RoleRoute allowedRoles={['Admin', 'HR']}><Contracts /></RoleRoute>} />
          <Route path="disciplinerewards" element={<RoleRoute allowedRoles={['Admin', 'HR']}><DisciplineRewards /></RoleRoute>} />
          <Route path="trainingcourses" element={<RoleRoute allowedRoles={['Admin', 'HR']}><TrainingCourses /></RoleRoute>} />
          <Route path="recruitment" element={<RoleRoute allowedRoles={['Admin']}><Recruitment /></RoleRoute>} />
          <Route path="interview" element={<RoleRoute allowedRoles={['Admin']}><Interview /></RoleRoute>} />
          <Route path="performance" element={<RoleRoute allowedRoles={['Admin']}><Performance /></RoleRoute>} />
          <Route path="assets" element={<RoleRoute allowedRoles={['Admin']}><Assets /></RoleRoute>} />
          <Route path="accounts" element={<RoleRoute allowedRoles={['Admin']}><AccountManagement /></RoleRoute>} />
          <Route path="reports" element={<RoleRoute allowedRoles={['Admin']}><Reports /></RoleRoute>} />
          <Route path="settings" element={<RoleRoute allowedRoles={['Admin']}><Settings /></RoleRoute>} />
        </Route>
        <Route path="*" element={<Navigate to="/dashboard" replace />} />
      </Routes>
    </BrowserRouter>
  );
}
