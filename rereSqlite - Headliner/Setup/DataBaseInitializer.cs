/*
*
* DataBaseInitializer.cs
*
* Copyright 2020 Yuichi Yoshii
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

using rereSqlite___Headliner.Data;

namespace rereSqlite___Headliner.Setup {
    public class DataBaseInitializer {
        public static void Run() {
            if (!AppBehind.Get.RunInitialize) return;
            new TagMaster().SetUp();
            new BinaryStorage().SetUp();
            new StringStorage().SetUp();
            new BinaryTags().SetUp();
            new StringTags().SetUp();
        }
    }
}