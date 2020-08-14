using System;
using System.IO;
using System.Xml;

public class XReader {
    private int currentNodeId;

    private int depth;

    private string directory;

    private string fileName;

    public string Directory {
        set => directory = value;
    }

    public string FileName {
        set => fileName = value;
    }

    protected string FullPath {
        get {
            if (string.IsNullOrEmpty(directory)) throw new ArgumentException(@"Directory is not assigned.");
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException(@"File is not assigned.");
            return directory + @"\" + fileName;
        }
    }
    
    public NodeEntity Node { get; private set; }
    
    public XReader() {
        currentNodeId = 1;
        depth = 1;
    }

    public void Parse() {
        using var sr = new StreamReader(FullPath);
        using var reader = XmlReader.Create(sr, new XmlReaderSettings {DtdProcessing = DtdProcessing.Parse});
        Node = new NodeEntity {NodeName = fileName, NodeId = 0, Depth = 0, IsComment = false};
        while (reader.Read()) {
            ParseElement(reader);
            ParseText(reader);
            ParseCDATA(reader);
            ParseEndElement(reader);
            ParseComment(reader);
        }
    }
    
    protected void ParseElement(XmlReader reader) {
        if (XmlNodeType.Element != reader.NodeType) return;

        var nodeName = reader.Name;
        var newNode = new NodeEntity
            {NodeName = nodeName, NodeId = currentNodeId, Depth = depth, IsComment = false};
        currentNodeId++;
        Node.FindTail(depth).AddChild(newNode);
        ParseAttributes(reader, newNode);
        
        if (!reader.IsEmptyElement) depth++;
    }

    protected void ParseText(XmlReader reader) {
        if (XmlNodeType.Text != reader.NodeType) return;

        var value = reader.Value;
        if (!string.IsNullOrEmpty(value)) Node.FindTail(depth).NodeValue = value.Trim();
    }

    protected void ParseCDATA(XmlReader reader) {
        if (XmlNodeType.CDATA != reader.NodeType) return;

        var value = reader.Value;
        if (!string.IsNullOrEmpty(value)) Node.FindTail(depth).NodeValue = value.Trim();
    }

    protected void ParseEndElement(XmlReader reader) {
        if (XmlNodeType.EndElement != reader.NodeType) return;

        depth--;
    }

    protected void ParseComment(XmlReader reader) {
        if (XmlNodeType.Comment != reader.NodeType) return;

        Node.FindTail(depth).AddChild(new NodeEntity {
            NodeName = @"Comment", NodeId = currentNodeId, Depth = depth, NodeValue = reader.Value.Trim(),
            IsComment = true
        });
        currentNodeId++;
    }
    
    private void ParseAttributes(XmlReader reader, NodeEntity currentNode) {
        var iLoopCount = reader.AttributeCount;
        for (var i = 0; iLoopCount > i; ++i) {
            reader.MoveToAttribute(i);
            var attrName = reader.LocalName;
            var attrValue = reader.GetAttribute(attrName);
            currentNode.AddAttr(new AttributeEntity {AttrName = attrName, AttrValue = attrValue});
        }
    }
}