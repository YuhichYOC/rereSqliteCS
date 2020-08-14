using System;
using System.IO;
using System.Text;

public class XWriter {
    private string directory;

    private string fileName;

    private NodeEntity node;

    public string Directory {
        set => directory = value;
    }

    public string FileName {
        set => fileName = value;
    }

    public NodeEntity Node {
        set => node = value;
    }

    public string WriterSetting {
        set {
            var r = new XReader {Directory = Path.GetDirectoryName(value), FileName = Path.GetFileName(value)};
            r.Parse();
            node.WriterSetting = r.Node;
        }
    }

    public void Write() {
        if (string.IsNullOrEmpty(directory)) throw new ArgumentException(@"Directory is not assigned.");
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentException(@"File is not assigned.");
        using var w = new StreamWriter(directory + @"\" + fileName, false, Encoding.UTF8);
        w.WriteLine(node.ToString());
    }
}