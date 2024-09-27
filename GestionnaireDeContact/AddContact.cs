using Microsoft.Win32;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static GestionnaireDeContact.JSInterop;

namespace GestionnaireDeContact
{
    [ComVisible(true)] // Permet à l'objet d'être accessible depuis le JavaScript
    public partial class AddContact : Form
    {
        public AddContact()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form_Load);
        }


        private void Form_Load(object sender, EventArgs e)
        {
            string htmlFilePath = "";

            try
            {
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                htmlFilePath = Path.Combine(projectDirectory, "addContact.html");

                webBrowser1.ObjectForScripting = new JSInterop(this);
                webBrowser1.Navigate(new Uri(htmlFilePath));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du fichier HTML : {ex.Message}");
            }

            MessageBox.Show($"Chemin HTML: {htmlFilePath}");

        }
    }
}