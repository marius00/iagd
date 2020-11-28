import * as React from 'react';
import './NewFeaturePromoter.css';
import { getFeatureSuggestion, markFeatureSuggestionSeen } from '../integration/integration';

// TODO: Improve animation
var unshownFeature = "";
class NewFeaturePromoter extends React.PureComponent<{}, object> {
  onClick() {
    markFeatureSuggestionSeen(unshownFeature)
    // @ts-ignore
    document.querySelectorAll('.new-feature-promoter')[0].style.display = "none";
  }

  render() {
    return <div className="new-feature-promoter" style={{display: "none"}} onClick={() => this.onClick()}>New feature!</div>;
  }

  static Activate() {
    unshownFeature = getFeatureSuggestion(); // "Global"
    if (unshownFeature === '') {
      console.debug("No unknown features");
      return;
    }

    let features = document.querySelectorAll(`[data-feature="${unshownFeature}"]`); // span[feature="setbonus"]
    if (features.length === 0) {
      console.debug(`No elements for feature ${unshownFeature}`);
      return;
    }

    let feature = features[0];
    var pos = getDocumentOffsetPosition(feature);
    console.log(pos);

    if (pos.left > 0 && pos.top > 0) {
      // @ts-ignore
      document.querySelectorAll('.new-feature-promoter')[0].style.top = `${pos.top}px`;

      // @ts-ignore
      document.querySelectorAll('.new-feature-promoter')[0].style.left = `${pos.left}px`;

      // @ts-ignore
      document.querySelectorAll('.new-feature-promoter')[0].style.display = "inline-block";
    } else {
      // @ts-ignore
      document.querySelectorAll('.new-feature-promoter')[0].style.display = "none";
    }
  }
}


function getDocumentOffsetPosition(el: any) {
  var position = {
    top: el.offsetTop,
    left: el.offsetLeft
  };
  if (el.offsetParent) {
    var parentPosition = getDocumentOffsetPosition(el.offsetParent);
    position.top += parentPosition.top;
    position.left += parentPosition.left;
  }
  return position;
}

// document.querySelectorAll('span[data-feature]')
// setbonus
export default NewFeaturePromoter;