using System;

namespace FastDFSCore.Scheduling
{
    /// <summary>定时任务
    /// </summary>
    public interface IScheduleService
    {
        /// <summary>开始定时任务
        /// </summary>
        void StartTask(string name, Action action, int dueTime, int period);

        /// <summary>停止定时任务
        /// </summary>
        void StopTask(string name);
    }
}
