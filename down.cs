using System.Data.SqlClient;
using System.Data;
using System;
using System.Windows.Forms;

namespace btapQLBH
{
    internal class down
    {
        public static SqlConnection con = new SqlConnection();
        public static string ConnectionString = "Data Source=DLINH-0406\\SQLEXPRESS;" + "Initial Catalog=QuanLyBanHang;" + "Integrated Security=True";

        public static void Connect()
        {
            con.ConnectionString = ConnectionString;
            try
            {
                if (con != null && con.State == ConnectionState.Closed)
                    con.Open();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void Close()
        {
            try
            {
                if (con != null && con.State == ConnectionState.Open)
                    con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void FillDataToCombo (ComboBox cmb, string sql, string value, string display)
        {
            SqlDataAdapter sqlDataAdapter= new SqlDataAdapter(sql, con);
            DataTable dt = new DataTable();
            sqlDataAdapter.Fill(dt);
            cmb.DataSource = dt;
            cmb.ValueMember = value;
            cmb.DisplayMember = display;
        }

        public static DataTable LoadDataToTable(string sql)
        {
            DataTable dt = new DataTable();
            SqlDataAdapter adapter = new SqlDataAdapter(sql, down.con);
            adapter.Fill(dt);
            return dt;

        }

        // Thêm phương thức ExecuteNonQuery
        public static void ExecuteNonQuery(string sql)
        {
            try
            {
                Connect(); // Mở kết nối
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.ExecuteNonQuery(); // Thực thi câu lệnh
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Close(); // Đóng kết nối
            }
        }
        public static string getValueFromMa(string sql)
        {
            string ketQua = "";
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader reader = cmd.ExecuteReader();
            return reader.GetString(0);
        }
    }
}