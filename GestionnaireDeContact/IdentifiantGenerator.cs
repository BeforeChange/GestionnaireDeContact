using System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace GestionnaireDeContact // Déclaration du namespace pour regrouper les classes liées à la gestion des contacts.
{
    public static class IdentifiantGenerator // Classe statique pour générer des identifiants uniques pour les contacts.
    {
        // Méthode statique pour générer un identifiant basé sur le prénom, le nom et le numéro de téléphone.
        public static string GenererIdentifiant(string prenom, string nom, string telephone)
        {
            // Concaténation des informations d'entrée pour créer une chaîne unique
            string input = $"{prenom}{nom}{telephone}";

            // Création d'une instance de l'algorithme de hachage SHA256
            using (SHA256 sha256 = SHA256.Create())
            {
                // Calcul du hachage de la chaîne d'entrée
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Utilisation de StringBuilder pour construire la chaîne hexadécimale
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes) // Parcours de chaque octet du tableau de hachage
                {
                    // Conversion de l'octet en représentation hexadécimale (2 caractères)
                    builder.Append(b.ToString("x2"));
                }

                // Retourne les 10 premiers caractères du hachage hexadécimal comme identifiant
                return builder.ToString().Substring(0, 10);
            }
        }
    }
}
