import { useEffect, useState } from 'react';
import api from '../api/axiosConfig';
import { PlusIcon, TrashIcon } from '@heroicons/react/24/outline';

interface Employee {
    id: number; name: string; title: string; department: string;
    email: string; phone: string; status: string; avatar: string; joinDate: string;
}

export default function Employees() {
    const [employees, setEmployees] = useState<Employee[]>([]);
    const [loading, setLoading] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [form, setForm] = useState({ name: '', title: '', department: '', email: '', phone: '', avatar: '' });

    const fetchEmployees = () => {
        setLoading(true);
        api.get('/employees?pageSize=50')
            .then(res => { if (res.data.success) setEmployees(res.data.data.items); })
            .finally(() => setLoading(false));
    };

    useEffect(() => { fetchEmployees(); }, []);

    const handleCreate = async () => {
        try {
            await api.post('/employees', { ...form, joinDate: new Date().toISOString() });
            setShowModal(false);
            setForm({ name: '', title: '', department: '', email: '', phone: '', avatar: '' });
            fetchEmployees();
        } catch { }
    };

    const handleDelete = async (id: number) => {
        if (!confirm('Delete this employee?')) return;
        await api.delete(`/employees/${id}`);
        fetchEmployees();
    };

    if (loading) return <div className="loading-spinner"><div className="spinner" /></div>;

    return (
        <>
            <div className="page-header">
                <h1>Employees</h1>
                <button className="btn btn-primary" onClick={() => setShowModal(true)}>
                    <PlusIcon style={{ width: 18, height: 18 }} /> Add Employee
                </button>
            </div>

            <div className="card">
                <div className="card-body" style={{ padding: 0 }}>
                    <table className="data-table">
                        <thead>
                            <tr>
                                <th>Employee</th>
                                <th>Title</th>
                                <th>Department</th>
                                <th>Email</th>
                                <th>Status</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            {employees.map(emp => (
                                <tr key={emp.id}>
                                    <td>
                                        <div className="employee-cell">
                                            <div className="employee-avatar">{emp.name.charAt(0)}</div>
                                            {emp.name}
                                        </div>
                                    </td>
                                    <td>{emp.title}</td>
                                    <td>{emp.department}</td>
                                    <td>{emp.email}</td>
                                    <td><span className={`status-badge ${emp.status.toLowerCase()}`}>{emp.status}</span></td>
                                    <td>
                                        <button className="btn btn-danger btn-sm" onClick={() => handleDelete(emp.id)}>
                                            <TrashIcon style={{ width: 14, height: 14 }} />
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>

            {showModal && (
                <div className="modal-overlay" onClick={() => setShowModal(false)}>
                    <div className="modal-content" onClick={e => e.stopPropagation()}>
                        <div className="modal-header">
                            <h3>Add Employee</h3>
                            <button onClick={() => setShowModal(false)} style={{ background: 'none', border: 'none', fontSize: 20, cursor: 'pointer' }}>✕</button>
                        </div>
                        <div className="modal-body">
                            {['name', 'title', 'department', 'email', 'phone'].map(field => (
                                <div className="form-group" key={field}>
                                    <label>{field.charAt(0).toUpperCase() + field.slice(1)}</label>
                                    <input
                                        value={(form as any)[field]}
                                        onChange={e => setForm({ ...form, [field]: e.target.value })}
                                        placeholder={`Enter ${field}`}
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
