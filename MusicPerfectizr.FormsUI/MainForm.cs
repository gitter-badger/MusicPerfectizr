﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using MusicPerfectizr.Domain;

namespace MusicPerfectizr.FormsUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            
            moveToFolderCheckBox.CheckedChanged += EnableFields;

            folderBrowserDialog1.Description = $"Choose folder with music...";
            pathToCopyTextBox.Enabled = false;
            chooseSecondDirectoryBtn.Enabled = false;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string secondDirPath = null;
            try
            {
                var dir = new DirectoryInfo(folderBrowserDialog2.SelectedPath);
                secondDirPath = dir.FullName;
            }
            catch (Exception)
            {
                Console.WriteLine($"Error: folderBrowserDialog2 don`t entered");
            }

            var userOptions = new UserOptions(GetFolding(), GetTitle(),
                                              moveToFolderCheckBox.Checked);
            var mp3Files = new DirectoryInfo(folderBrowserDialog1.SelectedPath)
                .GetFiles("*.mp3", SearchOption.AllDirectories);
            var fileOperator = new FileOperator(userOptions,
                                                secondDirPath);

            for (int i = 0; i < mp3Files.Length; i++)
            {
                fileOperator.DoStuff(mp3Files[i]);
                int percentage = (i + 1) * 100 / mp3Files.Length;
                backgroundWorker1.ReportProgress(percentage);
            }
        }
        // Back on the 'UI' thread and update progressBar
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // The progress percentage is a property of e
            progressBar1.Value = e.ProgressPercentage;
        }

        // Called when backgroundWorker ended his work
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show($"All done! We did it!");
        }

        // configure and launch backgroundWorker
        private void launchBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.SelectedPath == "")
            {
                MessageBox.Show($"Choose path");
                return;
            }

            backgroundWorker1.DoWork += backgroundWorker1_DoWork;
            backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
            backgroundWorker1.RunWorkerCompleted += backgroundWorker1_RunWorkerCompleted;
            backgroundWorker1.WorkerReportsProgress = true;
            backgroundWorker1.RunWorkerAsync();
        }

        private void chooseDirectoryBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                pathTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void chooseSecondDirectoryBtn_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                pathToCopyTextBox.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private Folding GetFolding()
        {
            var temp = Folding.AsIs;
            if (createFoldingRadioButton.Checked)
                temp = Folding.CreateFolding;
            else if (allMusicToOneFolderRadioButton.Checked)
                temp = Folding.AllMusicToOneFolder;
            return temp;
        }

        private Title GetTitle()
        {
            var temp = Title.AsIs;
            if (artistTitleRadioButton.Checked)
                temp = Title.ArtistTitle;
            else if (titleRadioButton.Checked)
                temp = Title.JustTitle;
            return temp;
        }

        //CheckedChanged ivent for moveToFolderCheckBox
        private void EnableFields(object sender, EventArgs e)
        {
            pathToCopyTextBox.Enabled = !pathToCopyTextBox.Enabled;
            chooseSecondDirectoryBtn.Enabled = !chooseSecondDirectoryBtn.Enabled;
        }
    }
}
