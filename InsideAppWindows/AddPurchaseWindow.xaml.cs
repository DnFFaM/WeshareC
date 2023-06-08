using System;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WeshareC.InsideAppWindows
{
    public partial class AddPurchaseWindow : Window
    {
        private readonly string connectionString;
        private string loggedInUserName;
        private SqlConnection connection;
        private string username;

        public AddPurchaseWindow(string userName, SqlConnection sqlConnection)
        {
            InitializeComponent();
            username = userName;
            connection = sqlConnection;

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            connectionString = configuration.GetConnectionString("MyConnectionString");
            loggedInUserName = userName;

            ReadAllGroups();
        }
        private void ReadAllGroups()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT DISTINCT GroupName FROM Groups";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string groupName = reader.GetString(0);
                                if (!cmbGroups.Items.Contains(groupName))
                                {
                                    cmbGroups.Items.Add(groupName);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading groups: " + ex.Message);
            }
        }
        private void BtnAddPurchase_Click(object sender, RoutedEventArgs e)
        {
            string item = txtItem.Text;
            decimal price;
            string selectedGroup = cmbGroups.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(item) || !decimal.TryParse(txtPrice.Text, out price) || string.IsNullOrEmpty(selectedGroup))
            {
                MessageBox.Show("Invalid input. Please enter both item, price, and select a group.");
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    // Check if the user is authorized to add a purchase to the selected group
                    if (!IsUserAuthorized(selectedGroup))
                    {
                        MessageBox.Show("You are not authorized to add a purchase to this group.");
                        return;
                    }

                    string query = "INSERT INTO Purchases (UserName, GroupName, Item, Price) VALUES (@Username, @GroupName, @Item, @Price)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@GroupName", selectedGroup);
                        command.Parameters.AddWithValue("@Item", item);

                        // Convert the price value to decimal before inserting
                        decimal priceValue;
                        if (decimal.TryParse(txtPrice.Text.Replace(".", ","), out priceValue))
                        {
                            command.Parameters.AddWithValue("@Price", priceValue);
                        }
                        else
                        {
                            // Handle invalid price input
                            MessageBox.Show("Invalid price format. Please enter a valid number.");
                            return;
                        }

                        command.ExecuteNonQuery();
                    }

                    MessageBox.Show("Purchase added successfully.");
                    txtItem.Clear();
                    txtPrice.Clear();
                    cmbGroups.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding purchase: " + ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
        private bool IsUserAuthorized(string groupName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM GroupData WHERE UserName = @Username AND GroupName = @GroupName";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@GroupName", groupName);
                        command.Parameters.AddWithValue("@Username", username);

                        int count = (int)command.ExecuteScalar();

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error checking user authorization: " + ex.Message);
                return false;
            }
        }
        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            string userName = loggedInUserName;
            UserChoise userChoise = new UserChoise(userName);
            userChoise.Show();
            Close();
        }
    }
}
