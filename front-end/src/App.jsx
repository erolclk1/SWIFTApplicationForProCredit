import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import TransactionForm from './components/TransactionForm';
import Home from './components/Home';
import AccountInfo from './components/AccountInfo';
import LastTransactions from './components/LastTransactions';
import Navbar from './components/Navbar';
import { useAuth } from './services/AuthContext';

const App = () => {
  const { token } = useAuth();

  // Only show navbar if logged in
  return (
    <>
      {token && <Navbar />}
      <Routes>
        <Route path="/login" element={<Login />} />
        <Route path="/register" element={<Register />} />
        {token ? (
          <>
            <Route path="/home" element={<Home />} />
            <Route path="/transaction" element={<TransactionForm />} />
            <Route path="/account-info" element={<AccountInfo />} />
            <Route path="/last-transactions" element={<LastTransactions />} />
            <Route path="/" element={<Navigate to="/home" replace />} />
          </>
        ) : (
          <Route path="*" element={<Navigate to="/login" replace />} />
        )}
      </Routes>
    </>
  );
};

export default App;
