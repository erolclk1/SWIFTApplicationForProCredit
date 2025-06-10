import React, { useState, useEffect } from 'react';
import axios from '../services/api';
import connection from '../services/signalR';

const TransactionForm = () => {
  const [form, setForm] = useState({
    senderName: '',
    senderIbanOrBic: '',
    receiverName: '',
    receiverIbanOrBic: '',
    amount: '',
    currency: '',
    details: ''
  });

  useEffect(() => {
    const setupSignalR = async () => {
      try {
        if (connection.state === "Disconnected") {
          await connection.start();
          console.log("✅ SignalR Connected in TransactionForm");
        }

        connection.off("ReceiveNotification"); // prevent duplicate handlers
        connection.on("ReceiveNotification", (message) => {
          console.log("📨 Received MT799:", message);
          alert(`📨 Received MT799 message: ${message}`);
        });
      } catch (err) {
        console.error("❌ SignalR setup failed in TransactionForm:", err);
      }
    };

    setupSignalR();

    return () => {
      connection.off("ReceiveNotification");
    };
  }, []);

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const mt103Payload = {
      orderingCustomer: `${form.senderIbanOrBic}\\n${form.senderName}`,
      beneficiaryCustomer: `${form.receiverIbanOrBic}\\n${form.receiverName}`,
      valueDateCurrencyAmount: `${form.currency}${form.amount}`,
      detailsOfCharges: form.details || 'SHA' // Optional fallback
    };

    try {
      await axios.post('/MTSwiftGenerator/SWIFTMT103MessageGenerator', mt103Payload);
      // The alert will come from the SignalR callback above
    } catch (err) {
      console.error("❌ Transaction failed:", err);
      alert("Transaction failed.");
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <h2>Create MT103 Transaction</h2>
      <input name="senderName" placeholder="Sender Name" onChange={handleChange} required />
      <input name="senderIbanOrBic" placeholder="Sender IBAN or BIC" onChange={handleChange} required />
      <input name="receiverName" placeholder="Receiver Name" onChange={handleChange} required />
      <input name="receiverIbanOrBic" placeholder="Receiver IBAN or BIC" onChange={handleChange} required />
      <input name="currency" placeholder="Currency (e.g. EUR)" onChange={handleChange} required />
      <input type="number" name="amount" placeholder="Amount" onChange={handleChange} required />
      <textarea name="details" placeholder="Details (Charges, optional)" onChange={handleChange} />
      <button type="submit">Send</button>
    </form>
  );
};

export default TransactionForm;
