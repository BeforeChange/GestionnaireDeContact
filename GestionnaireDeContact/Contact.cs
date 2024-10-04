using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace GestionnaireDeContact // Déclaration du namespace qui regroupe les classes liées à la gestion des contacts.
{
    public class Contact // Définition de la classe Contact, représentant un contact dans le gestionnaire.
    {
        // Propriétés publiques pour accéder aux informations du contact
        public string Id { get; set; } // Identifiant unique du contact
        public string FirstName { get; set; } // Prénom du contact
        public string LastName { get; set; } // Nom de famille du contact
        public bool Status { get; set; } // Statut du contact (professionnel ou non)
        public string Phone { get; set; } // Numéro de téléphone du contact
        public string Email { get; set; } // Adresse e-mail du contact
        public string Adresse { get; set; } // Adresse physique du contact

        // Constructeur de la classe Contact
        public Contact(string id, string lastName, string firstName, bool status, string phone, string email, string adresse)
        {
            // Initialisation des propriétés avec les valeurs fournies lors de la création d'un nouvel objet Contact
            Id = id; // Assignation de l'identifiant
            LastName = lastName; // Assignation du nom de famille
            FirstName = firstName; // Assignation du prénom
            Status = status; // Assignation du statut
            Phone = phone; // Assignation du numéro de téléphone
            Email = email; // Assignation de l'adresse e-mail
            Adresse = adresse; // Assignation de l'adresse physique
        }
    }
}
