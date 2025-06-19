import React, { useEffect, useState } from 'react';
import axios from '../services/api';

const AccountInfo = () => {
  const [info, setInfo] = useState(null);

  useEffect(() => {
    // Replace with your real API endpoint
    axios.get('/api/account/info')
      .then(res => setInfo(res.data))
      .catch(() => setInfo(null));
  }, []);

  return (
    <div className="container mt-4">
      <h2>Account Information</h2>
      {info ? (
        <div className="card p-4 mt-3">
          <div><b>Account Holder:</b> {info.owner}</div>
          <div><b>Account Number:</b> {info.accountNumber}</div>
          <div><b>Balance:</b> {info.balance} {info.currency}</div>
        </div>
      ) : (
        <div>Loading...</div>
      )}
    </div>
  );
};

export default AccountInfo;
