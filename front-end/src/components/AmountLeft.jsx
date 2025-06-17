import React, { useState } from 'react';
import axios from '../services/api';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../services/AuthContext';

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
      setToken(response.data); // Stores in context + localStorage
      navigate('/transaction'); // Redirects after login
    } catch (err) {
      console.error(err);
      alert('Login failed: ' + (err.response?.data || 'Unknown error'));
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Login</h2>
      <input name="email" placeholder="Email" onChange={handleChange} required />
      <input type="password" name="password" placeholder="Password" onChange={handleChange} required />
      <button type="submit">Login</button>
    </form>
  );
};

export default Login;