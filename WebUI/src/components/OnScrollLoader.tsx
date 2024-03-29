import {h} from "preact";
import {PureComponent} from "preact/compat";

interface Props {
  onTrigger: () => void;
}

class OnScrollLoader extends PureComponent<Props, {}> {

  constructor(props: Props) {
    super(props);

    this.handleScroll = this.handleScroll.bind(this);
  }

  handleScroll() {
    const windowHeight = 'innerHeight' in window ? window.innerHeight : document.documentElement.offsetHeight;
    const body = document.body;
    const html = document.documentElement;
    const docHeight = Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight,  html.scrollHeight, html.offsetHeight);
    const windowBottom = windowHeight + window.pageYOffset;
    if (windowBottom >= docHeight) {
      this.props.onTrigger();
    }
  }

  componentDidMount() {
    window.addEventListener('scroll', this.handleScroll);
  }

  componentWillUnmount() {
    window.removeEventListener('scroll', this.handleScroll);
  }

  render() {
    return null;
  }
}

export default OnScrollLoader;
