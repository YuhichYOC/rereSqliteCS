/*
*
* XReader.cs
*
* Copyright 2018 Yuichi Yoshii
*     吉井雄一 @ 吉井産業  you.65535.kir@gmail.com
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using System;
using System.IO;
using System.Xml;

public class XReader {
    private int currentNodeId;

    private int depth;

    private string directory;

    private string fileName;

    public XReader() {
        currentNodeId = 1;
        depth = 1;
    }

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