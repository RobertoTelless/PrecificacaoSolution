using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting
{
    public class ControlError
    {
        private List<string> _messages;
        public bool IsOK { get; set; }

        public ControlError()
        {
            this._messages = new List<string>();
        }

        public void HandleExeption(Exception ex)
        {
            _messages.Add(ex.Message);
            _messages.Add(ex.StackTrace);
        }

        public void HandleExeption(string menssage)
        {
            _messages.Add(menssage);
        }

        public string GetMenssage()
        {
            string mensagem = "";

            foreach(string m in _messages)
            {
                mensagem = string.Concat(mensagem, " ", m);
            }

            return mensagem;
        }
       
    }
}
