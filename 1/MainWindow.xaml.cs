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
            LoadTransactions(); // Загрузить существующие транзакции при запуске приложения
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                data.openConnection();

                decimal amount = decimal.Parse(txtСумма.Text); // Извлечь сумму из текстового поля
                string description = txtОписание.Text; // Извлечь описание из текстового поля
                string transactionType = (cmbТипТранзакции.SelectedItem as ComboBoxItem).Content.ToString(); // Извлечь тип транзакции из выпадающего списка

                if (selectedTransactionID == -1)
                {
                    // Если выбранной транзакции нет, вставить новую транзакцию в базу данных
                    string query = "INSERT INTO Transactions (Amount, Description, TransactionDate, TransactionType) " +
                                   "VALUES (@Amount, @Description, GETDATE(), @TransactionType)";

                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@Amount", amount);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@TransactionType", transactionType);

                        command.ExecuteNonQuery(); // Выполнить вставку
                    }
                }
                else
                {
                    // Если выбранная транзакция существует, обновить ее данные в базе данных
                    string query = "UPDATE Transactions " +
                                   "SET Amount = @Amount, Description = @Description, TransactionType = @TransactionType " +
                                   "WHERE TransactionID = @TransactionID";

                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@TransactionID", selectedTransactionID);
                        command.Parameters.AddWithValue("@Amount", amount);
                        command.Parameters.AddWithValue("@Description", description);
                        command.Parameters.AddWithValue("@TransactionType", transactionType);

                        command.ExecuteNonQuery(); // Выполнить обновление
                    }
                }

                data.closeConnection(); // Закрыть соединение с базой данных

                LoadTransactions(); // Перезагрузить отображение транзакций
                ResetForm(); // Сбросить форму для ввода новой транзакции
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message); // В случае ошибки, вывести сообщение
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)listView.SelectedItem;
                int transactionID = Convert.ToInt32(selectedRow["TransactionID"]); // Получить ID выбранной транзакции

                try
                {
                    data.openConnection();

                    string query = "DELETE FROM Transactions WHERE TransactionID = @TransactionID";

                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@TransactionID", transactionID);

                        command.ExecuteNonQuery(); // Выполнить удаление
                    }

                    data.closeConnection(); // Закрыть соединение с базой данных
                    LoadTransactions(); // Перезагрузить отображение транзакций
                    ResetForm(); // Сбросить форму
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления."); // В случае отсутствия выбранной транзакции, вывести сообщение
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)listView.SelectedItem;
                selectedTransactionID = Convert.ToInt32(selectedRow["TransactionID"]); // Запомнить ID выбранной транзакции
                txtСумма.Text = selectedRow["Amount"].ToString(); // Заполнить поля формы данными из выбранной транзакции
                txtОписание.Text = selectedRow["Description"].ToString();
                cmbТипТранзакции.Text = selectedRow["TransactionType"].ToString();
            }
            else
            {
                MessageBox.Show("Выберите запись для редактирования."); // В случае отсутствия выбранной транзакции, вывести сообщение
            }
        }

        private void LoadTransactions()
        {
            try
            {
                data.openConnection();

                string query = "SELECT * FROM Transactions"; // Запрос для выборки всех записей из таблицы Transactions

                using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        adapter.Fill(dataTable); // Заполнить таблицу данными из базы данных

                        listView.ItemsSource = dataTable.DefaultView; // Установить данные в элемент управления ListView
                    }
                }

                data.closeConnection(); // Закрыть соединение с базой данных
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message); // В случае ошибки, вывести сообщение
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count > 0)
            {
                DataRowView selectedRow = (DataRowView)listView.SelectedItem;
                int transactionID = Convert.ToInt32(selectedRow["TransactionID"]); // Получить ID выбранной транзакции

                try
                {
                    data.openConnection();

                    string query = "DELETE FROM Transactions WHERE TransactionID = @TransactionID"; // Запрос на удаление транзакции

                    using (SqlCommand command = new SqlCommand(query, data.GetConnection()))
                    {
                        command.Parameters.AddWithValue("@TransactionID", transactionID);

                        command.ExecuteNonQuery(); // Выполнить удаление
                    }

                    data.closeConnection(); // Закрыть соединение с базой данных
                    LoadTransactions(); // Перезагрузить отображение транзакций
                    ResetForm(); // Сбросить форму
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Произошла ошибка: " + ex.Message); // В случае ошибки, вывести сообщение
                }
            }
            else
            {
                MessageBox.Show("Выберите запись для удаления."); // В случае отсутствия выбранной транзакции, вывести сообщение
            }
        }
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "Сумма" || textBox.Text == "Описание")
            {
                textBox.Text = "";
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
        private void ResetForm()
        {
            selectedTransactionID = -1;
            txtСумма.Text = "Сумма";
            txtОписание.Text = "Описание";
            cmbТипТранзакции.SelectedIndex = -1;
        }
    }
}
