using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using static GestionnaireDeContact.GlobalContext;
namespace GestionnaireDeContact
{
    public class FilePath
    {
        string projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.FullName;

        public string Get(string fileName)
        {
            string filePath = Path.Combine(projectDirectory, fileName);
            return filePath;
        }
    }
}
