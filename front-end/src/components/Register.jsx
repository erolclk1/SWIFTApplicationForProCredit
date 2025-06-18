import React, { useState } from 'react';
import axios from '../services/api';
import { useNavigate, Link } from 'react-router-dom';
import Swal from 'sweetalert2';

const Register = () => {
  const [form, setForm] = useState({ email: '', password: '' , countrycode : '' , name : ''});
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    try {
      await axios.post('/Authentication/Register', form);
      Swal.fire({
        title: "Success!",
        text: "Registration complete. Please login.",
        icon: "success",
        confirmButtonText: "OK",
        timer: 3500,
      });
      navigate('/login');
    } catch (err) {
      Swal.fire({
        title: "Registration failed",
        text: err.response?.data || 'Unknown error',
        icon: "error"
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-6 col-lg-5">
          <div className="card shadow rounded-4">
            <div className="card-body p-4">
              <h2 className="mb-4 fw-bold text-center">Register</h2>
              <form onSubmit={handleSubmit}>
                <div className="mb-3">
                  <label className="form-label">Email</label>
                  <input name="email" className="form-control" placeholder="Email" onChange={handleChange} required disabled={loading} />
                </div>
                <div className="mb-3">
                  <label className="form-label">Password</label>
                  <input type="password" name="password" className="form-control" placeholder="Password" onChange={handleChange} required disabled={loading} />
                </div>
                <div className="mb-3">
                  <label className="form-label">CountryCode</label>
                  <input name="countrycode" className="form-control" placeholder="CountryCode" onChange={handleChange} required disabled={loading} />
                </div>
                <div className="mb-3">
                  <label className="form-label">Name</label>
                  <input type="name" name="password" className="form-control" placeholder="Name" onChange={handleChange} required disabled={loading} />
                </div>
                <div className="d-grid gap-2">
                  <button type="submit" className="btn btn-success btn-lg rounded-pill fw-bold" disabled={loading}>
                    {loading ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2"></span>
                        Registering...
                      </>
                    ) : (
                      "Register"
                    )}
                  </button>
                </div>
              </form>
              <div className="text-center mt-3">
                <span>Already have an account? </span>
                <Link to="/login" className="text-decoration-none fw-bold">Login</Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Register;
