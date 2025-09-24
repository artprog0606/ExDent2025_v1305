using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using AdvantShop.FilePath;
using AdvantShop.Helpers;

namespace AdvantShop.Statistic
{
    public class CommonStatistic
    {
        private static readonly object SyncObject = new object();
        private static readonly StatisticData Data = new StatisticData();
        private static Task<bool> _currentTask;
        private static CancellationTokenSource _cancellationToken;

        public static readonly string VirtualFileLogPath = FoldersHelper.GetPath(FolderType.PriceTemp, "StatisticLog.txt", true);
        public static readonly string FileLog = FoldersHelper.GetPathAbsolut(FolderType.PriceTemp, "StatisticLog.txt");


        private static void Init()
        {
            Data.ProcessedPercent = 0;
            Data.Processed = 0;
            Data.Total = 0;
            Data.IsRun = false;
            Data.IsBreaking = false;
            Data.Update = 0;
            Data.Add = 0;
            Data.Error = 0;
            Data.FileName = string.Empty;
            Data.FileUrl = string.Empty;
            Data.FileSize = string.Empty;
            Data.ZipFile = false;
            Data.CurrentProcess = string.Empty;
            Data.CurrentProcessName = string.Empty;
            if (!Directory.Exists(FoldersHelper.GetPathAbsolut(FolderType.PriceTemp)))
            {
                Directory.CreateDirectory(FoldersHelper.GetPathAbsolut(FolderType.PriceTemp));
            }
            FileHelpers.DeleteFile(FileLog);
        }

        public static StatisticData CurrentData
        {
            get
            {
                lock (SyncObject)
                {
                    Data.ProcessedPercent = Data.Total != 0 ? Convert.ToInt32(Data.Processed / ((float)Data.Total / 100)) : 0;
                    return Data;
                }
            }
        }

        public static Task<bool> StartNew(Action action, string currentProcess, string currentProcessName, string fileName = null)
        {
            if (IsRun)
                throw new Exception("The previous task was not completed.");

            Init();

            IsRun = true;

            CurrentProcess = currentProcess ?? string.Empty;
            CurrentProcessName = currentProcessName ?? string.Empty;
            FileName = fileName ?? string.Empty;

            _cancellationToken = new CancellationTokenSource();

            _currentTask = Task.Factory.StartNew(() =>
                {
                    Thread.CurrentThread.SetCulture();
                    using (_cancellationToken.Token.Register(Thread.CurrentThread.Abort))
                    {
                        action();
                    }

                    return true;
                },
                _cancellationToken.Token,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Current);

            return _currentTask.ContinueWith((antecedent) =>
            {
                IsRun = false;
                if (antecedent.Status == TaskStatus.Faulted)
                {
                    TotalErrorRow++;

                    var message = $"The CommonStatistic task failed. Action: {action.Method.DeclaringType}.{action.Method.Name}";
                    WriteLog(message);
                    if (antecedent.Exception != null)
                        Debug.Log.Error(message, antecedent.Exception.GetBaseException());
                    else
                        Debug.Log.Error(message);

                    return false;
                }
                return antecedent.Result;
            });
        }

        public static long TotalRow
        {
            get { lock (SyncObject) { return Data.Total; } }
            set { lock (SyncObject) { Data.Total = value; } }
        }

        public static long RowPosition
        {
            get { lock (SyncObject) { return Data.Processed; } }
            set { lock (SyncObject) { Data.Processed = value; } }
        }

        public static bool IsRun
        {
            get { lock (SyncObject) { return Data.IsRun; } }
            // отмена через Break()
            private set { lock (SyncObject) { Data.IsRun = value; } }
        }

        public static bool IsBreaking
        {
            get { lock (SyncObject) { return Data.IsBreaking; } }
            private set { lock (SyncObject) { Data.IsBreaking = value; } }
        }

        public static async Task Break()
        {
            if (IsRun && !IsBreaking)
            {
                if (_currentTask != null)
                {
                    IsBreaking = true;

                    // даем минуту на завршение
                    // либо принудительно завершаем
                    var delay = Task.Delay(60000);
                    await Task.WhenAny(delay, _currentTask).ContinueWith((antecedent) =>
                    {
                        if (antecedent.Result.Id == delay.Id)
                            _cancellationToken.Cancel(true);
                    });
                }
                else
                    IsRun = false;
            }
        }

        public static long TotalUpdateRow
        {
            get { lock (SyncObject) { return Data.Update; } }
            set { lock (SyncObject) { Data.Update = value; } }
        }

        public static long TotalAddRow
        {
            get { lock (SyncObject) { return Data.Add; } }
            set { lock (SyncObject) { Data.Add = value; } }
        }

        public static long TotalErrorRow
        {
            get { lock (SyncObject) { return Data.Error; } }
            set { lock (SyncObject) { Data.Error = value; } }
        }

        public static string FileName
        {
            get { lock (SyncObject) { return Data.FileName; } }
            set { lock (SyncObject) { Data.FileName = value; } }
        }

        public static string FileUrl
        {
            get { lock (SyncObject) { return Data.FileUrl; } }
            set { lock (SyncObject) { Data.FileUrl = value; } }
        }

        public static bool ZipFile
        {
            get { lock (SyncObject) { return Data.ZipFile; } }
            set { lock (SyncObject) { Data.ZipFile = value; } }
        }

        public static string FileSize
        {
            get { lock (SyncObject) { return Data.FileSize; } }
            set { lock (SyncObject) { Data.FileSize = value; } }
        }

        public static string CurrentProcess
        {
            get { lock (SyncObject) { return Data.CurrentProcess; } }
            set { lock (SyncObject) { Data.CurrentProcess = value; } }
        }

        public static string CurrentProcessName
        {
            get { lock (SyncObject) { return Data.CurrentProcessName; } }
            set { lock (SyncObject) { Data.CurrentProcessName = value; } }
        }

        public static void WriteLog(string message)
        {
            lock (SyncObject)
            {
                try
                {
                    using (var fs = new FileStream(FileLog, FileMode.Append, FileAccess.Write))
                    using (var sw = new StreamWriter(fs, Encoding.UTF8))
                        sw.WriteLine(message);
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                }
            }
        }

        public static string ReadLog()
        {
            var content = "";
            lock (SyncObject)
            {
                if (File.Exists(FileLog))
                {
                    using (var streamReader = new StreamReader(FileLog))
                        content = streamReader.ReadToEnd();
                }
            }
            return content;
        }
    }
}