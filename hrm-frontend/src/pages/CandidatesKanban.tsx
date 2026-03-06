import React, { useState, useEffect } from 'react';
import api from '../api/axiosConfig';

interface Candidate {
    id: number;
    name: string;
    email: string;
    phone: string;
    jobPostingId: number;
    jobTitle: string;
    status: string;
    appliedDate: string;
}

const COLUMNS = [
    { id: 'New', title: 'Mới ứng tuyển' },
    { id: 'Interviewing', title: 'Đang phỏng vấn' },
    { id: 'Offered', title: 'Đã Offer' },
    { id: 'Rejected', title: 'Đã Từ chối' }
];

export default function CandidatesKanban() {
    const [candidates, setCandidates] = useState<Candidate[]>([]);
    const [loading, setLoading] = useState(true);

    const fetchCandidates = async () => {
        try {
            const res = await api.get('/candidates?pageSize=100');
            if (res.data.success) {
                setCandidates(res.data.data.items);
            }
        } catch (err) {
            console.error('Failed to fetch candidates', err);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchCandidates();
    }, []);

    const handleDragStart = (e: React.DragEvent, id: number) => {
        e.dataTransfer.setData('candidateId', id.toString());
    };

    const handleDragOver = (e: React.DragEvent) => {
        e.preventDefault();
    };

    const handleDrop = async (e: React.DragEvent, status: string) => {
        e.preventDefault();
        const candidateIdStr = e.dataTransfer.getData('candidateId');
        if (!candidateIdStr) return;

        const candidateId = parseInt(candidateIdStr, 10);
        const candidate = candidates.find(c => c.id === candidateId);

        // Default initial status empty -> New. So if current is same as target, don't do anything.
        const currentStatus = candidate?.status || 'New';
        if (currentStatus === status) return;

        // Optimistically update UI
        setCandidates(prev => prev.map(c => c.id === candidateId ? { ...c, status } : c));

        try {
            await api.patch(`/candidates/${candidateId}/status`, { status });
        } catch (err) {
            console.error('Failed to update status', err);
            // Revert on failure
            fetchCandidates();
        }
    };

    if (loading) return <div className="loading-spinner"><div className="spinner" /></div>;

    return (
        <div className="fade-in" style={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
            <div className="page-header">
                <div>
                    <h1>Bảng Quản lý Ứng viên (Kanban)</h1>
                    <p style={{ color: 'var(--text-secondary)', fontSize: '14px', marginTop: '4px' }}>
                        Kéo thả ứng viên giữa các cột để cập nhật trạng thái tuyển dụng.
                    </p>
                </div>
            </div>

            <div style={{ display: 'flex', gap: '24px', flex: 1, overflowX: 'auto', paddingBottom: '24px' }}>
                {COLUMNS.map(column => {
                    const colCandidates = candidates.filter(c => {
                        const cStatus = c.status || 'New';
                        return cStatus === column.id;
                    });

                    return (
                        <div
                            key={column.id}
                            style={{
                                flex: '0 0 300px',
                                background: 'var(--surface-color)',
                                borderRadius: '12px',
                                border: '1px solid var(--border-color)',
                                display: 'flex',
                                flexDirection: 'column',
                                boxShadow: 'var(--shadow-sm)',
                            }}
                            onDragOver={handleDragOver}
                            onDrop={(e) => handleDrop(e, column.id)}
                        >
                            <div style={{ padding: '16px', borderBottom: '1px solid var(--border-color)', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
                                <h3 style={{ margin: 0, fontSize: '15px', fontWeight: 600 }}>{column.title}</h3>
                                <span style={{ background: 'var(--surface-container)', color: 'var(--text-secondary)', padding: '2px 8px', borderRadius: '12px', fontSize: '12px', fontWeight: 600 }}>
                                    {colCandidates.length}
                                </span>
                            </div>

                            <div style={{ padding: '16px', flex: 1, overflowY: 'auto', display: 'flex', flexDirection: 'column', gap: '12px' }}>
                                {colCandidates.map(c => (
                                    <div
                                        key={c.id}
                                        draggable
                                        onDragStart={(e) => handleDragStart(e, c.id)}
                                        style={{
                                            background: 'var(--bg-color)',
                                            padding: '16px',
                                            borderRadius: '8px',
                                            border: '1px solid var(--border-color)',
                                            cursor: 'grab',
                                            boxShadow: 'var(--shadow-sm)'
                                        }}
                                    >
                                        <div style={{ fontWeight: 600, color: 'var(--text-primary)', marginBottom: '4px' }}>{c.name}</div>
                                        <div style={{ fontSize: '13px', color: 'var(--text-secondary)', marginBottom: '8px' }}>{c.jobTitle}</div>

                                        <div style={{ display: 'flex', flexDirection: 'column', gap: '4px', fontSize: '12px', color: 'var(--text-tertiary)' }}>
                                            <div style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                                                <span>✉️</span> {c.email}
                                            </div>
                                            <div style={{ display: 'flex', alignItems: 'center', gap: '4px' }}>
                                                <span>📞</span> {c.phone}
                                            </div>
                                            <div style={{ display: 'flex', alignItems: 'center', gap: '4px', marginTop: '4px' }}>
                                                <span>📅</span> Ngày nộp: {new Date(c.appliedDate).toLocaleDateString()}
                                            </div>
                                        </div>
                                    </div>
                                ))}
                                {colCandidates.length === 0 && (
                                    <div style={{ textAlign: 'center', padding: '24px 0', color: 'var(--text-tertiary)', fontSize: '13px', border: '1px dashed var(--border-color)', borderRadius: '8px' }}>
                                        Chưa có ứng viên
                                    </div>
                                )}
                            </div>
                        </div>
                    );
                })}
            </div>
        </div>
    );
}
