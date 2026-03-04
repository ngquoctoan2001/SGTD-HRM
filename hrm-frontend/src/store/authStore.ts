import { create } from 'zustand';

interface User {
    id: number;
    username: string;
    name: string;
    email: string;
    role: string;
    avatarUrl: string;
}

interface AuthState {
    token: string | null;
    user: User | null;
    isAuthenticated: boolean;
    login: (token: string, user: User) => void;
    logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
    token: localStorage.getItem('hrm_token'),
    user: JSON.parse(localStorage.getItem('hrm_user') || 'null'),
    isAuthenticated: !!localStorage.getItem('hrm_token'),

    login: (token, user) => {
        localStorage.setItem('hrm_token', token);
        localStorage.setItem('hrm_user', JSON.stringify(user));
        set({ token, user, isAuthenticated: true });
    },

    logout: () => {
        localStorage.removeItem('hrm_token');
        localStorage.removeItem('hrm_user');
        set({ token: null, user: null, isAuthenticated: false });
    },
}));
