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
    public partial class Staff : Form
    {
        public int level;
        public String StaffID;
        public int val = 0;
        public int ID;

        public string connectionString = (@"Data Source=DESKTOP-5SU6VUS\SQLEXPRESS;Initial Catalog=BopitiyaCCdb;Integrated Security=True");

        public Staff()
        {
            InitializeComponent();
        }

        private void StaffBackBtn_Click(object sender, EventArgs e)
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

        private void AddStaffBtn_Click(object sender, EventArgs e)
        {

            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand comm = connection.CreateCommand();
            comm.CommandType = CommandType.Text;

            comm.CommandText = "SELECT COUNT(ID) AS ID FROM StaffTB";
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

            SqlCommand cmd = new SqlCommand("INSERT INTO StaffTB (ID,StaffFirstName,StaffSurname,StaffID,StaffType,RegNo,JoinedDate,UnitType,Unit) VALUES (@AIVal, @StaffFirstName, @StaffSurname, @StaffID, @StaffType, @RegNo, @JoinedDate, @UnitType, @Unit)", connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@StaffFirstName", AddStaffFNameInput.Text);
            cmd.Parameters.AddWithValue("@StaffSurname", AddStaffSNameInput.Text);
            cmd.Parameters.AddWithValue("@StaffID", AddStaffIDInput.Text);
            cmd.Parameters.AddWithValue("@StaffType", AddStaffTypeDropDown.Text);
            cmd.Parameters.AddWithValue("@RegNo", AddStaffRegNoInput.Text);
            cmd.Parameters.AddWithValue("@JoinedDate", AddStaffJoinedDate.SelectionStart);
            cmd.Parameters.AddWithValue("@UnitType", AddStaffUnitTypeDropDown.Text);
            cmd.Parameters.AddWithValue("@Unit", AddStaffUnitDropDown.Text);
            cmd.Parameters.AddWithValue("@AIVal", nxt);

            connection.Open();
            cmd.ExecuteNonQuery();

            MessageBox.Show("Staff has been Added Successfully...");

            ClearFields();
            StaffUITabControl.SelectedTab = ViewStaffTab;
        }


        public void ClearFields()
        {
            AddStaffFNameInput.Clear();
            AddStaffSNameInput.Clear();
            AddStaffIDInput.Clear();
            AddStaffTypeDropDown.SelectedIndex = -1;
            AddStaffRegNoInput.Clear();
            AddStaffUnitTypeDropDown.SelectedIndex = -1;
            AddStaffUnitDropDown.SelectedIndex = -1;
        }

        private void ViewStaffTab_Click(object sender, EventArgs e)
        {
            ViewStaff();
            StaffUITabControl.SelectedTab = ViewStaffTab;
        }

        private void Staff_Load(object sender, EventArgs e)
        {
            ViewStaff();
            StaffUITabControl.SelectedTab = ViewStaffTab;
        }

        private void ViewStaff()
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand readSubjectsQuery = new SqlCommand("Select * from StaffTB", con);
            DataTable dataTable = new DataTable();

            con.Open();

            SqlDataReader sqlDataReader = readSubjectsQuery.ExecuteReader();

            dataTable.Load(sqlDataReader);
            con.Close();

            ViewStaffDataGridView.AutoGenerateColumns = true;
            ViewStaffDataGridView.DataSource = dataTable;


        }

        private void ViewStaffDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            StaffID = ViewStaffDataGridView.CurrentRow.Cells[3].Value.ToString();
            ManageStaffFNameInput.Text = ViewStaffDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManageStaffSNameInput.Text = ViewStaffDataGridView.CurrentRow.Cells[2].Value.ToString();
            ManageStaffIdInput.Text = ViewStaffDataGridView.CurrentRow.Cells[3].Value.ToString();
            ManageStaffTypeDropDown.SelectedItem = ViewStaffDataGridView.CurrentRow.Cells[4].Value;
            ManageStaffRegNoInput.Text = ViewStaffDataGridView.CurrentRow.Cells[5].Value.ToString();
            ManageStaffJoinedDate.SetDate((DateTime)ViewStaffDataGridView.CurrentRow.Cells[6].Value);
            ManageStaffUnitTypeDropDown.SelectedItem = ViewStaffDataGridView.CurrentRow.Cells[7].Value;
            ManageStaffUnitDropDown.SelectedItem = ViewStaffDataGridView.CurrentRow.Cells[8].Value;
            val = 1;
            StaffUITabControl.SelectedTab = ManageStaffTab;
        }

        private void ManageStaffUpdateBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                //if (IsValidUpdate())
                //{
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand sqlCommand = new SqlCommand("UPDATE StaffTB SET StaffFirstName = @NewStaffFName, StaffSurname = @NewStaffSName, StaffID = @NewStaffID, StaffType = @NewStaffType, RegNo = @NewStaffRegNo, JoinedDate = @NewJoinedDate, UnitType = @NewUnitType, Unit = @NewUnit WHERE StaffID = @StaffID", connection);
                sqlCommand.CommandType = CommandType.Text;

                sqlCommand.Parameters.AddWithValue("@NewStaffFName", ManageStaffFNameInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewStaffSName", ManageStaffSNameInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewStaffID", ManageStaffIdInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewStaffType", ManageStaffTypeDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@NewStaffRegNo", ManageStaffRegNoInput.Text);
                sqlCommand.Parameters.AddWithValue("@NewJoinedDate", ManageStaffJoinedDate.SelectionStart);
                sqlCommand.Parameters.AddWithValue("@NewUnitType", ManageStaffUnitTypeDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@NewUnit", ManageStaffUnitDropDown.Text);
                sqlCommand.Parameters.AddWithValue("@StaffID", this.StaffID);

                connection.Open();

                sqlCommand.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Staff member Information has been Updated Sucessfully", "Confirmation");

                ClearUpdateFields();
                StaffUITabControl.SelectedTab = ViewStaffTab;
            }
            else
            {
                MessageBox.Show("Please Select a staff member to Update ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ClearUpdateFields()
        {
            ManageStaffIdInput.Clear();
            ManageStaffFNameInput.Clear();
            ManageStaffSNameInput.Clear();
            ManageStaffIdInput.Clear();
            ManageStaffTypeDropDown.SelectedIndex = -1;
            ManageStaffRegNoInput.Clear();
            ManageStaffUnitTypeDropDown.SelectedIndex = -1;
            ManageStaffUnitDropDown.SelectedIndex = -1;
        }

        private void ManageStaffRemoveBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                if (MessageBox.Show("This wiil Remove the staff permanently. Are You Sure?", "Remove staff member", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    SqlCommand command = new SqlCommand("DELETE FROM StaffTB WHERE StaffID = @StaffID", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@StaffID", this.StaffID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("Staff member remvoed successfully", "Confirmation");


                }

            }
            else
            {
                MessageBox.Show("Please Select a staff member to remove ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            ClearUpdateFields();
            StaffUITabControl.SelectedTab = ViewStaffTab;
        }
    }
}
