using System.Collections;
using System.Windows.Forms;

namespace SpriteEditor
{
    /// <summary>
    /// Contains extensions for Treenodes (NodeSorter, MoveUp, MoveDown)
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Move a node upward in the treeView.
        /// </summary>
        /// <param name="node">TreeNode to move up.</param>
        public static void MoveUp(this TreeNode node)
        {
            TreeNode parent = node.Parent;
            TreeView view = node.TreeView;
            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(node);
                if (index > 0)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index - 1, node);
                }
            }
            else if (node.TreeView.Nodes.Contains(node))
            {
                int index = view.Nodes.IndexOf(node);
                if (index > 0)
                {
                    view.Nodes.RemoveAt(index);
                    view.Nodes.Insert(index - 1, node);
                }
            }
        }

        /// <summary>
        /// Move a node downward in the treeView.
        /// </summary>
        /// <param name="node">TreeNode to move down.</param>
        public static void MoveDown(this TreeNode node)
        {
            TreeNode parent = node.Parent;
            TreeView view = node.TreeView;
            if (parent != null)
            {
                int index = parent.Nodes.IndexOf(node);
                if (index < parent.Nodes.Count - 1)
                {
                    parent.Nodes.RemoveAt(index);
                    parent.Nodes.Insert(index + 1, node);
                }
            }
            else if (view != null && view.Nodes.Contains(node))
            {
                int index = view.Nodes.IndexOf(node);
                if (index < view.Nodes.Count - 1)
                {
                    view.Nodes.RemoveAt(index);
                    view.Nodes.Insert(index + 1, node);
                }
            }
        }
    }

    /// <summary>
    /// Custom node sorter to correctly sequence nodes in treeView
    /// </summary>
    public class NodeSorter : IComparer
    {
        /// <summary>
        /// Custom compare function that follows IComparer specs.
        /// </summary>
        /// <param name="thisObj">Obj1 to compare.</param>
        /// <param name="otherObj">Obj2 to compare to Obj1.</param>
        /// <returns>-1 if Obj1 is larger, 0 if they are equal, 1 if Obj2 is larger</returns>
        public int Compare(object thisObj, object otherObj)
        {
            TreeNode thisNode = thisObj as TreeNode;
            TreeNode otherNode = otherObj as TreeNode;

            if (thisNode.Tag.Equals("Parameter") || thisNode.Tag.Equals("Value"))
            {
                return 0;
            }

            return thisNode.Text.CompareTo(otherNode.Text);
        }
    }
}
