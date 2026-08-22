import {h} from "preact";
import {PureComponent} from "preact/compat";

interface Props {
  onTrigger: () => void;
  isLoading?: boolean;

  /**
   * How far above the bottom of the document (in pixels) loading should kick in.
   * Defaults to roughly one screen height, so the next batch is already on its way
   * by the time the user reaches the end of the current one.
   */
  threshold?: number;
}

class OnScrollLoader extends PureComponent<Props, object> {
  // Guards against firing the same request on every single scroll event while a load is in flight.
  private triggered = false;
  private lastDocHeight = 0;
  private frame: number | null = null;

  constructor(props: Props) {
    super(props);

    this.handleScroll = this.handleScroll.bind(this);
    this.check = this.check.bind(this);
  }

  private getViewportHeight() {
    return 'innerHeight' in window ? window.innerHeight : document.documentElement.offsetHeight;
  }

  private getDocHeight() {
    const body = document.body;
    const html = document.documentElement;
    return Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
  }

  check() {
    this.frame = null;

    const docHeight = this.getDocHeight();

    // The document grew (new items rendered), so we're allowed to ask for more again.
    if (docHeight !== this.lastDocHeight) {
      this.lastDocHeight = docHeight;
      this.triggered = false;
    }

    if (this.triggered || this.props.isLoading) {
      return;
    }

    const windowHeight = this.getViewportHeight();
    const threshold = this.props.threshold !== undefined ? this.props.threshold : Math.max(600, windowHeight);
    const windowBottom = windowHeight + window.pageYOffset;

    if (windowBottom >= docHeight - threshold) {
      this.triggered = true;
      this.props.onTrigger();
    }
  }

  handleScroll() {
    // Scroll events fire far more often than we need to check; collapse them into one check per frame.
    if (this.frame === null) {
      this.frame = window.requestAnimationFrame(this.check);
    }
  }

  componentDidMount() {
    window.addEventListener('scroll', this.handleScroll, {passive: true} as any);
    window.addEventListener('resize', this.handleScroll);

    // The initial batch may not even fill the screen, in which case nobody would ever scroll.
    this.handleScroll();
  }

  componentDidUpdate(prevProps: Props) {
    if (prevProps.isLoading && !this.props.isLoading) {
      this.triggered = false;
    }

    this.handleScroll();
  }

  componentWillUnmount() {
    window.removeEventListener('scroll', this.handleScroll);
    window.removeEventListener('resize', this.handleScroll);

    if (this.frame !== null) {
      window.cancelAnimationFrame(this.frame);
      this.frame = null;
    }
  }

  render() {
    return null;
  }
}

export default OnScrollLoader;
