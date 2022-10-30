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
    public partial class Treatment : Form
    {
        public int level;
        public String TreatmentCode;
        public int val = 0;
        public int ID;

        public string connectionString = (@"Data Source=DESKTOP-5SU6VUS\SQLEXPRESS;Initial Catalog=BopitiyaCCdb;Integrated Security=True");

        public Treatment()
        {
            InitializeComponent();
            Treatment_Populate_DoctorID();
            Treatment_Populate_PatientID();
            Treatment_Populate_DiagnosisCode();
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


        private void AddTreatmentBtn_Click(object sender, EventArgs e)
        {

            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand comm = connection.CreateCommand();
            comm.CommandType = CommandType.Text;

            comm.CommandText = "SELECT COUNT(ID) AS ID FROM TreatmentTB";
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

            SqlCommand cmd = new SqlCommand("INSERT INTO TreatmentTB (ID,TreatmentCode, PatientID, DoctorID, DiagnosisCode, StartDate, Prescription, RequiredTests) VALUES (@AIVal, @TreatmentCode, @Pid, @Did, @DiagnosisCode, @StartDate, @Prescription, @RequiredTests)", connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@TreatmentCode", TreatmentCodeInput.Text);
            cmd.Parameters.AddWithValue("@Pid", TreatmentPatientIdDropDown.Text);
            cmd.Parameters.AddWithValue("@Did", TreatmentDoctorIdDropDown.Text);
            cmd.Parameters.AddWithValue("@DiagnosisCode", TreatmentDiagnosisCodeDropDown.Text);
            cmd.Parameters.AddWithValue("@StartDate", TreatmentStartDate.SelectionStart);
            cmd.Parameters.AddWithValue("@Prescription", TreatmentPrescriptionInput.Text);
            cmd.Parameters.AddWithValue("@RequiredTests", TreatmentRTInput.Text);
            cmd.Parameters.AddWithValue("@AIVal", nxt);

            connection.Open();
            cmd.ExecuteNonQuery();

            MessageBox.Show("Treatment added...");

            ClearFields();
            TreatmentUITabControl.SelectedTab = ViewTreatmentTab;
        }

        public void ClearFields()
        {
            TreatmentCodeInput.Clear();
            TreatmentPatientIdDropDown.SelectedIndex = -1;
            TreatmentDoctorIdDropDown.SelectedIndex = -1;
            TreatmentDiagnosisCodeDropDown.SelectedIndex = -1;
            TreatmentPrescriptionInput.Clear();
            TreatmentRTInput.Clear();
        }


        private void ViewTreatmentTab_Click(object sender, EventArgs e)
        {
            ViewTreatment();
            TreatmentUITabControl.SelectedTab = ViewTreatmentTab;
        }

        private void Treatment_Load(object sender, EventArgs e)
        {
            ViewTreatment();
            TreatmentUITabControl.SelectedTab = ViewTreatmentTab;
        }

        private void ViewTreatment()
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand readSubjectsQuery = new SqlCommand("Select * from TreatmentTB", con);
            DataTable dataTable = new DataTable();

            con.Open();

            SqlDataReader sqlDataReader = readSubjectsQuery.ExecuteReader();

            dataTable.Load(sqlDataReader);
            con.Close();

            ViewTreatmentDataGridView.AutoGenerateColumns = true;
            ViewTreatmentDataGridView.DataSource = dataTable;


        }

        private void ViewTreatmentDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ManageTreatment_Populate_DoctorID();
            ManageTreatment_Populate_PatientID();
            ManageTreatment_Populate_DiagnosisCode();

            TreatmentCode = ViewTreatmentDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManageTreatmentCodeInput.Text = ViewTreatmentDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManageTreatmentPatientIdDropDown.SelectedItem = ViewTreatmentDataGridView.CurrentRow.Cells[2].Value.ToString();
            ManageTreatmentDoctorIdDropDown.SelectedItem = ViewTreatmentDataGridView.CurrentRow.Cells[3].Value.ToString();
            ManageTreatmentDiagnosisCodeDropDown.SelectedItem = ViewTreatmentDataGridView.CurrentRow.Cells[4].Value.ToString();
            ManageTreatmentStartDate.SetDate((DateTime)ViewTreatmentDataGridView.CurrentRow.Cells[5].Value);
            ManageTreatmentPrescriptionInput.Text = ViewTreatmentDataGridView.CurrentRow.Cells[6].Value.ToString();
            ManageTreatmentRTInput.Text = ViewTreatmentDataGridView.CurrentRow.Cells[7].Value.ToString();
            val = 1;
            TreatmentUITabControl.SelectedTab = ManageTreatmentTab;
        }

        private void ManageTreatmentUpdateBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand sqlCommand = new SqlCommand("UPDATE TreatmentTB SET TreatmentCode = @NewTreatmentCode, PatientId = @NewPid, DoctorID = @NewDid, DiagnosisCode = @NewDiagnosisCode, StartDate = @NewStartDate, Prescription = @NewPrescription, RequiredTests = @NewRequiredTests WHERE TreatmentCode = @TreatmentCode", connection);
                sqlCommand.CommandType = CommandType.Text;

                sqlCommand.Parameters.AddWithValue("@NewTreatmentCode", ManageTreatmentCodeInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewPid", ManageTreatmentPatientIdDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@NewDid", ManageTreatmentDoctorIdDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@NewDiagnosisCode", ManageTreatmentDiagnosisCodeDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@NewStartDate", ManageTreatmentStartDate.SelectionStart);
                sqlCommand.Parameters.AddWithValue("@NewPrescription", ManageTreatmentPrescriptionInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewRequiredTests", ManageTreatmentRTInput.Text);
                sqlCommand.Parameters.AddWithValue("@TreatmentCode", this.TreatmentCode);

                connection.Open();

                sqlCommand.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Treatment Information has been Updated Sucessfully", "Confirmation");

                ClearUpdateFields();
                TreatmentUITabControl.SelectedTab = ViewTreatmentTab;
            }
            else
            {
                MessageBox.Show("Please Select a treatment to Update ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ClearUpdateFields()
        {
            ManageTreatmentCodeInput.Clear();
            ManageTreatmentPatientIdDropDown.SelectedIndex = -1;
            ManageTreatmentDoctorIdDropDown.SelectedIndex = -1;
            ManageTreatmentDiagnosisCodeDropDown.SelectedIndex = -1;
            ManageTreatmentPrescriptionInput.Clear();
            ManageTreatmentRTInput.Clear();
        }

        private void ManageTreatmentRemoveBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                if (MessageBox.Show("This wiil Remove the Treatment permanently. Are You Sure?", "Remove Treatment", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    SqlCommand command = new SqlCommand("DELETE FROM TreatmentTB WHERE TreatmentCode = @TreatmentCode", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@TreatmentCode", this.TreatmentCode);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("Treatment remvoed successfully", "Confirmation");


                }

            }
            else
            {
                MessageBox.Show("Please Select a treatment to remove ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            ClearUpdateFields();
            TreatmentUITabControl.SelectedTab = ViewTreatmentTab;
        }

        public void Treatment_Populate_PatientID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            TreatmentPatientIdDropDown.Items.Clear();

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
                TreatmentPatientIdDropDown.Items.Add(data_r["PatientId"].ToString());
            }

            connection.Close();
        }


        public void Treatment_Populate_DoctorID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            TreatmentDoctorIdDropDown.Items.Clear();

            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT DoctorID FROM DoctorTB";
            cmd.ExecuteNonQuery();

            DataTable data_t = new DataTable();
            SqlDataAdapter data_a = new SqlDataAdapter(cmd);
            data_a.Fill(data_t);


            foreach (DataRow data_r in data_t.Rows)
            {
                TreatmentDoctorIdDropDown.Items.Add(data_r["DoctorID"].ToString());
            }

            connection.Close();
        }

        public void Treatment_Populate_DiagnosisCode()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            TreatmentDiagnosisCodeDropDown.Items.Clear();

            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT DiagnosisCode FROM DiagnosisTB";
            cmd.ExecuteNonQuery();

            DataTable data_t = new DataTable();
            SqlDataAdapter data_a = new SqlDataAdapter(cmd);
            data_a.Fill(data_t);


            foreach (DataRow data_r in data_t.Rows)
            {
                TreatmentDiagnosisCodeDropDown.Items.Add(data_r["DiagnosisCode"].ToString());
            }

            connection.Close();
        }




        public void ManageTreatment_Populate_PatientID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            ManageTreatmentPatientIdDropDown.Items.Clear();

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
                ManageTreatmentPatientIdDropDown.Items.Add(data_r["PatientId"].ToString());
            }

            connection.Close();
        }


        public void ManageTreatment_Populate_DoctorID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            ManageTreatmentDoctorIdDropDown.Items.Clear();

            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT DoctorID FROM DoctorTB";
            cmd.ExecuteNonQuery();

            DataTable data_t = new DataTable();
            SqlDataAdapter data_a = new SqlDataAdapter(cmd);
            data_a.Fill(data_t);


            foreach (DataRow data_r in data_t.Rows)
            {
                ManageTreatmentDoctorIdDropDown.Items.Add(data_r["DoctorID"].ToString());
            }

            connection.Close();
        }

        public void ManageTreatment_Populate_DiagnosisCode()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            ManageTreatmentDiagnosisCodeDropDown.Items.Clear();

            connection.Open();
            SqlCommand cmd = connection.CreateCommand();
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "SELECT DiagnosisCode FROM DiagnosisTB";
            cmd.ExecuteNonQuery();

            DataTable data_t = new DataTable();
            SqlDataAdapter data_a = new SqlDataAdapter(cmd);
            data_a.Fill(data_t);


            foreach (DataRow data_r in data_t.Rows)
            {
                ManageTreatmentDiagnosisCodeDropDown.Items.Add(data_r["DiagnosisCode"].ToString());
            }

            connection.Close();
        }

    }
}
