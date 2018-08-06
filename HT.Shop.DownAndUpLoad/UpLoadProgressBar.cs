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
    public partial class UpLoadProgressBar : Form
    {
        
        public UpLoadProgressBar()
        {
            InitializeComponent();
        }

        public ProgressBar getProgressbar()
        {
            return progressBar1;
        }
    }
}
