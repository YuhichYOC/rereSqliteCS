/*
*
* FileSystemNodeModel.cs
*
* Copyright 2022 Yuichi Yoshii
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

using System.IO;

namespace OfWindow.Model {

    internal class FileSystemNodeModel {
        public string Name { set; get; } = string.Empty;

        public string Path { set; get; } = string.Empty;

        public bool Described { set; get; } = false;

        public bool IsDirectory { set; get; } = false;

        public bool IsExpanded { set; get; } = false;

        public bool IsSelected { set; get; } = false;

        public FileSystemNodeModel(string path) {
            if (DriveInfo.GetDrives().Any(d => d.Name == path)) {
                Name = path;
            }
            else {
                Name = System.IO.Path.GetFileName(path);
            }

            Path = path;
        }

        public IEnumerable<string> GetDirectories() {
            try {
                return Directory.GetDirectories(Path);
            }
            catch (Exception) {
                return Array.Empty<string>();
            }
        }

        public IEnumerable<string> GetFiles() {
            try {
                return Directory.GetFiles(Path);
            }
            catch (Exception) {
                return Array.Empty<string>();
            }
        }
    }
}