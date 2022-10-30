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
    public partial class Doctor : Form
    {
        public int level;
        public String DoctorID;
        public int val = 0;
        public int ID;

        public string connectionString = (@"Data Source=DESKTOP-5SU6VUS\SQLEXPRESS;Initial Catalog=BopitiyaCCdb;Integrated Security=True");

        public Doctor()
        {
            InitializeComponent();
        }



        private void DocBackBtn_Click(object sender, EventArgs e)
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

        private void AddDoctorBtn_Click(object sender, EventArgs e)
        {

            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand comm = connection.CreateCommand();
            comm.CommandType = CommandType.Text;

            comm.CommandText = "SELECT COUNT(ID) AS ID FROM DoctorTB";
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

            SqlCommand cmd = new SqlCommand("INSERT INTO DoctorTB (ID,DoctorFirstName,DoctorSurname,DoctorID,SLMCregNo,Specialization,JoinedDate,UnitType,Unit) VALUES (@AIVal, @DoctorFirstName, @DoctorSurname, @DoctorID, @SLMCregNo, @Specialization, @JoinedDate, @UnitType, @Unit)", connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@DoctorFirstName", AddDoctorFNameInput.Text);
            cmd.Parameters.AddWithValue("@DoctorSurname", AddDoctorSNameInput.Text);
            cmd.Parameters.AddWithValue("@DoctorID", AddDoctorIDInput.Text);
            cmd.Parameters.AddWithValue("@SLMCregNo", AddDoctorRegNoInput.Text);
            cmd.Parameters.AddWithValue("@Specialization", AddDoctorSpDropDown.Text);
            cmd.Parameters.AddWithValue("@JoinedDate", AddDoctorJoinedDate.SelectionStart);
            cmd.Parameters.AddWithValue("@UnitType", AddDoctorUnitTypeDropDown.Text);
            cmd.Parameters.AddWithValue("@Unit", AddDoctorUnitDropDown.Text);
            cmd.Parameters.AddWithValue("@AIVal", nxt);

            connection.Open();
            cmd.ExecuteNonQuery();

            MessageBox.Show("Doctor has been Added Successfully...");

            ClearFields();
            DoctorUITabControl.SelectedTab = ViewDoctorsTab;
        }

        public void ClearFields()
        {
            AddDoctorFNameInput.Clear();
            AddDoctorSNameInput.Clear();
            AddDoctorIDInput.Clear();
            AddDoctorRegNoInput.Clear();
            AddDoctorSpDropDown.SelectedIndex = -1;
            AddDoctorUnitTypeDropDown.SelectedIndex = -1;
            AddDoctorUnitDropDown.SelectedIndex = -1;
        }

        private void ViewDoctorsTab_Click(object sender, EventArgs e)
        {
            ViewDoctors();
            DoctorUITabControl.SelectedTab = ViewDoctorsTab;
        }

        private void ViewDoctors()
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand readSubjectsQuery = new SqlCommand("Select * from DoctorTB", con);
            DataTable dataTable = new DataTable();

            con.Open();

            SqlDataReader sqlDataReader = readSubjectsQuery.ExecuteReader();

            dataTable.Load(sqlDataReader);
            con.Close();

            ViewDoctorsDataGridView.AutoGenerateColumns = true;
            ViewDoctorsDataGridView.DataSource = dataTable;


        }

       

        private void ViewDoctorsDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DoctorID = ViewDoctorsDataGridView.CurrentRow.Cells[3].Value.ToString();
            ManageDoctorFNameInput.Text = ViewDoctorsDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManageDoctorSNameInput.Text = ViewDoctorsDataGridView.CurrentRow.Cells[2].Value.ToString();
            ManageDoctorIdInput.Text = ViewDoctorsDataGridView.CurrentRow.Cells[3].Value.ToString();
            ManageDoctorRegNoInput.Text = ViewDoctorsDataGridView.CurrentRow.Cells[4].Value.ToString();
            ManageDoctorSpDropDown.SelectedItem = ViewDoctorsDataGridView.CurrentRow.Cells[5].Value;
            ManageDoctorJoinedDate.SetDate((DateTime)ViewDoctorsDataGridView.CurrentRow.Cells[6].Value);
            ManageDoctorUnitTypeDropDown.SelectedItem = ViewDoctorsDataGridView.CurrentRow.Cells[7].Value;
            ManageDoctorUnitDropDown.SelectedItem = ViewDoctorsDataGridView.CurrentRow.Cells[8].Value;
            val = 1;
            DoctorUITabControl.SelectedTab = ManageDoctorTab;
        }

        private void ManageDoctorUpdateBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand sqlCommand = new SqlCommand("UPDATE DoctorTB SET DoctorFirstName = @NewDocFName, DoctorSurname = @NewDocSName, DoctorID = @NewDocID, SLMCregNo = @NewRegNo, Specialization = @NewSpecialization, JoinedDate = @NewJoinedDate, UnitType = @NewUnitType, Unit = @NewUnit WHERE DoctorID = @DocID", connection);

                sqlCommand.CommandType = CommandType.Text;

                sqlCommand.Parameters.AddWithValue("@NewDocFName", ManageDoctorFNameInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewDocSName", ManageDoctorSNameInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewDocID", ManageDoctorIdInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewRegNo", ManageDoctorRegNoInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewSpecialization", ManageDoctorSpDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@NewJoinedDate", ManageDoctorJoinedDate.SelectionStart);
                sqlCommand.Parameters.AddWithValue("@NewUnitType", ManageDoctorUnitTypeDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@NewUnit", ManageDoctorUnitDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@DocID", this.DoctorID);

                connection.Open();

                sqlCommand.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Doctor Information has been Updated Sucessfully", "Confirmation");

                ClearUpdateFields();
                DoctorUITabControl.SelectedTab = ViewDoctorsTab;
            }
            else
            {
                MessageBox.Show("Please Select a doctor to Update ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ClearUpdateFields()
        {
            ManageDoctorIdInput.Clear();
            ManageDoctorFNameInput.Clear();
            ManageDoctorSNameInput.Clear();
            ManageDoctorIdInput.Clear();
            ManageDoctorRegNoInput.Clear();
            ManageDoctorSpDropDown.SelectedIndex = -1;
            ManageDoctorUnitTypeDropDown.SelectedIndex = -1;
            ManageDoctorUnitDropDown.SelectedIndex = -1;
        }

        private void ManageDoctorRemoveBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                if (MessageBox.Show("This wiil Remove the Doctor permanently. Are You Sure?", "Remove Doctor", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    SqlCommand command = new SqlCommand("DELETE FROM DoctorTB WHERE DoctorID = @DoctorID", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@DoctorID", this.DoctorID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("Doctor remvoed successfully", "Confirmation");


                }

            }
            else
            {
                MessageBox.Show("Please Select a doctor to remove ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            ClearUpdateFields();
            DoctorUITabControl.SelectedTab = ViewDoctorsTab;
        }
    }


}
