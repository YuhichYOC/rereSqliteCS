/*
*
* RunningInformations.cs
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

using System;
using System.Windows.Controls;
using System.Windows.Media;

public partial class RunningInformations : Page {
    private AppBehind appBehind;

    public AppBehind AppBehind {
        set {
            appBehind = value;
            FontFamily = new FontFamily(appBehind.FontFamily);
            FontSize = appBehind.FontSize;
        }
    }

    public RunningInformations() {
        InitializeComponent();
    }

    public void AppendInfo(string info, Exception ex) {
        Append(null == ex ? info : info + '\n' + ex.Message + '\n' + ex.StackTrace);
    }

    private void Append(string info) {
        informations.Text = '\n' + DateTime.Now.ToString() + '\n' + info + '\n' + informations.Text;
    }
}