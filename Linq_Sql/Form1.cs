using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Linq_Sql
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        LINQ_SQLDataContext db;

        private void ClearTextBoxes()
        {
            foreach (Control control in this.Controls)
            {
                if (control is TextBox)
                {
                    TextBox txt=control as TextBox;
                    txt.Clear();
                }
            }
            NametextBox.Focus();
        }
        private void BindGridView()
        {
            db=new LINQ_SQLDataContext();
            dataGridView1.DataSource = db.Students.ToList();

        }


        private void Insertbutton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NametextBox.Text) ||
                string.IsNullOrWhiteSpace(AgetextBox.Text) ||
                string.IsNullOrWhiteSpace(ClasstextBox.Text))
            {
                MessageBox.Show(
                    "Please fill in all required fields.",
                    "Missing Information",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(AgetextBox.Text, out int age) ||
                !int.TryParse(ClasstextBox.Text, out int std))
            {
                MessageBox.Show(
                    "Age and Class must contain valid numbers.",
                    "Invalid Input",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            try
            {
                db = new LINQ_SQLDataContext();

                Student s = new Student
                {
                    studentName = NametextBox.Text,
                    gender = GendertextBox.Text,
                    age = age,
                    std = std
                };

                db.Students.InsertOnSubmit(s);
                db.SubmitChanges();

                MessageBox.Show(
                    "Student record has been saved successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                ClearTextBoxes();
                BindGridView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An unexpected error occurred:\n" + ex.Message,
                    "System Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        private void Clearbutton_Click(object sender, EventArgs e)
        {
            ClearTextBoxes();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindGridView();
        }

        int selectedId = 0;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                selectedId = Convert.ToInt32(row.Cells[0].Value);


                NametextBox.Text = row.Cells[1].Value.ToString();
                GendertextBox.Text = row.Cells[2].Value.ToString();
                AgetextBox.Text = row.Cells[3].Value.ToString();
                ClasstextBox.Text = row.Cells[4].Value.ToString();
            }
        }


        private void Updatebutton_Click(object sender, EventArgs e)
        {

            using (var db = new LINQ_SQLDataContext())
            {
                var student = db.Students.FirstOrDefault(x => x.Id == selectedId);

                if (student == null)
                {
                    MessageBox.Show(
                        "The selected record could not be found in the database.",
                        "Record Not Found",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return;
                }

                student.studentName = NametextBox.Text;
                student.gender = GendertextBox.Text;
                student.age = int.Parse(AgetextBox.Text);
                student.std = int.Parse(ClasstextBox.Text);

                db.SubmitChanges();
                MessageBox.Show(
                    "Student record updated successfully.",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            BindGridView();
            ClearTextBoxes();
        }

        private void Deletebutton_Click(object sender, EventArgs e)
        {
            if (selectedId == 0)
            {
                MessageBox.Show("Please select a record to delete");
                return;
            }

            var confirm = MessageBox.Show(
      "Are you sure you want to permanently delete this student record?",
      "Confirm Deletion",
      MessageBoxButtons.YesNo,
      MessageBoxIcon.Warning);


            if (confirm == DialogResult.Yes)
            {
                using (var db = new LINQ_SQLDataContext())
                {
                    var student = db.Students.SingleOrDefault(x => x.Id == selectedId);

                    if (student != null)
                    {
                        db.Students.DeleteOnSubmit(student);
                        db.SubmitChanges();

                        MessageBox.Show(
                            "Student record deleted successfully.",
                            "Deleted",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                }

                BindGridView();
                ClearTextBoxes();
                selectedId = 0;
            }
        }

    }
}
