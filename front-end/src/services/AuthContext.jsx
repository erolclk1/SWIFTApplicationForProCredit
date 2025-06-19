import React, { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [token, setTokenState] = useState(null);

  useEffect(() => {
    const storedToken = localStorage.getItem('token');
    if (storedToken) {
      setTokenState(storedToken);
    }
  }, []);

  const setToken = (newToken) => {
    localStorage.setItem('token', newToken);
    setTokenState(newToken);
  };

  const clearToken = () => {
    localStorage.removeItem('token');
    setTokenState(null);
  };

  return (
    <AuthContext.Provider value={{ token, setToken, clearToken }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => useContext(AuthContext);