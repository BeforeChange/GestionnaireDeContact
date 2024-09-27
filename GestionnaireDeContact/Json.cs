using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using static GestionnaireDeContact.Contact;

namespace GestionnaireDeContact
{
    public class Json
    {
        string jsonFilePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName, "contact.json");
        
        public void AddToJson(string prenom, string nom, bool status, string phone, string email, string adresse)
        {
            var json = File.ReadAllText(jsonFilePath);
            var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

            Contact contact = new Contact();
            contact.LastName = nom;
            contact.FirstName = prenom;
            contact.Phone = phone;
            contact.Email = email;
            contact.Adresse = adresse;
            contact.Status = status;

            contacts.Add(contact);

            var updatedJson = JsonConvert.SerializeObject(contacts, Formatting.Indented);

            File.WriteAllText(jsonFilePath, updatedJson);
        }

        public void RemoveInJson(string phoneNumber)
        {
            var json = File.ReadAllText(jsonFilePath);
            var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

            foreach (var contact in contacts)
            {
                if(contact.Phone == phoneNumber)
                {
                    contacts.Remove(contact);
                }
            }

            var updatedJson = JsonConvert.SerializeObject(contacts, Formatting.Indented);

            File.WriteAllText(jsonFilePath, updatedJson);
        }

        public void EditInJson(string prenom, string nom, bool status, string phone, string email, string adresse)
        {
            var json = File.ReadAllText(jsonFilePath);
            var contacts = JsonConvert.DeserializeObject<List<Contact>>(json);

            foreach (var contact in contacts)
            {
                if(contact.Phone == phone)
                {
                    contact.LastName = nom;
                    contact.FirstName = prenom;
                    contact.Phone = phone;
                    contact.Email = email;
                    contact.Adresse = adresse;
                    contact.Status = status;
                }
            }

            var updatedJson = JsonConvert.SerializeObject(contacts, Formatting.Indented);

            File.WriteAllText(jsonFilePath, updatedJson);
        }
    }
}
