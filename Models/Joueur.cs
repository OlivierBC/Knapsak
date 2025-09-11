using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CompareAttribute = System.ComponentModel.DataAnnotations.CompareAttribute;
#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Joueur
    {
        public const short MaxWeight = 50;
        public const short MaxHealth = 50; // >> changer le message d'erreur manuellement dans Models/SMTP/Requetes.SanteDemandee
        public const int DefaultDex = 100;
        public const int RequeteAmount = 3;
        public Joueur()
        {
            Evaluations = new HashSet<Evaluation>();
            Paniers = new HashSet<Panier>();
            SacADos = new HashSet<SacADos>();
            Statistiques = new HashSet<Statistique>();
            Montant = 1000;
            Dexterite = 0;
            PointsDeVie = 10;
            EstAdmin = 0;
        }

        public int IdJoueurs { get; set; }
        [Remote("AliasAvailable", "Accounts", AdditionalFields = "", HttpMethod = "POST", ErrorMessage = "Cet alias est déjà utilisé.")]
        [Display(Name = "Alias"), Required(ErrorMessage = "Obligatoire")]
        public string Alias { get; set; }
        [Display(Name = "Nom"), Required(ErrorMessage = "Obligatoire")]
        public string Nom { get; set; }
        [Display(Name = "Prenom"), Required(ErrorMessage = "Obligatoire")]
        public string Prenom { get; set; }
        public int Montant { get; set; }
        public int Dexterite { get; set; }
        public short PointsDeVie { get; set; }
        public short PoidsTotal { get; set; }
        [Display(Name = "Mot de passe")]
        [Required(ErrorMessage = "Obligatoire")]
        [StringLength(50, ErrorMessage = "Le mot de passe doit comporter au moins {2} caractères.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string MPasse { get; set; }
        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmer le mot de passe")]
        [Compare("MPasse", ErrorMessage = "Le mot de passe ne correspond pas")]
        public string ConfirmPassword { get; set; }
        public short EstAdmin { get; set; }
        [NotMapped]
        public int NbItemsPanier { get; set; } = 0;

        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual ICollection<Panier> Paniers { get; set; }
        public virtual ICollection<SacADos> SacADos { get; set; }
        public virtual ICollection<Statistique> Statistiques { get; set; }
        public virtual ICollection<Requetes> Requetes { get; set; }

        public int DexteriteAffiche => Joueur.DefaultDex + Dexterite - Math.Max(0, PoidsTotal - Joueur.MaxWeight);

        public string ToJsonSessionInfo =>

            JsonSerializer.Serialize(new
            {
                IdJoueurs = this.IdJoueurs.ToString(),
                Alias = this.Alias.ToString(),
                Montant = this.Montant.ToString(),
                Dexterite = this.Dexterite.ToString(),
                PoidsTotal = this.PoidsTotal.ToString(),
                PointsDeVie = this.PointsDeVie.ToString(),
                NbItemsPanier = this.NbItemsPanier.ToString(),
                EstAdmin = this.EstAdmin.ToString(),
            });
        
        static public Dictionary<string, string> FromJsonSessionInfo(string json)
        {
            if(json != null && json.Length >0)
            {
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json);
            }
            else
            {
                return null;
            }
        }
    }
}
