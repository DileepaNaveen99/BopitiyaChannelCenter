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
    public partial class Diagnosis : Form
    {

        public int level;
        public String DiagnosisCode;
        public int val = 0;
        public int ID;

        public string connectionString = (@"Data Source=DESKTOP-5SU6VUS\SQLEXPRESS;Initial Catalog=BopitiyaCCdb;Integrated Security=True");

        public Diagnosis()
        {
            InitializeComponent();
            Diagnosis_Populate_PatientID();
            Diagnosis_Populate_DoctorID();
        }

        private void DiagnosisBackBtn_Click(object sender, EventArgs e)
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


        private void AddDiagnosisBtn_Click(object sender, EventArgs e)
        {

            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand comm = connection.CreateCommand();
            comm.CommandType = CommandType.Text;

            comm.CommandText = "SELECT COUNT(ID) AS ID FROM DiagnosisTB";
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

            SqlCommand cmd = new SqlCommand("INSERT INTO DiagnosisTB (ID,DiagnosisCode, PatientID, DoctorID, Symptoms, Date, Diagnosis) VALUES (@AIVal, @DiagnosisCode, @Pid, @Did, @Symptoms, @Date, @Diagnosis)", connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@DiagnosisCode", DiagnosisCodeInput.Text);
            cmd.Parameters.AddWithValue("@Pid", DiagnosisPatientIdDropDown.Text);
            cmd.Parameters.AddWithValue("@Did", DiagnosisDoctorIdDropDown.Text);
            cmd.Parameters.AddWithValue("@Symptoms", DiagnosisSymptomsInput.Text);
            cmd.Parameters.AddWithValue("@Date", DiagnosisDate.SelectionStart);
            cmd.Parameters.AddWithValue("@Diagnosis", DiagnosisInput.Text);
            cmd.Parameters.AddWithValue("@AIVal", nxt);

            connection.Open();
            cmd.ExecuteNonQuery();

            MessageBox.Show("Diagnosis completed...");

            ClearFields();
            DiagnosisUITabControl.SelectedTab = ViewDiagnosisTab;
        }

        public void ClearFields()
        {
            DiagnosisCodeInput.Clear();
            DiagnosisPatientIdDropDown.SelectedIndex = -1;
            DiagnosisDoctorIdDropDown.SelectedIndex = -1;
            DiagnosisSymptomsInput.Clear();
            DiagnosisInput.Clear();
        }


        private void ViewDiagnosisTab_Click(object sender, EventArgs e)
        {
            ViewDiagnosis();
            DiagnosisUITabControl.SelectedTab = ViewDiagnosisTab;
        }

        
        private void Diagnosis_Load(object sender, EventArgs e)
        {
            ViewDiagnosis();
            DiagnosisUITabControl.SelectedTab = ViewDiagnosisTab;
        }


        private void ViewDiagnosis()
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand readSubjectsQuery = new SqlCommand("Select * from DiagnosisTB", con);
            DataTable dataTable = new DataTable();

            con.Open();

            SqlDataReader sqlDataReader = readSubjectsQuery.ExecuteReader();

            dataTable.Load(sqlDataReader);
            con.Close();

            ViewDiagnosisDataGridView.AutoGenerateColumns = true;
            ViewDiagnosisDataGridView.DataSource = dataTable;


        }


        private void ViewDiagnosisDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ManageDiagnosis_Populate_PatientID();
            ManageDiagnosis_Populate_DoctorID();

            DiagnosisCode = ViewDiagnosisDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManageDiagnosisCodeInput.Text = ViewDiagnosisDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManageDiagnosisPatientIdDropDown.SelectedItem = ViewDiagnosisDataGridView.CurrentRow.Cells[2].Value.ToString();
            ManageDiagnosisDoctorIdDropDown.SelectedItem = ViewDiagnosisDataGridView.CurrentRow.Cells[3].Value.ToString();
            ManageDiagnosisSymptomsInput.Text = ViewDiagnosisDataGridView.CurrentRow.Cells[4].Value.ToString();
            ManageDiagnosisDate.SetDate((DateTime)ViewDiagnosisDataGridView.CurrentRow.Cells[5].Value);
            ManageDiagnosisInput.Text = ViewDiagnosisDataGridView.CurrentRow.Cells[6].Value.ToString();
            val = 1;
            DiagnosisUITabControl.SelectedTab = ManageDiagnosisTab;
        }

        private void ManageDiagnosisUpdateBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand sqlCommand = new SqlCommand("UPDATE DiagnosisTB SET DiagnosisCode = @NewDiagnosisCode, PatientId = @NewPid, DoctorID = @NewDid, Symptoms = @NewSymptoms, Date = @NewDate, Diagnosis = @NewDiagnosis WHERE DiagnosisCode = @DiagnosisCode", connection);
                sqlCommand.CommandType = CommandType.Text;

                sqlCommand.Parameters.AddWithValue("@NewDiagnosisCode", ManageDiagnosisCodeInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewPid", ManageDiagnosisPatientIdDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@NewDid", ManageDiagnosisDoctorIdDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@NewSymptoms", ManageDiagnosisSymptomsInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewDate", ManageDiagnosisDate.SelectionStart);
                sqlCommand.Parameters.AddWithValue("@NewDiagnosis", ManageDiagnosisInput.Text);
                sqlCommand.Parameters.AddWithValue("@DiagnosisCode", this.DiagnosisCode);

                connection.Open();

                sqlCommand.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Diagnosis Information has been Updated Sucessfully", "Confirmation");

                ClearUpdateFields();
                DiagnosisUITabControl.SelectedTab = ViewDiagnosisTab;
            }
            else
            {
                MessageBox.Show("Please Select a diagnosis to Update ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ClearUpdateFields()
        {
            ManageDiagnosisCodeInput.Clear();
            ManageDiagnosisPatientIdDropDown.SelectedIndex = -1;
            ManageDiagnosisDoctorIdDropDown.SelectedIndex = -1;
            ManageDiagnosisSymptomsInput.Clear();
            ManageDiagnosisInput.Clear();
        }

        private void ManageDiagnosisRemoveBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                if (MessageBox.Show("This wiil Remove the Diagnosis permanently. Are You Sure?", "Remove Diagnosis", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    SqlCommand command = new SqlCommand("DELETE FROM DiagnosisTB WHERE DiagnosisID = @DiagnosisID", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@DiagnosisID", this.DiagnosisCode);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("Diagnosis remvoed successfully", "Confirmation");


                }

            }
            else
            {
                MessageBox.Show("Please Select a diagnosis to remove ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            ClearUpdateFields();
            DiagnosisUITabControl.SelectedTab = ViewDiagnosisTab;
        }

        public void Diagnosis_Populate_PatientID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            DiagnosisPatientIdDropDown.Items.Clear();

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
                DiagnosisPatientIdDropDown.Items.Add(data_r["PatientId"].ToString());
            }

            connection.Close();
        }


        public void Diagnosis_Populate_DoctorID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            DiagnosisDoctorIdDropDown.Items.Clear();

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
                DiagnosisDoctorIdDropDown.Items.Add(data_r["DoctorID"].ToString());
            }

            connection.Close();
        }



        public void ManageDiagnosis_Populate_PatientID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            ManageDiagnosisPatientIdDropDown.Items.Clear();

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
                ManageDiagnosisPatientIdDropDown.Items.Add(data_r["PatientId"].ToString());
            }

            connection.Close();
        }


        public void ManageDiagnosis_Populate_DoctorID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            ManageDiagnosisDoctorIdDropDown.Items.Clear();

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
                ManageDiagnosisDoctorIdDropDown.Items.Add(data_r["DoctorID"].ToString());
            }

            connection.Close();
        }
    }
}
