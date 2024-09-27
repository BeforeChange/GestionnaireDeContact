using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static GestionnaireDeContact.Contact;
using static GestionnaireDeContact.JSInterop;
using static GestionnaireDeContact.GlobalContext;

namespace GestionnaireDeContact
{
    [ComVisible(true)]
    public partial class confirmationPage : Form
    {
        string prenom;
        string nom;
        string telephone;
        string email;
        string adresse;
        bool professionel;

        public confirmationPage(string prenom, string nom, string telephone, string email, string adresse, bool professionnel)
        {
            MessageBox.Show(GlobalContext.identifiant.Generate(nom+prenom+telephone));
            InitializeComponent();
            this.prenom = prenom;
            this.nom = nom;
            this.telephone = telephone;
            this.email = email;
            this.adresse = adresse;
            this.professionel = professionnel;
            MessageBox.Show($"Prénom: {prenom}\nNom: {nom}\nTéléphone: {telephone}\nEmail: {email}\nAdresse: {adresse}\nProfessionnel: {professionnel}");
        }

        public void button_click(bool boolean)
        {
            if (boolean)
            {
                GlobalContext.json.AddToJson(prenom, nom, professionel, telephone, email, adresse);
            } else
            {
                this.Close();
            }
        }

        private void confirmationPage_Load(object sender, EventArgs e)
        {
            try
            {
                webBrowser2.ObjectForScripting = this;
                webBrowser2.Navigate(new Uri(GlobalContext.filePath.Get("confirmation.html")));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du fichier HTML : {ex.Message}");
            }
            MessageBox.Show($"Chemin HTML: {GlobalContext.filePath.Get("confirmation.html")}");
        }
    }
}
