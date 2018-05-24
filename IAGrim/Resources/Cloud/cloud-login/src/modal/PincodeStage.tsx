import * as React from 'react';
import './EmailLoginModal.css';
import SrcReactCodeInput from 'react-code-input';

interface Props {
  email: string;
  token: string;
  onCompletion: (success: boolean, token?: string) => void;
}

interface State {
  errorMessage?: string;
  code?: string;
}

class PincodeStage extends React.Component<Props> {
  state: State;

  constructor(props: Props) {
    super(props);
    this.state = {};
  }

  isCodeValid() {
    let re = /^\d+$/;
    return this.state.code !== undefined && this.state.code.length === 9 && re.test(this.state.code);
  }

  onValidateCode() {
    let self = this;
    const token = this.props.token;
    const uri = 'https://iagd.azurewebsites.net/api/VerifyEmailToken';
    // const uri = 'http://localhost:7071/api/VerifyEmailToken';
    const code = this.state.code as string;
    fetch(`${uri}?token=${token}&code=${code}`, {
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
        if (json.Success !== undefined) {
          this.props.onCompletion(json.Success, json.Token);
        }
        else {
          console.warn('Attempted to authenticate code, but the result status was undefined.');
          this.props.onCompletion(false);
        }
      })
      .catch((error) => {
        console.warn(error);
        self.setState({errorMessage: `${error}`});
      });
  }



  render() {
    let re = /^\d+$/;
    const showNonNumericError = this.state.code !== undefined && !re.test(this.state.code);

    return (
      <div>
        <h2>An E-Mail has been sent to <span className="email-label">{this.props.email}</span> with the verification code</h2>
        <div className="code-input">
          <SrcReactCodeInput
            type="text"
            fields={9}
            onChange={(e) => this.setState({code: e})}
          />
          {showNonNumericError && <div className="alert alert-warning">The code can only consist of numbers</div>}
          <input
            className={!this.isCodeValid() ? 'form-control btn btn-default' : 'form-control btn btn-primary'}
            type="button"
            value="Verify"
            disabled={!this.isCodeValid()}
            onClick={() => this.onValidateCode()}
          />
        </div>
      </div>
    );
  }

}

export default PincodeStage;

