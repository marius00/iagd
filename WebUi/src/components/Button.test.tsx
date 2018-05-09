import * as React from 'react';
import * as ReactDOM from 'react-dom';
import Button from './Button';

it('test doesnt fail on framework stuff', () => {
  const div = document.createElement('div');
  ReactDOM.render(<Button label="Daniel" count={1} onClick={console.debug} />, div);
});
