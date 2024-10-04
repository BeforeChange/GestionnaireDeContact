using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static GestionnaireDeContact.Contact;

namespace GestionnaireDeContact // Déclaration du namespace pour regrouper les classes liées à la gestion des contacts.
{
    public class Json // Définition de la classe Json, responsable de la gestion des opérations sur le fichier JSON contenant les contacts.
    {
        // Chemin d'accès au fichier JSON, construit en récupérant le répertoire parent de l'application et en ajoutant le nom du fichier
        string jsonFilePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "contact.json");

        // Méthode pour ajouter un nouveau contact au fichier JSON
        public void AddToJson(string id, string nom, string prenom, bool status, string phone, string email, string adresse)
        {
            // Lecture du contenu du fichier JSON
            var json = File.ReadAllText(jsonFilePath);
            // Désérialisation du JSON en une liste d'objets Contact
            var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

            // Création d'un nouvel objet Contact avec les informations fournies
            Contact contact = new Contact(id, nom, prenom, status, phone, email, adresse);

            // Ajout du nouveau contact à la liste
            contacts.Add(contact);

            // Sérialisation de la liste mise à jour en JSON avec indentation
            var updatedJson = JsonConvert.SerializeObject(contacts, Formatting.Indented);

            // Écriture de la nouvelle liste de contacts dans le fichier JSON
            File.WriteAllText(jsonFilePath, updatedJson);
        }

        // Méthode pour supprimer un contact du fichier JSON en fonction de son identifiant
        public void RemoveInJson(string id)
        {
            // Lecture du contenu du fichier JSON
            var json = File.ReadAllText(jsonFilePath);
            // Désérialisation du JSON en une liste d'objets Contact
            var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

            // Boucle à l'envers pour éviter les problèmes de modification de la collection pendant l'itération
            for (int i = contacts.Count - 1; i >= 0; i--)
            {
                // Vérification si l'identifiant du contact correspond à celui à supprimer
                if (contacts[i].Id == id)
                {
                    contacts.RemoveAt(i); // Suppression du contact
                }
            }

            // Sérialisation de la liste mise à jour en JSON
            var updatedJson = JsonConvert.SerializeObject(contacts, Formatting.Indented);
            // Écriture de la nouvelle liste de contacts dans le fichier JSON
            File.WriteAllText(jsonFilePath, updatedJson);
        }

        // Méthode pour modifier un contact dans le fichier JSON
        public void EditInJson(string id, string nom, string prenom, bool status, string phone, string email, string adresse)
        {
            // Lecture du contenu du fichier JSON
            var json = File.ReadAllText(jsonFilePath);
            // Désérialisation du JSON en une liste d'objets Contact
            var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

            // Boucle pour trouver le contact à modifier
            foreach (var contact in contacts)
            {
                if (contact.Id == id) // Vérification si l'identifiant du contact correspond
                {
                    // Mise à jour des informations du contact
                    contact.LastName = nom;
                    contact.FirstName = prenom;
                    contact.Phone = phone;
                    contact.Email = email;
                    contact.Adresse = adresse;
                    contact.Status = status;
                    // Régénération de l'identifiant du contact après modification

                    contact.Id = IdentifiantGenerator.GenererIdentifiant(prenom, nom, phone);
                }
            }
            // Sérialisation de la liste mise à jour en JSON
            var updatedJson = JsonConvert.SerializeObject(contacts, Formatting.Indented);

            // Écriture de la nouvelle liste de contacts dans le fichier JSON
            File.WriteAllText(jsonFilePath, updatedJson);
        }
    }
}
