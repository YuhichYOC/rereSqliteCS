using System.Collections.Generic;
using System.Linq;

public class NodeEntity {
    private int newNodeId;

    private int newDepth;

    public string NodeName { get; set; }

    public int NodeId { get; set; }

    public int Depth { get; set; }

    public string NodeValue { get; set; }

    public bool IsComment { get; set; }

    public List<AttributeEntity> AttrList { get; set; }

    public NodeEntity Parent { get; set; }

    public List<NodeEntity> Children { get; set; }

    public NodeEntity Root => null == Parent ? this : Parent.Root;

    public NodeEntity() {
        NodeId = 0;
        Depth = 0;
        IsComment = false;
        AttrList = new List<AttributeEntity>();
        Parent = null;
        Children = new List<NodeEntity>();
        writerSetting = null;
        indentSize = 2;
        newLineAfterOpeningBracket = true;
        newLineAfterClosingBracket = true;
        newLineAfterAttributes = true;
        newLineAfterNodeValue = true;
    }

    public void AddAttr(AttributeEntity arg) {
        AttrList.Add(arg);
    }

    public void AddAttr(string name, string value) {
        AttrList.Add(new AttributeEntity {AttrName = name, AttrValue = value});
    }

    public void AddChild(NodeEntity arg) {
        arg.Parent = this;
        Children.Add(arg);
    }

    public void AddChild(string name) {
        Children.Add(new NodeEntity {NodeName = name, Depth = Depth + 1, IsComment = false});
    }

    public void AddComment() {
        Children.Add(new NodeEntity {NodeName = @"Comment", Depth = Depth + 1, IsComment = true});
    }

    public bool IDExists(int id) {
        return id == Root.NodeId || IDExists(Root, id);
    }

    private bool IDExists(NodeEntity node, int id) {
        return node.Children.Exists(c => id == c.NodeId || IDExists(c, id));
    }

    public int TailID() {
        return TailID(Root);
    }

    private int TailID(NodeEntity node) {
        return 0 < node.Children.Count ? TailID(node.Children[^1]) : node.NodeId;
    }

    private int TailDepth() {
        return TailDepth(Root);
    }

    private int TailDepth(NodeEntity node) {
        return 0 < node.Children.Count ? node.Children.ConvertAll(TailDepth).Max() : node.Depth;
    }

    public bool AttrExists(string name) {
        return AttrList.Exists(a => a.NameEquals(name));
    }

    public string AttrByName(string name) {
        return AttrExists(name) ? AttrList.First(a => a.NameEquals(name)).AttrValue : @"";
    }

    public void RemoveAttrByName(string name) {
        if (AttrExists(name))
            AttrList.RemoveAt(AttrList.FindIndex(a => a.NameEquals(name)));
    }

    public bool NameEquals(string name) {
        return name.Equals(NodeName);
    }

    public NodeEntity Clone() {
        return new NodeEntity {
            NodeName = NodeName,
            NodeId = NodeId,
            Depth = Depth,
            NodeValue = NodeValue,
            IsComment = IsComment,
            Children = Children.Select(c => c.Clone()).ToList(),
            AttrList = AttrList.Select(a => a.Clone()).ToList()
        };
    }

    public NodeEntity CloneWithoutChildren() {
        return new NodeEntity {
            NodeName = NodeName,
            NodeId = NodeId,
            Depth = Depth,
            NodeValue = NodeValue,
            IsComment = IsComment,
            AttrList = AttrList.Select(a => a.Clone()).ToList()
        };
    }

    public void MoveUpByID(int id) {
        var p = FindByID(id).Parent;
        if (null == p) return;
        for (var i = 0; p.Children.Count > i; ++i)
            if (id == p.Children[i].NodeId)
                if (0 < i) {
                    var node = p.Children[i].Clone();
                    p.Children.RemoveAt(i);
                    p.Children.Insert(i - 1, node);
                    Refresh();
                    return;
                }
    }

    public void MoveDownByID(int id) {
        var p = FindByID(id).Parent;
        if (null == p) return;
        for (var i = 0; p.Children.Count > i; ++i)
            if (id == p.Children[i].NodeId)
                if (p.Children.Count - 1 > i) {
                    var node = p.Children[i].Clone();
                    p.Children.RemoveAt(i);
                    p.Children.Insert(i + 1, node);
                    Refresh();
                    return;
                }
    }

