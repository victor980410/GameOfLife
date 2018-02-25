using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace GameOfLife
{
    public partial class Form1 : Form
    {
        private static LifeClass life = new LifeClass();
        private static List<Bitmap> images = new List<Bitmap>();

        public Form1()
        {
            // everything starts here
            InitializeComponent();
            Field.Image = life.Draw();
            // timer interval is 0.3 seconds
            timer1.Interval = 300;
            timer1.Stop();
        }

        private void Field_MouseClick(object sender, MouseEventArgs e)
        {
            // changing value of specific cell
            Field.Image = life.ChangeCell(e.X, e.Y);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // each timer tick we calculate next generation and save picture of previous to list
            images.Add((Bitmap)Field.Image);
            Field.Image = life.NextGeneration();
        }

        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            images.Clear();
            timer1.Start();
        }

        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void tickToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            Field.Image = life.NextGeneration();
        }

        // this is code to create Gif image without additional library, honestly I dont know how it works I just copied it from one site.   
        private byte[] GifAnimation = { 33, 255, 11, 78, 69, 84, 83, 67, 65, 80, 69, 50, 46, 48, 3, 1, 0, 0, 0 };
        private byte[] Delay = { 30, 0 };
        public void WriteGifImg(byte[] B, BinaryWriter BW)
        {
            B[785] = Delay[0]; //5 secs delay
            B[786] = Delay[1];
            B[798] = (byte)(B[798] | 0X87);
            BW.Write(B, 781, 18);
            BW.Write(B, 13, 768);
            BW.Write(B, 799, B.Length - 800);
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(images.Count == 0)
            {
                MessageBox.Show("Nothing to export.");
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Animated GIF file (*.gif)|*.gif";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                timer1.Stop();
                string GifFile = sfd.FileName;
                MemoryStream MS = new MemoryStream();
                BinaryReader BR = new BinaryReader(MS);
                BinaryWriter BW = new BinaryWriter(new FileStream(GifFile, FileMode.Create));
                ((Image)images[0]).Save(MS, ImageFormat.Gif);
                byte[] B = MS.ToArray();
                B[10] = (byte)(B[10] & 0X78); //No global color table.
                BW.Write(B, 0, 13);
                BW.Write(GifAnimation);
                WriteGifImg(B, BW);
                for (int I = 1; I < images.Count; I++)
                {
                    MS.SetLength(0);
                    ((Image)images[I]).Save(MS, ImageFormat.Gif);
                    B = MS.ToArray();
                    WriteGifImg(B, BW);
                }
                BW.Write(B[B.Length - 1]);
                BW.Close();
                MS.Dispose();

                Process.Start(sfd.FileName);
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text files (*.txt)|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                timer1.Stop();
                try
                {
                    life.SaveSeed(sfd.FileName);
                }
                catch(Exception)
                {
                    MessageBox.Show("File is not valid.");
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog sfd = new OpenFileDialog();
            sfd.Filter = "Text files (*.txt)|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                timer1.Stop();
                life.LoadSeed(sfd.FileName);
                Field.Image = life.Draw();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            life.Clear();
            Field.Image = life.Draw();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
