import { useEffect, useState } from 'react';
import api from '../api/axiosConfig';
import { PlusIcon } from '@heroicons/react/24/outline';

// ==================== Generic CRUD Page Component ====================
interface CrudPageProps {
    title: string;
    endpoint: string;
    columns: { key: string; label: string }[];
    createFields?: { key: string; label: string; type?: string }[];
}

function CrudPage({ title, endpoint, columns, createFields }: CrudPageProps) {
    const [items, setItems] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [form, setForm] = useState<Record<string, string>>({});

    const fetchData = () => {
        setLoading(true);
        api.get(`/${endpoint}?pageSize=50`)
            .then(res => { if (res.data.success) setItems(res.data.data.items); })
            .catch(() => { })
            .finally(() => setLoading(false));
    };

    useEffect(() => { fetchData(); }, []);

    const handleCreate = async () => {
        try {
            await api.post(`/${endpoint}`, form);
            setShowModal(false);
            setForm({});
            fetchData();
        } catch { }
    };

    const handleDelete = async (id: number) => {
        if (!confirm(`Delete this ${title.slice(0, -1)}?`)) return;
        await api.delete(`/${endpoint}/${id}`);
        fetchData();
    };

    if (loading) return <div className="loading-spinner"><div className="spinner" /></div>;

    return (
        <>
            <div className="page-header">
                <h1>{title}</h1>
                {createFields && (
                    <button className="btn btn-primary" onClick={() => setShowModal(true)}>
                        <PlusIcon style={{ width: 18, height: 18 }} /> Add New
                    </button>
                )}
            </div>

            <div className="card">
                <div className="card-body" style={{ padding: 0 }}>
                    <table className="data-table">
                        <thead>
                            <tr>
                                {columns.map(col => <th key={col.key}>{col.label}</th>)}
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {items.length === 0 ? (
                                <tr><td colSpan={columns.length + 1} style={{ textAlign: 'center', padding: 40, color: '#94a3b8' }}>No data found</td></tr>
                            ) : items.map(item => (
                                <tr key={item.id}>
                                    {columns.map(col => (
                                        <td key={col.key}>
                                            {col.key === 'status' ? (
                                                <span className={`status-badge ${(item[col.key] || '').toLowerCase()}`}>{item[col.key]}</span>
                                            ) : item[col.key]}
                                        </td>
                                    ))}
                                    <td>
                                        <button className="btn btn-danger btn-sm" onClick={() => handleDelete(item.id)}>Delete</button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {showModal && createFields && (
                <div className="modal-overlay" onClick={() => setShowModal(false)}>
                    <div className="modal-content" onClick={e => e.stopPropagation()}>
                        <div className="modal-header">
                            <h3>Add {title.slice(0, -1)}</h3>
                            <button onClick={() => setShowModal(false)} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer' }}>✕</button>
                        </div>
                        <div className="modal-body">
                            {createFields.map(field => (
                                <div className="form-group" key={field.key}>
                                    <label>{field.label}</label>
                                    <input
                                        type={field.type || 'text'}
                                        value={form[field.key] || ''}
                                        onChange={e => setForm({ ...form, [field.key]: e.target.value })}
                                        placeholder={`Enter ${field.label.toLowerCase()}`}
                                    />
                                </div>
                            ))}
                        </div>
                        <div className="modal-footer">
                            <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Cancel</button>
                            <button className="btn btn-primary" onClick={handleCreate}>Create</button>
                        </div>
                    </div>
                </div>
            )}
        </>
    );
}

// ==================== Page Exports ====================
export function Attendance() {
    return <CrudPage title="Attendance Records" endpoint="attendance" columns={[
        { key: 'employeeName', label: 'Employee' },
        { key: 'date', label: 'Date' },
        { key: 'checkInTime', label: 'Check In' },
        { key: 'checkOutTime', label: 'Check Out' },
        { key: 'status', label: 'Status' },
    ]} />;
}

export function Leave() {
    return <CrudPage title="Leave Requests" endpoint="leave" columns={[
        { key: 'employeeName', label: 'Employee' },
        { key: 'type', label: 'Type' },
        { key: 'startDate', label: 'Start Date' },
        { key: 'endDate', label: 'End Date' },
        { key: 'status', label: 'Status' },
    ]} createFields={[
        { key: 'employeeId', label: 'Employee ID' },
        { key: 'type', label: 'Type (Annual/Sick/Unpaid)' },
        { key: 'startDate', label: 'Start Date', type: 'date' },
        { key: 'endDate', label: 'End Date', type: 'date' },
        { key: 'reason', label: 'Reason' },
    ]} />;
}

export function Payroll() {
    return <CrudPage title="Payroll Slips" endpoint="payroll" columns={[
        { key: 'employeeName', label: 'Employee' },
        { key: 'monthYear', label: 'Month/Year' },
        { key: 'basicSalary', label: 'Basic Salary' },
        { key: 'netSalary', label: 'Net Salary' },
        { key: 'status', label: 'Status' },
    ]} />;
}

export function Recruitment() {
    return <CrudPage title="Job Postings" endpoint="jobpostings" columns={[
        { key: 'title', label: 'Job Title' },
        { key: 'department', label: 'Department' },
        { key: 'location', label: 'Location' },
        { key: 'salaryRange', label: 'Salary Range' },
        { key: 'status', label: 'Status' },
    ]} createFields={[
        { key: 'title', label: 'Job Title' },
        { key: 'department', label: 'Department' },
        { key: 'location', label: 'Location' },
        { key: 'salaryRange', label: 'Salary Range' },
    ]} />;
}

export function Interview() {
    return <CrudPage title="Interview Schedules" endpoint="interviews" columns={[
        { key: 'candidateName', label: 'Candidate' },
        { key: 'jobTitle', label: 'Job Title' },
        { key: 'interviewType', label: 'Type' },
        { key: 'interviewerName', label: 'Interviewer' },
        { key: 'status', label: 'Status' },
    ]} />;
}

export function Performance() {
    return <CrudPage title="Performance Reviews" endpoint="performance" columns={[
        { key: 'employeeName', label: 'Employee' },
        { key: 'reviewerName', label: 'Reviewer' },
        { key: 'reviewPeriod', label: 'Period' },
        { key: 'rating', label: 'Rating' },
        { key: 'status', label: 'Status' },
    ]} />;
}

export function Assets() {
    return <CrudPage title="Assets" endpoint="assets" columns={[
        { key: 'assetTag', label: 'Asset Tag' },
        { key: 'name', label: 'Name' },
        { key: 'category', label: 'Category' },
        { key: 'status', label: 'Status' },
        { key: 'purchaseDate', label: 'Purchase Date' },
    ]} createFields={[
        { key: 'assetTag', label: 'Asset Tag' },
        { key: 'name', label: 'Name' },
        { key: 'category', label: 'Category (Electronics/Furniture/Vehicle)' },
        { key: 'purchaseDate', label: 'Purchase Date', type: 'date' },
    ]} />;
}

export function Reports() {
    return (
        <div>
            <div className="page-header"><h1>Reports</h1></div>
            <div className="card">
                <div className="card-body" style={{ textAlign: 'center', padding: 60, color: '#94a3b8' }}>
                    <p style={{ fontSize: 48, marginBottom: 16 }}>📊</p>
                    <h3 style={{ color: '#64748b', marginBottom: 8 }}>Reports Module</h3>
                    <p>Coming soon - Export payroll to PDF, generate analytics reports</p>
                </div>
            </div>
        </div>
    );
}
