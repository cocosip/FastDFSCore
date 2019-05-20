using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace FastDFSCore.Client
{
    public class ScheduleService : IScheduleService
    {
        private readonly ILogger _logger;
        private readonly object _lockObject = new object();
        private readonly Dictionary<string, TimerBasedTask> _taskDict = new Dictionary<string, TimerBasedTask>();

        public ScheduleService(ILoggerFactory loggerFactory, FDFSOption option)
        {
            _logger = loggerFactory.CreateLogger(option.LoggerName);
        }

        public void StartTask(string name, Action action, int dueTime, int period)
        {
            lock (_lockObject)
            {
                if (_taskDict.ContainsKey(name))
                {
                    return;
                }
                var timer = new Timer(TaskCallback, name, Timeout.Infinite, Timeout.Infinite);
                var task = new TimerBasedTask
                {
                    Name = name,
                    Action = action,
                    Timer = timer,
                    DueTime = dueTime,
                    Period = period,
                    Stopped = false
                };
                _taskDict.Add(name, task);
                timer.Change(dueTime, period);
            }
        }
        public void StopTask(string name)
        {
            lock (_lockObject)
            {
                TimerBasedTask task;
                if (_taskDict.TryGetValue(name, out task))
                {
                    task.Stopped = true;
                    task.Timer.Dispose();
                    _taskDict.Remove(name);
                }
            }
        }

        private void TaskCallback(object obj)
        {
            var taskName = (string)obj;
            TimerBasedTask task;
            if (_taskDict.TryGetValue(taskName, out task))
            {
                try
                {
                    if (!task.Stopped)
                    {
                        task.Timer.Change(Timeout.Infinite, Timeout.Infinite);
                        task.Action();
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception ex)
                {
                    throw new Exception($"Task has exception, name: {task.Name}, due: {task.DueTime}, period: {task.Period},error detail:{ex.Message}");
                }
                finally
                {
                    try
                    {
                        if (!task.Stopped)
                        {
                            task.Timer.Change(task.Period, task.Period);
                        }
                    }
                    catch (ObjectDisposedException) { }
                    catch (Exception ex)
                    {
                        throw new Exception($"Timer change has exception, name: {task.Name}, due: {task.DueTime}, period: {task.Period},error detail:{ex.Message}");
                    }
                }
            }
        }

        class TimerBasedTask
        {
            public string Name;
            public Action Action;
            public Timer Timer;
            public int DueTime;
            public int Period;
            public bool Stopped;
        }
    }
}
