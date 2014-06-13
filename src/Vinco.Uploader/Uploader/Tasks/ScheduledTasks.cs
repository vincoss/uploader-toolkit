using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Vinco.Uploader.Handlers;


namespace Vinco.Uploader.Tasks
{
    public enum SchedulerStatus : byte
    {
        Pending = 0,
        Running = 1,
        Paused = 2
    }

    public class ScheduledTasks : IDisposable
    {
        private const int DefaultSimultaneousUploads = 50;

        private readonly object _pendingLock = new object();
        private readonly object _runningLock = new object();

        private readonly List<ITask> _runningTaskList = new List<ITask>();
        private readonly List<ITask> _pendingTaskList = new List<ITask>();

        private static ScheduledTasks _instance;
        private readonly BackgroundWorker _worker;

        private ScheduledTasks()
        {
            Status = SchedulerStatus.Pending;
            _worker = new BackgroundWorker();
            _worker.DoWork += Worker_DoWork;
            MaxSimultaneousUploads = DefaultSimultaneousUploads;
        }

        #region Public methods

        public void Run()
        {
            if (Status == SchedulerStatus.Pending)
            {
                Status = SchedulerStatus.Running;
                _worker.RunWorkerAsync();
            }
        }

        public void Pause()
        {
            if (Status == SchedulerStatus.Running)
            {
                Status = SchedulerStatus.Paused;
            }
        }

        public void Resume()
        {
            if (Status == SchedulerStatus.Paused)
            {
                Status = SchedulerStatus.Running;
            }
        }

        public void Add(ITask task)
        {
            if (task == null)
            {
                throw new ArgumentNullException("task");
            }
            lock (_pendingLock)
            {
                _pendingTaskList.Add(task);
            }
        }

        public ITask CreateTask(UploadHandlerBase handler, UploadItemBase uploadItem)
        {
            return new Task(handler, uploadItem);
        }

        public void Dispose()
        {
            IDisposable disposable = (IDisposable)_worker;
            if(disposable != null)
            {
                disposable.Dispose();
            }
        }

        #endregion

        #region Private methods

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!_worker.CancellationPending) 
            {
                ITask task = null;
                lock (_pendingLock)
                {
                    if (Status == SchedulerStatus.Running)
                    {
                        if (RunningTaskCount < MaxSimultaneousUploads && PendingTaskCount > 0)
                        {
                            task = _pendingTaskList.FirstOrDefault();
                            if (task != null)
                            {
                                _pendingTaskList.Remove(task);
                                lock (_runningTaskList)
                                {
                                    _runningTaskList.Add(task);
                                }
                            }
                        }
                    }
                }
                if (task != null)
                {
                    Action action = () => TaskCompleted(task);
                    task.Run(action);
                }
            }
        }

        private void TaskCompleted(ITask task)
        {
            lock (_runningLock)
            {
                _runningTaskList.Remove(task);
                task.Dispose();
            }
        } 

        #endregion

        #region Properties

        public int RunningTaskCount
        {
            get
            {
                lock (_runningTaskList)
                {
                    return _runningTaskList.Count;
                }
            }
        }

        public int PendingTaskCount
        {
            get
            {
                lock (_pendingLock)
                {
                    return _pendingTaskList.Count;
                }
            }
        }

        public int MaxSimultaneousUploads { get; set; }

        public SchedulerStatus Status { get; private set; }

        public static ScheduledTasks Instance
        {
            get { return _instance ?? (_instance = new ScheduledTasks()); }
        } 

        #endregion
    }
}
