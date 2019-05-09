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
        bool play = true;
        public start()
        {
            InitializeComponent();
            player = new System.Media.SoundPlayer(projectPath + "\\Sounds\\Prayer.wav");
            player.Play();
        }

        private void bunifuImageButton1_Click(object sender, EventArgs e)
        {
            GraphicsForm GF = new GraphicsForm();
            loading LF = new loading();
            LF.Show();
            this.Hide();
            Thread t = new Thread(new ThreadStart(GF.Show));
            t.Start();
            Thread.Sleep(2000);
            LF.Close();
            t.Abort();
            player.Stop();
        }
        
        private void bunifuImageButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void bunifuCheckbox1_OnChange(object sender, EventArgs e)
        {
            if (play == true)
            {
                player.Stop();
                play = false;
            }
            else
            {
                player.Play();
                play = true;
            }
        }
    }
}
