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

namespace btapQLBH
{
    public partial class frmDMHang : Form
    {
        private bool isAdding; // Biến theo dõi trạng thái thêm mới
        private bool isEditing; // Biến theo dõi trạng thái chỉnh sửa
        public frmDMHang()
        {
            InitializeComponent();
            conn = new SqlConnection();
            down.Connect();
        }
        SqlConnection conn;

        private void frmDMHang_Load(object sender, EventArgs e)
        {
            //Load data vao combo chatlieu
            
            dataGridView_Load();
        }
        private void dataGridView_Load()
        {
            string sql = "select * from tblHang";
            DataTable dt = down.LoadDataToTable(sql);
            dataGridView.DataSource = dt;
            dataGridView.Columns[0].HeaderText = "Ma hang";
            dataGridView.Columns[1].HeaderText = "Ten hang";
            dataGridView.Columns[2].HeaderText = "Ma chat lieu";
            dataGridView.Columns[3].HeaderText = "So luong";
            dataGridView.Columns[4].HeaderText = "Don gia nhap";
            dataGridView.Columns[5].HeaderText = "Don gia ban";
            dataGridView.Columns[6].HeaderText = "Anh";
            dataGridView.Columns[7].HeaderText = "Ghi chu";
            dataGridView.AllowUserToAddRows = false;
        }
        // Phương thức thiết lập trạng thái của form (bật/tắt các điều khiển)
        private void SetFormState(bool enable)
        {
            txtMahang.Enabled = enable; // Bật/tắt ô nhập mã hàng
            txtTenhang.Enabled = enable; // Bật/tắt ô nhập tên hàng
            cboMachatlieu.Enabled = enable; // Bật/tắt ô nhập mã chất liệu
            txtSoluong.Enabled = enable; // Bật/tắt ô nhập số lượng
            txtDongianhap.Enabled = enable; // Bật/tắt ô nhập đơn giá nhập
            txtDongiaban.Enabled = enable; // Bật/tắt ô nhập đơn giá bán
            txtGhichu.Enabled = enable; // Bật/tắt ô nhập ghi chú
            picAnh.Enabled = enable; // Bật/tắt nút chọn ảnh
            btnLuu.Enabled = enable; // Bật/tắt nút Lưu
            btnThem.Enabled = !enable; // Bật/tắt nút Thêm (ngược với trạng thái enable)
            btnXoa.Enabled = !enable; // Bật/tắt nút Xóa
            btnSua.Enabled = !enable; // Bật/tắt nút Sửa
            btnBoqua.Enabled = enable; // Bật/tắt nút Bỏ qua
            btnOpen.Enabled = !enable; // Bật/tắt nút Hiển thị DS
            //btnTimkiem.Enabled = enable;
        }

        // Phương thức xóa trắng các ô nhập liệu trên form
        private void ClearForm()
        {
            txtMahang.Text = ""; // Xóa ô mã hàng
            txtTenhang.Text = ""; // Xóa ô tên hàng
            cboMachatlieu.Text = ""; // Xóa ô mã chất liệu
            txtSoluong.Text = ""; // Xóa ô số lượng
            txtDongianhap.Text = ""; // Xóa ô đơn giá nhập
            txtDongiaban.Text = ""; // Xóa ô đơn giá bán
            txtGhichu.Text = ""; // Xóa ô ghi chú
            picAnh.Image = null; // Xóa ảnh trong PictureBox (nếu có)
        }

