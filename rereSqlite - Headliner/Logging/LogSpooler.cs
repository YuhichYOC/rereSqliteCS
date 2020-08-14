/*
*
* LogSpooler.cs
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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;
using log4net.Config;

public class LogSpooler {
    private readonly List<string> errorLogLines;
    private readonly List<string> warnLogLines;
    private readonly List<string> infoLogLines;

    private int safeTicks;
    private int ticks;
    private Timer timer;

    private enum LogType {
        Error,
        Warn,
        Info
    }

    public int SafeTicks {
        set => safeTicks = value;
    }

    public LogSpooler() {
        var repo = LogManager.GetRepository(Assembly.GetCallingAssembly());
        XmlConfigurator.Configure(repo, new FileInfo(@"./SaveLog.config"));

        errorLogLines = new List<string>();
        warnLogLines = new List<string>();
        infoLogLines = new List<string>();

        ticks = 0;
        safeTicks = 0;
    }

    public void Start() {
        var callback = new TimerCallback(OnUpdate);
        timer = new Timer(callback, null, 1000, 1000);
    }

    private void OnUpdate(object arg) {
        FlushAny(LogType.Error);
        FlushAny(LogType.Warn);
        FlushAny(LogType.Info);
        ticks++;
        if (0 < safeTicks && ticks >= safeTicks) Dispose();
    }

    private void FlushAny(LogType logType) {
        if (LogType.Error == logType && 0 == errorLogLines.Count) return;
        if (LogType.Warn == logType && 0 == warnLogLines.Count) return;
        if (LogType.Info == logType && 0 == infoLogLines.Count) return;

        var message = new StringBuilder();
        message.AppendLine();

        List<string> SubList(List<string> logLines, int toIndex) {
            return logLines.GetRange(0, toIndex);
        }

        StringBuilder AppendedSubList(List<string> appendList, StringBuilder target) {
            appendList.ForEach(row => target.AppendLine(row));
            return target;
        }

        var lastIndex = logType switch {
            LogType.Error => errorLogLines.Count - 1,
            LogType.Warn => warnLogLines.Count - 1,
            _ => infoLogLines.Count - 1
        };

        switch (logType) {
            case LogType.Error:
                LogManager.GetLogger(Assembly.GetCallingAssembly(), @"ErrorLog")
                    .Error(AppendedSubList(SubList(errorLogLines, lastIndex), message).ToString());
                errorLogLines.RemoveRange(0, lastIndex + 1);
                break;
            case LogType.Warn:
                LogManager.GetLogger(Assembly.GetCallingAssembly(), @"WarnLog")
                    .Warn(AppendedSubList(SubList(warnLogLines, lastIndex), message).ToString());
                warnLogLines.RemoveRange(0, lastIndex + 1);
                break;
            default:
                LogManager.GetLogger(Assembly.GetCallingAssembly(), @"InfoLog")
                    .Info(AppendedSubList(SubList(infoLogLines, lastIndex), message).ToString());
                infoLogLines.RemoveRange(0, lastIndex + 1);
                break;
        }

        ticks = 0;
    }

    public void AppendError(string message) {
        try {
            errorLogLines.Add(message);
        }
        catch (Exception ex) {
            HandleAppendException(LogType.Error, message, ex);
        }
    }

    public void AppendError(string message, Exception e) {
        try {
            errorLogLines.Add(message);
            errorLogLines.Add(e.Message);
            errorLogLines.Add(e.StackTrace);
        }
        catch (Exception ex) {
            HandleAppendException(LogType.Error, message, ex);
        }
    }

    public void AppendWarn(string message) {
        try {
            warnLogLines.Add(message);
        }
        catch (Exception ex) {
            HandleAppendException(LogType.Warn, message, ex);
        }
    }

    public void AppendInfo(string message) {
        try {
            infoLogLines.Add(message);
        }
        catch (Exception ex) {
            HandleAppendException(LogType.Info, message, ex);
        }
    }

    private void HandleAppendException(LogType logType, string message, Exception ex) {
        const string loggerName = @"LogOperatorLog";
        var description = @"Exception during " + logType switch {
            LogType.Error => @"AppendError" + '\n',
            LogType.Warn => @"AppendWarn" + '\n',
            _ => @"AppendInfo" + '\n'
        };
        LogManager.GetLogger(Assembly.GetCallingAssembly(), loggerName).Error(description + message, ex);
    }

    public void Dispose() {
        timer.Change(Timeout.Infinite, Timeout.Infinite);
        FlushAny(LogType.Error);
        FlushAny(LogType.Warn);
        FlushAny(LogType.Info);
    }
}