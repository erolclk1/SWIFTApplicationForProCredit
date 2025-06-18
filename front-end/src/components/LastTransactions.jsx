import React, { useEffect, useState } from 'react';
import axios from '../services/api';

const LastTransactions = () => {
  const [txs, setTxs] = useState([]);

  useEffect(() => {
    // Replace with your real API endpoint
    axios.get('/api/account/last-transactions')
      .then(res => setTxs(res.data))
      .catch(() => setTxs([]));
  }, []);

  return (
    <div className="container mt-4">
      <h2>Last Transactions</h2>
      <div className="table-responsive">
        <table className="table table-striped table-hover mt-3">
          <thead>
            <tr>
              <th>Date</th>
              <th>Amount</th>
              <th>Currency</th>
              <th>Recipient</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {txs.length === 0 && (
              <tr>
                <td colSpan="5" className="text-center">No transactions found.</td>
              </tr>
            )}
            {txs.map((tx, i) => (
              <tr key={i}>
                <td>{tx.date}</td>
                <td>{tx.amount}</td>
                <td>{tx.currency}</td>
                <td>{tx.recipient}</td>
                <td>{tx.status}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default LastTransactions;
