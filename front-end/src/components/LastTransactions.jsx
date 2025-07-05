import React, { useEffect, useState } from 'react';
import axios from '../services/api';

const LastTransactions = () => {
  const [txs, setTxs] = useState([]);

  useEffect(() => {
    axios.get('/Swift/GetTransactionHistory')
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
              <th>Date of transaction</th>
              <th>TransactionID</th>
              <th>Your Name</th>
              <th>Amount</th>
              <th>Currency</th>
              <th>Other Party Name</th>
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
                  <td>{new Date(tx.createdAt).toLocaleString()}</td>
                  <td>{tx.transactionId}</td>
                  <td>{tx.yourName}</td>
                  <td className={tx.amount < 0 ? "text-danger" : "text-success"}>
                    {tx.amount.toFixed(2)}
                  </td>
                  <td>{tx.currency}</td>
                  <td>{tx.receiverName}</td>
                </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
};

export default LastTransactions;
