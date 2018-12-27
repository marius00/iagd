import * as React from 'react';
import './EmailLoginModal.css';

interface Props {
  onCompletion: (email: string, token: string) => void;
}

interface State {
  email?: string;
  errorMessage?: string;
}

class EmailStage extends React.Component<Props> {
  state: State;

  constructor(props: Props) {
    super(props);
    this.state = {};
  }

  onSendEmail() {
    const email = this.state.email as string;
    if (this.validateEmail(email)) {
      let self = this;
      const uri = 'https://iagd.azurewebsites.net/api/ValidateEmail';
      // const uri = 'http://localhost:7071/api/ValidateEmail';
      fetch(`${uri}?email=${email}`, {
          method: 'POST',
          headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
          }
        }
      )
        .then((response) => {
          if (!response.ok) {
            console.log(response);
            throw Error(`Got response ${response.status}, ${response.statusText}`);
          }
          return response;
        })
        .then((response) => response.json())
        .then((json) => {
          if (json.Token !== undefined) {
            this.props.onCompletion(email, json.Token);
          } // Microsoft seems completely and utterly unable to help themselves, and may randomly change how json is generated in azure functions.
          else if (json.token !== undefined) {
            this.props.onCompletion(email, json.token);
          }
          else {
            console.warn('Attempted to fetch token for email, but token was undefined.');
          }
        })
        .catch((error) => {
          console.warn(error);
          self.setState({errorMessage: `${error}`});
        });

    } else {
      this.setState({errorMessage: 'This is not a valid e-mail address'});
    }
  }

  onEmailChange(email: string) {
    if (this.state.errorMessage) {
      this.setState({errorMessage: undefined});
    }
    else {
      this.setState({email: email});
    }
  }

  validateEmail(email: string) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
  }

  render() {
    return (
      <div>
        <h2>Please enter your e-mail address</h2>

        <span> A Pin Code will be sent to verify your identity</span>
        <div className="email-form">
          <div className="form-group">
            <input
              className="form-control"
              autoFocus={true}
              type="email"
              placeholder="Your e-mail address"
              required={true}
              max="255"
              onChange={(e) => this.onEmailChange(e.target.value)}
              onKeyPress={(e) => e.key === 'Enter' ? this.onSendEmail() : ''}
            />
            {this.state.errorMessage && <div className="alert alert-warning">{this.state.errorMessage}</div>}
            <input
              className="form-control btn btn-primary"
              type="button"
              value="Send"
              onClick={() => this.onSendEmail()}
            />
          </div>
        </div>
      </div>
    );
  }
}

export default EmailStage;

