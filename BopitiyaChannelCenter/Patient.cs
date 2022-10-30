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
    public partial class Patient : Form
    {
        public int level;
        public String PatientID;
        public int val = 0;
        public int ID;

        public string connectionString = (@"Data Source=DESKTOP-5SU6VUS\SQLEXPRESS;Initial Catalog=BopitiyaCCdb;Integrated Security=True");

        public Patient()
        {
            InitializeComponent();
        }

        private void PatientBackBtn_Click(object sender, EventArgs e)
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
        
        private void AddPatientBtn_Click(object sender, EventArgs e)
        {

            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand comm = connection.CreateCommand();
            comm.CommandType = CommandType.Text;

            comm.CommandText = "SELECT COUNT(ID) AS ID FROM PatientTB";
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

            SqlCommand cmd = new SqlCommand("INSERT INTO PatientTB (ID,PatientID,PatientFullName,Age,Gender,ContactNo,Address,GuardianName) VALUES (@AIVal, @PatientId, @PatientFullName, @Age, @Gender, @ContactNo, @Address, @GuardianName)", connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@PatientId", AddPatientIdInput.Text);
            cmd.Parameters.AddWithValue("@PatientFullName", AddPatientFullNameInput.Text);
            cmd.Parameters.AddWithValue("@Age", AddPatientAgeInput.Text);
            cmd.Parameters.AddWithValue("@Gender", AddPatientGenderDropDown.Text);
            cmd.Parameters.AddWithValue("@ContactNo", AddPatientContactNoInput.Text);
            cmd.Parameters.AddWithValue("@Address", AddPatientAddressInput.Text);
            cmd.Parameters.AddWithValue("@GuardianName", AddPatientGuardianInput.Text);
            cmd.Parameters.AddWithValue("@AIVal", nxt);

            connection.Open();
            cmd.ExecuteNonQuery();

            MessageBox.Show("Patient has been Added Successfully...");

            ClearFields();
            PatientUITabControl.SelectedTab = ViewPatientTab;
        }

        public void ClearFields()
        {
            AddPatientIdInput.Clear();
            AddPatientFullNameInput.Clear();
            AddPatientAgeInput.Clear();
            AddPatientGenderDropDown.SelectedIndex = -1;
            AddPatientContactNoInput.Clear();
            AddPatientAddressInput.Clear();
            AddPatientGuardianInput.Clear();
        }

        private void ViewPatientTab_Click_1(object sender, EventArgs e)
        {
            ViewPatient();
            PatientUITabControl.SelectedTab = ViewPatientTab;
        }

        private void Patient_Load(object sender, EventArgs e)
        {
            ViewPatient();
            PatientUITabControl.SelectedTab = ViewPatientTab;
        }


        private void ViewPatient()
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand readSubjectsQuery = new SqlCommand("Select * from PatientTB", con);
            DataTable dataTable = new DataTable();

            con.Open();

            SqlDataReader sqlDataReader = readSubjectsQuery.ExecuteReader();

            dataTable.Load(sqlDataReader);
            con.Close();

            ViewPatientDataGridView.AutoGenerateColumns = true;
            ViewPatientDataGridView.DataSource = dataTable;


        }


        private void ViewPatientDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            PatientID = ViewPatientDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManagePatientIdInput.Text = ViewPatientDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManagePatientFullNameInput.Text = ViewPatientDataGridView.CurrentRow.Cells[2].Value.ToString();
            ManagePatientAgeInput.Text = ViewPatientDataGridView.CurrentRow.Cells[3].Value.ToString();
            ManagePatientGenderDropDown.SelectedItem = ViewPatientDataGridView.CurrentRow.Cells[4].Value;
            ManagePatientContactNoInput.Text = ViewPatientDataGridView.CurrentRow.Cells[5].Value.ToString();
            ManagePatientAddressInput.Text = ViewPatientDataGridView.CurrentRow.Cells[6].Value.ToString();
            ManagePatientGuardianInput.Text = ViewPatientDataGridView.CurrentRow.Cells[7].Value.ToString();
            val = 1;
            PatientUITabControl.SelectedTab = ManagePatientTab;
        }

        private void ManagePatientUpdateBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand sqlCommand = new SqlCommand("UPDATE PatientTB SET PatientId = @NewPatientId, PatientFullName = @NewPatientFullName, Age = @NewPatientAge, Gender = @NewPatientGender, ContactNo = @NewPatientContactNo, Address = @NewPatientAddress, GuardianName = @NewPatientGuardian WHERE PatientId = @PatientID", connection);
                sqlCommand.CommandType = CommandType.Text;

                sqlCommand.Parameters.AddWithValue("@NewPatientId", ManagePatientIdInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewPatientFullName", ManagePatientFullNameInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewPatientAge", ManagePatientAgeInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewPatientGender", ManagePatientGenderDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@NewPatientContactNo", ManagePatientContactNoInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewPatientAddress", ManagePatientAddressInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewPatientGuardian", ManagePatientGuardianInput.Text);
                sqlCommand.Parameters.AddWithValue("@PatientID", this.PatientID);

                connection.Open();

                sqlCommand.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Patient Information has been Updated Sucessfully", "Confirmation");

                ClearUpdateFields();
                PatientUITabControl.SelectedTab = ViewPatientTab;
            }
            else
            {
                MessageBox.Show("Please Select a patient to Update ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ClearUpdateFields()
        {
            ManagePatientIdInput.Clear();
            ManagePatientFullNameInput.Clear();
            ManagePatientAgeInput.Clear();
            ManagePatientGenderDropDown.SelectedIndex = -1;
            ManagePatientContactNoInput.Clear();
            ManagePatientAddressInput.Clear();
            ManagePatientGuardianInput.Clear();
        }

        private void ManagePatientRemoveBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                if (MessageBox.Show("This wiil Remove the patient permanently. Are You Sure?", "Remove patient member", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    SqlCommand command = new SqlCommand("DELETE FROM PatientTB WHERE PatientId = @PatientID", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@PatientID", this.PatientID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("Patient member remvoed successfully", "Confirmation");


                }

            }
            else
            {
                MessageBox.Show("Please Select a patient member to remove ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            ClearUpdateFields();
            PatientUITabControl.SelectedTab = ViewPatientTab;
        }

        
    }
}
