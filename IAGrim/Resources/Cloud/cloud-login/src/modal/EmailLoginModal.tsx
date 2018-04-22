import * as React from 'react';
import Modal from 'react-modal';
import FaClose from 'react-icons/lib/fa/close';
import './EmailLoginModal.css';
import EmailStage from './EmailStage';
import PincodeStage from './PincodeStage';

interface Props {
  visible: boolean;
  onClose: () => void;
}

enum Stage {
  Email, Code, InvalidCode
}

interface State {
  stage: Stage;
  email?: string;
  errorMessage?: string;
  code?: string;
  token?: string;
}

class EmailLoginModal extends React.Component<Props> {
  state: State;

  constructor(props: Props) {
    super(props);
    this.state = {stage: Stage.Email};
  }

  validateEmail(email: string) {
    var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
    return re.test(String(email).toLowerCase());
  }

  isCodeValid() {
    let re = /^\d+$/;
    return this.state.code !== undefined && this.state.code.length === 9 && re.test(this.state.code);
  }

  renderInvalidCode() {
    return (
      <div>
        <h2>You have entered an invalid verification code</h2>
        <br/>
        <br/>
        <br/>
        <input type="button" value="Close" className="btn btn-primary" onClick={() => this.props.onClose()}/>
      </div>
    );
  }

  onEmailStageComplete(email: string, token: string) {
    this.setState({stage: Stage.Code, email: email, token: token});
  }

  onCodeStageComplete(success: boolean, token?: string) {
    console.log('yay?', success, token);
    if (success) {
      document.location.href = `https://auth.iagd.dreamcrash.org/token/${token}`;
    } else {
      this.setState({stage: Stage.InvalidCode});
    }
  }

  render() {
    let stage = this.state.stage;
    return (
      <div>
        <Modal
          isOpen={true}
          onRequestClose={() => this.props.onClose()}
          contentLabel="Verify your e-mail"
          ariaHideApp={false}
        >
          <div className="email-modal">
            <span className="modal-btn-close" onClick={() => this.props.onClose()}>
              <FaClose/>
            </span>

            {stage === Stage.Email && <EmailStage onCompletion={(email, token) => this.onEmailStageComplete(email, token)} />}
            {stage === Stage.Code && <PincodeStage
              onCompletion={(success: boolean, token?: string) => this.onCodeStageComplete(success, token)}
              email={this.state.email as string}
              token={this.state.token as string}
            />
            }
            {stage === Stage.InvalidCode && this.renderInvalidCode()}
          </div>
        </Modal>
      </div>
    );
  }
}

export default EmailLoginModal;

