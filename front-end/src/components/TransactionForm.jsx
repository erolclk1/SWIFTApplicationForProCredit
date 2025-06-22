import React, { useState, useEffect } from 'react';
import axios from '../services/api';
import connection from '../services/signalR';
import Swal from 'sweetalert2';
import 'sweetalert2/dist/sweetalert2.min.css';

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

  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const setupSignalR = async () => {
      try {
        if (connection.state === "Disconnected") {
          await connection.start();
          console.log("✅ SignalR Connected in TransactionForm");
        }

        connection.off("ReceiveNotification"); // prevent duplicate handlers
        connection.on("ReceiveNotification", (message) => {
          const isError = message.narrativeMessage.startsWith("Someting went wrong with the transaction here is the message")
          setLoading(false); // hide spinner if active
          Swal.fire({
            title: isError ? 'Transaction Failed' : 'Transaction Complete!',
            html: `
              <div style="text-align:center; font-weight:bold;">
                ${message.transactionReference}
              </div>
              <div style="margin-top:10px;">
                ${message.narrativeMessage}
              </div>
            `,
            icon: isError ? 'error' : 'success',
            confirmButtonText: 'OK',
            timer: 6000,
          });
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
    setLoading(true);

    const mt103Payload = {
      orderingCustomer: `${form.senderIbanOrBic}\n${form.senderName}`,
      beneficiaryCustomer: `${form.receiverIbanOrBic}\n${form.receiverName}`,
      valueDateCurrencyAmount: `${form.currency}${form.amount}`,
      detailsOfCharges: form.details || 'SHA'
    };

    try {
      await axios.post('/MTSwiftGenerator/SWIFTMT103MessageGenerator', mt103Payload);
      // SignalR will show the result modal when backend notifies
    } catch (err) {
      setLoading(false);
      Swal.fire({
        title: 'Error',
        text: "Transaction failed. Please try again.",
        icon: 'error'
      });
    }
  };

  return (
    <div className="container mt-5">
      <div className="row justify-content-center">
        <div className="col-md-8 col-lg-6">
          <div className="card shadow-lg rounded-4">
            <div className="card-body p-4">
              <h2 className="card-title mb-4 text-center fw-bold">Create MT103 Transaction</h2>
              <form onSubmit={handleSubmit}>

                <div className="mb-3">
                  <label className="form-label">Sender Name</label>
                  <input name="senderName" className="form-control" placeholder="Sender Name" onChange={handleChange} value={form.senderName} required disabled={loading} />
                </div>

                <div className="mb-3">
                  <label className="form-label">Sender IBAN or BIC</label>
                  <input name="senderIbanOrBic" className="form-control" placeholder="Sender IBAN or BIC" onChange={handleChange} value={form.senderIbanOrBic} required disabled={loading} />
                </div>

                <div className="mb-3">
                  <label className="form-label">Receiver Name</label>
                  <input name="receiverName" className="form-control" placeholder="Receiver Name" onChange={handleChange} value={form.receiverName} required disabled={loading} />
                </div>

                <div className="mb-3">
                  <label className="form-label">Receiver IBAN or BIC</label>
                  <input name="receiverIbanOrBic" className="form-control" placeholder="Receiver IBAN or BIC" onChange={handleChange} value={form.receiverIbanOrBic} required disabled={loading} />
                </div>

                <div className="row">
                  <div className="col-md-6 mb-3">
                    <label className="form-label">Currency (e.g. EUR)</label>
                    <input name="currency" className="form-control" placeholder="Currency" onChange={handleChange} value={form.currency} required disabled={loading} />
                  </div>
                  <div className="col-md-6 mb-3">
                    <label className="form-label">Amount</label>
                    <input type="number" name="amount" className="form-control" placeholder="Amount" onChange={handleChange} value={form.amount} required disabled={loading} />
                  </div>
                </div>

                <div className="mb-3">
                  <label className="form-label">Details (Charges, optional)</label>
                  <textarea name="details" className="form-control" placeholder="Details (Charges, optional)" onChange={handleChange} value={form.details} disabled={loading} />
                </div>

                <div className="d-grid gap-2">
                  <button type="submit" className="btn btn-primary btn-lg rounded-pill fw-bold" disabled={loading}>
                    {loading ? (
                      <>
                        <span className="spinner-border spinner-border-sm me-2"></span>
                        Sending...
                      </>
                    ) : (
                      "Send"
                    )}
                  </button>
                </div>
              </form>
              {loading && (
                <div className="text-center mt-3">
                  <span className="spinner-border text-primary" role="status"></span>
                  <div className="small mt-2 text-muted">Waiting for transaction confirmation...</div>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TransactionForm;
