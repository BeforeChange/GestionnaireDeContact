using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
namespace GestionnaireDeContact
{
    [ComVisible(true)] // Attribut qui rend la classe ou l'interface visible à COM (Component Object Model).
    public partial class Form1 : Form
    {
        // Déclaration d'instances pour gérer les pages et les opérations JSON.
        ListesPages list = new ListesPages(); // Instance pour gérer la navigation entre les pages.
        Json json = new Json(); // Instance pour gérer les opérations sur le fichier JSON.
        string page; // Variable pour stocker le nom de la page actuelle.

        public Form1()
        {
            InitializeComponent(); // Initialisation des composants de la fenêtre.
            // Abonne l'événement Load à la méthode Form1_Load pour effectuer des actions lors du chargement du formulaire.
            this.Load += new EventHandler(Form1_Load);
            // Abonne l'événement DocumentCompleted à la méthode WebBrowser1_DocumentCompleted pour gérer les événements de chargement du document dans le WebBrowser.
            webBrowser1.DocumentCompleted += WebBrowser1_DocumentCompleted;
        }

        // Méthode qui se déclenche lorsque le document du WebBrowser a fini de se charger.
        private void WebBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // Définit l'objet pour le scripting JavaScript dans le WebBrowser, permettant d'accéder aux méthodes de la classe Form1 depuis le JavaScript.
            webBrowser1.ObjectForScripting = this;
        }

        // Méthode pour ouvrir la fenêtre d'ajout de contact.
        public void AddWindowOpen()
        {
            // Change la page actuelle vers "addContact" et met à jour le WebBrowser avec cette nouvelle page.
            page = list.NaviguerVers("addContact", webBrowser1);
        }

        public void ProfilWindowOpen(string id)
        {
            string htmlFilePath; // Variable pour stocker le chemin du fichier HTML.
            string jsonFilePath; // Variable pour stocker le chemin du fichier JSON.
            // Navigue vers la page de profil du contact.
            page = list.NaviguerVers("profileContact", webBrowser1);

            try
            {
                // Récupère le répertoire du projet et construit les chemins des fichiers HTML et JSON.
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                htmlFilePath = Path.Combine(projectDirectory, "profileContact.html");
                jsonFilePath = Path.Combine(projectDirectory, "contact.json");

                // Vérifie si les fichiers existent.
                if (File.Exists(htmlFilePath) && File.Exists(jsonFilePath))
                {
                    // Charge le document HTML.
                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.Load(htmlFilePath);

                    // Lit le contenu du fichier JSON et désérialise en une liste de contacts.
                    var json = File.ReadAllText(jsonFilePath);
                    var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

                    // Trouve le contact à afficher en fonction de l'ID fourni.
                    var contactToDisplay = contacts.FirstOrDefault(c => c.Id == id);
                    if (contactToDisplay != null)
                    {
                        // Met à jour le titre avec le nom du contact.
                        var nameHeader = htmlDoc.GetElementbyId("contactName");
                        if (nameHeader != null)
                        {
                            nameHeader.InnerHtml = $"Profil de {contactToDisplay.FirstName} {contactToDisplay.LastName}";
                        }

                        // Met à jour le statut du contact.
                        var statusParagraph = htmlDoc.GetElementbyId("contactStatus");
                        if (statusParagraph != null)
                        {
                            statusParagraph.SetAttributeValue("class", contactToDisplay.Status ? "status pro" : "status not-pro");
                            statusParagraph.InnerHtml = contactToDisplay.Status ? "Pro" : "Non-Pro"; // Affiche "Pro" ou "Non-Pro".
                        }

                        // Met à jour les informations détaillées du contact.
                        var infoDiv = htmlDoc.GetElementbyId("info");
                        if (infoDiv != null)
                        {
                            // Met à jour chaque champ d'information avec les données du contact.
                            infoDiv.SelectSingleNode("div[@id='ligne1PC']/div[@id='firstName']").InnerHtml = contactToDisplay.FirstName;
                            infoDiv.SelectSingleNode("div[@id='ligne1PC']/div[@id='lastName']").InnerHtml = contactToDisplay.LastName;
                            infoDiv.SelectSingleNode("div[@id='contactPhone']").InnerHtml = contactToDisplay.Phone;
                            infoDiv.SelectSingleNode("div[@id='contactAddress']").InnerHtml = contactToDisplay.Adresse;
                            infoDiv.SelectSingleNode("div[@id='contactEmail']").InnerHtml = contactToDisplay.Email; // Assurez-vous que ce champ existe.
                        }
                    }
                    else
                    {
                        // Affiche un message si le contact n'est pas trouvé.
                        MessageBox.Show("Contact introuvable avec cet ID.");
                    }

                    // Sauvegarde les modifications apportées au document HTML.
                    htmlDoc.Save(htmlFilePath);
                    // Navigue vers le fichier HTML mis à jour dans le WebBrowser.
                    webBrowser1.Navigate(new Uri(htmlFilePath));
                }
                else
                {
                    // Affiche un message d'erreur si les fichiers n'existent pas.
                    MessageBox.Show($"Le fichier {htmlFilePath} ou {jsonFilePath} n'existe pas.");
                }
            }
            catch (Exception ex)
            {
                // Affiche un message d'erreur en cas d'exception lors du chargement du fichier HTML.
                MessageBox.Show($"Erreur lors du chargement du fichier HTML : {ex.Message}");
            }
        }

