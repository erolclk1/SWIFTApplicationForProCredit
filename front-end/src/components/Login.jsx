import React, { useState } from 'react';
import axios from '../services/api';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../services/AuthContext';
import Swal from 'sweetalert2';

const Login = () => {
  const [form, setForm] = useState({ email: '', password: '' });
  const { setToken } = useAuth();
  const navigate = useNavigate();

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const response = await axios.post('/Authentication/Login', form);
      if (response.status === 204) {
    Swal.fire({
      title: "Login failed",
      text: "No account matches the provided credentials.",
      icon: "warning"
    });
    return;
  }
      setToken(response.data);
      navigate('/transaction');
    } catch (err) {
      Swal.fire({
        title: "Login failed",
        text: err.response?.data || 'Unknown error',
        icon: "error"
      });
    }
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-6 col-lg-5">
          <div className="card shadow rounded-4">
            <div className="card-body p-4">
              <h2 className="mb-4 fw-bold text-center">Login</h2>
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label className="form-label">Email</label>
                  <input type="email" name="email" className="form-control" placeholder="Email" onChange={handleChange} required />
                </div>
                <div className="mb-3">
                  <label className="form-label">Password</label>
                  <input type="password" name="password" className="form-control" placeholder="Password" onChange={handleChange} required />
                </div>
                <div className="d-grid gap-2">
                  <button type="submit" className="btn btn-primary btn-lg rounded-pill fw-bold">Login</button>
                </div>
              </form>
              <div className="text-center mt-3">
                <span>Don't have an account? </span>
                <Link to="/register" className="text-decoration-none fw-bold">Register</Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Login;
