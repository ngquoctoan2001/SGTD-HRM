import { useEffect, useState } from 'react';
import api from '../api/axiosConfig';

export default function OrgChart() {
    const [departments, setDepartments] = useState<any[]>([]);
    const [employees, setEmployees] = useState<any[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        Promise.all([
            api.get('/departments?pageSize=100'),
            api.get('/employees?pageSize=500')
        ]).then(([depsRes, empsRes]) => {
            if (depsRes.data.success) setDepartments(depsRes.data.data.items);
            if (empsRes.data.success) setEmployees(empsRes.data.data.items);
            setLoading(false);
        }).catch(() => setLoading(false));
    }, []);

    if (loading) return <div className="loading-spinner"><div className="spinner" /></div>;

    return (
        <div className="fade-in">
            <div className="page-header">
                <div>
                    <h1>Sơ đồ tổ chức</h1>
                    <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginTop: '4px' }}>
                        Cấu trúc nhân sự toàn công ty
                    </p>
                </div>
            </div>

            <div className="card" style={{ overflowX: 'auto', padding: '40px' }}>
                <div className="org-tree">
                    <ul>
                        <li>
                            <div className="org-node root">
                                <h3>HRM Pro</h3>
                                <p>Ban Giám Đốc</p>
                            </div>
                            <ul>
                                {departments.map(dept => {
                                    const deptEmployees = employees.filter(e => e.departmentId === dept.id);
                                    return (
                                        <li key={dept.id}>
                                            <div className="org-node dept">
                                                <h4>{dept.name}</h4>
                                                <span>{deptEmployees.length} nhân sự</span>
                                            </div>
                                            {deptEmployees.length > 0 && (
                                                <ul>
                                                    {deptEmployees.map(emp => (
                                                        <li key={emp.id}>
                                                            <div className="org-node emp">
                                                                <div className="emp-avatar">{emp.name.charAt(0)}</div>
                                                                <div className="emp-info">
                                                                    <strong>{emp.name}</strong>
                                                                    <span>{emp.title}</span>
                                                                </div>
                                                            </div>
                                                        </li>
                                                    ))}
                                                </ul>
                                            )}
                                        </li>
                                    );
                                })}
                            </ul>
                        </li>
                    </ul>
                </div>
            </div>
            <style>{`
                .org-tree ul {
                    padding-top: 20px; position: relative;
                    transition: all 0.5s;
                    display: flex; justify-content: center;
                }
                .org-tree li {
                    float: left; text-align: center;
                    list-style-type: none;
                    position: relative;
                    padding: 20px 5px 0 5px;
                    transition: all 0.5s;
                }
                .org-tree li::before, .org-tree li::after {
                    content: ''; position: absolute; top: 0; right: 50%;
                    border-top: 2px solid #cbd5e1;
                    width: 50%; height: 20px;
                }
                .org-tree li::after {
                    right: auto; left: 50%; border-left: 2px solid #cbd5e1;
                }
                .org-tree li:only-child::after, .org-tree li:only-child::before {
                    display: none;
                }
                .org-tree li:only-child { padding-top: 0; }
                .org-tree li:first-child::before, .org-tree li:last-child::after {
                    border: 0 none;
                }
                .org-tree li:last-child::before {
                    border-right: 2px solid #cbd5e1;
                    border-radius: 0 5px 0 0;
                }
                .org-tree li:first-child::after {
                    border-radius: 5px 0 0 0;
                }
                .org-tree ul ul::before {
                    content: ''; position: absolute; top: 0; left: 50%;
                    border-left: 2px solid #cbd5e1;
                    width: 0; height: 20px;
                }
                .org-node {
                    background: var(--surface);
                    border: 1px solid var(--border-light);
                    padding: 12px 20px;
                    text-decoration: none;
                    color: var(--text-primary);
                    font-family: inherit;
                    font-size: 14px;
                    display: inline-block;
                    border-radius: 12px;
                    transition: all 0.3s;
                    box-shadow: var(--shadow-sm);
                    min-width: 140px;
                }
                .org-node:hover {
                    background: var(--primary-light);
                    border-color: var(--primary);
                    transform: translateY(-2px);
                    box-shadow: var(--shadow-md);
                }
                .org-node.root {
                    background: var(--primary);
                    color: white;
                    border: none;
                }
                .org-node.root h3 { margin: 0 0 4px 0; font-size: 18px; color: white; }
                .org-node.root p { margin: 0; font-size: 13px; opacity: 0.9; }
                
                .org-node.dept {
                    border-top: 4px solid var(--primary);
                }
                .org-node.dept h4 { margin: 0 0 4px 0; font-size: 15px; color: var(--text-primary); }
                .org-node.dept span { font-size: 12px; color: var(--text-tertiary); }
                
                .org-node.emp {
                    display: flex; align-items: center; gap: 12px;
                    padding: 8px 16px; min-width: 180px;
                    text-align: left;
                }
                .emp-avatar {
                    width: 36px; height: 36px; border-radius: 50%;
                    background: var(--surface-container);
                    color: var(--primary); font-weight: 600;
                    display: flex; align-items: center; justify-content: center;
                }
                .emp-info strong { display: block; font-size: 14px; color: var(--text-primary); }
                .emp-info span { display: block; font-size: 12px; color: var(--text-secondary); margin-top: 2px; }
            `}</style>
        </div>
    );
}