        Contact contact; // Déclaration d'une variable pour stocker l'objet Contact.

        public void ConfirmationWindowOpen(string prenom, string nom, string telephone, string email, string adresse, bool professionnel)
        {
            // Génère un identifiant unique pour le contact à partir de son prénom, nom et numéro de téléphone.
            string identifiant = IdentifiantGenerator.GenererIdentifiant(prenom, nom, telephone);
            // Crée une nouvelle instance de Contact avec les informations fournies.
            contact = new Contact(identifiant, nom, prenom, professionnel, telephone, email, adresse);
            // Navigue vers la page de confirmation.
            page = list.NaviguerVers("confirmation", webBrowser1);
        }

        public void DeleteContact(string prenom, string nom, string telephone)
        {
            // Génère un identifiant unique pour le contact à supprimer.
            string identifiant = IdentifiantGenerator.GenererIdentifiant(prenom, nom, telephone);
            // Supprime le contact correspondant du fichier JSON.
            json.RemoveInJson(identifiant);
            // Navigue vers la page d'accueil après la suppression.
            page = list.NaviguerVers("home", webBrowser1);

            refresh();
        }

        public void SaveContact(string prenom, string nom, string telephone, string adresse, bool isPro, string email)
        {
            // Modifie les informations du contact dans le fichier JSON avec les nouvelles valeurs fournies.
            json.EditInJson(idEnModif, nom, prenom, isPro, telephone, email, adresse);
            // Navigue vers la page d'accueil après la sauvegarde.
            page = list.NaviguerVers("home", webBrowser1);

            refresh();
        }

