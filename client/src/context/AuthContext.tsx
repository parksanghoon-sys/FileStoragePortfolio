import React, { createContext, useState, useContext, ReactNode, useEffect } from 'react';
import axios from 'axios';

interface AuthContextType {
  isAuthenticated: boolean;
  user: { id: number; username: string; email: string } | null;
  token: string | null;
  login: (email: string, password: string) => Promise<boolean>;
  logout: () => void;
  register: (username: string, email: string, password: string) => Promise<boolean>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [isAuthenticated, setIsAuthenticated] = useState<boolean>(false);
  const [user, setUser] = useState<{ id: number; username: string; email: string } | null>(null);
  const [token, setToken] = useState<string | null>(null);

  const API_GATEWAY_URL = 'http://localhost:5000';

  useEffect(() => {
    // Check for token in localStorage on initial load
    const storedToken = localStorage.getItem('accessToken');
    const storedUser = localStorage.getItem('user');
    if (storedToken && storedUser) {
      setToken(storedToken);
      setUser(JSON.parse(storedUser));
      setIsAuthenticated(true);
    }
  }, []);

  const login = async (email: string, password: string): Promise<boolean> => {
    try {
      const response = await axios.post(`${API_GATEWAY_URL}/identity/api/auth/login`, {
        email,
        password,
      });
      const { accessToken, username, email: userEmail } = response.data.data;
      const userId = response.data.data.id; // Assuming ID is returned

      setToken(accessToken);
      setUser({ id: userId, username, email: userEmail });
      setIsAuthenticated(true);
      localStorage.setItem('accessToken', accessToken);
      localStorage.setItem('user', JSON.stringify({ id: userId, username, email: userEmail }));
      return true;
    } catch (error) {
      console.error('Login failed', error);
      return false;
    }
  };

  const logout = () => {
    setToken(null);
    setUser(null);
    setIsAuthenticated(false);
    localStorage.removeItem('accessToken');
    localStorage.removeItem('user');
  };

  const register = async (username: string, email: string, password: string): Promise<boolean> => {
    try {
      await axios.post(`${API_GATEWAY_URL}/identity/api/auth/register`, {
        username,
        email,
        password,
      });
      return true;
    } catch (error) {
      console.error('Registration failed', error);
      return false;
    }
  };

  return (
    <AuthContext.Provider value={{
      isAuthenticated,
      user,
      token,
      login,
      logout,
      register,
    }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
