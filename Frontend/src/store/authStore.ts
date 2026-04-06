import { create } from 'zustand';
import { User, getAuth, setAuth, removeAuth } from '@/lib/auth';

interface AuthState {
  user: User | null;
  isAuthenticated: boolean;
  login: (user: User) => void;
  logout: () => void;
  initialize: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  isAuthenticated: false,
  login: (user: User) => {
    setAuth(user);
    set({ user, isAuthenticated: true });
  },
  logout: () => {
    removeAuth();
    set({ user: null, isAuthenticated: false });
  },
  initialize: () => {
    const user = getAuth();
    set({ user, isAuthenticated: !!user });
  },
}));