        string idEnModif;
        public void EditContact(string prenom, string nom, string telephone)
        {
            // Génère un identifiant unique pour le contact à partir de son prénom, nom et numéro de téléphone.
            idEnModif = IdentifiantGenerator.GenererIdentifiant(prenom, nom, telephone);

            string htmlFilePath; // Déclaration d'une variable pour le chemin du fichier HTML.
            string jsonFilePath; // Déclaration d'une variable pour le chemin du fichier JSON.

            // Navigue vers la page d'édition du contact.
            page = list.NaviguerVers("editContact", webBrowser1);

            try
            {
                // Obtient le répertoire du projet en remontant dans l'arborescence des dossiers.
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                // Combine le répertoire du projet avec le nom du fichier HTML pour l'édition de contact.
                htmlFilePath = Path.Combine(projectDirectory, "editContact.html");
                // Combine le répertoire du projet avec le nom du fichier JSON contenant les contacts.
                jsonFilePath = Path.Combine(projectDirectory, "contact.json");

                // Vérifie si les fichiers HTML et JSON existent.
                if (File.Exists(htmlFilePath) && File.Exists(jsonFilePath))
                {
                    // Lit le contenu du fichier JSON et le désérialise en une liste d'objets Contact.
                    var json = File.ReadAllText(jsonFilePath);
                    var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

                    // Variable pour savoir si le contact a été trouvé
                    bool contactFound = false;

                    // Utilisation de foreach pour parcourir les contacts
                    foreach (var contact in contacts)
                    {
                        // Vérifie si l'ID du contact correspond à l'identifiant généré.
                        if (contact.Id == idEnModif)
                        {
                            contactFound = true; // Marque comme trouvé
                            var htmlDoc = new HtmlAgilityPack.HtmlDocument(); // Crée une instance de HtmlDocument.
                            htmlDoc.Load(htmlFilePath); // Charge le fichier HTML dans le document.

                            // Remplacer les valeurs dans les éléments HTML par les informations du contact
                            var firstNameInput = htmlDoc.GetElementbyId("firstNameInput");
                            if (firstNameInput != null)
                            {
                                firstNameInput.SetAttributeValue("value", contact.FirstName); // Met à jour le champ prénom.
                            }

                            var lastNameInput = htmlDoc.GetElementbyId("lastNameInput");
                            if (lastNameInput != null)
                            {
                                lastNameInput.SetAttributeValue("value", contact.LastName); // Met à jour le champ nom.
                            }

                            var contactPhoneInput = htmlDoc.GetElementbyId("contactPhoneInput");
                            if (contactPhoneInput != null)
                            {
                                contactPhoneInput.SetAttributeValue("value", contact.Phone); // Met à jour le champ téléphone.
                            }

                            var emailInput = htmlDoc.GetElementbyId("emailInput");
                            if (emailInput != null)
                            {
                                emailInput.SetAttributeValue("value", contact.Email); // Met à jour le champ email.
                            }

                            var addressInput = htmlDoc.GetElementbyId("addressInput");
                            if (addressInput != null)
                            {
                                addressInput.SetAttributeValue("value", contact.Adresse); // Met à jour le champ adresse.
                            }

                            var proCheckbox = htmlDoc.GetElementbyId("pro");
                            if (proCheckbox != null)
                            {
                                // Met à jour l'état de la case à cocher "pro" en fonction du statut du contact.
                                proCheckbox.SetAttributeValue("checked", contact.Status ? "checked" : null);
                            }

                            // Enregistre les modifications apportées au document HTML.
                            htmlDoc.Save(htmlFilePath);
                            // Navigue vers le fichier HTML mis à jour.
                            webBrowser1.Navigate(new Uri(htmlFilePath));
                            break; // Sortir de la boucle une fois le contact trouvé
                        }
                    }

                    // Vérifiez si le contact a été trouvé
                    if (!contactFound)
                    {
                        // Affiche un message d'erreur si le contact n'a pas été trouvé.
                        MessageBox.Show($"Contact introuvable avec cet ID : {idEnModif}. Vérifiez le fichier JSON.");
                    }
                }
                else
                {
                    // Affiche un message d'erreur si les fichiers HTML ou JSON n'existent pas.
                    MessageBox.Show($"Le fichier {htmlFilePath} ou {jsonFilePath} n'existe pas.");
                }
            }
            catch (Exception ex)
            {
                // Affiche un message d'erreur en cas d'exception lors du chargement du fichier HTML.
                MessageBox.Show($"Erreur lors du chargement du fichier HTML : {ex.Message}");
            }
        }


        public void Annuler()
        {
            // Navigue vers la page d'accueil dans le navigateur Web intégré.
            page = list.NaviguerVers("home", webBrowser1);
        }

