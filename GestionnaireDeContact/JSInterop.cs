using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GestionnaireDeContact
{
    [ComVisible(true)]
    public class JSInterop
    {
        private Form _form;

        public JSInterop(Form form)
        {
            _form = form;
        }

        public void InputValueChanged(string newValue)
        {
            var method = _form.GetType().GetMethod("InputValueChanged");
            if (method != null)
            {
                method.Invoke(_form, new object[] { newValue });
            }
        }

        public void AddWindowOpen()
        {
            var method = _form.GetType().GetMethod("AddWindowOpen");
            if (method != null)
            {
                method.Invoke(_form, null);
            }
        }

        public void ReceiveFormData(string prenom, string nom, string telephone, string email, string adresse, bool professionnel)
        {
            confirmationPage confirmationPage = new confirmationPage(prenom, nom, telephone, email, adresse, professionnel);
            confirmationPage.Show();
        }
    }
}