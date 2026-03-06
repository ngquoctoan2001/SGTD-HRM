import CrudPage from './CrudPages';

export default function TrainingCourses() {
    return (
        <CrudPage
            title="Quản lý Đào tạo"
            description="Tổ chức các khoá học nâng cao năng lực nhân sự"
            endpoint="trainingcourses"
            columns={[
                { key: 'courseName', label: 'Tên khoá học' },
                { key: 'instructor', label: 'Giảng viên' },
                { key: 'startDate', label: 'Ngày bắt đầu' },
                { key: 'endDate', label: 'Kết thúc' },
                { key: 'status', label: 'Trạng thái' }
            ]}
            createFields={[
                { key: 'courseName', label: 'Tên khoá học', type: 'text' },
                { key: 'instructor', label: 'Giảng viên phụ trách', type: 'text' },
                { key: 'description', label: 'Mô tả tóm tắt', type: 'text' },
                { key: 'startDate', label: 'Ngày bắt đầu', type: 'date' },
                { key: 'endDate', label: 'Ngày kết thúc', type: 'date' },
                { key: 'status', label: 'Trạng thái (Upcoming/In Progress/Completed)', type: 'text' }
            ]}
        />
    );
}
