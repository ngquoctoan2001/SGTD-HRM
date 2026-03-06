import CrudPage from './CrudPages';

export default function DisciplineRewards() {
    return (
        <CrudPage
            title="Khen thưởng & Kỷ luật"
            description="Quản lý quyết định khen thưởng và kỷ luật nhân viên"
            endpoint="disciplinerewards"
            columns={[
                { key: 'employeeName', label: 'Nhân viên' },
                { key: 'type', label: 'Hình thức' },
                { key: 'decisionDate', label: 'Ngày quyết định' },
                { key: 'amount', label: 'Số tiền (VND)' },
                { key: 'reason', label: 'Lý do' },
                { key: 'status', label: 'Trạng thái' }
            ]}
            createFields={[
                { key: 'employeeId', label: 'Nhân viên', type: 'select', source: 'employees' },
                { key: 'type', label: 'Hình thức (Reward/Discipline)', type: 'text' },
                { key: 'decisionDate', label: 'Ngày quyết định', type: 'date' },
                { key: 'amount', label: 'Số tiền (Nhập số)', type: 'number' },
                { key: 'reason', label: 'Lý do cụ thể', type: 'text' },
                { key: 'status', label: 'Trạng thái (Pending/Approved)', type: 'text' }
            ]}
        />
    );
}
