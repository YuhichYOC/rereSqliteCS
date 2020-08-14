using System.Windows.Controls;

public class XMLTree {
    private XMLNode root;
    
    public TreeView OwnTree { get; private set; }

    public void Prepare(TreeView arg) {
        OwnTree = arg;
    }

    public void Fill(NodeEntity node) {
        root = new XMLNode {Node = node};
        root.Fill();
        OwnTree.Items.Add(root);
    }
}