        public void refresh()
        {
            string htmlFilePath; // Déclare une variable pour le chemin du fichier HTML.
            string jsonFilePath; // Déclare une variable pour le chemin du fichier JSON.

            try
            {
                // Obtient le répertoire parent du dossier de base de l'application pour accéder aux fichiers HTML et JSON.
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                htmlFilePath = Path.Combine(projectDirectory, "index.html"); // Chemin vers le fichier HTML.
                jsonFilePath = Path.Combine(projectDirectory, "contact.json"); // Chemin vers le fichier JSON.

                // Vérifie si les fichiers HTML et JSON existent.
                if (File.Exists(htmlFilePath) && File.Exists(jsonFilePath))
                {
                    // Crée une instance d'un document HTML à l'aide de HtmlAgilityPack.
                    var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                    htmlDoc.Load(htmlFilePath); // Charge le contenu du fichier HTML.

                    // Récupère le div 'list' et 'resultat' dans le document HTML.
                    var contactDiv = htmlDoc.GetElementbyId("list");
                    var resultatDiv = htmlDoc.GetElementbyId("resultat");

                    // Vérifie si le div 'resultat' existe, puis le vide.
                    if (resultatDiv != null)
                    {
                        resultatDiv.RemoveAllChildren();
                    }

                    // Vérifie si le div 'list' existe.
                    if (contactDiv != null)
                    {
                        // Vide le contenu du div 'list' avant d'ajouter de nouveaux contacts.
                        contactDiv.RemoveAllChildren();

                        // Lit le contenu du fichier JSON et le désérialise en une liste d'objets Contact.
                        var json = File.ReadAllText(jsonFilePath);
                        var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

                        // Pour chaque contact dans la liste désérialisée, crée un nouveau div et ajoute ses informations au HTML.
                        foreach (var contact in contacts)
                        {
                            var newContactDiv = htmlDoc.CreateElement("div"); // Crée un nouveau div pour le contact.
                            newContactDiv.SetAttributeValue("class", "contact"); // Définit la classe CSS pour le style.
                            newContactDiv.SetAttributeValue("onclick", $"ProfilWindowOpen(\"{contact.Id}\")"); // Ajoute l'événement onclick pour ouvrir le profil.

                            // Définit le contenu HTML du nouveau div avec les informations du contact.
                            newContactDiv.InnerHtml = $@"
                            <div class=""contact-info"">
                                <h2>{contact.LastName}</h2><h2>{contact.FirstName}</h2>
                                <p class=""status {contact.Status.ToString().ToLower()}"">{(contact.Status ? "Pro" : "Non-Pro")}</p>
                            </div>
                            <p>Téléphone : {contact.Phone}</p>
                            <p>Email : {contact.Email}</p>
                            <p>Adresse : {contact.Adresse}</p>
                        ";

                            // Ajoute le nouveau div de contact au div parent 'list'.
                            contactDiv.AppendChild(newContactDiv);
                        }

                        // Sauvegarde les modifications apportées au document HTML.
                        htmlDoc.Save(htmlFilePath);
                    }
                    else
                    {
                        // Affiche un message si le div 'list' n'est pas trouvé.
                        Console.WriteLine("Le div avec l'id 'list' n'a pas été trouvé");
                    }

                    // Recharge le fichier HTML modifié dans le WebBrowser pour afficher les contacts mis à jour.
                    webBrowser1.Navigate(new Uri(htmlFilePath));
                }
                else
                {
                    // Si l'un des fichiers n'existe pas, affiche un message d'erreur.
                    MessageBox.Show($"Le fichier {htmlFilePath} ou {jsonFilePath} n'existe pas.");
                }
            }
            catch (Exception ex)
            {
                // Capture toute exception et affiche un message d'erreur.
                MessageBox.Show($"Erreur lors du chargement du fichier HTML : {ex.Message}");
            }
        }

        public void AjoutContact(bool boolean)
        {
            // Vérifie si le paramètre 'boolean' est vrai.
            if (boolean == true)
            {
                // Ajoute un nouveau contact à la liste JSON en utilisant les propriétés de l'objet 'contact'.
                json.AddToJson(contact.Id, contact.LastName, contact.FirstName, contact.Status, contact.Phone, contact.Email, contact.Adresse);

                // Navigue vers la page d'accueil dans le navigateur Web intégré.
                page = list.NaviguerVers("home", webBrowser1);

                refresh();
            }
            else
            {
                // Si le paramètre 'boolean' est faux, navigue vers la page d'accueil.
                page = list.NaviguerVers("home", webBrowser1);
            }
        }


