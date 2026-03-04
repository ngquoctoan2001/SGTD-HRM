import { useState, FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuthStore } from '../store/authStore';
import api from '../api/axiosConfig';

export default function Login() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const { login } = useAuthStore();
    const navigate = useNavigate();

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const res = await api.post('/auth/login', { username, password });
            if (res.data.success) {
                login(res.data.data.token, res.data.data.user);
                navigate('/dashboard');
            } else {
                setError(res.data.message || 'Login failed');
            }
        } catch (err: any) {
            setError(err.response?.data?.message || 'Network error');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="login-page">
            <div className="login-card">
                <div style={{ textAlign: 'center', marginBottom: 24 }}>
                    <div style={{
                        width: 56, height: 56, background: '#4361ee', borderRadius: 14,
                        display: 'inline-flex', alignItems: 'center', justifyContent: 'center',
                        fontSize: 28, marginBottom: 16
                    }}>📋</div>
                    <h1>HRM System</h1>
                    <p>Sign in to your account to continue</p>
                </div>

                {error && <div className="login-error">{error}</div>}

                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label>Username</label>
                        <input
                            type="text"
                            value={username}
                            onChange={e => setUsername(e.target.value)}
                            placeholder="Enter your username"
                            required
                        />
                    </div>

                    <div className="form-group">
                        <label>Password</label>
                        <input
                            type="password"
                            value={password}
                            onChange={e => setPassword(e.target.value)}
                            placeholder="Enter your password"
                            required
                        />
                    </div>

                    <button type="submit" className="btn btn-primary" disabled={loading}>
                        {loading ? 'Signing in...' : 'Sign In'}
                    </button>
                </form>

                <p style={{ textAlign: 'center', marginTop: 20, fontSize: 13, color: '#94a3b8' }}>
                    Demo: admin / Admin@123
                </p>
            </div>
        </div>
    );
}