        // Phương thức điền dữ liệu từ dòng được chọn trên DataGridView vào các ô nhập liệu
        private void PopulateFormFromGrid()
        {
            if (dataGridView.SelectedRows.Count > 0) // Kiểm tra xem có dòng nào được chọn không
            {
                DataGridViewRow row = dataGridView.SelectedRows[0]; // Lấy dòng được chọn
                txtMahang.Text = row.Cells["MaHang"].Value.ToString(); // Điền mã hàng
                txtTenhang.Text = row.Cells["TenHang"].Value.ToString(); // Điền tên hàng
                cboMachatlieu.Text = row.Cells["MaChatLieu"].Value.ToString(); // Điền mã chất liệu
                txtSoluong.Text = row.Cells["SoLuong"].Value.ToString(); // Điền số lượng
                txtDongianhap.Text = row.Cells["DonGiaNhap"].Value.ToString(); // Điền đơn giá nhập
                txtDongiaban.Text = row.Cells["DonGiaBan"].Value.ToString(); // Điền đơn giá bán
                txtGhichu.Text = row.Cells["GhiChu"].Value.ToString(); // Điền ghi chú
                // Tải ảnh nếu có (giả sử ảnh được lưu dưới dạng đường dẫn)
                if (!string.IsNullOrEmpty(row.Cells["Anh"].Value.ToString()))
                {
                    picAnh.Image = Image.FromFile(row.Cells["Anh"].Value.ToString());
                }
            }
        }