        public void InputValueChanged(string newValue)
        {
            // Obtient le répertoire parent du dossier de base de l'application pour accéder aux fichiers HTML et JSON.
            string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;

            // Définit les chemins des fichiers HTML et JSON en utilisant le répertoire projet.
            string htmlFilePath = Path.Combine(projectDirectory, "index.html"); // Chemin vers le fichier HTML.
            string jsonFilePath = Path.Combine(projectDirectory, "contact.json"); // Chemin vers le fichier JSON.

            try
            {
                // Crée une instance d'un document HTML à l'aide de HtmlAgilityPack.
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.Load(htmlFilePath); // Charge le contenu du fichier HTML.

                // Lit le contenu du fichier JSON et le désérialise en une liste d'objets Contact.
                var json = File.ReadAllText(jsonFilePath);
                var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

                // Filtre les contacts en fonction de la valeur d'entrée 'newValue'.
                var filteredContacts = contacts.Where(c =>
                    // Vérifie si le nom, le prénom, le téléphone ou l'adresse commencent par la valeur saisie (insensible à la casse).
                    c.LastName.StartsWith(newValue, StringComparison.OrdinalIgnoreCase) ||
                    c.FirstName.StartsWith(newValue, StringComparison.OrdinalIgnoreCase) ||
                    c.Phone.StartsWith(newValue, StringComparison.OrdinalIgnoreCase) ||
                    c.Adresse.StartsWith(newValue, StringComparison.OrdinalIgnoreCase)
                ).ToList(); // Convertit le résultat en liste.

                // Récupère le div 'resultat' dans le document HTML pour y afficher les résultats filtrés.
                var contactDiv = htmlDoc.GetElementbyId("resultat");
                if (contactDiv != null)
                {
                    // Vide le contenu du div 'resultat' avant d'y ajouter les nouveaux contacts.
                    contactDiv.InnerHtml = "";

                    // Pour chaque contact filtré, crée un nouveau div et ajoute ses informations au HTML.
                    foreach (var contact in filteredContacts)
                    {
                        // Crée un nouveau div pour représenter le contact.
                        var newContactDiv = htmlDoc.CreateElement("div");
                        newContactDiv.SetAttributeValue("class", "result-contact"); // Définit la classe CSS pour le style.
                        newContactDiv.SetAttributeValue("onclick", $"ProfilWindowOpen(\"{contact.Id}\")"); // Ajoute l'événement onclick pour ouvrir le profil.

                        // Définit le contenu HTML du nouveau div avec les informations du contact.
                        newContactDiv.InnerHtml = $@"
                    <div class=""result-info"">
                        <h2>{contact.LastName}</h2><h2>{contact.FirstName}</h2>
                        <p class=""status {contact.Status.ToString().ToLower()}"">{(contact.Status ? "Pro" : "Non-Pro")}</p>
                    </div>
                    <p>Téléphone : {contact.Phone}</p>
                    <p>Email : {contact.Email}</p>
                    <p>Adresse : {contact.Adresse}</p>
                ";

                        // Ajoute le nouveau div de contact au div parent 'resultat'.
                        contactDiv.AppendChild(newContactDiv);
                    }

                    // Sauvegarde les modifications apportées au document HTML.
                    htmlDoc.Save(htmlFilePath);

                    // Recharge le fichier HTML modifié dans le WebBrowser pour afficher les résultats filtrés.
                    webBrowser1.Navigate(new Uri(htmlFilePath));
                }
                else
                {
                    // Si le div 'resultat' n'est pas trouvé, affiche un message d'erreur.
                    MessageBox.Show("Le div avec l'id 'resultat' n'a pas été trouvé.");
                }
            }
            catch (Exception ex)
            {
                // Capture toute exception et affiche un message d'erreur.
                MessageBox.Show($"Erreur lors de la mise à jour du contenu : {ex.Message}");
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            refresh();
        }

    }
}