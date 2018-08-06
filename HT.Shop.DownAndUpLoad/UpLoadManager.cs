//using HT.Shop.Utils;
//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text;
//using System.Windows.Forms;

//namespace DownAndUpLoadManager
//{
//    public class UpLoadManager
//    {
//        /// <summary> 
//        /// 将本地文件上传到指定的服务器(HttpWebRequest方法) 
//        /// </summary> 
//        /// <param name="address">文件上传到的服务器</param> 
//        /// <param name="fileNamePath">要上传的本地文件（全路径）</param> 
//        /// <param name="saveName">文件上传后的名称</param> 
//        /// <param name="progressBar">上传进度条</param> 
//        /// <returns>成功返回1，失败返回0</returns> 


//        public delegate void ProcessChange(object sender, int process);
//        public event ProcessChange onProcessChange;
//        public int Upload_Request(string address, string fileNamePath, string saveName, ProgressBar progressBar)
//        {
//            int returnValue = 0;     // 要上传的文件 
//            FileStream fs = new FileStream(fileNamePath, FileMode.Open, FileAccess.Read);
//            BinaryReader r = new BinaryReader(fs);     //时间戳 

//            HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(address));
//            httpReq.Method = "POST";     //对发送的数据不使用缓存 
//            httpReq.AllowWriteStreamBuffering = false;     //设置获得响应的超时时间（300秒） 
//            httpReq.Timeout = 300000;
//            long length = fs.Length;
//            httpReq.ContentType = "multipart/form-data";
//            httpReq.ContentLength = length;

//            try
//            {
//                progressBar.Maximum = int.MaxValue;
//                progressBar.Minimum = 0;
//                progressBar.Value = 0;
//                //每次上传4k
//                int bufferLength = 4096;
//                byte[] buffer = new byte[bufferLength]; //已上传的字节数 
//                long offset = 0;         //开始上传时间 
//                DateTime startTime = DateTime.Now;
//                int size = r.Read(buffer, 0, bufferLength);
//                Stream postStream = httpReq.GetRequestStream();         //发送请求头部消息 
//                while (size > 0)
//                {
//                    postStream.Write(buffer, 0, size);
//                    offset += size;
//                    progressBar.Value = (int)(offset * (int.MaxValue / length));
//                    if (onProcessChange != null)
//                    {
//                        onProcessChange(this, progressBar.Value);
//                    }
//                    TimeSpan span = DateTime.Now - startTime;
//                    double second = span.TotalSeconds;
//                    if (second > 0.001)
//                    {
//                    }
//                    else
//                    {
//                    }

//                    Application.DoEvents();
//                    size = r.Read(buffer, 0, bufferLength);
//                }
//                //添加尾部的时间戳 
//                //postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
//                postStream.Close();         //获取服务器端的响应 
//                WebResponse webRespon = httpReq.GetResponse();
//                Stream s = webRespon.GetResponseStream();
//                //读取服务器端返回的消息
//                StreamReader sr = new StreamReader(s);
//                String sReturnString = sr.ReadLine();
//                s.Close();
//                sr.Close();
//                var json = sReturnString;
//                var resultLogin = JsonConvert.DeserializeObject<Error>(json);
//                if (resultLogin.error == "0")
//                {
//                    returnValue = 1;
//                }
//                else
//                {
//                    returnValue = 0;

//                }
//            }
//            catch (Exception ex)
//            {
//                //Logger.Trace(ex);

//                returnValue = 0;

//            }
//            finally
//            {
//                fs.Close();
//                r.Close();
//            }
//            return returnValue;
//        }




//        public int Upload_Request_files(List<UploadFile> files, ProgressBar progressBar, Form form, TextBox lable)
//        {
//            int returnValue = 0;     // 要上传的文件 
//            long allOffset = 0;
//            long allLength = 0;
//            progressBar.Maximum = int.MaxValue;
//            progressBar.Minimum = 0;
//            progressBar.Value = 0;
//            try
//            {
//                foreach (var item in files)
//                {
//                    FileStream fs = new FileStream(item.Filepath, FileMode.Open, FileAccess.Read);
//                    allLength += fs.Length;
//                    fs.Close();
//                }
//            }
//            catch (Exception ex)
//            {