        // Phương thức kiểm tra dữ liệu nhập vào trước khi lưu
        private bool CheckData()
        {
            if (string.IsNullOrWhiteSpace(txtMahang.Text)) // Kiểm tra mã hàng có trống không
            {
                MessageBox.Show("Mã hàng không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtMahang.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTenhang.Text)) // Kiểm tra tên hàng có trống không
            {
                MessageBox.Show("Tên hàng không được để trống!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtTenhang.Focus();
                return false;
            }
            if (!int.TryParse(txtSoluong.Text, out _)) // Kiểm tra số lượng có phải là số nguyên không
            {
                MessageBox.Show("Số lượng phải là số nguyên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtSoluong.Focus();
                return false;
            }
            if (!decimal.TryParse(txtDongianhap.Text, out _)) // Kiểm tra đơn giá nhập có phải là số không
            {
                MessageBox.Show("Đơn giá nhập phải là số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDongianhap.Focus();
                return false;
            }
            if (!decimal.TryParse(txtDongiaban.Text, out _)) // Kiểm tra đơn giá bán có phải là số không
            {
                MessageBox.Show("Đơn giá bán phải là số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtDongiaban.Focus();
                return false;
            }
            return true;
        }

        // Sự kiện nút Thêm: Chuẩn bị form để thêm mới một hàng
        private void btnThem_Click(object sender, EventArgs e)
        {
            isAdding = true; // Đặt trạng thái là đang thêm mới
            isEditing = false; // Không phải trạng thái chỉnh sửa
            ClearForm(); // Xóa trắng form
            SetFormState(true); // Bật các ô nhập liệu
            txtMahang.Focus(); // Đặt con trỏ vào ô mã hàng
        }

        // Sự kiện nút Sửa: Chuẩn bị form để chỉnh sửa một hàng
        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0) // Kiểm tra xem có dòng nào được chọn không
            {
                MessageBox.Show("Vui lòng chọn một hàng để sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isAdding = false; // Không phải trạng thái thêm mới
            isEditing = true; // Đặt trạng thái là đang chỉnh sửa
            SetFormState(true); // Bật các ô nhập liệu
            txtMahang.Enabled = false; // Vô hiệu hóa ô mã hàng (không cho sửa khóa chính)
            txtTenhang.Focus(); // Đặt con trỏ vào ô tên hàng
        }

        // Sự kiện nút Lưu: Lưu dữ liệu (thêm mới hoặc cập nhật)
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (!CheckData()) // Kiểm tra dữ liệu trước khi lưu
            {
                return;
            }

            try
            {
                string sql;
                if (isAdding) // Nếu đang ở trạng thái thêm mới
                {
                    sql = $"INSERT INTO tblHang (MaHang, TenHang, MaChatLieu, SoLuong, DonGiaNhap, DonGiaBan, Anh, GhiChu) " +
                          $"VALUES ('{txtMahang.Text.Trim()}', '{txtTenhang.Text.Trim()}', '{cboMachatlieu.Text.Trim()}', " +
                          $"{int.Parse(txtSoluong.Text)}, {decimal.Parse(txtDongianhap.Text)}, {decimal.Parse(txtDongiaban.Text)}, " +
                          $"'{(picAnh.Image != null ? picAnh.ImageLocation : "")}', '{txtGhichu.Text.Trim()}')";
                }
                else // Nếu đang ở trạng thái chỉnh sửa
                {
                    sql = $"UPDATE tblHang SET TenHang = '{txtTenhang.Text.Trim()}', MaChatLieu = '{cboMachatlieu.Text.Trim()}', " +
                          $"SoLuong = {int.Parse(txtSoluong.Text)}, DonGiaNhap = {decimal.Parse(txtDongianhap.Text)}, " +
                          $"DonGiaBan = {decimal.Parse(txtDongiaban.Text)}, Anh = '{(picAnh.Image != null ? picAnh.ImageLocation : "")}', " +
                          $"GhiChu = '{txtGhichu.Text.Trim()}' WHERE MaHang = '{txtMahang.Text.Trim()}'";
                }

                down.ExecuteNonQuery(sql); // Thực thi câu lệnh SQL
                MessageBox.Show("Lưu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                dataGridView_Load(); // Làm mới DataGridView
                SetFormState(false); // Tắt các ô nhập liệu
                isAdding = false; // Đặt lại trạng thái thêm mới
                isEditing = false; // Đặt lại trạng thái chỉnh sửa
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Sự kiện nút Xóa: Xóa một hàng được chọn
        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0) // Kiểm tra xem có dòng nào được chọn không
            {
                MessageBox.Show("Vui lòng chọn một hàng để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa hàng này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string maHang = dataGridView.SelectedRows[0].Cells["MaHang"].Value.ToString(); // Lấy mã hàng
                    string sql = $"DELETE FROM tblHang WHERE MaHang = '{maHang}'"; // Câu lệnh SQL xóa
                    down.ExecuteNonQuery(sql); // Thực thi câu lệnh xóa
                    MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dataGridView_Load(); // Làm mới DataGridView
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Sự kiện nút Bỏ qua: Hủy thao tác hiện tại
        private void btnBoqua_Click(object sender, EventArgs e)
        {
            ClearForm(); // Xóa trắng form
            SetFormState(false); // Tắt các ô nhập liệu
            isAdding = false; // Đặt lại trạng thái thêm mới
            isEditing = false; // Đặt lại trạng thái chỉnh sửa
            if (dataGridView.SelectedRows.Count > 0) // Nếu có dòng được chọn, điền lại dữ liệu
            {
                PopulateFormFromGrid();
            }
        }

        // Sự kiện nút Hiển thị DS: Làm mới DataGridView
        private void btnOpen_Click(object sender, EventArgs e)
        {
            dataGridView_Load(); // Tải lại dữ liệu vào DataGridView
        }

        // Sự kiện khi chọn một dòng trên DataGridView: Điền dữ liệu vào form
        private void dataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (!isAdding && !isEditing) // Chỉ điền dữ liệu nếu không ở trạng thái thêm mới hoặc chỉnh sửa
            {
                PopulateFormFromGrid();
            }
        }

        // Sự kiện nút chọn ảnh: Mở hộp thoại để chọn file ảnh
        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"; // Lọc các định dạng ảnh
                if (openFileDialog.ShowDialog() == DialogResult.OK) // Nếu người dùng chọn file
                {
                    picAnh.Image = Image.FromFile(openFileDialog.FileName); // Hiển thị ảnh trong PictureBox
                    picAnh.ImageLocation = openFileDialog.FileName; // Lưu đường dẫn ảnh
                }
            }
        }

        private void dataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string maChatLieu="";
            if (dataGridView.SelectedRows.Count == 0) 
            {
                MessageBox.Show("Không có dữ liệu để chọn");

            }

        }

    }
}
