public class AttributeEntity {
    public string AttrName { get; set; }

    public string AttrValue { get; set; }

    public bool NameEquals(string arg) {
        return AttrName.Equals(arg);
    }

    public bool ValueEquals(string arg) {
        return AttrValue.Equals(arg);
    }

    public AttributeEntity Clone() {
        return new AttributeEntity {AttrName = AttrName, AttrValue = AttrValue};
    }

    public override string ToString() {
        return AttrName + @"=" + @"""" + AttrValue + @"""";
    }
}