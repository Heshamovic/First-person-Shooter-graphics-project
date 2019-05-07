using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Graphics
{
    public partial class start : Form
    {
        string projectPath = Directory.GetParent(Environment.CurrentDirectory).Parent.FullName;
        System.Media.SoundPlayer player;
        public start()
        {
            InitializeComponent();
            player = new System.Media.SoundPlayer(projectPath + "\\Sounds\\Prayer.wav");
            player.Play();

        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            GraphicsForm form1 = new GraphicsForm();
            loading form2 = new loading();
            form2.Show();
            this.Hide();
            Thread t = new Thread(new ThreadStart(form1.Show));
            t.Start();
            Thread.Sleep(200);
            form2.Hide();
            t.Abort();
            player.Stop();
        }
        
        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
