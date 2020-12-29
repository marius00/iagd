import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import OnlineApp from './OnlineApp';
import reportWebVitals from './reportWebVitals';
const getenv = require('getenv');
const buildTarget = getenv.string('REACT_APP_BUILD_TARGET', 'GDIA');

console.log('Build target:', buildTarget, process.env.REACT_APP_BUILD_TARGET);
if (buildTarget === 'ONLINE') {
  const url = getenv.string('REACT_APP_SERVER_URL', 'https://api.iagd.evilsoft.net');
  ReactDOM.render(
    <React.StrictMode>
      <OnlineApp url={url} />
    </React.StrictMode>,
    document.getElementById('root')
  );
} else {
  ReactDOM.render(
    <React.StrictMode>
      <App/>
    </React.StrictMode>,
    document.getElementById('root')
  );
}

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
