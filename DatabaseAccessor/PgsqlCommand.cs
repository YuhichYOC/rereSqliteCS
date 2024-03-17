/*
*
* PgsqlCommand.cs
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

using Npgsql;
using NpgsqlTypes;

namespace DatabaseAccessor {

    public class PgsqlCommand : ICommand {
        private readonly NpgsqlCommand command;

        public PgsqlCommand(NpgsqlCommand command) => this.command = command;

        public void AddParameter(string name, object value) => command.Parameters.Add(new NpgsqlParameter(name, value));

        public void AddParameter(string name, int type, object value) {
            command.Parameters.Add(name, (NpgsqlDbType)type);
            command.Parameters[name].Value = value;
        }

        internal NpgsqlCommand Self => command;
    }
}