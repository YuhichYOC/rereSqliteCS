using System.Windows.Controls;

public class XMLNode : TreeViewItem {
    public NodeEntity Node { get; set; }

    public void Fill() {
        Header = Node.NodeName;
        Name = Node.NodeName;
        Tag = Node.CloneWithoutChildren();
        Node.Children.ForEach(c => Fill(this, c));
    }

    private void Fill(XMLNode arg1, NodeEntity arg2) {
        var add = new XMLNode {Header = arg2.NodeName, Name = arg2.NodeName, Tag = arg2.CloneWithoutChildren()};
        arg2.Children.ForEach(c => Fill(add, c));
        arg1.Items.Add(add);
    }
}