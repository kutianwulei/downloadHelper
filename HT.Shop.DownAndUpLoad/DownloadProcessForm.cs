using HT.Ship.Com.models.response;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HT.Ship.DownAndUpLoad
{
    public partial class DownloadProcessForm : Form
    {
        private BackgroundWorker _BGWorker = new BackgroundWorker();
        private DownloadManager _manager = null;
        private Logging _log = null;
        private ResponseFileModel _entity;
        private string _floderPath="";
        public DownloadProcessForm(ResponseFileModel entity,string floderpath)
        {
            InitializeComponent();

            _BGWorker.WorkerReportsProgress = true;
            _BGWorker.WorkerSupportsCancellation = true;

            _BGWorker.DoWork += _BGWorker_DoWork;
            _BGWorker.RunWorkerCompleted += _BGWorker_RunWorkerCompleted;
            _BGWorker.ProgressChanged += _BGWorker_ProgressChanged;

            this.Shown += InstallProcessForm_Shown;

            _entity = entity;
            _floderPath = floderpath;
            this.label1.Text = "正在载入 "+entity.filename + "…";
            _log = new Logging("log.txt");
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            _BGWorker.CancelAsync();
            _manager.Cancel();
        }

        private void InstallProcessForm_Shown(object sender, EventArgs e)
        {
            // 当窗口打开后就开始后台的安装
            _BGWorker.RunWorkerAsync();
        }

        private void _BGWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DownloadInfo info = e.UserState as DownloadInfo;
            if (info != null)
            {
                if (info.Persent==100)
                {
                    this.Close();
                }
                this.progressBar1.Value = info.Persent;            
                _log.Log("Persent is " + info.Persent + "% .");

                if (info.Message == "已取消下载")
                {
                    this.Close();
                }
                if (info.Message == "下载失败")
                {
                    this.label1.Text = "下载失败";
                }
                this.Text = "载入…（" + info.Persent + "%)  "+info.Spead+" "+info.RemainingTime;
            }
        }

        private void _BGWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // 下载已完成
            //this.label1.Text = "下载已完成";
            this.BtnCancel.Visible = false;
            this.BtnOK.Visible = true;
            this.BtnOK.Location = this.BtnCancel.Location;
        }

        private void _BGWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bgWorker = sender as BackgroundWorker;

            _manager = new DownloadManager(bgWorker);

            System.IO.Directory.CreateDirectory(_floderPath);
            double filelength =Convert.ToDouble(_entity.filesize);
            string aFileanme=_entity.fileinnerid+ System.IO.Path.GetExtension(_entity.filename);
            _manager.DownloadFile(_entity.fileurl, _floderPath + aFileanme, filelength);

        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }

    internal class DownloadInfo
    {
        public int Persent { get; set; }

        public string Message { get; set; }
        public string Spead { get; set; }
        public string RemainingTime { get; set; }
    }
}
