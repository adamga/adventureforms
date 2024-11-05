using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FormsApp
{
    public partial class MainForm : Form
    {
        private BindingSource bindingSource;
        private SqlDataAdapter dataAdapter;
        private DataTable dataTable;

        public MainForm()
        {
            InitializeComponent();
            InitializeDataGridView();
            InitializeBindingSource();
            LoadData();
            LoadViews(); // P8289
        }

        private void InitializeDataGridView()
        {
            dataGridView.DataSource = bindingSource;
            dataGridView.AutoGenerateColumns = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView.MultiSelect = false;
        }

        private void InitializeBindingSource()
        {
            bindingSource = new BindingSource();
        }

        private void LoadData()
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["AdventureWorks2014"].ConnectionString;
            string query = "SELECT * FROM HumanResources.vEmployee"; // Example view

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                dataAdapter = new SqlDataAdapter(query, connection);
                SqlCommandBuilder commandBuilder = new SqlCommandBuilder(dataAdapter);

                dataTable = new DataTable();
                dataAdapter.Fill(dataTable);
                bindingSource.DataSource = dataTable;
            }
        }

        private void LoadViews() // P80fc
        {
            DataAccess dataAccess = new DataAccess();
            DataTable viewsTable = dataAccess.GetData("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.VIEWS");

            viewComboBox.Items.Clear();
            foreach (DataRow row in viewsTable.Rows)
            {
                viewComboBox.Items.Add(row["TABLE_NAME"].ToString());
            }

            if (viewComboBox.Items.Count > 0)
            {
                viewComboBox.SelectedIndex = 0;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            DataRow newRow = dataTable.NewRow();
            // Populate newRow with data from input controls
            dataTable.Rows.Add(newRow);
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                DataRow row = ((DataRowView)dataGridView.SelectedRows[0].DataBoundItem).Row;
                // Populate input controls with data from row
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                DataRow row = ((DataRowView)dataGridView.SelectedRows[0].DataBoundItem).Row;
                row.Delete();
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            dataAdapter.Update(dataTable);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            dataTable.RejectChanges();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            string filter = searchTextBox.Text;
            bindingSource.Filter = $"LastName LIKE '%{filter}%'"; // Example filter
        }

        private void viewComboBox_SelectedIndexChanged(object sender, EventArgs e) // P493c
        {
            string selectedView = viewComboBox.SelectedItem.ToString();
            string query = $"SELECT * FROM {selectedView}";

            DataAccess dataAccess = new DataAccess();
            DataTable dataTable = dataAccess.GetData(query);

            bindingSource.DataSource = dataTable;
        }
    }
}
