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

using log4net;
using log4net.Config;
using System.Reflection;
using System.Text;

public class LogSpooler : IDisposable {
    private readonly List<string> errorLogLines;
    private readonly List<string> infoLogLines;
    private readonly List<string> warnLogLines;

    private int safeTicks;
    private int ticks;
    private Timer? timer;

    public static LogSpooler Self { get; private set; } = new LogSpooler();

    public LogSpooler() {
        var repo = LogManager.GetRepository(Assembly.GetCallingAssembly());
        XmlConfigurator.Configure(repo, new FileInfo(@"./SaveLog.config"));

        errorLogLines = new List<string>();
        warnLogLines = new List<string>();
        infoLogLines = new List<string>();

        ticks = 0;
        safeTicks = 0;
    }

    public int SafeTicks {
        set => safeTicks = value;
    }

    public void Start() {
        var callback = new TimerCallback(OnUpdate);
        timer = new Timer(callback, null, 1000, 1000);
    }

    private void OnUpdate(object? arg) {
        FlushAny(LogType.ERROR);
        FlushAny(LogType.WARN);
        FlushAny(LogType.INFO);
        ticks++;
        if (0 < safeTicks && ticks >= safeTicks) Dispose();
    }

    private void FlushAny(LogType logType) {
        if (LogType.ERROR == logType && 0 == errorLogLines.Count) return;
        if (LogType.WARN == logType && 0 == warnLogLines.Count) return;
        if (LogType.INFO == logType && 0 == infoLogLines.Count) return;

        var count = logType switch {
            LogType.ERROR => errorLogLines.Count,
            LogType.WARN => warnLogLines.Count,
            _ => infoLogLines.Count
        };

        switch (logType) {
            case LogType.ERROR:
                LogManager.GetLogger(Assembly.GetCallingAssembly(), @"ErrorLog")
                    .Error(errorLogLines.GetRange(0, count).Aggregate(new StringBuilder(), (sb, l) => sb.AppendLine(l)).ToString());
                errorLogLines.RemoveRange(0, count);
                break;

            case LogType.WARN:
                LogManager.GetLogger(Assembly.GetCallingAssembly(), @"WarnLog")
                    .Warn(warnLogLines.GetRange(0, count).Aggregate(new StringBuilder(), (sb, l) => sb.AppendLine(l)).ToString());
                warnLogLines.RemoveRange(0, count);
                break;

            default:
                LogManager.GetLogger(Assembly.GetCallingAssembly(), @"InfoLog")
                    .Info(infoLogLines.GetRange(0, count).Aggregate(new StringBuilder(), (sb, l) => sb.AppendLine(l)).ToString());
                infoLogLines.RemoveRange(0, count);
                break;
        }

        ticks = 0;
    }

    public void AppendError(string message) {
        try {
            errorLogLines.Add(message);
        }
        catch (Exception ex) {
            HandleAppendException(LogType.ERROR, message, ex);
        }
    }

    public void AppendError(string message, Exception e) {
        try {
            errorLogLines.Add(message);
            errorLogLines.Add(e.Message);
            errorLogLines.Add(e.StackTrace ?? string.Empty);
        }
        catch (Exception ex) {
            HandleAppendException(LogType.ERROR, message, ex);
        }
    }

    public void AppendWarn(string message) {
        try {
            warnLogLines.Add(message);
        }
        catch (Exception ex) {
            HandleAppendException(LogType.WARN, message, ex);
        }
    }

    public void AppendInfo(string message) {
        try {
            infoLogLines.Add(message);
        }
        catch (Exception ex) {
            HandleAppendException(LogType.INFO, message, ex);
        }
    }

    private static void HandleAppendException(LogType logType, string message, Exception ex) {
        const string loggerName = @"LogOperatorLog";
        var description = @"Exception during " + logType switch {
            LogType.ERROR => @"AppendError" + '\n',
            LogType.WARN => @"AppendWarn" + '\n',
            _ => @"AppendInfo" + '\n'
        };
        LogManager.GetLogger(Assembly.GetCallingAssembly(), loggerName).Error(description + message, ex);
    }

    public void Dispose() {
        if (timer == null) return;
        timer.Change(Timeout.Infinite, Timeout.Infinite);
        FlushAny(LogType.ERROR);
        FlushAny(LogType.WARN);
        FlushAny(LogType.INFO);
        GC.SuppressFinalize(this);
    }

    private enum LogType {
        ERROR,
        WARN,
        INFO
    }
}