using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataSetDemo
{
    public partial class Form1 : Form
    {
        string strCon = @"Data Source=DESKTOP-FTT4EST\SQLEXPRESS;Initial Catalog=DBBusiness;Integrated Security=True";
        DataSet dsBusi;
        DataTable DTBInfo;
        SqlConnection con;
        SqlDataAdapter adapter;
        public Form1() 
        {
            InitializeComponent();
            
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            con = new SqlConnection(strCon);
            dsBusi = new DataSet();
            adapter = new SqlDataAdapter(
                " SELECT * " +
                " FROM BInfo " +
                " ORDER By ID ", con);
            FillDsAndDt();
            
        }

        private void FillDsAndDt()
        {
            dsBusi.Clear();
            adapter.Fill(dsBusi, "BInfo");
            DTBInfo = dsBusi.Tables["BInfo"];
            dataGridView1.DataSource = DTBInfo;
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            FillDsAndDt();
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            DataRow dr = DTBInfo.NewRow();
            dr["ID"] = txtID.Text;
            dr["Owner"] = txtOwner.Text;
            dr["BusinessName"] = txtBName.Text;
            dr["Employees"] = txtEmployees.Text;

            DTBInfo.Rows.Add(dr);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in DTBInfo.Rows)
            {
                if(row.RowState != DataRowState.Deleted && row["ID"].ToString() == txtID.Text)
                {
                    row["Owner"] = txtOwner.Text;
                    row["BusinessName"] = txtBName.Text;
                    row["Employees"] = txtEmployees.Text;
                }
            }

        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in DTBInfo.Rows)
            {
                if (row.RowState != DataRowState.Deleted && row["ID"].ToString() == txtID.Text)
                {
                    row.Delete();
                }
            }

        }

        private void btnUpdateDB_Click(object sender, EventArgs e)
        {
            new SqlCommandBuilder(adapter);
            adapter.Update(DTBInfo);
        }

        private void btnSPSBV_Click(object sender, EventArgs e)
        {
            SqlCommand comm = new SqlCommand("SearchUser", con);
            comm.CommandType = CommandType.StoredProcedure;

            SqlParameter parID = new SqlParameter("ID", txtID.Text);
            parID.Direction = ParameterDirection.Input;
            comm.Parameters.Add(parID);

            SqlParameter parOwner = new SqlParameter("Owner", SqlDbType.NVarChar, 50);
            parOwner.Direction = ParameterDirection.Output;
            comm.Parameters.Add(parOwner);

            SqlParameter parErr = new SqlParameter();
            parErr.Direction = ParameterDirection.ReturnValue;
            comm.Parameters.Add(parErr);

            comm.Connection.Open();
            comm.ExecuteNonQuery();
            comm.Connection.Close();

            if ((int)parErr.Value == 0)
                MessageBox.Show(parOwner.Value.ToString());
            else
                MessageBox.Show("ERROR OCCURDED");
        }
    }
}
