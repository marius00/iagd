import * as React from 'react';
import ParseTree, { GetTotalSum, JsTreeNode } from '../../logic/TreeParser';
import CheckboxTree from 'react-checkbox-tree';
import 'react-checkbox-tree/lib/react-checkbox-tree.css';
import { Component } from './types';
import './Tree.css';
import translate from '../../translations/EmbeddedTranslator';

interface Props {
  selectedRecipe: Component;
}

interface InternalState {
  checked: string[];
  expanded: string[];
  nodes: JsTreeNode[];
}
export class Tree extends React.Component<Props, {}> {
  state: InternalState;

  constructor(props: Props) {
    super(props);

    const tree = ParseTree(props.selectedRecipe);
    const nodes = [tree.rootNode];
    this.state = {
      checked: tree.checked,
      expanded: tree.expanded,
      nodes: nodes
    };
  }

  componentWillReceiveProps(props: Props) {
    const tree = ParseTree(props.selectedRecipe);
    const nodes = [tree.rootNode];

    this.setState({
      checked: tree.checked,
      expanded: tree.expanded,
      nodes: nodes
    });
  }

  render() {
    const missingComponents = this.state.nodes.length > 0 ? GetTotalSum(this.state.nodes[0], this.state.checked) : [];

    return (
      <div>
        {this.props.selectedRecipe.name !== '' &&
        <div className="crafting-container">
          <div>
            <h3>{translate('crafting.header.recipeName', this.props.selectedRecipe.name)}</h3>
            <CheckboxTree
              nodes={this.state.nodes}
              checked={this.state.checked}
              expanded={this.state.expanded}
              onCheck={checked => {
                this.setState({
                  checked: checked
                });
              }
            }
              onExpand={expanded => this.setState({ expanded })}
            />
          </div>
          <div id="itemSum">
            <h3>{translate('crafting.header.currentlyLacking')}</h3>
            <ul>
              { missingComponents.map((component) =>
                <li key={'missing-component-' + component.label}>
                  {component.icon} {component.sum}x {component.label}
                </li>
              )}
            </ul>
          </div>
        </div>
        }
        <br />
      </div>
    );
  }
}
