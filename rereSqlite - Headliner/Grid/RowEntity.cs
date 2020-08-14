/*
*
* RowEntity.cs
*
* Copyright 2017 Yuichi Yoshii
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

using System.Collections.Generic;
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