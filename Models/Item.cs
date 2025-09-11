using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Globalization;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class Item
    {
        public Item()
        {
            Armes = new HashSet<Arme>();
            Evaluations = new HashSet<Evaluation>();
            Paniers = new HashSet<Panier>();
            SacADos = new HashSet<SacADos>();
            FlagDispo = 1;
        }
        public static float RetourSurVente => 0.6f;
        public int IdItems { get; set; }
        public string Nom { get; set; }
        public int QuantiteStock { get; set; }
        public string TypeItem { get; set; }
        public string Description { get; set; }
        public decimal? PrixUnitaire { get; set; }
        public short? Poids { get; set; }
        public short Utilite { get; set; }
        public string LienImage { get; set; }
        public short FlagDispo { get; set; }

        public virtual Armure Armure { get; set; }
        public virtual Medicament Medicament { get; set; }
        public virtual Munition Munition { get; set; }
        public virtual Nourriture Nourriture { get; set; }
        public virtual ICollection<Arme> Armes { get; set; }
        public virtual ICollection<Evaluation> Evaluations { get; set; }
        public virtual ICollection<Panier> Paniers { get; set; }
        public virtual ICollection<SacADos> SacADos { get; set; }

        public string GetCssForClass()
        {
            return GetCssForClass(TypeItem);
        }
        public static string GetCssForClass(string type)
        {
            switch (type)
            {
                case "Armes":
                    return "type-arme";
                case "Medicaments":
                    return "type-medicament";
                case "Armures":
                    return "type-armure";
                case "Nourritures":
                    return "type-nourriture";
                case "Munitions":
                    return "type-munition";
                default:
                    return "type-unknown";
            }
        }
        public string GetCssForClassBackground()
        {
            switch (TypeItem)
            {
                case "Armes":
                    return "#ffcccc";
                case "Medicaments":
                    return "#d4f7d4";
                case "Armures":
                    return "#b3d9ff";
                case "Nourritures":
                    return " #fff5cc";
                case "Munitions":
                    return "#e6e6e6";
                default:
                    return "type-unknown";
            }
        }
        public bool CheckIfConsummable() => (IsNourriture || IsMedicament) ? true : throw new NotConsummable();
        public bool IsNourriture => this.TypeItem == "Nourritures";
        public bool IsMedicament => this.TypeItem == "Medicaments";
        public short GetHPGainM()
        {
            return (short)Medicament.GainDeVie;
        }
        public short GetHPGainN()
        {
            return (short)Nourriture.GainDeVie;
        }
        public (int, float) GetEvalsInfo()
        {
            if (Evaluations == null || !Evaluations.Any())
                return (0, 0);

            int count = Evaluations.Count();
            float average = (float)Math.Round(Evaluations.Average(e => e.NbEtoiles), 1);
            return (count, average);
        }
        public Dictionary<int, int> GetEvaluationDistribution()
        {
            var distribution = Enumerable.Range(1, 5).ToDictionary(i => i, i => 0);

            foreach (var eval in Evaluations)
            {
                if (distribution.ContainsKey(eval.NbEtoiles))
                {
                    distribution[eval.NbEtoiles]++;
                }
            }

            return distribution;
        }

    }
    class ItemNotFound : Exception { }
    class NotConsummable : Exception { }
    class CantRemoveForUtility : Exception { }
}
