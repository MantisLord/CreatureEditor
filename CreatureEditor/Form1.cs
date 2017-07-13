using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CreatureEditor
{
    public partial class frmMain : Form
    {
        private List<Creature> creatures = new List<Creature>();
        private Creature activeCreature;
        private Creature originalCreature;
        
        enum MechanicImmunities
        {
            Charm           = 1,
            Confused        = 2,
            Disarm          = 4,
            Distract        = 8,
            Fear            = 16,
            Fumble          = 32,
            Root            = 64,
            Pacify          = 128,
            Silence         = 256,
            Sleep           = 512,
            Snare           = 1024,
            Stun            = 2048,
            Freeze          = 4096,
            Knockout        = 8192,
            Bleed           = 16384,
            Bandage         = 32768,
            Polymorph       = 65536,
            Banish          = 131072,
            Shield          = 262144,
            Shackle         = 524288,
            Mount           = 1048576,
            Persuade        = 2097152,
            Turn            = 4194304,
            Horror          = 8388608,
            Invulnerability = 16777216,
            Interrupt       = 33554432,
            Daze            = 67108864,
            Discovery       = 134217728,
            ImmuneShield    = 268435456,
            Sapped          = 536870912,
        }

        enum SchoolImmunities
        {
            Normal  = 1,
            Holy    = 2,
            Fire    = 4,
            Nature  = 8,
            Frost   = 16,
            Shadow  = 32,
            Arcane  = 64,
        }

        enum UnitClass
        {
            Warrior = 1,
            Paladin = 2,
            Rogue   = 4,
            Mage    = 8,
        }

        enum DamageSchool
        {
            Physical    = 0,
            Holy        = 1,
            Fire        = 2,
            Nature      = 3,
            Frost       = 4,
            Shadow      = 5,
            Arcane      = 6,
        }

        enum Rank
        {
            Normal      = 0,
            Elite       = 1,
            RareElite   = 2,
            WorldBoss   = 3,
            Rare        = 4,
        }

        enum CreatureType
        {
            None            = 0,
            Beast           = 1,
            Dragonkin       = 2,
            Demon           = 3,
            Elemental       = 4,
            Giant           = 5,
            Undead          = 6,
            Humanoid        = 7,
            Critter         = 8,
            Mechanical      = 9,
            NotSpecified    = 10,
            Totem           = 11,
            NonCombatPet    = 12,
            GasCloud        = 13,
        }
        
        enum Family
        {
            Wolf            = 1,
            Cat             = 2,
            Spider          = 3,
            Bear            = 4,
            Boar            = 5,
            Crocolisk       = 6,
            CarrionBird     = 7,
            Crab            = 8,
            Gorilla         = 9,
            Raptor          = 10,
            Tallstrider     = 11,
            Felhunter       = 12,
            Voidwalker      = 13,
            Succubus        = 14,
            Doomguard       = 15,
            Scorpid         = 16,
            Turtle          = 17,
            Imp             = 18,
            Bat             = 19,
            Hyena           = 20,
            BirdOfPrey      = 21,
            WindSerpent     = 22,
            RemoteControl   = 23,
            Felguard        = 24,
            Dragonhawk      = 25,
            Ravager         = 26,
            WarpStalker     = 27,
            Sporebat        = 28,
            NetherRay       = 29,
            Serpent         = 30,
            Moth            = 31,
            Chimaera        = 32,
            Devilsaur       = 33,
            Ghoul           = 34,
            Silithid        = 35,
            Worm            = 36,
            Rhino           = 37,
            Wasp            = 38,
            CoreHound       = 39,
            SpiritBeast     = 40,
        }
          
        enum Expansion
        {
            Vanilla     = 0,
            Outland     = 1,
            Northrend   = 2,
        }

        public frmMain()
        {
            InitializeComponent();
            load();
        }

        private void load()
        {
            ddlClass.Items.AddRange(Enum.GetNames(typeof(UnitClass)));
            ddlDamageSchool.Items.AddRange(Enum.GetNames(typeof(DamageSchool)));
            ddlRank.Items.AddRange(Enum.GetNames(typeof(Rank)));
            ddlCreatureType.Items.AddRange(Enum.GetNames(typeof(CreatureType)));
            ddlExpansion.Items.AddRange(Enum.GetNames(typeof(Expansion)));
            ddlFamily.Items.AddRange(Enum.GetNames(typeof(Family)));

            DBConnect connect = new DBConnect();
            creatures = connect.GetCreatures();
            cbName.DataSource = creatures;
        }

        private void displayCreatureData()
        {
            txtName.Text = activeCreature.Name;
            txtTitle.Text = activeCreature.Title;
            txtMinLevel.Text = activeCreature.MinLevel;
            txtMaxLevel.Text = activeCreature.MaxLevel;
            txtLeash.Text = activeCreature.Leash;
            txtWalk.Text = activeCreature.SpeedWalk;
            txtRun.Text = activeCreature.SpeedRun;

            txtMultiplierArmor.Text = activeCreature.ArmorMultiplier;
            txtMultiplierDamage.Text = activeCreature.DamageMultiplier;
            txtMultiplierExperience.Text = activeCreature.ExperienceMultiplier;
            txtMultiplierHealth.Text = activeCreature.HealthMultiplier;
            txtMultiplierMana.Text = activeCreature.ManaMultiplier;
            txtDamageVariance.Text = activeCreature.DamageVariance;

            txtResistanceArcane.Text = activeCreature.ResistanceArcane;
            txtResistanceFire.Text = activeCreature.ResistanceFire;
            txtResistanceFrost.Text = activeCreature.ResistanceFrost;
            txtResistanceNature.Text = activeCreature.ResistanceNature;
            txtResistanceShadow.Text = activeCreature.ResistanceShadow;

            txtRate.Text = (Convert.ToDouble(activeCreature.Rate) / 1000).ToString();
            populateMechanicImmunities(Convert.ToInt64(activeCreature.MechanicImmuneMask));
            populateSchoolImmunities(Convert.ToInt64(activeCreature.SchoolImmuneMask));

            ddlClass.SelectedItem = Enum.GetName(typeof(UnitClass), Convert.ToInt32(activeCreature.UnitClass));
            ddlDamageSchool.SelectedItem = Enum.GetName(typeof(DamageSchool), Convert.ToInt32(activeCreature.DamageSchool));
            ddlRank.SelectedItem = Enum.GetName(typeof(Rank), Convert.ToInt32(activeCreature.Rank));
            ddlCreatureType.SelectedItem = Enum.GetName(typeof(CreatureType), Convert.ToInt32(activeCreature.CreatureType));
            ddlExpansion.SelectedItem = Enum.GetName(typeof(Expansion), Convert.ToInt32(activeCreature.Expansion));
            ddlFamily.SelectedItem = Enum.GetName(typeof(Family), Convert.ToInt32(activeCreature.Family));
              
            recalculateCombatStats();
        }

        private void updateClassLevelStats()
        {
            Creature result = creatures.FirstOrDefault(a => a.MinLevel == activeCreature.MinLevel && a.UnitClass == activeCreature.UnitClass);
            activeCreature.BaseDamageExp0 = result.BaseDamageExp0;
            activeCreature.BaseDamageExp1 = result.BaseDamageExp1;
            activeCreature.BaseHealthExp0 = result.BaseHealthExp0;
            activeCreature.BaseHealthExp1 = result.BaseHealthExp1;
            activeCreature.BaseArmor = result.BaseArmor;
            activeCreature.BaseMana = result.BaseMana;
            activeCreature.BaseMeleeAttackPower = result.BaseMeleeAttackPower;
            activeCreature.BaseRangedAttackPower = result.BaseRangedAttackPower;
        }

        private void recalculateCombatStats()
        {
            double health, mana, armor, attackPower, minDmg, maxDmg, baseHealth, baseDamage;
            health = mana = armor = attackPower = minDmg = maxDmg = baseHealth = baseDamage = 0.0;

            try
            {
                mana = Convert.ToDouble(activeCreature.BaseMana) * Convert.ToDouble(activeCreature.ManaMultiplier);
                armor = Convert.ToDouble(activeCreature.BaseArmor) * Convert.ToDouble(activeCreature.ArmorMultiplier);
                attackPower = Convert.ToDouble(activeCreature.BaseMeleeAttackPower);

                switch (activeCreature.Expansion)
                {
                    case "0":
                        baseDamage = Convert.ToDouble(activeCreature.BaseDamageExp0);
                        baseHealth = Convert.ToDouble(activeCreature.BaseHealthExp0);
                        break;
                    case "1":
                        baseHealth = Convert.ToDouble(activeCreature.BaseHealthExp1);
                        baseDamage = Convert.ToDouble(activeCreature.BaseDamageExp1);
                        break;
                }

                health = baseHealth * Convert.ToDouble(activeCreature.HealthMultiplier);
                minDmg = (baseDamage * Convert.ToDouble(activeCreature.DamageVariance) + attackPower / 14.0) * (Convert.ToDouble(activeCreature.Rate) / 1000.0) * Convert.ToDouble(activeCreature.DamageMultiplier);
                maxDmg = (baseDamage * Convert.ToDouble(activeCreature.DamageVariance) * 1.5 + attackPower / 14.0) * (Convert.ToDouble(activeCreature.Rate) / 1000.0) * Convert.ToDouble(activeCreature.DamageMultiplier);
            }
            catch
            {
                // do nothing, user likely entered bad data
            }
            finally
            { 
                // fill in results
                txtHealth.Text = health.ToString();
                txtMinDmg.Text = minDmg.ToString();
                txtMaxDmg.Text = maxDmg.ToString();
                txtMana.Text = mana.ToString();
                txtAttackPower.Text = attackPower.ToString();
                txtArmor.Text = armor.ToString();
            }
        }

        private void populateMechanicImmunities(long mask)
        {
            var allMechanics = Enum.GetValues(typeof(MechanicImmunities));
            Array.Sort(allMechanics);
            Array.Reverse(allMechanics);

            clbMechanicImmune.Items.Clear();
            txtMechanicImmuneMask.Text = mask.ToString();

            foreach (MechanicImmunities m in allMechanics)
            {
                if (mask >= (long)m)
                {
                    mask -= (long)m;
                    clbMechanicImmune.Items.Add(m, true);
                }
                else
                {
                    clbMechanicImmune.Items.Add(m, false);
                }
            }

            clbMechanicImmune.Refresh();
        }

        private void populateSchoolImmunities(long mask)
        {
            var allSchools = Enum.GetValues(typeof(SchoolImmunities));
            Array.Sort(allSchools);
            Array.Reverse(allSchools);

            clbSchoolImmune.Items.Clear();
            txtSchoolImmuneMask.Text = mask.ToString();

            foreach (SchoolImmunities m in allSchools)
            {
                if (mask >= (long)m)
                {
                    mask -= (long)m;
                    clbSchoolImmune.Items.Add(m, true);
                }
                else
                {
                    clbSchoolImmune.Items.Add(m, false);
                }
            }

            clbSchoolImmune.Refresh();
        }

        private void displayMechanicImmuneMaskText()
        {
            long total = 0;
            foreach (MechanicImmunities m in clbMechanicImmune.CheckedItems)
            {
                total += (long)m;
            }

            txtMechanicImmuneMask.Text = total.ToString();
        }

        private void displaySchoolImmuneMaskText()
        {
            long total = 0;
            foreach (SchoolImmunities m in clbSchoolImmune.CheckedItems)
            {
                total += (long)m;
            }

            txtSchoolImmuneMask.Text = total.ToString();
        }

        private void showCreatureById()
        {
            var creatureId = txtEntry.Text;
            cbName.SelectedValue = creatureId;
            Creature result = creatures.FirstOrDefault(a => a.Entry == creatureId);
            if (result != null)
            {
                activeCreature = result;
                originalCreature = (Creature)result.Clone();
                displayCreatureData();
                recalculateCombatStats();
            }
            else
            {
                MessageBox.Show("No creature found.");
            }
        }

        private void generateSQL()
        {
            // update activeCreature object with user's changes (fields used for combat stat calculation should already be up to date because of their handlers)
            CreatureType ct;
            Expansion ex;
            Family fam;
            Rank rnk;
            DamageSchool ds;

            activeCreature.Name = txtName.Text;
            activeCreature.Title = txtTitle.Text;

            Enum.TryParse<CreatureType>(ddlCreatureType.Text, out ct);
            activeCreature.CreatureType = ((int)ct).ToString();

            Enum.TryParse<Expansion>(ddlExpansion.Text, out ex);
            activeCreature.Expansion = ((int)ex).ToString();

            if (ddlFamily.SelectedItem != null)
            {
                Enum.TryParse<Family>(ddlFamily.Text, out fam);
                activeCreature.Family = ((int)fam).ToString();
            }

            Enum.TryParse<Rank>(ddlRank.Text, out rnk);
            activeCreature.Rank = ((int)rnk).ToString();

            Enum.TryParse<DamageSchool>(ddlDamageSchool.Text, out ds);
            activeCreature.DamageSchool = ((int)ds).ToString();
            
            activeCreature.SpeedRun = txtRun.Text;
            activeCreature.SpeedWalk = txtWalk.Text;
            activeCreature.MinLevel = txtMinLevel.Text;
            activeCreature.MaxLevel = txtMaxLevel.Text;
            activeCreature.Leash = txtLeash.Text;
            activeCreature.MechanicImmuneMask = txtMechanicImmuneMask.Text;
            activeCreature.SchoolImmuneMask = txtSchoolImmuneMask.Text;
            activeCreature.ResistanceArcane = txtResistanceArcane.Text;
            activeCreature.ResistanceFire = txtResistanceFire.Text;
            activeCreature.ResistanceFrost = txtResistanceFrost.Text;
            activeCreature.ResistanceNature = txtResistanceNature.Text;
            activeCreature.ResistanceShadow = txtResistanceShadow.Text;

            string update = "UPDATE `creature_temmplate` SET ";

            // compare current creature to what was initially loaded from DB

            if (activeCreature.Name != originalCreature.Name) update += "`Name`='" + activeCreature.Name + "', ";
            if (activeCreature.Title != originalCreature.Title) update += "`Title`='" + activeCreature.Title + "', ";
            if (activeCreature.CreatureType != originalCreature.CreatureType) update += "`CreatureType`='" + activeCreature.CreatureType + "', ";
            if (activeCreature.Expansion != originalCreature.Expansion) update += "`Expansion`='" + activeCreature.Expansion + "', ";
            if (activeCreature.Family != originalCreature.Family) update += "`Family`='" + activeCreature.Family + "', ";
            if (activeCreature.Rank != originalCreature.Rank) update += "`Rank`='" + activeCreature.Rank + "', ";
            if (activeCreature.DamageSchool != originalCreature.DamageSchool) update += "`DamageSchool`='" + activeCreature.DamageSchool + "', ";
            if (activeCreature.SpeedRun != originalCreature.SpeedRun) update += "`SpeedRun`='" + activeCreature.SpeedRun + "', ";
            if (activeCreature.SpeedWalk != originalCreature.SpeedWalk) update += "`SpeedWalk`='" + activeCreature.SpeedWalk + "', ";
            if (activeCreature.MinLevel != originalCreature.MinLevel) update += "`MinLevel`='" + activeCreature.MinLevel + "', ";
            if (activeCreature.MaxLevel != originalCreature.MaxLevel) update += "`MaxLevel`='" + activeCreature.MaxLevel + "', ";
            if (activeCreature.Leash != originalCreature.Leash) update += "`LeashRange`='" + activeCreature.Leash + "', ";
            if (activeCreature.MechanicImmuneMask != originalCreature.MechanicImmuneMask) update += "`MechanicImmuneMask`='" + activeCreature.MechanicImmuneMask + "', ";
            if (activeCreature.SchoolImmuneMask != originalCreature.SchoolImmuneMask) update += "`SchoolImmuneMask`='" + activeCreature.SchoolImmuneMask + "', ";
            if (activeCreature.ResistanceArcane != originalCreature.ResistanceArcane) update += "`ResistanceArcane`='" + activeCreature.ResistanceArcane + "', ";
            if (activeCreature.ResistanceFire != originalCreature.ResistanceFire) update += "`ResistanceFire`='" + activeCreature.ResistanceFire + "', ";
            if (activeCreature.ResistanceFrost != originalCreature.ResistanceFrost) update += "`ResistanceFrost`='" + activeCreature.ResistanceFrost + "', ";
            if (activeCreature.ResistanceNature != originalCreature.ResistanceNature) update += "`ResistanceNature`='" + activeCreature.ResistanceNature + "', ";
            if (activeCreature.ResistanceShadow != originalCreature.ResistanceShadow) update += "`ResistanceShadow`='" + activeCreature.ResistanceShadow + "', ";

            if (activeCreature.HealthMultiplier != originalCreature.HealthMultiplier) update += "`HealthMultiplier`='" + activeCreature.HealthMultiplier + "', ";
            if (activeCreature.ManaMultiplier != originalCreature.ManaMultiplier) update += "`PowerMultiplier`='" + activeCreature.ManaMultiplier + "', ";
            if (activeCreature.DamageMultiplier != originalCreature.DamageMultiplier) update += "`DamageMultiplier`='" + activeCreature.DamageMultiplier + "', ";
            if (activeCreature.ExperienceMultiplier != originalCreature.ExperienceMultiplier) update += "`ExperienceMultiplier`='" + activeCreature.ExperienceMultiplier + "', ";
            if (activeCreature.Rate != originalCreature.Rate) update += "`MeleeBaseAttackTime`='" + activeCreature.Rate + "', ";
            if (activeCreature.DamageVariance != originalCreature.DamageVariance) update += "`DamageVariance`='" + activeCreature.DamageVariance + "', ";

            update = update.Substring(0, update.Length - 2);
            update += " WHERE `Entry`='" + activeCreature.Entry + "';";

            MessageBox.Show(update);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            showCreatureById();
        }

        private void ddlName_SelectedIndexChanged(object sender, EventArgs e)
        {
            var creatureId = cbName.SelectedValue;
            if (creatureId != null)
            {
                txtEntry.Text = creatureId.ToString();
                showCreatureById();
            }
        }

        private void clbMechanicImmune_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            displayMechanicImmuneMaskText();
        }

        private void txtMultiplierHealth_TextChanged(object sender, EventArgs e)
        {
            activeCreature.HealthMultiplier = txtMultiplierHealth.Text;
            recalculateCombatStats();
        }

        private void txtMultiplierMana_TextChanged(object sender, EventArgs e)
        {
            activeCreature.ManaMultiplier = txtMultiplierMana.Text;
            recalculateCombatStats();
        }

        private void txtMultiplierDamage_TextChanged(object sender, EventArgs e)
        {
            activeCreature.DamageMultiplier = txtMultiplierDamage.Text;
            recalculateCombatStats();
        }

        private void txtMultiplierArmor_TextChanged(object sender, EventArgs e)
        {
            activeCreature.ArmorMultiplier = txtMultiplierArmor.Text;
            recalculateCombatStats();
        }

        private void txtMultiplierExperience_TextChanged(object sender, EventArgs e)
        {
            activeCreature.ExperienceMultiplier = txtMultiplierExperience.Text;
        }

        private void txtMinLevel_TextChanged(object sender, EventArgs e)
        {
            activeCreature.MinLevel = txtMinLevel.Text;
            updateClassLevelStats();
            recalculateCombatStats();
        }

        private void ddlClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlClass.SelectedValue != null && ddlClass.SelectedValue.ToString() != null)
            {
                activeCreature.UnitClass = ((UnitClass)Enum.Parse(typeof(UnitClass), ddlClass.SelectedItem.ToString())).ToString();
                updateClassLevelStats();
                recalculateCombatStats();
            }
        }

        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(txtRate.Text))
            { 
                activeCreature.Rate = (Convert.ToDouble(txtRate.Text) * 1000).ToString();
                recalculateCombatStats();
            }
        }

        private void txtDamageVariance_TextChanged(object sender, EventArgs e)
        {
            activeCreature.DamageVariance = txtDamageVariance.Text;
            recalculateCombatStats();
        }

        private void btnGenerateSQL_Click(object sender, EventArgs e)
        {
            generateSQL();
        }

        private void clbSchoolImmune_SelectedIndexChanged(object sender, EventArgs e)
        {
            displaySchoolImmuneMaskText();
        }
    }
}
