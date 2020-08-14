using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    private ILog logClient;

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

        List<string> SubList(List<string> logLines, int toIndex) => logLines.GetRange(0, toIndex);

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
                LogManager.GetLogger(Assembly.GetCallingAssembly(), @"ErrorLog").Error(AppendedSubList(SubList(errorLogLines, lastIndex), message).ToString());
                errorLogLines.RemoveRange(0, lastIndex + 1);
                break;
            case LogType.Warn:
                LogManager.GetLogger(Assembly.GetCallingAssembly(), @"WarnLog").Warn(AppendedSubList(SubList(warnLogLines, lastIndex), message).ToString());
                warnLogLines.RemoveRange(0, lastIndex + 1);
                break;
            default:
                LogManager.GetLogger(Assembly.GetCallingAssembly(), @"InfoLog").Info(AppendedSubList(SubList(infoLogLines, lastIndex), message).ToString());
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