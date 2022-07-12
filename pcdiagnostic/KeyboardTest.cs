using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pcdiagnostic
{
    public partial class KeyboardTest : Form
    {
        public KeyboardTest()
        {
            InitializeComponent();
        }


        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void KeyboardTest_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                String panelName = "panel" + e.KeyCode.ToString();
                var matches = this.Controls.Find(panelName, true);

                System.Diagnostics.Debug.WriteLine(e.KeyValue);

                Panel tempPanel = matches[0] as Panel;
                tempPanel.BackColor = Color.Red;
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine(error);
            }
        }

        private void KeyboardTest_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                String panelName = "panel" + e.KeyCode.ToString();
                var matches = this.Controls.Find(panelName, true);

                Panel tempPanel = matches[0] as Panel;
                tempPanel.BackColor = Color.Yellow;
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine(error);
            }
        }

        private void KeyboardTest_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
            {
                e.IsInputKey = true;
            }
        }
    }
}
