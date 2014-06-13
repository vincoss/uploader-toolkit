using System;
using System.Linq;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using Vinco.Uploader.Handlers;
using Vinco.Uploader.Tasks;
using Vinco.Uploader;
using Vinco.Uploader.Util;


namespace ConsoleClient
{
    class Program
    {
        private string _uploadFromPath = @"E:\_DevelopmentResources\";
        private string _validateToPath = @"E:\_Log\Uploads\{0}";
        private string _invalidResults = @"E:\_Log\Invalid\{0}";

        private Uri _uploadUri = new Uri("http://uploader.vincoss.com/upload", UriKind.Absolute);
        private Uri _cancelUri = new Uri("http://uploader.vincoss.com/cancel", UriKind.Absolute);

        private int _totalFilesCount;
        private int _uploadedFilesCount;

        static void Main(string[] args)
        {
            new Program().UploadFiles();

            Console.Read();
        }

        #region Upload methods

        private void UploadFiles()
        {
            // Find files
            IList<string> files = files = GetFiles(_uploadFromPath).ToList();

            _totalFilesCount = files.Count();

            Console.WriteLine("Found total files : {0}", _totalFilesCount);

            // Start uploader
            ScheduledTasks.Instance.Run();

            int runningUploads = 0;
            while (runningUploads < _totalFilesCount)
            {
                int batch = runningUploads - _uploadedFilesCount;

                if (batch < 50)
                {
                    string file = files[runningUploads];
                    try
                    {
                        // Get file to upload
                        var info = new FileInfo(file);
                        var fileWrapper = new FileInfoWrapper(info);

                        // Create upload item
                        var uploadItem = new UploadItem(fileWrapper);
                        uploadItem.ProgressChanged += UploadItem_ProgressChanged;
                        uploadItem.UploadCompleted += UploadItem_UploadCompleted;

                        // Create upload handler
                        var handler = new ChunkedHttpUploader(0)
                        {
                            UploadUri = _uploadUri,
                            CancelUri = _cancelUri
                        };

                        // Create upload task
                        var task = ScheduledTasks.Instance.CreateTask(handler, uploadItem);

                        // Add task into schedules
                        ScheduledTasks.Instance.Add(task);
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    runningUploads++;
                }
            }
        }

        private void UploadItem_ProgressChanged(object sender, EventArgs e)
        {
            UploadItemBase uploadItem = sender as UploadItemBase;

            Console.WriteLine();
            Console.WriteLine("***** Progress");
            Console.WriteLine("Name:    {0}", uploadItem.Name);
            Console.WriteLine("Length:  {0}", FileHelper.FormatBytes(uploadItem.Length));
        }

        private void UploadItem_UploadCompleted(object sender, EventArgs e)
        {
            Interlocked.Increment(ref _uploadedFilesCount);

            UploadItemBase uploadItem = sender as UploadItemBase;

            // Show results
            Console.WriteLine();
            Console.WriteLine("***** Completed");
            Console.WriteLine("Name:    {0}", uploadItem.Name);
            Console.WriteLine("Length:  {0}", FileHelper.FormatBytes(uploadItem.Length));
            Console.WriteLine("Status:  {0}", uploadItem.Status);
            Console.WriteLine("Elapsed: {0}", uploadItem.Elapsed);
            Console.WriteLine("Message: {0}", uploadItem.Message);

            // Get paths
            string originalFilePath = uploadItem.FileInfo.FullName;
            string toPath = string.Format("{0}-{1}", uploadItem.Id, uploadItem.Name);
            string uploadedFilePath = string.Format(_validateToPath, toPath);

            // Compare uploaded file to original
            if (uploadItem.Status == FileUploadStatus.Complete)
            {
                bool result = FileEquals(originalFilePath, uploadedFilePath);

                if (result == false)
                {
                    File.WriteAllText(string.Format(_invalidResults, toPath), originalFilePath);
                }

                Console.WriteLine("Valid:   {0}", result);

                // Remove the file
                File.Delete(uploadedFilePath);
            }

            Console.WriteLine("Total Files Uploaded: {0} of {1}", _uploadedFilesCount, _totalFilesCount);
        } 

        #endregion

        #region Compare files

        private bool FileEquals(string leftFileName, string rightFileName)
        {
            using (var left = new FileStream(leftFileName, FileMode.Open, FileAccess.Read))
            using (var right = new FileStream(rightFileName, FileMode.Open, FileAccess.Read))
            {
                return CompareStreams(left, right);
            }
        }

        private bool CompareStreams(Stream leftStream, Stream rightStream)
        {
            const int BUFFER_SIZE = 4096;
            byte[] left = new byte[BUFFER_SIZE];
            byte[] right = new byte[BUFFER_SIZE];
            while (true)
            {
                int leftLength = leftStream.Read(left, 0, BUFFER_SIZE);
                int rightLength = rightStream.Read(right, 0, BUFFER_SIZE);

                if (leftLength != rightLength)
                {
                    return false;
                }
                if (leftLength == 0)
                {
                    return true;
                }
                bool result = CompareBytes(left, left.Length, right, right.Length);
                if(!result)
                {
                    return false;
                }
                if (leftLength == 0)
                {
                    return true;
                }
            }
        }

        private bool CompareBytes(byte[] bufferOne, int lengthOne, byte[] bufferTwo, int lengthTwo)
        {
            if (lengthOne != lengthTwo)
            {
                return false;
            }
            for (int i = 0; i < lengthOne; i++)
            {
                if (bufferOne[i] != bufferTwo[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static IEnumerable<string> GetFiles(string path)
        {
            Queue<string> queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        queue.Enqueue(subDir);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                string[] files = null;
                try
                {
                    files = Directory.GetFiles(path);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
                if (files != null)
                {
                    for (int i = 0; i < files.Length; i++)
                    {
                        yield return files[i];
                    }
                }
            }
        }

        #endregion 
    }
}
