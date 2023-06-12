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
using System.Globalization;

namespace WeshareC.InsideAppWindows
{
    public partial class GroupCalculate : Window
    {
        private string loggedInUserName;
        private string username;
        private SqlConnection connection;
        private string connectionString;

        public GroupCalculate(string userName, SqlConnection sqlConnection)
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

            LoadGroupNames();
        }

        private void LoadGroupNames()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT DISTINCT GroupName FROM Purchases";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string groupName = reader.GetString(0);
                                groupNameComboBox.Items.Add(groupName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading group names: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void groupNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selectedGroupName = groupNameComboBox.SelectedItem as string;

            if (!string.IsNullOrEmpty(selectedGroupName))
            {
                CalculateFairPayments(selectedGroupName);
            }
        }
        private void CalculateFairPayments(string groupName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Check if the logged-in user's name is associated with the group in the GroupData table
                    string checkUserQuery = "SELECT COUNT(*) FROM GroupData WHERE GroupName = @GroupName AND LOWER(UserName) = LOWER(@LoggedInUser)";
                    using (SqlCommand checkUserCommand = new SqlCommand(checkUserQuery, connection))
                    {
                        checkUserCommand.Parameters.AddWithValue("@GroupName", groupName);
                        checkUserCommand.Parameters.AddWithValue("@LoggedInUser", loggedInUserName);

                        int userCount = (int)checkUserCommand.ExecuteScalar();

                        if (userCount == 0)
                        {
                            resultTextBox.Text = "You are not associated with this group.";
                            return;
                        }
                    }

                    // Retrieve all purchases for the group associated with the logged-in user
                    string selectQuery = "SELECT UserName, Item, Price FROM Purchases WHERE GroupName = @GroupName";
                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@GroupName", groupName);

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Dictionary<string, decimal> userExpenses = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

                            StringBuilder purchasesBuilder = new StringBuilder();

                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0) && !reader.IsDBNull(1) && !reader.IsDBNull(2))
                                {
                                    string userName = reader.GetString(0);
                                    string itemName = reader.GetString(1);
                                    decimal price = reader.GetDecimal(2);

                                    // Display each purchase
                                    purchasesBuilder.AppendFormat("User: {0}, Item: {1}, Price: {2:C}\n", userName, itemName, price);

                                    userName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(userName.ToLower());

                                    if (userExpenses.ContainsKey(userName))
                                        userExpenses[userName] += price;
                                    else
                                        userExpenses[userName] = price;
                                }
                            }

                            // Close the existing DataReader before executing the next query
                            reader.Close();

                            // Add users who haven't made any purchases
                            string selectUsersQuery = "SELECT DISTINCT UserName FROM GroupData WHERE GroupName = @GroupName";
                            using (SqlCommand selectUsersCommand = new SqlCommand(selectUsersQuery, connection))
                            {
                                selectUsersCommand.Parameters.AddWithValue("@GroupName", groupName);

                                using (SqlDataReader userReader = selectUsersCommand.ExecuteReader())
                                {
                                    while (userReader.Read())
                                    {
                                        if (!userReader.IsDBNull(0))
                                        {
                                            string userName = userReader.GetString(0);
                                            userName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(userName.ToLower());

                                            if (!userExpenses.ContainsKey(userName))
                                                userExpenses[userName] = 0;
                                        }
                                    }
                                }
                            }

                            // Calculate fair payments
                            if (userExpenses.Count > 0) // Check if there are purchases available
                            {
                                decimal totalExpenses = userExpenses.Values.Sum();
                                decimal averageExpense = totalExpenses / userExpenses.Count;

                                StringBuilder resultBuilder = new StringBuilder();
                                resultBuilder.AppendLine("Purchases:");
                                resultBuilder.Append(purchasesBuilder.ToString());
                                resultBuilder.AppendLine();
                                resultBuilder.AppendFormat("Total Expenses: {0:C}\n", totalExpenses);
                                resultBuilder.AppendLine();

                                Dictionary<string, decimal> payments = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

                                foreach (var kvp in userExpenses)
                                {
                                    string userName = kvp.Key;
                                    decimal totalSpent = kvp.Value;
                                    decimal paymentAmount = totalSpent - averageExpense;

                                    resultBuilder.AppendFormat("User: {0}, Total Spent: {1:C}", userName, totalSpent);

                                    if (paymentAmount > 0)
                                        resultBuilder.AppendFormat(" (Owes: {0:C})", paymentAmount);
                                    else if (paymentAmount < 0)
                                        resultBuilder.AppendFormat(" (Is Owed: {0:C})", Math.Abs(paymentAmount));

                                    resultBuilder.AppendLine();

                                    payments[userName] = paymentAmount;
                                }

                                resultBuilder.AppendLine();

                                var positivePayments = payments.Where(p => p.Value > 0).OrderByDescending(p => p.Value);
                                var negativePayments = payments.Where(p => p.Value < 0).OrderBy(p => p.Value);

                                foreach (var kvp in positivePayments)
                                {
                                    string userName = kvp.Key;
                                    decimal paymentAmount = kvp.Value;

                                    foreach (var kvp2 in negativePayments)
                                    {
                                        string otherUserName = kvp2.Key;
                                        decimal otherPaymentAmount = kvp2.Value;

                                        decimal transferAmount = Math.Min(Math.Abs(otherPaymentAmount), paymentAmount);
                                        paymentAmount -= transferAmount;
                                        otherPaymentAmount += transferAmount;

                                        resultBuilder.AppendFormat("User: {0} should pay {1:C} to {2}\n", otherUserName, transferAmount, userName);

                                        if (paymentAmount == 0)
                                            break;
                                    }
                                }

                                resultTextBox.Text = resultBuilder.ToString();
                            }
                            else
                            {
                                resultTextBox.Text = "No purchases found for the selected group and user.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error calculating fair payments: " + ex.Message);
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
