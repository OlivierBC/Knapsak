
using Microsoft.EntityFrameworkCore;
namespace Knapsak_CFTW.Models
{
    public class SessionService
    {
        private readonly ApplicationDbContext _db;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(ApplicationDbContext db, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
        }

        private ISession Session => _httpContextAccessor.HttpContext.Session;

        private const string SessionKey = "playerId";

        public Joueur GetConnected(bool throwError = true)
        {
            if (!IsConnected())
            {
                if (throwError)
                    throw new NotConnectedException();
                return null;
            }

            var idStr = Session.GetString(SessionKey);
            if (!int.TryParse(idStr, out var id))
            {
                if (throwError)
                    throw new NotConnectedException();
                return null;
            }

            return _db.Joueurs.Include(j => j.SacADos).Include(j => j.Paniers).FirstOrDefault(j => j.IdJoueurs == id);
        }

        public bool IsConnected()
        {
            return !string.IsNullOrEmpty(Session.GetString(SessionKey)) && int.TryParse(Session.GetString(SessionKey), out _);
        }

        public void ClearSession()
        {
            SetSessionId("");
        }

        public bool IsAdmin()
        {
            return IsConnected() && GetConnected(false)?.EstAdmin == 1;
        }

        public Item GetItem(int id)
        {
            return _db.Items.FirstOrDefault(i => i.IdItems == id) ?? throw new ItemNotFound();
        }

        public void CheckItemAvailability(Item item, int quantite)
        {
            if (item.QuantiteStock < quantite || item.FlagDispo == 0)
                throw new NotAvailableException();
        }

        public int GetCurrentWeight(int idJoueur)
        {
            return _db.SacADos
                .Where(i => i.IdJoueur == idJoueur)
                .Sum(i => i.Quantite * (i.IdItemNavigation.Poids ?? 0));
        }

        public int GetNbItemsPanier(int idJoueur)
        {
            return _db.Paniers
                .Where(i => i.IdJoueur == idJoueur)
                .Sum(i => i.Quantite);
        }

        public int GetPanierWeight(int idJoueur)
        {
            return _db.Paniers
                .Where(i => i.IdJoueur == idJoueur)
                .Sum(i => i.Quantite * (i.IdItemNavigation.Poids ?? 0));
        }

        public int GetPanierPrice(int idJoueur)
        {
            return _db.Paniers
                .Where(p => p.IdJoueur == idJoueur)
                .Join(_db.Items,
                      panier => panier.IdItem,
                      item => item.IdItems,
                      (panier, item) => panier.Quantite * (int)item.PrixUnitaire)
                .Sum();
        }
        public void SetSessionId(string id)
        {
            Session.SetString(SessionKey, id);
        }
        public void SetSessionId(int id)
        {
            Session.SetString(SessionKey, id.ToString());
        }
        public void UpdateSessionPlayerInfo()
        {
            var joueur = GetConnected(false);
            if (joueur == null)
            {
                ClearSession();
                return;
            }

            joueur.PoidsTotal = (short)GetCurrentWeight(joueur.IdJoueurs);
            joueur.NbItemsPanier = GetNbItemsPanier(joueur.IdJoueurs);
            joueur.PointsDeVie = Math.Clamp(joueur.PointsDeVie, (short)0, Joueur.MaxHealth);

            if (joueur.DexteriteAffiche > 100)
            {
                joueur.Dexterite -= (joueur.DexteriteAffiche - 100);
            }

            _db.Joueurs.Update(joueur);
            _db.SaveChanges();
            SetSessionId(joueur.IdJoueurs);
        }

        public short GetGainVie(int idItem) => GetGainVie(GetItem(idItem));

        public short GetGainVie(Item item)
        {
            short? gainVie = item.IsNourriture
                ? _db.Nourritures.First(n => n.IdItem == item.IdItems).GainDeVie
                : item.IsMedicament
                    ? _db.Medicaments.First(m => m.IdItem == item.IdItems).GainDeVie
                    : 0;

            return (short)(gainVie ?? 0);
        }
        public List<Evaluation> GetEvalsWithComments(int idItem)
        {
            return _db.Items
            .Where(i => i.IdItems == idItem)
            .Include(i => i.Evaluations)
                .ThenInclude(e => e.IdJoueurNavigation)
            .SelectMany(i => i.Evaluations)
            .Where(e => !string.IsNullOrEmpty(e.Commentaire))
            .ToList();
        }
        public bool ConnectedPlayerHasItem(int idItem)
        {
            var joueur = GetConnected(false);
            if (joueur != null)
                return joueur.SacADos.Where(i => i.IdItem == idItem).Any();
            else
                return false;
        }
    }
    
    public class NotConnectedException : Exception { }

}