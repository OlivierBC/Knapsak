using System;
using Knapsak_CFTW.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Knapsak_CFTW.Models
{
    public partial class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Arme> Armes { get; set; }
        public virtual DbSet<Armure> Armures { get; set; }
        public virtual DbSet<Enigme> Enigmes { get; set; }
        public virtual DbSet<Reponse> Reponses { get; set; }
        public virtual DbSet<Evaluation> Evaluations { get; set; }
        public virtual DbSet<Item> Items { get; set; }
        public virtual DbSet<Joueur> Joueurs { get; set; }
        public virtual DbSet<Medicament> Medicaments { get; set; }
        public virtual DbSet<Munition> Munitions { get; set; }
        public virtual DbSet<Nourriture> Nourritures { get; set; }
        public virtual DbSet<Panier> Paniers { get; set; }
        public virtual DbSet<Difficulte> Difficultes { get; set; }
        public virtual DbSet<SacADos> SacADos { get; set; }
        public virtual DbSet<Statistique> Statistiques { get; set; }
        public virtual DbSet<Requetes> Requetes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Arme>(entity =>
            {
                entity.HasKey(e => new { e.IdItem, e.IdMunitions })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.IdMunitions, "FK_Armes_Munitions_idx");

                entity.Property(e => e.IdItem)
                    .HasColumnType("int(11)")
                    .HasColumnName("idItem");

                entity.Property(e => e.IdMunitions)
                    .HasColumnType("int(11)")
                    .HasColumnName("idMunitions");

                entity.Property(e => e.Efficacite)
                    .HasColumnType("int(11)")
                    .HasColumnName("efficacite");

                entity.Property(e => e.Genre)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("genre");

                entity.HasOne(d => d.IdItemNavigation)
                    .WithMany(p => p.Armes)
                    .HasForeignKey(d => d.IdItem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Armes_Items");

                entity.HasOne(d => d.IdMunitionsNavigation)
                    .WithMany(p => p.Armes)
                    .HasForeignKey(d => d.IdMunitions)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Armes_Munitions");
            });

            modelBuilder.Entity<Armure>(entity =>
            {
                entity.HasKey(e => e.IdItem)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdItem)
                    .HasColumnType("int(11)")
                    .HasColumnName("idItem");

                entity.Property(e => e.Matiere)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("matiere");

                entity.Property(e => e.Taille)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("taille");

                entity.HasOne(d => d.IdItemNavigation)
                    .WithOne(p => p.Armure)
                    .HasForeignKey<Armure>(d => d.IdItem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Armures_Items");
            });

            modelBuilder.Entity<Enigme>(entity =>
            {
                entity.HasKey(e => e.IdEnigmes)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Difficulte, "fk_Enigmes_diffculte");

                entity.HasIndex(e => e.IdEnigmes, "idEnigmes_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.IdEnigmes)
                    .HasColumnType("int(11)")
                    .HasColumnName("idEnigmes");

                entity.Property(e => e.Difficulte)
                    .HasColumnType("tinyint(4)")
                    .HasColumnName("difficulte");

                entity.Property(e => e.EstActif)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("estActif");

                entity.Property(e => e.Question)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("question");

                entity.HasOne(d => d.DifficulteNavigation)
                    .WithMany(p => p.Enigmes)
                    .HasForeignKey(d => d.Difficulte)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_Enigmes_diffculte");
            });

            modelBuilder.Entity<Evaluation>(entity =>
            {
                entity.HasKey(e => new { e.IdJoueur, e.IdItem })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.IdItem, "FK_idItems_Items_idx");

                entity.Property(e => e.IdJoueur)
                    .HasColumnType("int(11)")
                    .HasColumnName("idJoueur");

                entity.Property(e => e.IdItem)
                    .HasColumnType("int(11)")
                    .HasColumnName("idItem");

                entity.Property(e => e.Commentaire)
                    .HasMaxLength(200)
                    .HasColumnName("commentaire")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.NbEtoiles)
                    .IsRequired()
                    .HasColumnType("smallint(6)")
                    .HasColumnName("nbEtoiles");

                entity.HasOne(d => d.IdItemNavigation)
                    .WithMany(p => p.Evaluations)
                    .HasForeignKey(d => d.IdItem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Evaluations_Items");

                entity.HasOne(d => d.IdJoueurNavigation)
                    .WithMany(p => p.Evaluations)
                    .HasForeignKey(d => d.IdJoueur)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Evaluations_Joueurs");
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.IdItems)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.IdItems, "idItems_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.IdItems)
                    .HasColumnType("int(11)")
                    .HasColumnName("idItems");

                entity.Property(e => e.Description)
                    .HasMaxLength(100)
                    .HasColumnName("description")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.FlagDispo)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("flagDispo")
                    .HasDefaultValue("1");

                entity.Property(e => e.LienImage)
                    .IsRequired()
                    .HasMaxLength(150)
                    .HasColumnName("lienImage");

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("nom");

                entity.Property(e => e.Poids)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("poids")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.PrixUnitaire)
                    .HasColumnType("decimal(10,0)")
                    .HasColumnName("prixUnitaire")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.QuantiteStock)
                    .HasColumnType("int(11)")
                    .HasColumnName("quantiteStock");

                entity.Property(e => e.TypeItem)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("typeItem");

                entity.Property(e => e.Utilite)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("utilite");
            });

            modelBuilder.Entity<Joueur>(entity =>
            {
                entity.HasKey(e => e.IdJoueurs)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.Alias, "alias_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.IdJoueurs, "idJoueurs_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.IdJoueurs)
                    .HasColumnType("int(11)")
                    .HasColumnName("idJoueurs");

                entity.Property(e => e.Alias)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("alias");

                entity.Property(e => e.Dexterite)
                    .HasColumnType("int(11)")
                    .HasColumnName("dexterite")
                    .HasDefaultValueSql("10");

                entity.Property(e => e.EstAdmin)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("estAdmin")
                    .HasDefaultValue("0");

                entity.Property(e => e.MPasse)
                    .IsRequired()
                    .HasColumnType("longtext")
                    .HasColumnName("mPasse");

                entity.Property(e => e.Montant)
                    .HasColumnType("int(11)")
                    .HasColumnName("montant")
                    .HasDefaultValueSql("1000");

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("nom");

                entity.Property(e => e.PoidsTotal)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("poidsTotal")
                    .HasDefaultValueSql("'100'");

                entity.Property(e => e.PointsDeVie)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("pointsDeVie")
                    .HasDefaultValueSql("'50'");

                entity.Property(e => e.Prenom)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("prenom");
            });

            modelBuilder.Entity<Medicament>(entity =>
            {
                entity.HasKey(e => e.IdItem)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdItem)
                    .HasColumnType("int(11)")
                    .HasColumnName("idItem");

                entity.Property(e => e.Duree)
                    .HasColumnType("int(11)")
                    .HasColumnName("duree")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Effet)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("effet");

                entity.Property(e => e.EffetIndesirable)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("effetIndesirable");

                entity.Property(e => e.GainDeVie)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("gainDeVie")
                    .HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.IdItemNavigation)
                    .WithOne(p => p.Medicament)
                    .HasForeignKey<Medicament>(d => d.IdItem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Medicaments_Items");
            });

            modelBuilder.Entity<Munition>(entity =>
            {
                entity.HasKey(e => e.IdItem)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdItem)
                    .HasColumnType("int(11)")
                    .HasColumnName("idItem");

                entity.Property(e => e.Calibre)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("calibre");

                entity.HasOne(d => d.IdItemNavigation)
                    .WithOne(p => p.Munition)
                    .HasForeignKey<Munition>(d => d.IdItem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Munitions_Items");
            });

            modelBuilder.Entity<Nourriture>(entity =>
            {
                entity.HasKey(e => e.IdItem)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdItem)
                    .HasColumnType("int(11)")
                    .HasColumnName("idItem");

                entity.Property(e => e.Calories)
                    .HasColumnType("int(11)")
                    .HasColumnName("calories")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.ComposantNutritif)
                    .HasMaxLength(30)
                    .HasColumnName("composantNutritif")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.GainDeVie)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("gainDeVie")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Mineral)
                    .HasMaxLength(30)
                    .HasColumnName("mineral")
                    .HasDefaultValueSql("'NULL'");

                entity.HasOne(d => d.IdItemNavigation)
                    .WithOne(p => p.Nourriture)
                    .HasForeignKey<Nourriture>(d => d.IdItem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Nourritures_Items");
            });

            modelBuilder.Entity<Panier>(entity =>
            {
                entity.HasKey(e => new { e.IdJoueur, e.IdItem })
                    .HasName("PRIMARY");

                entity.ToTable("Panier");

                entity.HasIndex(e => e.IdItem, "FK_idItems_Items_idx");

                entity.Property(e => e.IdJoueur)
                    .HasColumnType("int(11)")
                    .HasColumnName("idJoueur");

                entity.Property(e => e.IdItem)
                    .HasColumnType("int(11)")
                    .HasColumnName("idItem");

                entity.Property(e => e.Quantite)
                    .HasColumnType("int(11)")
                    .HasColumnName("quantite");

                entity.HasOne(d => d.IdItemNavigation)
                    .WithMany(p => p.Paniers)
                    .HasForeignKey(d => d.IdItem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Paniers_Items");

                entity.HasOne(d => d.IdJoueurNavigation)
                    .WithMany(p => p.Paniers)
                    .HasForeignKey(d => d.IdJoueur)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Paniers_Joueurs");
            });

            modelBuilder.Entity<Difficulte>(entity =>
            {
                entity.HasKey(e => e.IdDifficulte)
                    .HasName("PRIMARY");

                entity.ToTable("Difficulte");

                entity.HasIndex(e => e.IdDifficulte, "difficulte_UNIQUE")
                    .IsUnique();

                entity.Property(e => e.IdDifficulte)
                    .HasColumnType("tinyint(4)")
                    .HasColumnName("idDifficulte");

                entity.Property(e => e.GainCaps)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("gainCaps");

                entity.Property(e => e.Nom)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnName("nom");

                entity.Property(e => e.PerteVie)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("perteVie");
            });

            modelBuilder.Entity<Reponse>(entity =>
            {
                entity.HasKey(e => e.IdReponses)
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.IdEnigmes, "FK_Reponses_Enigmes");

                entity.Property(e => e.IdReponses)
                    .HasColumnType("int(11)")
                    .HasColumnName("idReponses");

                entity.Property(e => e.EstBonneRep)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("estBonneRep");

                entity.Property(e => e.IdEnigmes)
                    .HasColumnType("int(11)")
                    .HasColumnName("idEnigmes");

                entity.Property(e => e.ReponseText)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("reponse");

                entity.HasOne(d => d.IdEnigmesNavigation)
                    .WithMany(p => p.Reponses)
                    .HasForeignKey(d => d.IdEnigmes)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Reponses_Enigmes");
            });

            modelBuilder.Entity<SacADos>(entity =>
            {
                entity.HasKey(e => new { e.IdJoueur, e.IdItem })
                    .HasName("PRIMARY");

                entity.ToTable("SacADos");

                entity.HasIndex(e => e.IdItem, "FK_SacADos_Items_idx");

                entity.Property(e => e.IdJoueur)
                    .HasColumnType("int(11)")
                    .HasColumnName("idJoueur");

                entity.Property(e => e.IdItem)
                    .HasColumnType("int(11)")
                    .HasColumnName("idItem");

                entity.Property(e => e.Quantite)
                    .HasColumnType("int(11)")
                    .HasColumnName("quantite");

                entity.HasOne(d => d.IdItemNavigation)
                    .WithMany(p => p.SacADos)
                    .HasForeignKey(d => d.IdItem)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SacADos_Items");

                entity.HasOne(d => d.IdJoueurNavigation)
                    .WithMany(p => p.SacADos)
                    .HasForeignKey(d => d.IdJoueur)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_SacADos_Joueurs");
            });

            modelBuilder.Entity<Statistique>(entity =>
            {
                entity.HasKey(e => new { e.IdJoueur, e.IdEnigme })
                    .HasName("PRIMARY");

                entity.HasIndex(e => e.IdEnigme, "FK_Statistiques_Enigmes_idx");

                entity.Property(e => e.IdJoueur)
                    .HasColumnType("int(11)")
                    .HasColumnName("idJoueur");

                entity.Property(e => e.IdEnigme)
                    .HasColumnType("int(11)")
                    .HasColumnName("idEnigme");

                entity.Property(e => e.NbRate)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("nbRate");

                entity.Property(e => e.NbReussi)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("nbReussi");

                entity.HasOne(d => d.IdEnigmeNavigation)
                    .WithMany(p => p.Statistiques)
                    .HasPrincipalKey(p => p.IdEnigmes)
                    .HasForeignKey(d => d.IdEnigme)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Statistiques_Enigmes");

                entity.HasOne(d => d.IdJoueurNavigation)
                    .WithMany(p => p.Statistiques)
                    .HasForeignKey(d => d.IdJoueur)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Statistiques_Joueurs");
            });

            modelBuilder.Entity<Requetes>(entity =>
            {
                entity.HasKey(e => e.IdRequete)
                    .HasName("PRIMARY");

                entity.Property(e => e.IdRequete)
                    .HasColumnType("int(11)")
                    .HasColumnName("IdRequete");

                entity.Property(e => e.IdJoueurs)
                    .HasColumnType("int(11)")
                    .HasColumnName("IdJoueurs");

                entity.Property(e => e.CapsulesDemandes)
                    .HasColumnType("int(11)")
                    .HasColumnName("CapsulesDemandes");

                entity.Property(e => e.EstReglee)
                    .HasColumnType("smallint(6)")
                    .HasColumnName("EstReglee")
                    .HasDefaultValue(0);

                entity.HasOne(d => d.Joueur)
                    .WithMany(p => p.Requetes)
                    .HasForeignKey(d => d.IdJoueurs)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Requetes_Joueurs");
            });


            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
