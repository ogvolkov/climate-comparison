import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import { unregister } from './registerServiceWorker';
import 'react-app-polyfill/ie11';

ReactDOM.render(<App />, document.getElementById('root'));
unregister();
