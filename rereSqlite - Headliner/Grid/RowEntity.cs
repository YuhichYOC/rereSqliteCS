using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Dynamic;

public class RowEntity : DynamicObject {
    private readonly Dictionary<string, object> data = new Dictionary<string, object>();

    public override bool TryGetMember(GetMemberBinder binder, out object result) {
        return data.TryGetValue(binder.Name, out result);
    }

    public bool TryGetMember(string bindName, out object value) {
        value = data[bindName];
        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value) {
        data[binder.Name] = value;
        return true;
    }

    public bool TrySetMember(string bindName, object value) {
        data[bindName] = value;
        return true;
    }
}