using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DownAndUpLoadManager
{
    public partial class UploadProgressForm : Form
    {

        public UploadProgressForm()
        {
            InitializeComponent();
           
        }

      

        /// <summary>
        /// 获取进度条
        /// </summary>
        /// <returns></returns>
        public ProgressBar getProgressbar()
        {
            return progressBar1;
        }

        /// <summary>
        /// 获取进度条
        /// </summary>
        /// <returns></returns>
        public void setButtontext(string s)
        {
            BtnCancel.Text = s;
        }


        /// <summary>
        /// 获取进度条
        /// </summary>
        /// <returns></returns>
        public TextBox getLable()
        {
            return textBox_toast;
        }


        /// <summary>
        /// 显示上传信息
        /// </summary>
        /// <param name="toast"></param>
        public void setToast(string toast)
        {
            textBox_toast.AppendText(toast+ "\r\n");
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        
    }
}
