import * as React from 'react';
import * as Guid from 'guid';
import * as _ from 'lodash';

interface TreeDataNode {
  name: string;
  bitmap: string;
  record: string;
  cost: TreeDataNode[];

  numCraftable: number;
  numOwned: number;
  numRequired: number;
  isComplete: boolean;
}

export interface JsTreeNode {
  label: string;
  value: string;
  children: JsTreeNode[];
  icon: JSX.Element;
  checked: boolean;
  pureLabel: string; // This is just code smell at this point. Need to make a separate tree for the 'missing items' list. -- perhaps generated IDs via a function?
  missing: number; // TODO: This really doesn't belong here.
}

interface Tree {
  rootNode: JsTreeNode;
  checked: string[];
  expanded: string[];
}

interface RequiredItemList {
  label: string;
  icon: JSX.Element;
  sum: number;
}

function gatherChecked(list: string[], node: JsTreeNode, parentChecked: boolean) {
  if (node.checked || parentChecked) {
    list.push(node.value);
  }

  for (let i = 0; i < node.children.length; i++) {
    gatherChecked(list, node.children[i], node.checked || parentChecked);
  }
}

function gatherExpanded(list: string[], node: JsTreeNode) {
  const isExpanded = !node.checked;

  if (isExpanded) {
    list.push(node.value);

    for (let i = 0; i < node.children.length; i++) {
      gatherExpanded(list, node.children[i]);
    }
  }
}

function parseNode(node: TreeDataNode): JsTreeNode {
  return {
    label: `${node.numOwned}/${node.numRequired} ${node.name}`,
    icon: <img src={node.bitmap} />,
    children: node.cost.map(parseNode),
    checked: node.numOwned >= node.numRequired,
    missing: Math.max(0, node.numRequired - node.numOwned),
    pureLabel: node.name,
    value: Guid.raw() // TODO: Make this a representation of where it is in the tree instead?
  };
}

function ParseTree(node: TreeDataNode): Tree {
  const rootNode = parseNode(node);

  let checked = [];
  gatherChecked(checked, rootNode, false);

  let expanded = [];
  gatherExpanded(expanded, rootNode);

  return {
    rootNode: rootNode,
    checked: checked,
    expanded: expanded
  };
}

// tslint:disable-next-line
export function flatMap<T>(obj: T, children: (T) => T[]): T[] {
  let nodes = [obj];

  const deeper = children(obj);
  for (let i = 0; i < deeper.length; i++) {
    const childNode: T = deeper[i];
    nodes = nodes.concat(flatMap(childNode, children));
  }

  return nodes;
}

export function GetTotalSum(rootNode: JsTreeNode, checkedNodes: string[]): RequiredItemList[] {
  const nodes = flatMap(rootNode, (n) => n.children)
    .filter(node => node.children.length === 0)
    .filter(node => checkedNodes.indexOf(node.value) === -1);

  let tmp = _(nodes).groupBy('pureLabel').map((group, key) => {
    return {
      label: key,
      icon: nodes.filter(n => n.pureLabel === key)[0].icon,
      sum: _.sumBy(group, 'missing')
    };

  }).value();

  return tmp;
}

export default ParseTree;
