/*
*
* IAccessor.cs
*
* Copyright 2023 Yuichi Yoshii
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

using System.Data;

namespace DatabaseAccessor {

    public interface IAccessor : IDisposable {
        string DataSource { set; }

        int PortNumber { set; }

        string Tenant { set; }

        string UserId { set; }

        string Password { set; }

        string QueryString { set; }

        void Open();

        ITransaction Begin();

        ICommand CreateCommand();

        void ExecuteNonQuery();

        void ExecuteNonQuery(ICommand command);

        object ExecuteScalar();

        object ExecuteScalar(ICommand command);

        DataSet Fetch();

        DataSet Fetch(ICommand command);

        void Close();
    }
}