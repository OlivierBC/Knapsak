using System;
using System.Collections.Generic;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Reponse
    {
        public int IdReponses { get; set; }
        public int IdEnigmes { get; set; }
        public string ReponseText { get; set; }
        public short EstBonneRep { get; set; }

        public virtual Enigme IdEnigmesNavigation { get; set; }
        public string GetAnswerColor(int index)
        {
            string[] options = new string[]
            {
                "#fff90080",
                "#ffcccc",
                "#d4f7d4",
                "#ffab6690"
            };
            return $"style=background-color:{options[index]};";
        }
    }
}