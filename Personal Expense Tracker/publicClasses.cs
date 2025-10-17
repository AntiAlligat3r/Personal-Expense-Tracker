using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Personal_Expense_Tracker
{
    public class publicClasses
    {
        private string _username;
        public bool exitProgram()
        {
            DialogResult closeProgram = MessageBox.Show("Are you sure you want to exit?", "Close Program", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return closeProgram == DialogResult.Yes ? true : false;
        }

        public string getUsername()
        {
            return _username;
        }
        public void setUsername(string username)
        {
            _username = username;
        }
    }
}
