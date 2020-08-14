using System.Windows.Controls;

public class FileSystemTree {
    private FileSystemNode root;
    
    public TreeView OwnTree { get; private set; }

    public void Prepare(TreeView arg) {
        OwnTree = arg;
    }

    public void Fill(string path) {
        root = new FileSystemNode {Header = path, Name = path, FullPath = path};
        root.Fill();
        OwnTree.Items.Add(root);
    }
}