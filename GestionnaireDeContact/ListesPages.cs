using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace GestionnaireDeContact // Déclaration du namespace pour regrouper les classes liées à la gestion des contacts.
{
    // Enumération qui représente les différentes pages de l'application.
    public enum Pages
    {
        home, // Page d'accueil
        ListeContacts, // Page de la liste des contacts
        addContact, // Page pour ajouter un contact
        editContact, // Page pour modifier un contact
        profileContact, // Page pour afficher le profil d'un contact
        confirmation // Page de confirmation après une action
    }

    internal class ListesPages // Classe interne pour gérer les pages de l'application.
    {
        // Dictionnaire pour associer chaque page à son fichier HTML correspondant.
        private Dictionary<Pages, string> pagesDict;

        // Constructeur de la classe ListesPages
        public ListesPages()
        {
            // Initialisation du dictionnaire avec les associations de pages.
            pagesDict = new Dictionary<Pages, string>
            {
                { Pages.home, "index.html" },
                { Pages.addContact, "addContact.html" },
                { Pages.editContact, "editContact.html" },
                { Pages.profileContact, "profileContact.html" },
                { Pages.confirmation, "confirmation.html" }
            };
        }

        // Méthode pour obtenir le nom de fichier HTML associé à une page donnée.
        public string GetFormForPage(Pages page)
        {
            // Vérifie si la page est présente dans le dictionnaire
            if (pagesDict.ContainsKey(page))
            {
                return pagesDict[page]; // Retourne le fichier associé à la page
            }
            else
            {
                throw new ArgumentException("Page non définie."); // Lance une exception si la page n'est pas trouvée
            }
        }

        // Méthode pour naviguer vers une page donnée à l'aide d'un WebBrowser
        public string NaviguerVers(string page, WebBrowser webBrowser)
        {
            // Tente de convertir la chaîne de caractères en énumération Pages
            if (Enum.TryParse(page, true, out Pages pageEnum))
            {
                // Obtient le répertoire du projet
                string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;
                // Obtient le fichier HTML pour la page spécifiée
                string pageToNavigate = GetFormForPage(pageEnum);
                string htmlFilePath = Path.Combine(projectDirectory, pageToNavigate); // Crée le chemin complet vers le fichier HTML

                // Navigue vers l'URL du fichier HTML dans le WebBrowser
                webBrowser.Navigate(new Uri(htmlFilePath));
                return pageToNavigate; // Retourne le nom de la page naviguée
            }
            else
            {
                // Affiche un message d'erreur si la page spécifiée n'est pas valide
                MessageBox.Show($"La page '{page}' n'est pas valide.");
            }
            return " "; // Retourne une chaîne vide par défaut
        }
    }
}
