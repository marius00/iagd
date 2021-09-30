if (process.env.NODE_ENV === 'development') {
  // Must use require here as import statements are only allowed
  // to exist at top-level.
  require("preact/debug");
}

import './style/index.css';
import App from './components/App';


export default App;
