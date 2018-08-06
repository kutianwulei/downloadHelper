using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Collections;

namespace HT.Ship.DownAndUpLoad
{
    /// <summary>
    /// 表单上传
    /// </summary>
    public class HttpUploader
    {
        private ArrayList bytesArray;
        private Encoding encoding = Encoding.UTF8;
        private string boundary = String.Empty;
        public HttpUploader()
        {
            bytesArray = new ArrayList();
            string flag = DateTime.Now.Ticks.ToString("x");
            boundary = "---------------------------" + flag;
        }


        /// <summary>
        /// 合并请求数据
        /// </summary>
        /// <returns></returns>
        private byte[] MergeContent()
        {
            int length = 0;
            int readLength = 0;
            string endBoundary = "--" + boundary + "--\r\n";
            byte[] endBoundaryBytes = encoding.GetBytes(endBoundary);

            bytesArray.Add(endBoundaryBytes);

            foreach (byte[] bTemp in bytesArray)
            {
                length += bTemp.Length;
            }

            byte[] bytes = new byte[length];

            foreach (byte[] bTemp in bytesArray)
            {
                bTemp.CopyTo(bytes, readLength);
                readLength += bTemp.Length;
            }

            return bytes;
        }

        /// <summary>
        /// 上传
        /// </summary>
        /// <param name="requestUrl">请求url</param>
        /// <param name="responseText">响应</param>
        /// <returns></returns>
        public bool Upload(String requestUrl, out String responseText)
        {
            responseText = "";
            if (String.IsNullOrEmpty(requestUrl) == true)
            {
                return false;
            }
            WebClient webClient = new WebClient();
            webClient.Headers.Add("Content-Type", "multipart/form-data; boundary=" + boundary);

            byte[] responseBytes;
            byte[] bytes = MergeContent();

            try
            {
                responseBytes = webClient.UploadData(requestUrl, "POST", bytes);
                responseText = System.Text.Encoding.UTF8.GetString(responseBytes);
                return true;
            }
            catch (WebException ex)
            {
                responseText = ex.Message;
            }

            return false;
        }

        /// <summary>
        /// 设置表单数据字段
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="fieldValue">字段值</param>
        /// <returns></returns>
        public void SetFieldValue(String fieldName, String fieldValue)
        {
            string httpRow = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}\r\n";
            string httpRowData = String.Format(httpRow, fieldName, fieldValue);

            bytesArray.Add(encoding.GetBytes(httpRowData));
        }

        /// <summary> 
        /// 设置表单文件数据
        /// </summary>
        /// <param name="fieldName">字段名</param>
        /// <param name="filename">字段值</param>
        /// <param name="contentType">内容内型</param>
        /// <param name="fileBytes">文件字节流</param>
        /// <returns></returns>
        public void SetFieldValue(String fieldName, String filename, String contentType, Byte[] fileBytes)
        {
            string end = "\r\n";
            string httpRow = "--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string httpRowData = String.Format(httpRow, fieldName, filename, contentType);

            byte[] headerBytes = encoding.GetBytes(httpRowData);
            byte[] endBytes = encoding.GetBytes(end);
            byte[] fileDataBytes = new byte[headerBytes.Length + fileBytes.Length + endBytes.Length];

            headerBytes.CopyTo(fileDataBytes, 0);
            fileBytes.CopyTo(fileDataBytes, headerBytes.Length);
            endBytes.CopyTo(fileDataBytes, headerBytes.Length + fileBytes.Length);

            bytesArray.Add(fileDataBytes);
        }

    }
}

//客户端代码
//string fileFullName = @"c:\test.txt", filedValue = "hello_world", responseText = "";
//FileStream fs = new FileStream(fileFullName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
//byte[] fileBytes = new byte[fs.Length];
//fs.Read(fileBytes, 0, fileBytes.Length);
//fs.Close(); fs.Dispose();
 
//HttpRequestClient httpRequestClient = new HttpRequestClient();
//httpRequestClient.SetFieldValue("key", filedValue);
//httpRequestClient.SetFieldValue("uploadfile", Path.GetFileName(fileFullName), "application/octet-stream", fileBytes);
//httpRequestClient.Upload(NormalBotConfig.Instance.GetUploadFileUrl(), out responseText);