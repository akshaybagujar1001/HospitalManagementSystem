import Cookies from 'js-cookie';

export interface User {
  userId: number;
  email: string;
  role: string;
  firstName: string;
  lastName: string;
  token: string;
}

export const setAuth = (user: User) => {
  Cookies.set('token', user.token, { expires: 7 });
  Cookies.set('user', JSON.stringify(user), { expires: 7 });
};

export const getAuth = (): User | null => {
  const userStr = Cookies.get('user');
  if (!userStr) return null;
  return JSON.parse(userStr);
};

export const removeAuth = () => {
  Cookies.remove('token');
  Cookies.remove('user');
};

export const isAuthenticated = (): boolean => {
  return !!Cookies.get('token');
};

export const hasRole = (role: string): boolean => {
  const user = getAuth();
  return user?.role === role;
};

