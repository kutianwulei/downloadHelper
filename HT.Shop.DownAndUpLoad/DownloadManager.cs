using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace HT.Ship.DownAndUpLoad
{
    public class DownloadManager
    {
        private Stopwatch _downloadStopWatch = null;
        private bool _cancelDownload = false;
        private BackgroundWorker _bgWorker = null;
        private static readonly int BufferSize = 32768;

        public DownloadManager(BackgroundWorker bgWorker)
        {
            _bgWorker = bgWorker;
            _downloadStopWatch = new Stopwatch();
        }
        /// <summary>
        /// 传入文件参数
        /// </summary>
        /// <param name="url"></param>
        /// <param name="fileName"></param>
        /// <param name="filelength"></param>
        /// <returns></returns>
        public bool DownloadFile(string url, string fileName,double filelength)
        {
            bool isDownloadSuccessfully = false;
            try
            {
                if (!this.WebClientDownloadInstallerFile(url, fileName, filelength))
                {
                    isDownloadSuccessfully = false;
                }

                isDownloadSuccessfully = true;
            }
            catch
            {
                isDownloadSuccessfully = false;
                _bgWorker.ReportProgress(0, new DownloadInfo() { Message = "下载失败" });
            }

            return isDownloadSuccessfully;
        }

        public void Cancel()
        {
            this._cancelDownload = true;
        }

        private bool IsResume(string url, string fileName)
        {
            string tempFileName = fileName + ".temp";

            if (File.Exists(tempFileName) )
            {

                return true;
            }
           
            return false;
        }

        private bool WebClientDownloadInstallerFile(string url, string fileName,double filelength)
        {
            DateTime startTime = DateTime.Now;

            bool resumeDownload = IsResume(url, fileName);
            string tempFileName = fileName + ".temp";
            bool isDownloadSuccessfully = false;
            FileMode fm = FileMode.Create;
            Stream stream = null;
            FileStream fileStream = null;
            long resumDownload=0;
            HttpWebResponse response = null;
            this._downloadStopWatch.Start();
            try
            {
                Uri installerUrl = new Uri(url);
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
                if (resumeDownload)
                {
                    FileInfo fn = new FileInfo(tempFileName);
                    httpWebRequest.AddRange(fn.Length);
                    //httpWebRequest.Headers.Add("range",fn.Length.ToString());
                    resumDownload = fn.Length;
                    fm = FileMode.Append;
                }

                response = (HttpWebResponse)httpWebRequest.GetResponse();
                stream = response.GetResponseStream();

                double contentLength = DownloadManager.GetContentLength(response);
                if (contentLength <= 0)
                    contentLength = filelength;
                byte[] buffer = new byte[BufferSize];
                long downloadedLength = 0;
                int currentDataLength;
                if (resumeDownload)
                {
                    FileInfo fn = new FileInfo(tempFileName);
                    downloadedLength = fn.Length;
                }
                fileStream = new FileStream(tempFileName, fm);

                while ((currentDataLength = stream.Read(buffer, 0, BufferSize)) > 0 && !this._cancelDownload)
                {
                    fileStream.Write(buffer, 0, currentDataLength);
                    downloadedLength += (long)currentDataLength;
                    TimeSpan span = DateTime.Now - startTime;
                    double second = span.TotalSeconds;
                    string spead = "";
                    if (second > 0.001)
                    {
                        spead =((downloadedLength - resumDownload) / 1024 / second).ToString("0.00") + "KB/秒";
                    }
                    if (this._downloadStopWatch.ElapsedMilliseconds > 1000)
                    {
                        this._downloadStopWatch.Reset();
                        this._downloadStopWatch.Start();

                        double doubleDownloadPersent = 0.0;
                        if (contentLength > 0.0)
                        {
                            doubleDownloadPersent = (double)downloadedLength / contentLength;
                            if (doubleDownloadPersent > 1.0)
                            {
                                doubleDownloadPersent = 1.0;
                            }
                        }

                        int intDownloadPersent = (int)(doubleDownloadPersent * 100);

                        double len= contentLength - downloadedLength;
                        double seconds = len / (downloadedLength /second);

                        DownloadInfo info = new DownloadInfo() { Message = "xxx", Persent = intDownloadPersent, Spead = spead, RemainingTime = TimeSpan.FromSeconds(seconds).ToString("g").Substring(0,8) 
                    };

                        DownloadingStatusChanged(info);
                    }
                }

                if (this._cancelDownload)
                {
                    DownloadInfo info = new DownloadInfo() { Message = "已取消下载", Persent = 100 };
                    DownloadingStatusChanged(info);
                }
                else if (currentDataLength >= 0)
                {
                    // downlown correct
                    isDownloadSuccessfully = true;
                }
            }
            catch(Exception ex)
            {
                _bgWorker.ReportProgress(0, new DownloadInfo() { Message = "下载失败" });

                isDownloadSuccessfully = false;
                //Logger.Trace(ex);

                // todo

            }
            finally
            {
                this._downloadStopWatch.Stop();
                if (fileStream != null)
                {
                    fileStream.Flush();
                    fileStream.Close();
                }
                if (stream != null)
                {
                    stream.Close();
                }

                if (response != null)
                {
                    response.Close();
                }
            }
            if (isDownloadSuccessfully)
            {
                if (File.Exists(fileName))
                {
                    Util.DeleteFileIfExists(fileName);
                }
                DownloadInfo info = new DownloadInfo() { Message = "xxx", Persent = 100 };
                DownloadingStatusChanged(info);
                File.Move(tempFileName, fileName);

                string tempFileInfoName = fileName + ".temp.info";
                if (File.Exists(tempFileInfoName))
                {
                    Util.DeleteFileIfExists(tempFileInfoName);
                }
            }
            return isDownloadSuccessfully;
        }

        private static double GetContentLength(HttpWebResponse res)
        {
            double result = 0.0;
            if (res.Headers["Content-Length"] != null)
            {
                string s = res.Headers["Content-Length"];
                if (!double.TryParse(s, out result))
                {
                    result = 0.0;
                }
            }
            return result;
        }

        private static bool GetAcceptRanges(WebResponse res)
        {
            if (res.Headers["Accept-Ranges"] != null)
            {
                string s = res.Headers["Accept-Ranges"];
                if (s == "none")
                {
                    return false;
                }
            }
            return true;
        }

        private static string GetEtag(WebResponse res)
        {
            if (res.Headers["ETag"] != null)
            {
                return res.Headers["ETag"];
            }
            return null;
        }

        private void DownloadingStatusChanged(DownloadInfo info)
        {
            if (_bgWorker != null && info != null)
            {
                _bgWorker.ReportProgress(0, info);
            }
        }
    }
}
