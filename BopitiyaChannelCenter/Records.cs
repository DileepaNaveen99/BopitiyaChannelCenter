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
    public partial class Records : Form
    {

        public int level;
        public String RecordID;
        public int val = 0;
        public int ID;

        public string connectionString = (@"Data Source=DESKTOP-5SU6VUS\SQLEXPRESS;Initial Catalog=BopitiyaCCdb;Integrated Security=True");

        public Records()
        {
            InitializeComponent();
            Record_Populate_PatientID();
        }

        private void RecordBackBtn_Click(object sender, EventArgs e)
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


        private void AddRecordBtn_Click(object sender, EventArgs e)
        {

            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand comm = connection.CreateCommand();
            comm.CommandType = CommandType.Text;

            comm.CommandText = "SELECT COUNT(ID) AS ID FROM RecordTB";
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

            SqlCommand cmd = new SqlCommand("INSERT INTO RecordTB (ID,RecordID, UnitType, Unit, PatientID, NoOfDays) VALUES (@AIVal, @Rid, @UnitType, @Unit, @Pid, @NoOfDays)", connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@Rid", AddRecordRid.Text);
            cmd.Parameters.AddWithValue("@UnitType", AddRecordUTdw.Text);
            cmd.Parameters.AddWithValue("@Unit", AddRecordUnitdw.Text);
            cmd.Parameters.AddWithValue("@Pid", AddRecordPiddw.Text);
            cmd.Parameters.AddWithValue("@NoOfDays", AddRecordNoOfDays.Text);
            cmd.Parameters.AddWithValue("@AIVal", nxt);

            connection.Open();
            cmd.ExecuteNonQuery();

            MessageBox.Show("Record added...");

            ClearFields();
            RecordUITabControl.SelectedTab = ViewRecordTab;
        }

        public void ClearFields()
        {
            AddRecordRid.Clear();
            AddRecordUTdw.SelectedIndex = -1;
            AddRecordUnitdw.SelectedIndex = -1;
            AddRecordPiddw.SelectedIndex = -1;
            AddRecordNoOfDays.Clear();
            

        }

        private void ViewRecordTab_Click(object sender, EventArgs e)
        {
            ViewRecord();
            RecordUITabControl.SelectedTab = ViewRecordTab;
        }

        private void Record_Load(object sender, EventArgs e)
        {
            ViewRecord();
            RecordUITabControl.SelectedTab = ViewRecordTab;
        }

        private void ViewRecord()
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand readSubjectsQuery = new SqlCommand("Select * from RecordTB", con);
            DataTable dataTable = new DataTable();

            con.Open();

            SqlDataReader sqlDataReader = readSubjectsQuery.ExecuteReader();

            dataTable.Load(sqlDataReader);
            con.Close();

            ViewRecordDataGridView.AutoGenerateColumns = true;
            ViewRecordDataGridView.DataSource = dataTable;


        }


        private void ViewRecordDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
            ManageRecord_Populate_PatientID();

            RecordID = ViewRecordDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManageRecordRid.Text = ViewRecordDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManageRecordUTdw.SelectedItem = ViewRecordDataGridView.CurrentRow.Cells[2].Value.ToString();
            ManageRecordUnitdw.SelectedItem = ViewRecordDataGridView.CurrentRow.Cells[3].Value.ToString();
            ManageRecordPiddw.SelectedItem = ViewRecordDataGridView.CurrentRow.Cells[4].Value.ToString();
            ManageRecordNoOfDays.Text = ViewRecordDataGridView.CurrentRow.Cells[5].Value.ToString();
            
            val = 1;
            RecordUITabControl.SelectedTab = ManageRecordTab;
        }

        private void ManageRecordUpdateBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand sqlCommand = new SqlCommand("UPDATE RecordTB SET RecordID = @NewRecordID, UnitType = @NewUnitType, Unit = @NewUnit, PatientID = @NewPid, NoOfDays = @NewNoOfDays WHERE RecordID = @RecordID", connection);
                sqlCommand.CommandType = CommandType.Text;

                sqlCommand.Parameters.AddWithValue("@NewRecordID", ManageRecordRid.Text);
                sqlCommand.Parameters.AddWithValue("@NewUnitType", ManageRecordUTdw.Text);
                sqlCommand.Parameters.AddWithValue("@NewUnit", ManageRecordUnitdw.Text);
                sqlCommand.Parameters.AddWithValue("@NewPid", ManageRecordPiddw.Text);
                sqlCommand.Parameters.AddWithValue("@NewNoOfDays", ManageRecordNoOfDays.Text);
                sqlCommand.Parameters.AddWithValue("@RecordID", this.RecordID);

                connection.Open();

                sqlCommand.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Record Information has been Updated Sucessfully", "Confirmation");

                ClearUpdateFields();
                RecordUITabControl.SelectedTab = ViewRecordTab;
            }
            else
            {
                MessageBox.Show("Please Select a Record to Update ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ClearUpdateFields()
        {
            ManageRecordRid.Clear();
            ManageRecordUTdw.SelectedIndex = -1;
            ManageRecordUnitdw.SelectedIndex = -1;
            ManageRecordPiddw.SelectedIndex = -1;
            ManageRecordNoOfDays.Clear();
           
        }

        private void ManageRecordRemoveBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                if (MessageBox.Show("This wiil Remove the Record permanently. Are You Sure?", "Remove Lab Record", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    SqlCommand command = new SqlCommand("DELETE FROM RecordTB WHERE RecordID = @RecordID", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@RecordID", this.RecordID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("Record remvoed successfully", "Confirmation");


                }

            }
            else
            {
                MessageBox.Show("Please Select a Record to remove ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            ClearUpdateFields();
            RecordUITabControl.SelectedTab = ViewRecordTab;
        }

        public void Record_Populate_PatientID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            AddRecordPiddw.Items.Clear();

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
                AddRecordPiddw.Items.Add(data_r["PatientId"].ToString());
            }

            connection.Close();
        }


       


        public void ManageRecord_Populate_PatientID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            ManageRecordPiddw.Items.Clear();

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
                ManageRecordPiddw.Items.Add(data_r["PatientId"].ToString());
            }

            connection.Close();
        }

        
    }
}
