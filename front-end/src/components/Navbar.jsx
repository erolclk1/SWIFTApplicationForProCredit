import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../services/AuthContext';

const Navbar = () => {
  const { clearToken } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    clearToken();
    navigate('/login');
  };

  return (
    <nav className="navbar navbar-expand-lg navbar-light bg-light shadow-sm">
      <div className="container">
        <Link className="navbar-brand fw-bold" to="/home">SWIFT Portal</Link>
        <div>
          <ul className="navbar-nav me-auto mb-2 mb-lg-0">
            <li className="nav-item">
              <Link className="nav-link" to="/account-info">Account Balance</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/transaction">Make a Transaction</Link>
            </li>
            <li className="nav-item">
              <Link className="nav-link" to="/last-transactions">Last Transactions</Link>
            </li>
          </ul>
        </div>
        <button className="btn btn-outline-danger ms-3" onClick={handleLogout}>
          Logout
        </button>
      </div>
    </nav>
  );
};

export default Navbar;
