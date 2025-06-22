import React, { useEffect, useState } from 'react';
import axios from '../services/api';

const AccountInfo = () => {
  const [info, setInfo] = useState(null);

  useEffect(() => {
    axios.get('/Swift/GetUserInfo')
      .then(res => setInfo(res.data))
      .catch(() => setInfo(null));
  }, []);

  return (
    <div className="container mt-4">
      <h2>Account Information</h2>
      {info ? (
        <div className="table-responsive mt-3">
          <table className="table table-striped table-hover">
            <thead>
              <tr>
                <th>User Name</th>
                <th>Email</th>
                <th>Country Code</th>
                <th>Balance</th>
                <th>Currency</th>
                <th>IBAN/BIC</th>
              </tr>
            </thead>
            <tbody>
              <tr>
                <td>{info.userName}</td>
                <td>{info.email}</td>
                <td>{info.countryCode}</td>
                <td className="fw-bold">{info.balance.toFixed(2)}</td>
                <td>{info.currency}</td>
                <td>{info.ibanOrBic}</td>
              </tr>
            </tbody>
          </table>
        </div>
      ) : (
        <div>Loading...</div>
      )}
    </div>
  );
};

export default AccountInfo;