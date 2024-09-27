using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using static GestionnaireDeContact.Form1;
using static GestionnaireDeContact.Contact;
using static GestionnaireDeContact.JSInterop;

namespace GestionnaireDeContact
{
    public partial class Form1 : Form
    {
        private bool isDragging = false;
        private Point lastCursor;
        private Point lastForm;

        public AddContact _form2;

        public Form1()
        {
            InitializeComponent();

            _form2 = new AddContact();

            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string htmlFilePath = Path.Combine(projectDirectory, "index.html");
            string backupFilePath = Path.Combine(projectDirectory, "index_backup.json");

            this.Load += new EventHandler(Form1_Load);

            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted;
        }

        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            webBrowser1.ObjectForScripting = new JSInterop(this);
        }

        public void AddWindowOpen()
        {
            _form2.Show();
        }

        public void InputValueChanged(string newValue)
        {
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
            string htmlFilePath = Path.Combine(projectDirectory, "index.html");
            string jsonFilePath = Path.Combine(projectDirectory, "contact.json");

            try
            {
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.Load(htmlFilePath);

                var json = File.ReadAllText(jsonFilePath);
                var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);
                var filteredContacts = contacts.Where(c => c.LastName.StartsWith(newValue, StringComparison.OrdinalIgnoreCase)).ToList();

                var contactDiv = htmlDoc.GetElementbyId("resultat");
                if (contactDiv != null)
                {
                    contactDiv.InnerHtml = "";

                    foreach (var contact in filteredContacts)
                    {
                        var newContactDiv = htmlDoc.CreateElement("div");
                        newContactDiv.SetAttributeValue("class", "result-contact");
                        newContactDiv.InnerHtml = $@"
                            <div class=""result-info"">
                                <h3>{contact.LastName}</h3>
                                <p class=""status {contact.Status.ToString().ToLower()}"">{contact.Status}</p>
                            </div>
                            <p>Téléphone : {contact.Phone}</p>
                            <p>Email : {contact.Email}</p>
                        ";

                        contactDiv.AppendChild(newContactDiv);
                    }

                    htmlDoc.Save(htmlFilePath);

                    webBrowser1.Navigate(new Uri(htmlFilePath));
                }
                else
                {
                    MessageBox.Show("Le div avec l'id 'resultat' n'a pas été trouvé.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la mise à jour du contenu : {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string htmlFilePath;
            string jsonFilePath;

            try
            {
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                htmlFilePath = Path.Combine(projectDirectory, "index.html");
                jsonFilePath = Path.Combine(projectDirectory, "contact.json");

                if (File.Exists(htmlFilePath) && File.Exists(jsonFilePath))
                {
                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.Load(htmlFilePath);

                    var contactDiv = htmlDoc.GetElementbyId("list");
                    var resultatDiv = htmlDoc.GetElementbyId("resultat");

                    if (resultatDiv != null)
                    {
                        resultatDiv.RemoveAllChildren();
                    }
                    else
                    {
                        Console.WriteLine("Le div avec l'id 'resultat' n'a pas été trouvé");
                    }

                    if (contactDiv != null)
                    {
                        contactDiv.RemoveAllChildren();

                        var json = File.ReadAllText(jsonFilePath);
                        var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

                        foreach (var contact in contacts)
                        {
                            var newContactDiv = htmlDoc.CreateElement("div");
                            newContactDiv.SetAttributeValue("class", "contact");
                            newContactDiv.InnerHtml = $@"
                                <div class=""contact-info"">
                                    <h2>{contact.LastName}</h2>
                                    <p class=""status {contact.Status.ToString().ToLower()}"">{contact.Status}</p>
                                </div>
                                <p>Téléphone : {contact.Phone}</p>
                                <p>Email : {contact.Email}</p>
                                <p>Adresse : {contact.Adresse}</p>
                            ";

                            contactDiv.AppendChild(newContactDiv);
                        }

                        htmlDoc.Save(htmlFilePath);
                    }
                    else
                    {
                        Console.WriteLine("Le div avec l'id 'list' n'a pas été trouvé");
                    }

                    webBrowser1.Navigate(new Uri(htmlFilePath));
                }
                else
                {
                    MessageBox.Show($"Le fichier {htmlFilePath} ou {jsonFilePath} n'existe pas.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement du fichier HTML : {ex.Message}");
            }

        }
    }
}