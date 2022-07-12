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
    public partial class Form2 : Form
    {
        private bool isWhite = true;

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Form2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                this.Hide();
            } else if (e.KeyData == Keys.Space)
            {
                System.Diagnostics.Debug.WriteLine("space");
                if (isWhite)
                {
                    System.Diagnostics.Debug.WriteLine("white");
                    this.BackColor = Color.Black;
                    isWhite = false;
                } else
                {
                    System.Diagnostics.Debug.WriteLine("black");
                    this.BackColor = Color.White;
                    isWhite = true;
                }
            }
        }
    }
}