    public void MoveByID(int moveFrom, int moveTo) {
        if (moveFrom == moveTo) return;
        var nf = FindByID(moveFrom);
        if (null == nf) return;
        var pf = nf.Parent;
        if (null == pf) return;
        var pt = FindByID(moveTo).Parent;
        if (null == pt) return;
        nf = nf.Clone();
        for (var j = 0; pf.Children.Count > j; ++j)
            if (moveFrom == pf.Children[j].NodeId) {
                pf.Children.RemoveAt(j);
                break;
            }

        for (var i = 0; pt.Children.Count > i; ++i)
            if (moveTo == pt.Children[i].NodeId) {
                pt.Children.Insert(i, nf);
                break;
            }

        Refresh();
    }

    public void RemoveByID(int id) {
        var p = FindByID(id).Parent;
        if (null == p) return;
        for (var i = 0; p.Children.Count > i; ++i)
            if (id == p.Children[i].NodeId) {
                p.Children.RemoveAt(i);
                Refresh();
                return;
            }
    }

    public void Refresh() {
        newNodeId = 0;
        newDepth = 0;
        Root.NodeId = newNodeId;
        Root.Depth = newDepth;
        Refresh(Root);
    }

    private void Refresh(NodeEntity node) {
        newDepth++;
        foreach (var child in Children) {
            newNodeId++;
            child.NodeId = newNodeId;
            child.Depth = newDepth;
            if (0 == child.Children.Count) continue;
            Refresh(child);
            newDepth--;
        }
    }

    #region -- Find --

    public NodeEntity Find(string tagName) {
        return Children.Find(c => c.NameEquals(tagName));
    }

    public NodeEntity Find(string tagName, string attrName, string attrValue) {
        return Children.Find(c =>
            c.NameEquals(tagName) && c.AttrExists(attrName) && c.AttrByName(attrName).Equals(attrValue));
    }

    public NodeEntity Find(string tagName, string attr1Name, string attr1Value, string attr2Name, string attr2Value) {
        return Children.Find(c =>
            c.NameEquals(tagName) && c.AttrExists(attr1Name) && c.AttrByName(attr1Name).Equals(attr1Value) &&
            c.AttrExists(attr2Name) && c.AttrByName(attr2Name).Equals(attr2Value));
    }

    public NodeEntity FindByID(int id) {
        return FindByID(Root, id);
    }

    private NodeEntity FindByID(NodeEntity node, int id) {
        foreach (var c in node.Children) {
            if (id == c.NodeId) return c;
            var ret = FindByID(c, id);
            if (null != ret) return ret;
        }

        return null;
    }

    public NodeEntity FindTail(int depth) {
        return 1 == depth ? this : FindTail(Children[^1], --depth);
    }

    private NodeEntity FindTail(NodeEntity node, int depth) {
        return 1 == depth ? node : FindTail(node.Children[^1], --depth);
    }

    #region -- Derivative Find --

    /// <summary>
    ///     自分自身の子ノードから type が Dir かつ name が引数に一致する最初のノードを返す
    /// </summary>
    /// <param name="name">
    ///     取得したいタグの name 属性値
    /// </param>
    /// <returns>
    ///     ノード
    /// </returns>
    public NodeEntity Dir(string name) {
        return Find(@"Item", @"type", @"Dir", @"name", name);
    }

    /// <summary>
    ///     自分自身の子ノードから type が File かつ name が引数に一致する最初のノードを返す
    /// </summary>
    /// <param name="name">
    ///     取得したいタグの name 属性値
    /// </param>
    /// <returns>
    ///     ノード
    /// </returns>
    public NodeEntity File(string name) {
        return Find(@"Item", @"type", @"File", @"name", name);
    }

    /// <summary>
    ///     自分自身の子ノードから type が Tag かつ name が引数に一致する最初のノードを返す
    /// </summary>
    /// <param name="name">
    ///     取得したいタグの name 属性値
    /// </param>
    /// <returns>
    ///     ノード
    /// </returns>
    public NodeEntity Tag(string name) {
        return Find(@"Item", @"type", @"Tag", @"name", name);
    }

