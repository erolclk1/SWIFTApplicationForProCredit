import React, { useEffect, useState } from 'react';
import { connectSignalR } from '../services/signalR';

const Notifications = () => {
  const [messages, setMessages] = useState([]);

  useEffect(() => {
    const connection = connectSignalR();

    connection.on('ReceiveNotification', (data) => {
      setMessages((prev) => [...prev, data]);
    });

    return () => {
      connection.stop();
    };
  }, []);

  return (
    <div>
      <h2>MT799 Notifications</h2>
      {messages.map((msg, index) => (
        <pre key={index}>{JSON.stringify(msg, null, 2)}</pre>
      ))}
    </div>
  );
};

export default Notifications;
