import * as signalR from '@microsoft/signalr';

  const connection = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:7220/notificationHub')
    .withAutomaticReconnect()
    .build();

  connection
    .start()
    .then(() => console.log('SignalR Connected'))
    .catch((err) => console.error('SignalR Connection Error: ', err));

  export default connection;