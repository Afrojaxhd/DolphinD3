﻿using Dolphin.Enum;
using System;
using System.IO;

namespace Dolphin.Service
{
    public class LogService : ILogService
    {
        private readonly Log internalLog;
        private readonly string savePath;

        public LogService(Log internalLog)
        {
            this.internalLog = internalLog;
            this.savePath = "log.txt";
        }

        public event EventHandler<LogEntryEventArgs> EntryAdded;

        public void AddEntry(object origin, string message, LogLevel logLevel = LogLevel.Info, Exception ex = null)
        {
            // TODO: Add the origin to the logmesasge (Filename or something) origin.GetType().Namespace

            var currentTime = DateTime.Now;
            var fullMessage = $"[{currentTime}]---LogLevel: {logLevel}, Type: {origin.GetType().FullName}, Message: {message}";

            if (ex != null)
            {
                fullMessage += $", Exception: {ex}";
                message += $", Exception: {ex}";
            }

            internalLog.Entries.Add(fullMessage);
            if (internalLog.Entries.Count >= 100)
            {
                SaveLog();
                internalLog.Entries.Clear();
            }

            Execute.AndForgetAsync(() => EntryAdded?.Invoke(this, new LogEntryEventArgs { Message = message, LogLevel = logLevel, FullMessage = fullMessage, Time = currentTime, Type = origin.GetType().FullName }));
        }

        public void SaveLog()
        {
            try
            {
                //File.AppendAllLines(savePath, internalLog.Entries);
            }
            catch { } // TODO:
        }
    }
}