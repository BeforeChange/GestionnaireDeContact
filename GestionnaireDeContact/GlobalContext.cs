using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GestionnaireDeContact.Json;
using static GestionnaireDeContact.FilePath;
using static GestionnaireDeContact.Identifiant;

namespace GestionnaireDeContact
{
    public static class GlobalContext
    {
        public static Json json = new Json();
        public static FilePath filePath = new FilePath();
        public static Identifiant identifiant = new Identifiant();
    }
}
