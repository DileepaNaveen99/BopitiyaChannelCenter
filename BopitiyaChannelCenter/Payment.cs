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
    public partial class Payment : Form
    {

        public int level;
        public String PaymentID;
        public int val = 0;
        public int ID;

        public string connectionString = (@"Data Source=DESKTOP-5SU6VUS\SQLEXPRESS;Initial Catalog=BopitiyaCCdb;Integrated Security=True");

        public Payment()
        {
            InitializeComponent();
            Payment_Populate_PatientID();
        }

        private void PaymentBackBtn_Click(object sender, EventArgs e)
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


        private void AddPaymentBtn_Click(object sender, EventArgs e)
        {

            SqlConnection connection = new SqlConnection(connectionString);

            connection.Open();
            SqlCommand comm = connection.CreateCommand();
            comm.CommandType = CommandType.Text;

            comm.CommandText = "SELECT COUNT(ID) AS ID FROM PaymentTB";
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

            SqlCommand cmd = new SqlCommand("INSERT INTO PaymentTB (ID,PaymentID, PatientID, Amount, Date) VALUES (@AIVal, @PaymentID, @Pid, @Amount, @Date)", connection);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@PaymentID", AddPaymentId.Text);
            cmd.Parameters.AddWithValue("@Pid", AddPaymentPiddw.Text);
            cmd.Parameters.AddWithValue("@Amount", AddPaymentAmount.Text);
            cmd.Parameters.AddWithValue("@Date", AddPaymentDate.SelectionStart);
            cmd.Parameters.AddWithValue("@AIVal", nxt);

            connection.Open();
            cmd.ExecuteNonQuery();

            MessageBox.Show("Payment added...");

            ClearFields();
            PaymentUITabControl.SelectedTab = ViewPaymentTab;
        }

        public void ClearFields()
        {
            AddPaymentId.Clear();
            AddPaymentPiddw.SelectedIndex = -1;
            AddPaymentAmount.Clear();


        }

        private void ViewPaymentTab_Click_1(object sender, EventArgs e)
        {
            ViewPayment();
            PaymentUITabControl.SelectedTab = ViewPaymentTab;
        }

        private void Payment_Load(object sender, EventArgs e)
        {
            ViewPayment();
            PaymentUITabControl.SelectedTab = ViewPaymentTab;
        }


        private void ViewPayment()
        {
            SqlConnection con = new SqlConnection(connectionString);

            SqlCommand readSubjectsQuery = new SqlCommand("Select * from PaymentTB", con);
            DataTable dataTable = new DataTable();

            con.Open();

            SqlDataReader sqlDataReader = readSubjectsQuery.ExecuteReader();

            dataTable.Load(sqlDataReader);
            con.Close();

            ViewPaymentDataGridView.AutoGenerateColumns = true;
            ViewPaymentDataGridView.DataSource = dataTable;


        }

        private void ViewPaymentDataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            ManageRecord_Populate_PatientID();

            PaymentID = ViewPaymentDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManagePaymentId.Text = ViewPaymentDataGridView.CurrentRow.Cells[1].Value.ToString();
            ManagePaymentPiddw.SelectedItem = ViewPaymentDataGridView.CurrentRow.Cells[2].Value.ToString();
            ManagePaymentAmount.Text = ViewPaymentDataGridView.CurrentRow.Cells[3].Value.ToString();
            ManagePaymentDate.SetDate((DateTime)ViewPaymentDataGridView.CurrentRow.Cells[4].Value);
            val = 1;
            PaymentUITabControl.SelectedTab = ManagePaymentTab;
        }

        private void ManagePaymentUpdateBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                SqlCommand sqlCommand = new SqlCommand("UPDATE PaymentTB SET PaymentID = @NewPaymentID, PatientID = @NewPid, Amount = @NewAmount, Date = @NewDate WHERE PaymentID = @PaymentID", connection);
                sqlCommand.CommandType = CommandType.Text;

                sqlCommand.Parameters.AddWithValue("@NewPaymentID", ManagePaymentId.Text);
                sqlCommand.Parameters.AddWithValue("@NewPid", ManagePaymentPiddw.Text);
                sqlCommand.Parameters.AddWithValue("@NewAmount", ManagePaymentAmount.Text);
                sqlCommand.Parameters.AddWithValue("@NewDate", ManagePaymentDate.SelectionStart);
                sqlCommand.Parameters.AddWithValue("@PaymentID", this.PaymentID);

                connection.Open();

                sqlCommand.ExecuteNonQuery();

                connection.Close();

                MessageBox.Show("Payment Information has been Updated Sucessfully", "Confirmation");

                ClearUpdateFields();
                PaymentUITabControl.SelectedTab = ViewPaymentTab;
            }
            else
            {
                MessageBox.Show("Please Select a Payment to Update ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public void ClearUpdateFields()
        {
            ManagePaymentId.Clear();
            ManagePaymentPiddw.SelectedIndex = -1;
            ManagePaymentAmount.Clear();

        }

        private void ManagePaymentRemoveBtn_Click(object sender, EventArgs e)
        {
            if (val > 0)
            {
                SqlConnection connection = new SqlConnection(connectionString);

                if (MessageBox.Show("This wiil Remove the Payment permanently. Are You Sure?", "Remove Lab Payment", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    SqlCommand command = new SqlCommand("DELETE FROM PaymentTB WHERE PaymentID = @PaymentID", connection);
                    command.CommandType = CommandType.Text;

                    command.Parameters.AddWithValue("@PaymentID", this.PaymentID);

                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();

                    MessageBox.Show("Payment remvoed successfully", "Confirmation");


                }

            }
            else
            {
                MessageBox.Show("Please Select a Payment to remove ", "Select?", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            ClearUpdateFields();
            PaymentUITabControl.SelectedTab = ViewPaymentTab;
        }

        public void Payment_Populate_PatientID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            AddPaymentPiddw.Items.Clear();

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
                AddPaymentPiddw.Items.Add(data_r["PatientId"].ToString());
            }

            connection.Close();
        }





        public void ManageRecord_Populate_PatientID()
        {
            SqlConnection connection = new SqlConnection(connectionString);

            ManagePaymentPiddw.Items.Clear();

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
                ManagePaymentPiddw.Items.Add(data_r["PatientId"].ToString());
            }

            connection.Close();
        }

        
    }
}
