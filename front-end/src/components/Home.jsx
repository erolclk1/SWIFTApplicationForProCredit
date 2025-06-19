import React from 'react';
import { Link } from 'react-router-dom';

const Home = () => (
  <div className="container mt-5 text-center">
    <h2 className="mb-4 fw-bold">Welcome to Your SWIFT Account</h2>
    <div className="row justify-content-center">
      <div className="col-md-4 mb-3">
        <Link to="/account-info" className="btn btn-primary btn-lg w-100 rounded-4 shadow">Visit your account balance</Link>
      </div>
      <div className="col-md-4 mb-3">
        <Link to="/transaction" className="btn btn-success btn-lg w-100 rounded-4 shadow">Make a transaction to a user</Link>
      </div>
      <div className="col-md-4 mb-3">
        <Link to="/last-transactions" className="btn btn-info btn-lg w-100 rounded-4 shadow">Last transactions</Link>
      </div>
    </div>
  </div>
);

export default Home;
