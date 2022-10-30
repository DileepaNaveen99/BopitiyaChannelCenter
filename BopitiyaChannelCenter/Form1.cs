using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BopitiyaChannelCenter
{
    public partial class Form1 : Form
    {

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {

                case 0x84:
                    base.WndProc(ref m);
                    if ((int)m.Result == 0x1)
                        m.Result = (IntPtr)0x2;

                    return;

            }

            base.WndProc(ref m);
        }

        public Form1()
        {
            InitializeComponent();
        }


        private void HomeCloseBtn_Click(object sender, EventArgs e)
        {

            Application.Exit();

        }

        private void HomeMinimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void ProjectTitle_Click(object sender, EventArgs e)
        {

        }

        private void DoctorManageBtn_Click(object sender, EventArgs e)
        {

            Doctor navDoctor = new Doctor();
            navDoctor.Show();
            this.Hide();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Staff navStaff = new Staff();
            navStaff.Show();
            this.Hide();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Patient navPatient = new Patient();
            navPatient.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Diagnosis navDiagnosis = new Diagnosis();
            navDiagnosis.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Treatment navTreatment = new Treatment();
            navTreatment.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            LabReport navLabReport = new LabReport();
            navLabReport.Show();
            this.Hide();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Records navRecords = new Records();
            navRecords.Show();
            this.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Payment navPayment = new Payment();
            navPayment.Show();
            this.Hide();
        }
    }
}