    /// <summary>
    ///     自分自身の子ノードから type が Attr かつ name が引数に一致する最初のノードを返す
    /// </summary>
    /// <param name="name">
    ///     取得したいタグの name 属性値
    /// </param>
    /// <returns>
    ///     ノード
    /// </returns>
    public NodeEntity Attr(string name) {
        return Find(@"Item", @"type", @"Attr", @"name", name);
    }

    /// <summary>
    ///     自分自身の子ノードから name が引数に一致する最初の Category を返す
    /// </summary>
    /// <param name="name">
    ///     取得したいタグの name 属性値
    /// </param>
    /// <returns>
    ///     ノード
    /// </returns>
    public NodeEntity SubCategory(string name) {
        return Find(@"Category", @"name", name);
    }

    /// <summary>
    ///     自分自身の孫ノードから name が引数に一致する最初の Category を返す
    /// </summary>
    /// <param name="par1Name">
    ///     取得したい子タグの name 属性値
    /// </param>
    /// <param name="par2Name">
    ///     取得したい孫タグの name 属性値
    /// </param>
    /// <returns>
    ///     ノード
    /// </returns>
    public NodeEntity SubCategory(string par1Name, string par2Name) {
        return Find(@"Category", @"name", par1Name).Find(@"Category", @"name", par2Name);
    }

    /// <summary>
    ///     自分自身の曾孫ノードから name が引数に一致する最初の Category を返す
    /// </summary>
    /// <param name="par1Name">
    ///     取得したい子タグの name 属性値
    /// </param>
    /// <param name="par2Name">
    ///     取得したい孫タグの name 属性値
    /// </param>
    /// <param name="par3Name">
    ///     取得したい曾孫タグの name 属性値
    /// </param>
    /// <returns>
    ///     ノード
    /// </returns>
    public NodeEntity SubCategory(string par1Name, string par2Name, string par3Name) {
        return Find(@"Category", @"name", par1Name).Find(@"Category", @"name", par2Name)
            .Find(@"Category", @"name", par3Name);
    }

    /// <summary>
    ///     自分自身の子ノードから name が引数に一致する最初の Command を返す
    /// </summary>
    /// <param name="name">
    ///     取得したい子タグの name 属性値
    /// </param>
    /// <returns>
    ///     ノード
    /// </returns>
    public NodeEntity Command(string name) {
        return Find(@"Command", @"name", name);
    }

    /// <summary>
    ///     自分自身の子ノードから name が引数に一致する最初の Param を返す
    /// </summary>
    /// <param name="name">
    ///     取得したい子タグの name 属性値
    /// </param>
    /// <returns>
    ///     ノード
    /// </returns>
    public NodeEntity Param(string name) {
        return Find(@"Param", @"name", name);
    }

    /// <summary>
    ///     自分自身の子ノードから name が引数に一致する最初の Command から name が引数に一致する最初の Param を返す
    /// </summary>
    /// <param name="par1Name">
    ///     取得したい Command の name 属性値
    /// </param>
    /// <param name="par2Name">
    ///     取得したい Param の name 属性値
    /// </param>
    /// <returns>
    ///     ノード
    /// </returns>
    public NodeEntity Param(string par1Name, string par2Name) {
        return Command(par1Name).Param(par2Name);
    }

    #endregion -- Derivative Find --

    #endregion -- Find --

    #region -- Writer --

    private NodeEntity writerSetting;
    private int indentSize;
    private bool newLineAfterOpeningBracket;
    private bool newLineAfterClosingBracket;
    private bool newLineAfterAttributes;
    private bool newLineAfterNodeValue;

    public NodeEntity WriterSetting {
        set {
            writerSetting = value;
            indentSize = IndentSize;
            newLineAfterOpeningBracket = NewLineAfterOpeningBracket;
            newLineAfterClosingBracket = NewLineAfterClosingBracket;
            newLineAfterAttributes = NewLineAfterAttributes;
            newLineAfterNodeValue = NewLineAfterNodeValue;
            if (0 < Children.Count)
                Children.ForEach(c => c.WriterSetting = value);
        }
    }

    private int IndentSize {
        get {
            if (writerSetting?.Find(@"Writer") == null || writerSetting.Find(@"Writer").Find(@"Setting") == null ||
                writerSetting.Find(@"Writer").Find(@"Setting").Find(@"IndentSize") == null)
                return 2;
            return int.TryParse(writerSetting.Find(@"Writer").Find(@"Setting").Find(@"IndentSize").NodeValue,
                out var ret)
                ? ret
                : 2;
        }
    }