//                //Logger.Trace(ex);
//            }

//            foreach (var item in files)
//            {
//                lable.AppendText(item.Filename + " 上传中…" + "\r\n");

//                FileStream fs = new FileStream(item.Filepath, FileMode.Open, FileAccess.Read);
//                BinaryReader r = new BinaryReader(fs);     //时间戳 

//                HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create(new Uri(item.Address));
//                httpReq.Method = "POST";     //对发送的数据不使用缓存 
//                httpReq.AllowWriteStreamBuffering = false;     //设置获得响应的超时时间（300秒） 
//                httpReq.Timeout = 30000000;
//                long length = fs.Length;
//                httpReq.ContentType = "multipart/form-data";
//                httpReq.ContentLength = length;

//                try
//                {

//                    //每次上传4k

//                    int bufferLength = 4096;
//                    byte[] buffer = new byte[bufferLength]; //已上传的字节数 
//                    long offset = 0;         //开始上传时间 
//                    DateTime startTime = DateTime.Now;
//                    int size = r.Read(buffer, 0, bufferLength);
//                    Stream postStream = httpReq.GetRequestStream();         //发送请求头部消息 
//                    while (size > 0)
//                    {
//                        postStream.Write(buffer, 0, size);
//                        offset += size;
//                        allOffset += size;
//                        //progressBar.Value = (int)(offset * (int.MaxValue / length));
//                        //form.Text = "上传…" + "(" + offset * 100 / length + "%)";
//                        progressBar.Value = (int)(allOffset * (int.MaxValue / allLength));
//                        form.Text = "上传…" + "(" + allOffset * 100 / allLength + "%)";
//                        TimeSpan span = DateTime.Now - startTime;
//                        double second = span.TotalSeconds;
//                        if (second > 0.001)
//                        {
//                            //lblSpeed.Text = "平均速度：" + (offset / 1024 / second).ToString("0.00") + "KB/秒";
//                        }
//                        else
//                        {
//                            //lblSpeed.Text = " 正在连接…";
//                        }
//                        Application.DoEvents();
//                        size = r.Read(buffer, 0, bufferLength);
//                    }
//                    //添加尾部的时间戳 
//                    //postStream.Write(boundaryBytes, 0, boundaryBytes.Length);
//                    postStream.Close();         //获取服务器端的响应 
//                    WebResponse webRespon = httpReq.GetResponse();
//                    Stream s = webRespon.GetResponseStream();
//                    //读取服务器端返回的消息
//                    StreamReader sr = new StreamReader(s);
//                    String sReturnString = sr.ReadLine();
//                    s.Close();
//                    sr.Close();
//                    var json = sReturnString;
//                    var resultLogin = JsonConvert.DeserializeObject<Error>(json);
//                    if (resultLogin.error == "0")
//                    {
//                        returnValue = 1;
//                        lable.AppendText(item.Filename + " 上传成功…" + "\r\n");
//                    }
//                    else
//                    {
//                        returnValue = 0;
//                        lable.AppendText(item.Filename + " 上传失败…" + "\r\n");

//                        //ShowError.ShowErrorMessage(ErrorHelper.getValue()[resultLogin.error].ToString());

//                    }
//                }
//                catch (Exception ex)
//                {
//                    //Logger.Trace(ex);

//                    returnValue = 0;

//                }
//                finally
//                {
//                    fs.Close();
//                    r.Close();
//                }
//            }
//            return returnValue;
//        }
//        /// <summary>
//        /// 自定义上传文件类型
//        /// </summary>
//        public class UploadFile
//        {
//            private string address;
//            private string filepath;
//            private string filename;
//            public string Address
//            {
//                get
//                {
//                    return address;
//                }

//                set
//                {
//                    address = value;
//                }
//            }

//            public string Filepath
//            {
//                get
//                {
//                    return filepath;
//                }

//                set
//                {
//                    filepath = value;
//                }
//            }

//            public string Filename
//            {
//                get
//                {
//                    return filename;
//                }

//                set
//                {
//                    filename = value;
//                }
//            }
//        }
//    }
//}
