import CrudPage from './CrudPages';

export default function Contracts() {
    return (
        <CrudPage
            title="Hợp đồng lao động"
            description="Quản lý hợp đồng của nhân viên"
            endpoint="contracts"
            columns={[
                { key: 'employeeName', label: 'Nhân viên' },
                { key: 'contractType', label: 'Loại hợp đồng' },
                { key: 'startDate', label: 'Ngày bắt đầu' },
                { key: 'endDate', label: 'Ngày kết thúc' },
                { key: 'basicSalary', label: 'Lương cơ bản' },
                { key: 'status', label: 'Trạng thái' }
            ]}
            createFields={[
                { key: 'employeeId', label: 'Nhân viên', type: 'select', source: 'employees' },
                { key: 'contractType', label: 'Loại (Thử việc/Chính thức)', type: 'text' },
                { key: 'startDate', label: 'Ngày bắt đầu', type: 'date' },
                { key: 'endDate', label: 'Ngày kết thúc (Có thể bỏ trống)', type: 'date' },
                { key: 'basicSalary', label: 'Lương cơ bản', type: 'number' },
                { key: 'status', label: 'Trạng thái (Active/Expired)', type: 'text' },
                { key: 'fileUrl', label: 'Đường dẫn file Hợp đồng', type: 'text' }
            ]}
        />
    );
}