    private bool NewLineAfterOpeningBracket {
        get {
            if (writerSetting?.Find(@"Writer") == null || writerSetting.Find(@"Writer").Find(@"Setting") == null ||
                writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine") == null || writerSetting.Find(@"Writer")
                    .Find(@"Setting").Find(@"NewLine").Find(@"OpeningBracket") == null)
                return true;
            return writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"OpeningBracket").NodeValue
                .Equals(@"YES");
        }
    }

    private bool NewLineAfterClosingBracket {
        get {
            if (writerSetting?.Find(@"Writer") == null || writerSetting.Find(@"Writer").Find(@"Setting") == null ||
                writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine") == null || writerSetting.Find(@"Writer")
                    .Find(@"Setting").Find(@"NewLine").Find(@"ClosingBracket") == null)
                return true;
            return writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"ClosingBracket").NodeValue
                .Equals(@"YES");
        }
    }

    private bool NewLineAfterAttributes {
        get {
            if (writerSetting?.Find(@"Writer") == null || writerSetting.Find(@"Writer").Find(@"Setting") == null ||
                writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine") == null || writerSetting.Find(@"Writer")
                    .Find(@"Setting").Find(@"NewLine").Find(@"AfterAttrElements") == null)
                return true;
            return writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"AfterAttrElements")
                .NodeValue.Equals(@"YES");
        }
    }

    private bool NewLineAfterNodeValue {
        get {
            if (writerSetting?.Find(@"Writer") == null || writerSetting.Find(@"Writer").Find(@"Setting") == null ||
                writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine") == null || writerSetting.Find(@"Writer")
                    .Find(@"Setting").Find(@"NewLine").Find(@"AfterNodeValue") == null)
                return true;
            return writerSetting.Find(@"Writer").Find(@"Setting").Find(@"NewLine").Find(@"AfterNodeValue").NodeValue
                .Equals(@"YES");
        }
    }

    public override string ToString() {
        var ret = string.Empty;
        if (IsComment) {
            ret += ToStringComment();
        }
        else if (Children.Count > 0) {
            ret += ToStringStart();
            if (newLineAfterClosingBracket) ret += "\r\n";
            foreach (var item in Children) ret += item + "\r\n";
            ret += ToStringEnd();
        }
        else if (NodeValue != null && !NodeValue.Equals(string.Empty)) {
            ret += ToStringStart();
            if (newLineAfterClosingBracket) ret += "\r\n";
            ret += Indent(1) + NodeValue;
            if (newLineAfterNodeValue) ret += "\r\n";
            ret += ToStringEnd();
        }
        else {
            ret += ToStringEmpty();
        }

        return ret;
    }

    private string ToStringStart() {
        var ret = Indent(0);
        if (AttrList.Count > 0) {
            ret += @"<" + NodeName;
            if (newLineAfterOpeningBracket) ret += "\r\n";
            foreach (var item in AttrList) {
                ret += Indent(1) + item;
                if (newLineAfterAttributes) ret += "\r\n";
            }

            ret += Indent(1) + @">";
        }
        else {
            ret += @"<" + NodeName + @">";
        }

        return ret;
    }

    private string ToStringEnd() {
        var ret = Indent(0);
        ret += @"</" + NodeName + @">";
        return ret;
    }

    private string ToStringEmpty() {
        var ret = Indent(0);
        if (AttrList.Count > 0) {
            ret += @"<" + NodeName;
            if (newLineAfterOpeningBracket) ret += "\r\n";
            foreach (var item in AttrList) {
                ret += Indent(1) + item;
                if (newLineAfterAttributes) ret += "\r\n";
            }

            ret += Indent(1) + @"/>";
        }
        else {
            ret += @"<" + NodeName + @"/>";
        }

        return ret;
    }

    private string ToStringComment() {
        var ret = Indent(0);
        ret += @"<!-- " + NodeValue + @" -->";
        return ret;
    }

    private string Indent(int plus) {
        var ret = string.Empty;
        for (var i = 0; i < (Depth + plus) * indentSize; i++) ret += @" ";
        return ret;
    }

    #endregion -- Writer --
}