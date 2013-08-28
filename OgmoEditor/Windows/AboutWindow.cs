using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Deployment.Application;

namespace OgmoEditor.Windows
{
    public partial class AboutWindow : Form
    {
        public AboutWindow()
        {
            InitializeComponent();

            if (ApplicationDeployment.IsNetworkDeployed)
                versionLabel.Text = "Version " + ApplicationDeployment.CurrentDeployment.CurrentVersion;
            else
                versionLabel.Text = "Debug Mode";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
            Ogmo.MainWindow.Activate();
            Ogmo.MainWindow.EnableEditing();
        }

        private void donateButton_Click(object sender, EventArgs e)
        {
            Ogmo.DonationLink();
        }

        private void websiteButton_Click(object sender, EventArgs e)
        {
            Ogmo.WebsiteLink();
        }
    }
}
