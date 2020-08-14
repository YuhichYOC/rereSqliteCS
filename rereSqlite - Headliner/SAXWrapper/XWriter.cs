/*
*
* XWriter.cs
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
using System.Text;

public class XWriter {
    private string directory;

    private string fileName;

    private NodeEntity node;

    public string Directory {
        set => directory = value;
    }

    public string FileName {
        set => fileName = value;
    }

    public NodeEntity Node {
        set => node = value;
    }

    public string WriterSetting {
        set {
            var r = new XReader {Directory = Path.GetDirectoryName(value), FileName = Path.GetFileName(value)};
            r.Parse();
            node.WriterSetting = r.Node;
        }
    }

    public void Write() {
        if (string.IsNullOrEmpty(directory)) throw new ArgumentException(@"Directory is not assigned.");
        if (string.IsNullOrEmpty(fileName)) throw new ArgumentException(@"File is not assigned.");
        using var w = new StreamWriter(directory + @"\" + fileName, false, Encoding.UTF8);
        w.WriteLine(node.ToString());
    }
}