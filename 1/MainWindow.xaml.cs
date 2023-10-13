using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace _1
{
    public partial class MainWindow : Window
    {
        Data data = new Data();
        private int selectedTransactionID = -1;

        public MainWindow()
        {
            InitializeComponent();
            LoadTransactions();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                data.openConnection();

                decimal amount = decimal.Parse(txtСумма.Text);
                string description = txtОписание.Text;
                string transactionType = (cmbТипТранзакции.SelectedItem as ComboBoxItem).Content.ToString();

                if (selectedTransactionID == -1)
                {
                    string query = "INSERT INTO Transactions (Amount, Description, TransactionDate, TransactionType) " +
                                   "VALUES (@Amount, @Description, GETDATE(), @TransactionType)";

                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@Amount", amount);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@TransactionType", transactionType);

                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    string query = "UPDATE Transactions " +
                                   "SET Amount = @Amount, Description = @Description, TransactionType = @TransactionType " +
                                   "WHERE TransactionID = @TransactionID";

                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@TransactionID", selectedTransactionID);
                        command.Parameters.AddWithValue("@Amount", amount);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@TransactionType", transactionType);

                        command.ExecuteNonQuery();
                    }
                }

                data.closeConnection();

                LoadTransactions();
                ResetForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }
        private void txtСумма_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsNumeric(e.Text))
            {
                e.Handled = true; // Отменить ввод неправильных символов
            }
        }

        private bool IsNumeric(string text)
        {
            return !string.IsNullOrEmpty(text) && text.All(char.IsDigit);
        }


        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)listView.SelectedItem;
                int transactionID = Convert.ToInt32(selectedRow["TransactionID"]);

                try
                {
                    data.openConnection();

                    string query = "DELETE FROM Transactions WHERE TransactionID = @TransactionID";

                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@TransactionID", transactionID);

                        command.ExecuteNonQuery();
                    }

                    data.closeConnection();
                    LoadTransactions();
                    ResetForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)listView.SelectedItem;
                selectedTransactionID = Convert.ToInt32(selectedRow["TransactionID"]);
                txtСумма.Text = selectedRow["Amount"].ToString();
                txtОписание.Text = selectedRow["Description"].ToString();
                cmbТипТранзакции.Text = selectedRow["TransactionType"].ToString();
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования.");
            }
        }

        private void LoadTransactions()
        {
            try
            {
                data.openConnection();

                string query = "SELECT * FROM Transactions";

                using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        listView.ItemsSource = dataTable.DefaultView;
                    }
                }

                data.closeConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }
        private void Delete(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)listView.SelectedItem;
                int transactionID = Convert.ToInt32(selectedRow["TransactionID"]);

                try
                {
                    data.openConnection();

                    string query = "DELETE FROM Transactions WHERE TransactionID = @TransactionID";

                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@TransactionID", transactionID);

                        command.ExecuteNonQuery();
                    }

                    data.closeConnection();
                    LoadTransactions();
                    ResetForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления.");
            }
        }


        private void ResetForm()
        {
            selectedTransactionID = -1;
            txtСумма.Text = "Сумма";
            txtОписание.Text = "Описание";
            cmbТипТранзакции.SelectedIndex = -1;
        }
    }
}
