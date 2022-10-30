using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace BopitiyaChannelCenter
{
    public partial class LabReport : Form
    {

        public int level;
        public String LabReportID;
        public int val = 0;
        public int ID;

        public string connectionString = (@"Data Source=DESKTOP-5SU6VUS\SQLEXPRESS;Initial Catalog=BopitiyaCCdb;Integrated Security=True");

        public LabReport()
        {
            InitializeComponent();
            LabReport_Populate_PatientID();
            LabReport_Populate_StaffID();
        }

        private void TreatmentBackBtn_Click(object sender, EventArgs e)
        {

            Form1 navHome = new Form1();
            navHome.Show();
            this.Hide();
        }


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


        private void AddLabReportBtn_Click(object sender, EventArgs e)
        {

            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand comm = connection.CreateCommand();
            comm.CommandType = CommandType.Text;

            comm.CommandText = "SELECT COUNT(ID) AS ID FROM LabReportTB";
            comm.ExecuteNonQuery();

            DataTable data_t = new DataTable();

            SqlDataAdapter data_a = new SqlDataAdapter(comm);
            data_a.Fill(data_t);

            int nxt = 0;
            foreach (DataRow data_r in data_t.Rows)
            {
                string next = data_r["ID"].ToString();
                nxt = Int16.Parse(next);
                nxt = ++nxt;
            }
            connection.Close();

            SqlCommand cmd = new SqlCommand("INSERT INTO LabReportTB (ID,LabReportID, PatientID, StaffID, ReportType, Date) VALUES (@AIVal, @Rid, @Pid, @Sid, @Type, @Date)", connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Rid", LRIDInput.Text);
            cmd.Parameters.AddWithValue("@Pid", LRPatientIdDropDown.Text);
            cmd.Parameters.AddWithValue("@Sid", LRStaffIDDropDown.Text);
            cmd.Parameters.AddWithValue("@Type", LRReportTypeDropDown.Text);
            cmd.Parameters.AddWithValue("@Date", LRDate.SelectionStart);
            cmd.Parameters.AddWithValue("@AIVal", nxt);

            connection.Open();
            cmd.ExecuteNonQuery();

            MessageBox.Show("Lab Report added...");

            ClearFields();
            LabReportUITabControl.SelectedTab = ViewLabReportTab;
        }

        public void ClearFields()
        {
            LRIDInput.Clear();
            LRPatientIdDropDown.SelectedIndex = -1;
            LRStaffIDDropDown.SelectedIndex = -1;
            LRReportTypeDropDown.SelectedIndex = -1;
            
        }

        private void ViewLabReportTab_Click_1(object sender, EventArgs e)
        {
            ViewLabReport();
            LabReportUITabControl.SelectedTab = ViewLabReportTab;
        }

        private void LabReport_Load(object sender, EventArgs e)
        {
            ViewLabReport();
            LabReportUITabControl.SelectedTab = ViewLabReportTab;
        }

        private void ViewLabReport()
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand readSubjectsQuery = new SqlCommand("Select * from LabReportTB", con);
            DataTable dataTable = new DataTable();

            con.Open();

            SqlDataReader sqlDataReader = readSubjectsQuery.ExecuteReader();

            dataTable.Load(sqlDataReader);
            con.Close();

            ViewLabReportDataGridView.AutoGenerateColumns = true;
            ViewLabReportDataGridView.DataSource = dataTable;


        }


        private void ViewLabReporttDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ManageLabReport_Populate_StaffID();
            ManageLabReport_Populate_PatientID();

            LabReportID = ViewLabReportDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManageLRID.Text = ViewLabReportDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManageLRPIDdw.SelectedItem = ViewLabReportDataGridView.CurrentRow.Cells[2].Value.ToString();
            ManageLRSIDdw.SelectedItem = ViewLabReportDataGridView.CurrentRow.Cells[3].Value.ToString();
            ManageLRTypedw.SelectedItem = ViewLabReportDataGridView.CurrentRow.Cells[4].Value.ToString();
            ManageLRDate.SetDate((DateTime)ViewLabReportDataGridView.CurrentRow.Cells[5].Value);
            val = 1;
            LabReportUITabControl.SelectedTab = ManageLabReportTab;
        }

        private void ManageLabReportUpdateBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand sqlCommand = new SqlCommand("UPDATE LabReportTB SET LabReportID = @NewLabReportID, PatientId = @NewPid, StaffID = @NewSid, ReportType = @NewType, Date = @NewDate WHERE LabReportID = @LabReportID", connection);
                sqlCommand.CommandType = CommandType.Text;

                sqlCommand.Parameters.AddWithValue("@NewLabReportID", ManageLRID.Text);
                sqlCommand.Parameters.AddWithValue("@NewPid", ManageLRPIDdw.Text);
                sqlCommand.Parameters.AddWithValue("@NewSid", ManageLRSIDdw.Text);
                sqlCommand.Parameters.AddWithValue("@NewType", ManageLRTypedw.Text);
                sqlCommand.Parameters.AddWithValue("@NewDate", ManageLRDate.SelectionStart);
                sqlCommand.Parameters.AddWithValue("@LabReportID", this.LabReportID);

                connection.Open();

                sqlCommand.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Lab Report Information has been Updated Sucessfully", "Confirmation");

                ClearUpdateFields();
                LabReportUITabControl.SelectedTab = ViewLabReportTab;
            }
            else
            {
                MessageBox.Show("Please Select a Lab Report to Update ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ClearUpdateFields()
        {
            ManageLRID.Clear();
            ManageLRPIDdw.SelectedIndex = -1;
            ManageLRSIDdw.SelectedIndex = -1;
            ManageLRTypedw.SelectedIndex = -1;
        }

        private void ManageLabReportRemoveBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                if (MessageBox.Show("This wiil Remove the Lab Report permanently. Are You Sure?", "Remove Lab Report", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    SqlCommand command = new SqlCommand("DELETE FROM LabReportTB WHERE LabReportID = @LabReportID", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@LabReportID", this.LabReportID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("LabReport remvoed successfully", "Confirmation");


                }

            }
            else
            {
                MessageBox.Show("Please Select a Lab Report to remove ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            ClearUpdateFields();
            LabReportUITabControl.SelectedTab = ViewLabReportTab;
        }

        public void LabReport_Populate_PatientID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            LRPatientIdDropDown.Items.Clear();

            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT PatientId FROM PatientTB";
            cmd.ExecuteNonQuery();

            DataTable data_t = new DataTable();
            SqlDataAdapter data_a = new SqlDataAdapter(cmd);
            data_a.Fill(data_t);


            foreach (DataRow data_r in data_t.Rows)
            {
                LRPatientIdDropDown.Items.Add(data_r["PatientId"].ToString());
            }

            connection.Close();
        }


        public void LabReport_Populate_StaffID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            LRStaffIDDropDown.Items.Clear();

            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT StaffID FROM StaffTB";
            cmd.ExecuteNonQuery();

            DataTable data_t = new DataTable();
            SqlDataAdapter data_a = new SqlDataAdapter(cmd);
            data_a.Fill(data_t);


            foreach (DataRow data_r in data_t.Rows)
            {
                LRStaffIDDropDown.Items.Add(data_r["StaffID"].ToString());
            }

            connection.Close();
        }


        public void ManageLabReport_Populate_PatientID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            ManageLRPIDdw.Items.Clear();

            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT PatientId FROM PatientTB";
            cmd.ExecuteNonQuery();

            DataTable data_t = new DataTable();
            SqlDataAdapter data_a = new SqlDataAdapter(cmd);
            data_a.Fill(data_t);


            foreach (DataRow data_r in data_t.Rows)
            {
                ManageLRPIDdw.Items.Add(data_r["PatientId"].ToString());
            }

            connection.Close();
        }


        public void ManageLabReport_Populate_StaffID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            ManageLRSIDdw.Items.Clear();

            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT StaffID FROM StaffTB";
            cmd.ExecuteNonQuery();

            DataTable data_t = new DataTable();
            SqlDataAdapter data_a = new SqlDataAdapter(cmd);
            data_a.Fill(data_t);


            foreach (DataRow data_r in data_t.Rows)
            {
                ManageLRSIDdw.Items.Add(data_r["StaffID"].ToString());
            }

            connection.Close();
        }

       
    }
}
