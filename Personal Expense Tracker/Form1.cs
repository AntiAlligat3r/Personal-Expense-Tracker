using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Personal_Expense_Tracker
{
    public partial class frmCreationLogin : Form
    {
        const int keySize = 128;
        const int iterations = 350000;
        HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public frmCreationLogin()
        {
            InitializeComponent();
        }
        private SqlConnection openDbConnection()
        {
            SqlConnection connString = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\falls\Documents\C# Portfolio projects\Personal Expense Tracker\Personal Expense Tracker\dbExpenseTracker.mdf;Integrated Security=True");
            try
            {
                connString.Open();
                return connString;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return null;
            }
        }

        private void btnShowAccountPanel_Click(object sender, EventArgs e)
        {
            pnlAccountCreation.Visible = true;
            pnlLogin.Visible = false;
        }

        private bool validateAccountCreationInput()
        {
            bool _specialChars;
            bool _numbers;
            bool _uppercase;
            bool _validEmail;
            string invalidAccountDetails = "";

            MatchCollection numberInPassword = Regex.Matches(txtPassword.Text, @"[0-9]+");
            MatchCollection specialCharInPassword = Regex.Matches(txtPassword.Text, @"[^a-zA-Z0-9\s]");
            MatchCollection upperInPassword = Regex.Matches(txtPassword.Text, @"[A-Z]");
            MatchCollection validAtEmail = Regex.Matches(txtEmail.Text, @"[/@]");
            MatchCollection validDotEmail = Regex.Matches(txtEmail.Text, @"[/.]");

            _numbers = numberInPassword.Count > 0 ? true : false;
            _specialChars = specialCharInPassword.Count > 0 ? true : false;
            _uppercase = upperInPassword.Count > 0 ? true : false;
            _validEmail = (validAtEmail.Count == 1) && (validDotEmail.Count == 1) ? true : false;

            invalidAccountDetails += !_numbers ? "Password must include Numerical values between 0-9\n" : "";
            invalidAccountDetails += !_specialChars ? "Password must include Special characters\n" : "";
            invalidAccountDetails += !_uppercase ? "Password must include an Upper case letter between A-Z\n" : "";

            if (invalidAccountDetails != "")
            {
                lblErrorPassword.Text = invalidAccountDetails;
                lblErrorPassword.Visible = true;
            }
            else
                lblErrorPassword.Visible = false;

            invalidAccountDetails = "";
            invalidAccountDetails += !_validEmail ? "Email is not Valid\n" : "";

            if (invalidAccountDetails != "")
            {
                lblErrorEmail.Text = invalidAccountDetails;
                lblErrorEmail.Visible = true;
            }
            else
                lblErrorEmail.Visible = false;

            return ((_specialChars = true) && (_validEmail = true) && (_uppercase = true) && (_numbers = true)) ? true : false;
        }
        
        private string hashPassword(string password, out byte[] saltKey)
        {
            new RNGCryptoServiceProvider().GetBytes(saltKey = new byte[keySize]);
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, saltKey, iterations);

            byte[] hashKey = pbkdf2.GetBytes(keySize);
            byte[] hashPasswordBytes = new byte[(keySize * 2)];
            Array.Copy(saltKey, 0, hashPasswordBytes, 0, keySize);
            Array.Copy(hashKey, 0, hashPasswordBytes, keySize, (keySize*2));

            string hashedPassword = Convert.ToBase64String(hashPasswordBytes);
            return hashedPassword;
        }
        private void btnCreateAccount_Click(object sender, EventArgs e)
        {
            if(validateAccountCreationInput())
            {
                SqlConnection connString = openDbConnection();
                try
                {
                    
                    using (SqlCommand createUserQuery = new SqlCommand("INSERT INTO Users (Username,Password,Email) Values(@username,@password,@email)",connString))
                    {
                        createUserQuery.Parameters.AddWithValue("@username", txtUsername.Text);
                        createUserQuery.Parameters.AddWithValue("@password", txtPassword.Text);
                        createUserQuery.Parameters.AddWithValue("@email", txtEmail.Text);
                        createUserQuery.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                connString.Close();
            }
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            if(txtPassword.Text != txtConfirmPassword.Text)
            {
                lblErrorPassword.Text = "Passwords do not match";
                lblErrorPassword.Visible = true;
                txtConfirmPassword.BackColor = Color.Red;
                btnCreateAccount.Enabled = false;
            }
        }

        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            if (txtConfirmPassword.Text == txtPassword.Text)
            {
                lblErrorPassword.Visible = false;
                txtConfirmPassword.BackColor = SystemColors.Window;
                btnCreateAccount.Enabled = true;
            }
        }

        private void ckbxShowPassword_Click(object sender, EventArgs e)
        {
            txtConfirmPassword.UseSystemPasswordChar = ckbxShowPassword.Checked ? false : true;
            txtPassword.UseSystemPasswordChar = ckbxShowPassword.Checked ? false : true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string _password = hashPassword(txtPassword.Text, out byte[] saltKey);
            MessageBox.Show(_password);
        }
    }
}
