using System;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Windows.Forms;

namespace UserCreator
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();

            var currDomain = Domain.GetCurrentDomain();
            txtDomain.Text = currDomain.Name;

            txtUsername.Focus();
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            var valid = false;
            foreach (var txt in Controls.OfType<TextBox>().Select(control => control))
            {
                valid = CheckForEmpty(txt);
            }

            try
            {
                if (!valid) return;
                CreateNewUser();
                var msg = string.Format("User '{0}' created successfully!", txtUsername.Text);
                MessageBox.Show(msg, "Congrats", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Error occured \n\n" + exc, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        #region HELPERS

        /// <summary>
        /// Show error for text controls if the text is blank.
        /// </summary>
        /// <param name="textBox">The textbox control</param>
        /// <returns>Return true if there is text else false</returns>
        private bool CheckForEmpty(Control textBox)
        {
            bool valid;

            if (textBox.Text.Trim().Length > 0)
            {
                valid = true;
                errorProvider.SetError(textBox, string.Empty);
            }
            else
            {
                valid = false;
                errorProvider.SetIconPadding(textBox, 5);
                errorProvider.SetError(textBox, "Value can't be blank.");
            }

            return valid;
        }

        /// <summary>
        /// Creates a new user on Active Directory
        /// </summary>
        public void CreateNewUser()
        {
            var principalContext = GetPrincipalContext(txtDomain.Text, txtAdminUser.Text, txtAdminPassword.Text);

            var userPrincipal = new UserPrincipalEx(principalContext, txtUsername.Text, txtPassword.Text,true);

            //User details
            userPrincipal.UserPrincipalName = txtUsername.Text;
            userPrincipal.GivenName = txtGivenName.Text;
            userPrincipal.Surname = txtSurname.Text;
            //userPrincipal.PasswordNeverExpires = true;
            //userPrincipal.AllowReversiblePasswordEncryption = true;
            userPrincipal.Save();

            userPrincipal.SetNormalAccount = 512;
            userPrincipal.Save();

        }

        /// <summary>
        /// Gets the base principal context
        /// </summary>
        /// <returns>Returns the PrincipalContext object</returns>
        public PrincipalContext GetPrincipalContext(string sDomain, string sServiceUser, string sServicePassword)
        {
            var oPrincipalContext = new PrincipalContext(ContextType.Domain, sDomain, sServiceUser, sServicePassword);
            return oPrincipalContext;
        }

        /// <summary>
        /// Gets a certain user on Active Directory
        /// </summary>
        /// <param name="sUserName">The username to get</param>
        /// <returns>Returns the UserPrincipal Object</returns>
        public UserPrincipal GetUser(string sUserName)
        {
            PrincipalContext oPrincipalContext = GetPrincipalContext(txtDomain.Text, txtAdminUser.Text, txtAdminPassword.Text);

            UserPrincipal oUserPrincipal =
               UserPrincipal.FindByIdentity(oPrincipalContext, sUserName);
            return oUserPrincipal;
        }


        #endregion
    }
}
