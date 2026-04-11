import { h, render } from 'preact';
import App from './components/App';
import './style/index.css';

render(h(App, null), document.getElementById('app') as HTMLElement);
