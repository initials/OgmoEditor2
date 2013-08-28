using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OgmoEditor.Windows.Utilities
{
    public class UtilityForm : Form
    {
        public UtilityForm()
        {
            FormClosed += OnFormClosed;
            Ogmo.OnProjectClose += OnProjectClose;
        }

        private void OnProjectClose(Project project)
        {
            Close();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Ogmo.OnProjectClose -= OnProjectClose;
        }
    }
}
