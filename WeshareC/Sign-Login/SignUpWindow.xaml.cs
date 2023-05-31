using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WeshareC
{
    public partial class SignUpWindow : Window
    {
        private readonly string connectionString;

        public SignUpWindow()
        {
            InitializeComponent();

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            connectionString = configuration.GetConnectionString("MyConnectionString");
        }

        private void btnSignUp_Click(object sender, RoutedEventArgs e)
        {
            string UserName = txtUsername.Text;
            string Pass = txtPassword.Password;

            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Pass))
            {
                MessageBox.Show("Please enter a username and password.");
                return;
            }

            if (UserExists(UserName))
            {
                MessageBox.Show("Username already exists. Please choose a different username.");
                return;
            }

            if (CreateUser(UserName, Pass))
            {
                MessageBox.Show("User created successfully. You can now log in with your credentials.");
                ClearFields();
                // Optionally, you can navigate to the login window after successful sign-up
                // LoginWindow loginWindow = new LoginWindow();
                // loginWindow.Show();
                // Close();
            }
            else
            {
                MessageBox.Show("Error creating user. Please try again later.");
            }
        }

        private bool UserExists(string UserName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM UserInfo WHERE UserName = @Username";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", UserName);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private bool CreateUser(string UserName, string Pass)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO UserInfo (UserName, Pass) VALUES (@Username, @Password)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", UserName);
                    command.Parameters.AddWithValue("@Password", Pass);
                    int rowsAffected = command.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }

        private void ClearFields()
        {
            txtUsername.Text = string.Empty;
            txtPassword.Password = string.Empty;
        }

        private void GoBack_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            Close();
        }
    }
}
