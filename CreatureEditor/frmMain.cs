using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CreatureEditor
{
    public partial class frmMain : Form
    {
        private List<Creature> creatures = new List<Creature>();
        private List<ClassLevelStatistic> basestats = new List<ClassLevelStatistic>();
        private List<EquipmentTemplate> equipment = new List<EquipmentTemplate>();

        private Creature activeCreature;
        private Creature originalCreature;
        private ClassLevelStatistic activeClassLevelStats;
        private EquipmentTemplate activeEquipment;
        private bool ignoreTextChanges = false;
        
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
            Unknown     = 5,
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
            HorseCustom     = 10,                    // not exist in DBC but used for horse like beasts in DB
            Raptor          = 11,
            Tallstrider     = 12,
            Felhunter       = 15,
            Voidwalker      = 16,
            Succubus        = 17,
            Doomguard       = 19,
            Scorpid         = 20,
            Turtle          = 21,
            Imp             = 23,
            Bat             = 24,
            Hyena           = 25,
            Owl             = 26,
            WindSerpent     = 27,
            RemoteControl   = 28,
            Felguard        = 29,
            Dragonhawk      = 30,
            Ravager         = 31,
            WarpStalker     = 32,
            Sporebat        = 33,
            NetherRay       = 34,
            Serpent         = 35,
            SeaLion         = 36
        }
          
        enum Expansion
        {
            Vanilla     = 0,
            Outland     = 1,
            Northrend   = 2,
        }

        enum CreatureExtraFlags
        {
            InstanceBind        = 0x00000001,       // creature kill bind instance with killer and killer's group
            Civilian            = 0x00000002,       // not aggro (ignore faction/reputation hostility)
            NoParry             = 0x00000004,       // creature can't parry
            NoParryHasten       = 0x00000008,       // creature can't counter-attack at parry
            NoBlock             = 0x00000010,       // creature can't block
            NoCrush             = 0x00000020,       // creature can't do crush attacks
            NoXPAtKill          = 0x00000040,       // creature kill not provide XP
            Invisible           = 0x00000080,       // creature is always invisible for player (mostly trigger creatures)
            NotTauntable        = 0x00000100,       // creature is immune to taunt auras and effect attack me
            AggroZone           = 0x00000200,       // creature sets itself in combat with zone on aggro
            Guard               = 0x00000400,       // creature is a guard
            NoCallAssist        = 0x00000800,       // creature shouldn't call for assistance on aggro
            Active              = 0x00001000,       // creature is active object. Grid of this creature will be loaded and creature set as active
            MMapForceEnable     = 0x00002000,       // creature is forced to use MMaps
            MMapForceDisable    = 0x00004000,       // creature is forced to NOT use MMaps
            WalkInWater         = 0x00008000,       // creature is forced to walk in water even it can swim
            HaveNoSwimAnimation = 0x00010000,       // we have to not set "swim" animation or creature will have "no animation"
            NoMelee             = 0x00020000,       // creature can't melee
            FarView             = 0x00040000,       // creature with far view
            DontDismountOnAggro = 0x00080000,       // creature is mounted and should fight mounted
            IgnoreUsedPosition  = 0x00100000,       // ignore creature when checking used positions around target
            CountSpawns         = 0x00200000,       // count creature spawns in Map*
        }

        enum CreatureTypeFlags
        {
            Tameable        = 0x00000001,       // Tameable by any hunter
            GhostVisible    = 0x00000002,       // Creatures which can _also_ be seen when player is a ghost, used in CanInteract function by client, can't be attacked
            Unk3            = 0x00000004,       // "BOSS" flag for tooltips
            Unk4            = 0x00000008,
            Unk5            = 0x00000010,       // controls something in client tooltip related to creature faction
            Unk6            = 0x00000020,       // may be sound related
            Unk7            = 0x00000040,       // may be related to attackable / not attackable creatures with spells, used together with lua_IsHelpfulSpell/lua_IsHarmfulSpell
            Unk8            = 0x00000080,       // has something to do with unit interaction / quest status requests
            HerbLoot        = 0x00000100,       // Can be looted by herbalist
            MiningLoot      = 0x00000200,       // Can be looted by miner
            Unk11           = 0x00000400,       // no idea, but it used by client
            Unk12           = 0x00000800,       // related to possibility to cast spells while mounted
            CanAssist       = 0x00001000,       // Can aid any player (and group) in combat. Typically seen for escorting NPC's
            Unk14           = 0x00002000,       // checked from calls in Lua_PetHasActionBar
            Unk15           = 0x00004000,       // Lua_UnitGUID, client does guid_low &= 0xFF000000 if this flag is set
            EngineerLoot    = 0x00008000,       // Can be looted by engineer
        }

        enum UnitDynFlags
        {
            None            = 0x0000,
            Lootable        = 0x0001,
            TrackUnit       = 0x0002,
            Tapped          = 0x0004,       // Lua_UnitIsTapped
            Rooted          = 0x0008,
            SpecialInfo     = 0x0010,
            Dead            = 0x0020,
            ReferAFriend    = 0x0040,
        }

        enum NPCFlags
        {
            None                = 0x00000000,
            Gossip              = 0x00000001,       // 100%
            QuestGiver          = 0x00000002,       // guessed, probably ok
            Unk1                = 0x00000004,
            Unk2                = 0x00000008,
            Trainer             = 0x00000010,       // 100%
            TrainerClass        = 0x00000020,       // 100%
            TrainerProfession   = 0x00000040,       // 100%
            Vender              = 0x00000080,       // 100%
            VenderAmmo          = 0x00000100,       // 100%, general goods vendor
            VendorFood          = 0x00000200,       // 100%
            VendorPoison        = 0x00000400,       // guessed
            VendorReagent       = 0x00000800,       // 100%
            Repair              = 0x00001000,       // 100%
            FlightMaster        = 0x00002000,       // 100%
            SpiritHealer        = 0x00004000,       // guessed
            SpiritGuide         = 0x00008000,       // guessed
            Innkeeper           = 0x00010000,       // 100%
            Banker              = 0x00020000,       // 100%
            Petitioner          = 0x00040000,       // 100% 0xC0000 = guild petitions, 0x40000 = arena team petitions
            TabardDesigner      = 0x00080000,       // 100%
            BattleMaster        = 0x00100000,       // 100%
            Auctioneer          = 0x00200000,       // 100%
            StableMaster        = 0x00400000,       // 100%
            GuildBanker         = 0x00800000,       // cause client to send 997 opcode
            SpellClick          = 0x01000000,       // cause client to send 1015 opcode (spell click), dynamic, set at loading and don't must be set in DB
        }

        enum UnitFlags
        {
            Unk0                = 0x00000001,
            NonAttackable       = 0x00000002,           // not attackable
            NonMovingDeprecated = 0x00000004,           // TODO: Needs research
            PlayerControlled    = 0x00000008,           // players, pets, totems, guardians, companions, charms, any units associated with players
            Rename              = 0x00000010,
            Preparation         = 0x00000020,           // don't take reagents for spells with SPELL_ATTR_EX5_NO_REAGENT_WHILE_PREP
            Unk6                = 0x00000040,
            NotAttackable1      = 0x00000080,           // ?? (UNIT_FLAG_PVP_ATTACKABLE | UNIT_FLAG_NOT_ATTACKABLE_1) is NON_PVP_ATTACKABLE - blue color target
            ImmuneToPlayer      = 0x00000100,           // 2.0.8 - (OOC Out Of Combat) Can not be attacked when not in combat. Removed if unit for some reason enter combat (flag probably removed for the attacked and it's party/group only)
            ImmuneToNpc         = 0x00000200,           // makes you unable to attack everything. Almost identical to our "civilian"-term. Will ignore it's surroundings and not engage in combat unless "called upon" or engaged by another unit.
            Looting             = 0x00000400,           // loot animation
            PetInCombat         = 0x00000800,           // in combat?, 2.0.8 Possibly Unkillable
            PvP                 = 0x00001000,
            Silenced            = 0x00002000,           // silenced, 2.1.1
            Unk14               = 0x00004000,           // 2.0.8
            Swimming            = 0x00008000,           // controls water swimming animation - TODO: confirm whether dynamic or static
            Unk16               = 0x00010000,           // removes attackable icon, if on yourself, cannot assist self but can cast TARGET_SELF spells
            Pacified            = 0x00020000,
            Stunned             = 0x00040000,           // stunned, 2.1.1
            InCombat            = 0x00080000,
            TaxiFlight          = 0x00100000,           // disable casting at client side spell not allowed by taxi flight (mounted?), probably used with 0x4 flag
            Disarmed            = 0x00200000,           // disable melee spells casting..., "Required melee weapon" added to melee spells tooltip.
            Confused            = 0x00400000,
            Fleeing             = 0x00800000,
            Possessed           = 0x01000000,           // remote control e.g. Eyes of the Beast: let master use melee attack, make unit unselectable via mouse for master in world (as if it was own character)
            NotSelectable       = 0x02000000,
            Skinnable           = 0x04000000,
            Mount               = 0x08000000,
            Unk28               = 0x10000000,
            Unk29               = 0x20000000,           // used in Feign Death spell
            Sheathe             = 0x40000000,
        }

        enum MovementType
        {
            Waypoint    = 2,
            Random      = 1,
            Idle        = 0,
        }

        enum InhabitType
        {
            GroundOnly              = 1,
            WaterOnly               = 2,
            GroundAndWater          = 3,
            AlwaysFlying            = 4,
            OverGroundAlwaysFly     = 5,
            OverWaterAlwaysFly      = 6,
            AlwaysFlyOverAnything   = 7,
        }

        enum RegenerateStats
        {
            NoRegen         = 0,
            Health          = 1,
            Power           = 2,
            HealthAndPower  = 3,
        }

        public frmMain()
        {
            InitializeComponent();
        }

        public void load(string serv, string db, string u, string pw, string pt)
        {
            ddlClass.Items.AddRange(Enum.GetNames(typeof(UnitClass)));
            ddlDamageSchool.Items.AddRange(Enum.GetNames(typeof(DamageSchool)));
            ddlRank.Items.AddRange(Enum.GetNames(typeof(Rank)));
            ddlCreatureType.Items.AddRange(Enum.GetNames(typeof(CreatureType)));
            ddlExpansion.Items.AddRange(Enum.GetNames(typeof(Expansion)));
            ddlFamily.Items.AddRange(Enum.GetNames(typeof(Family)));
            ddlMoveType.Items.AddRange(Enum.GetNames(typeof(MovementType)));
            ddlInhabitType.Items.AddRange(Enum.GetNames(typeof(InhabitType)));
            ddlRegenStats.Items.AddRange(Enum.GetNames(typeof(RegenerateStats)));

            DBConnect connect = new DBConnect(serv, db, u, pw, pt);
            creatures = connect.GetCreatures();
            equipment = connect.GetEquipment();
            basestats = connect.GetClassLevelStats();
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
            txtFaction.Text = activeCreature.Faction;
            txtModel1.Text = activeCreature.ModelId1;
            txtModel2.Text = activeCreature.ModelId2;
            txtModel3.Text = activeCreature.ModelId3;
            txtModel4.Text = activeCreature.ModelId4;

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

            txtLootDeath.Text = activeCreature.LootId;
            txtLootPickPocket.Text = activeCreature.PickpocketLootId;
            txtLootSkinning.Text = activeCreature.SkinningLootId;
            txtEquipmentTemplateId.Text = activeCreature.EquipmentTemplateId;

            int money = Convert.ToInt32(activeCreature.MinLootGold);
            int copper = money % 100;
            money = (money - copper) / 100;
            int silver = money % 100;
            int gold = (money - silver) / 100;

            txtMinMoneyC.Text = copper.ToString();
            txtMinMoneyG.Text = gold.ToString();
            txtMinMoneyS.Text = silver.ToString();

            money = Convert.ToInt32(activeCreature.MaxLootGold);
            copper = money % 100;
            money = (money - copper) / 100;
            silver = money % 100;
            gold = (money - silver) / 100;

            txtMaxMoneyC.Text = copper.ToString();
            txtMaxMoneyG.Text = gold.ToString();
            txtMaxMoneyS.Text = silver.ToString();

            txtRate.Text = (Convert.ToDouble(activeCreature.Rate) / 1000).ToString();
            populateFlagValues(typeof(MechanicImmunities), Convert.ToInt32(activeCreature.MechanicImmuneMask), clbMechanicImmune, txtMechanicImmuneMask);
            populateFlagValues(typeof(SchoolImmunities), Convert.ToInt32(activeCreature.SchoolImmuneMask), clbSchoolImmune, txtSchoolImmuneMask);
            populateFlagValues(typeof(UnitFlags), Convert.ToInt32(activeCreature.UnitFlags), clbUnitFlags, txtUnitFlagsMask);
            populateFlagValues(typeof(UnitDynFlags), Convert.ToInt32(activeCreature.DynamicFlags), clbDynamicFlags, txtDynamicFlagsMask);
            populateFlagValues(typeof(CreatureTypeFlags), Convert.ToInt32(activeCreature.CreatureTypeFlags), clbCreatureTypeFlags, txtCreatureTypeFlagsMask);
            populateFlagValues(typeof(NPCFlags), Convert.ToInt32(activeCreature.NpcFlags), clbNPCFlags, txtNPCFlagsMask);
            populateFlagValues(typeof(CreatureExtraFlags), Convert.ToInt32(activeCreature.ExtraFlags), clbExtraFlags, txtExtraFlagsMask);

            ddlClass.SelectedItem = Enum.GetName(typeof(UnitClass), Convert.ToInt32(activeCreature.UnitClass));
            ddlDamageSchool.SelectedItem = Enum.GetName(typeof(DamageSchool), Convert.ToInt32(activeCreature.DamageSchool));
            ddlRank.SelectedItem = Enum.GetName(typeof(Rank), Convert.ToInt32(activeCreature.Rank));
            ddlCreatureType.SelectedItem = Enum.GetName(typeof(CreatureType), Convert.ToInt32(activeCreature.CreatureType));
            ddlExpansion.SelectedItem = Enum.GetName(typeof(Expansion), Convert.ToInt32(activeCreature.Expansion));
            ddlFamily.SelectedItem = Enum.GetName(typeof(Family), Convert.ToInt32(activeCreature.Family));
            ddlMoveType.SelectedItem = Enum.GetName(typeof(MovementType), Convert.ToInt32(activeCreature.MovementType));
            ddlInhabitType.SelectedItem = Enum.GetName(typeof(InhabitType), Convert.ToInt32(activeCreature.InhabitType));
            ddlRegenStats.SelectedItem = Enum.GetName(typeof(RegenerateStats), Convert.ToInt32(activeCreature.RegenerateStats));
              
            recalculateCombatStats();
        }

        private void updateEquipment()
        {
            EquipmentTemplate result = equipment.FirstOrDefault(a => a.Entry == activeCreature.EquipmentTemplateId);

            if (result != null)
            {
                activeEquipment = result;
                txtEquipLeft.Text = activeEquipment.EquipEntry1;
                txtEquipRight.Text = activeEquipment.EquipEntry2;
                txtEquipRange.Text = activeEquipment.EquipEntry3;
            }
            else
            {
                txtEquipLeft.Text = "";
                txtEquipRight.Text = "";
                txtEquipRange.Text = "";
            }
        }

        private void updateClassLevelStats()
        {
            ClassLevelStatistic result = basestats.FirstOrDefault(a => a.Level == activeCreature.MinLevel && a.Class == activeCreature.UnitClass);

            if (result != null)
            {
                activeClassLevelStats = result;
                txtBaseDamageExp0.Text = activeClassLevelStats.BaseDamageExp0;
                txtBaseDamageExp1.Text = activeClassLevelStats.BaseDamageExp1;
                txtBaseHealthExp0.Text = activeClassLevelStats.BaseHealthExp0;
                txtBaseHealthExp1.Text = activeClassLevelStats.BaseHealthExp1;
                txtBaseArmor.Text = activeClassLevelStats.BaseArmor;
                txtBaseMana.Text = activeClassLevelStats.BaseMana;
                txtBaseRangeAP.Text = activeClassLevelStats.BaseRangedAttackPower;
                txtBaseMeleeAP.Text = activeClassLevelStats.BaseMeleeAttackPower;
            }
        }

        private void recalculateCombatStats()
        {
            if (ignoreTextChanges)
                return;

            double health, mana, armor, attackPower, minDmg, maxDmg, baseHealth, baseDamage;
            health = mana = armor = attackPower = minDmg = maxDmg = baseHealth = baseDamage = 0.0;

            try
            {
                mana = Convert.ToDouble(activeClassLevelStats.BaseMana) * Convert.ToDouble(activeCreature.ManaMultiplier);
                armor = Convert.ToDouble(activeClassLevelStats.BaseArmor) * Convert.ToDouble(activeCreature.ArmorMultiplier);
                attackPower = Convert.ToDouble(activeClassLevelStats.BaseMeleeAttackPower);

                switch (activeCreature.Expansion)
                {
                    case "0":
                        baseDamage = Convert.ToDouble(activeClassLevelStats.BaseDamageExp0);
                        baseHealth = Convert.ToDouble(activeClassLevelStats.BaseHealthExp0);
                        break;
                    case "1":
                        baseHealth = Convert.ToDouble(activeClassLevelStats.BaseHealthExp1);
                        baseDamage = Convert.ToDouble(activeClassLevelStats.BaseDamageExp1);
                        break;
                }

                health = baseHealth * Convert.ToDouble(activeCreature.HealthMultiplier);
                minDmg = (baseDamage * Convert.ToDouble(activeCreature.DamageVariance) + attackPower / 14.0) * (Convert.ToDouble(activeCreature.Rate) / 1000.0) * Convert.ToDouble(activeCreature.DamageMultiplier);
                maxDmg = (baseDamage * Convert.ToDouble(activeCreature.DamageVariance) * 1.5 + attackPower / 14.0) * (Convert.ToDouble(activeCreature.Rate) / 1000.0) * Convert.ToDouble(activeCreature.DamageMultiplier);

                ignoreTextChanges = true;
                txtHealth.Text = health.ToString();
                txtMinDmg.Text = minDmg.ToString();
                txtMaxDmg.Text = maxDmg.ToString();
                txtMana.Text = mana.ToString();
                txtAttackPower.Text = attackPower.ToString();
                txtArmor.Text = armor.ToString();
                ignoreTextChanges = false;
            }
            catch
            {
                // do nothing, user likely entered bad data
            }
        }
        
        private void populateFlagValues(Type t, int mask, CheckedListBox clb, TextBox txt)
        {
            clb.Items.Clear();
            var allValues = Enum.GetValues(t);
            Array.Sort(allValues);
            Array.Reverse(allValues);
            foreach (var m in allValues)
            {
                if (mask >= (int)m)
                {
                    mask -= (int)m;
                    clb.Items.Add(m, true);
                }
                else
                {
                    clb.Items.Add(m, false);
                }
            }
            displayFlagMaskText(clb, txt);
        }

        private void displayFlagMaskText(CheckedListBox clb, TextBox txt)
        {
            int total = 0;
            foreach (int m in clb.CheckedItems)
            {
                total += (int)m;
            }
            txt.Text = total.ToString();
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
                updateClassLevelStats();
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
            InhabitType it;
            RegenerateStats rs;

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

            Enum.TryParse<InhabitType>(ddlInhabitType.Text, out it);
            activeCreature.InhabitType = ((int)it).ToString();

            Enum.TryParse<RegenerateStats>(ddlRegenStats.Text, out rs);
            activeCreature.RegenerateStats = ((int)rs).ToString();
            
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
            activeCreature.UnitFlags = txtUnitFlagsMask.Text;
            activeCreature.CreatureTypeFlags = txtCreatureTypeFlagsMask.Text;
            activeCreature.ExtraFlags = txtExtraFlagsMask.Text;
            activeCreature.DynamicFlags = txtDynamicFlagsMask.Text;
            activeCreature.NpcFlags = txtNPCFlagsMask.Text;
            activeCreature.LootId = txtLootDeath.Text;
            activeCreature.SkinningLootId = txtLootSkinning.Text;
            activeCreature.PickpocketLootId = txtLootPickPocket.Text;
            activeCreature.EquipmentTemplateId = txtEquipmentTemplateId.Text;
            activeCreature.MinLootGold = (Convert.ToInt32(txtMinMoneyC.Text) + Convert.ToInt32(txtMinMoneyG.Text) * 1000 + Convert.ToInt32(txtMinMoneyS.Text) * 100).ToString();
            activeCreature.MaxLootGold = (Convert.ToInt32(txtMaxMoneyC.Text) + Convert.ToInt32(txtMaxMoneyG.Text) * 1000 + Convert.ToInt32(txtMaxMoneyS.Text) * 100).ToString();

            string update = "UPDATE `creature_template` SET ";

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
            if (activeCreature.UnitFlags != originalCreature.UnitFlags) update += "`UnitFlags`='" + activeCreature.UnitFlags + "', ";
            if (activeCreature.CreatureTypeFlags != originalCreature.CreatureTypeFlags) update += "`CreatureTypeFlags`='" + activeCreature.CreatureTypeFlags + "', ";
            if (activeCreature.ExtraFlags != originalCreature.ExtraFlags) update += "`ExtraFlags`='" + activeCreature.ExtraFlags + "', ";
            if (activeCreature.DynamicFlags != originalCreature.DynamicFlags) update += "`DynamicFlags`='" + activeCreature.DynamicFlags + "', ";
            if (activeCreature.NpcFlags != originalCreature.NpcFlags) update += "`NpcFlags`='" + activeCreature.NpcFlags + "', ";
            if (activeCreature.MinLootGold != originalCreature.MinLootGold) update += "`MinLootGold`='" + activeCreature.MinLootGold + "', ";
            if (activeCreature.MaxLootGold != originalCreature.MaxLootGold) update += "`MaxLootGold`='" + activeCreature.MaxLootGold + "', ";
            if (activeCreature.InhabitType != originalCreature.InhabitType) update += "`InhabitType`='" + activeCreature.InhabitType + "', ";
            if (activeCreature.RegenerateStats != originalCreature.RegenerateStats) update += "`RegenerateStats`='" + activeCreature.RegenerateStats + "', ";

            if (activeCreature.LootId != originalCreature.LootId) update += "`LootId`='" + activeCreature.LootId + "', ";
            if (activeCreature.SkinningLootId != originalCreature.SkinningLootId) update += "`SkinningLootId`='" + activeCreature.SkinningLootId + "', ";
            if (activeCreature.PickpocketLootId != originalCreature.PickpocketLootId) update += "`PickpocketLootId`='" + activeCreature.PickpocketLootId + "', ";
            if (activeCreature.EquipmentTemplateId != originalCreature.EquipmentTemplateId) update += "`EquipmentTemplateId`='" + activeCreature.EquipmentTemplateId + "', ";

            if (activeCreature.HealthMultiplier != originalCreature.HealthMultiplier) update += "`HealthMultiplier`='" + activeCreature.HealthMultiplier + "', ";
            if (activeCreature.ManaMultiplier != originalCreature.ManaMultiplier) update += "`PowerMultiplier`='" + activeCreature.ManaMultiplier + "', ";
            if (activeCreature.DamageMultiplier != originalCreature.DamageMultiplier) update += "`DamageMultiplier`='" + activeCreature.DamageMultiplier + "', ";
            if (activeCreature.ExperienceMultiplier != originalCreature.ExperienceMultiplier) update += "`ExperienceMultiplier`='" + activeCreature.ExperienceMultiplier + "', ";
            if (activeCreature.Rate != originalCreature.Rate) update += "`MeleeBaseAttackTime`='" + activeCreature.Rate + "', ";
            if (activeCreature.DamageVariance != originalCreature.DamageVariance) update += "`DamageVariance`='" + activeCreature.DamageVariance + "', ";

            if (update.Length == 31)
            {
                MessageBox.Show("No changes detected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            update = update.Substring(0, update.Length - 2);
            update += " WHERE `Entry`='" + activeCreature.Entry + "';";
            
            Form f = new Form();
            f.Text = "SQL Result";
            TextBox t = new TextBox();
            t.Dock = DockStyle.Fill;
            t.Multiline = true;
            t.WordWrap = true;
            t.Text = update;
            f.Controls.Add(t);
            f.ShowDialog();
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

        private void btnGenerateSQL_Click(object sender, EventArgs e)
        {
            generateSQL();
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
        private void txtMaxLevel_TextChanged(object sender, EventArgs e)
        {
            activeCreature.MaxLevel = txtMaxLevel.Text;
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

        private void clbMechanicImmune_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayFlagMaskText(clbMechanicImmune, txtMechanicImmuneMask);
        }
        private void clbSchoolImmune_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayFlagMaskText(clbSchoolImmune, txtSchoolImmuneMask);
        }
        private void clbExtraFlags_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayFlagMaskText(clbExtraFlags, txtExtraFlagsMask);
        }
        private void clbNPCFlags_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayFlagMaskText(clbNPCFlags, txtNPCFlagsMask);
        }
        private void clbCreatureTypeFlags_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayFlagMaskText(clbCreatureTypeFlags, txtCreatureTypeFlagsMask);
        }
        private void clbUnitFlags_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayFlagMaskText(clbUnitFlags, txtUnitFlagsMask);
        }
        private void clbDynamicFlags_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayFlagMaskText(clbDynamicFlags, txtDynamicFlagsMask);
        }

        private void txtHealth_TextChanged(object sender, EventArgs e)
        {
            if (ignoreTextChanges)
                return;

            double health = Convert.ToDouble(txtHealth.Text);
            double baseHealth = 0.0, multiplierHealth = 0.0;

            switch (activeCreature.Expansion)
            {
                case "0":
                    baseHealth = Convert.ToDouble(activeClassLevelStats.BaseHealthExp0);
                    break;
                case "1":
                    baseHealth = Convert.ToDouble(activeClassLevelStats.BaseHealthExp1);
                    break;
            }

            multiplierHealth = health / baseHealth;
            ignoreTextChanges = true;
            txtMultiplierHealth.Text = multiplierHealth.ToString();
            ignoreTextChanges = false;
        }
        private void txtMana_TextChanged(object sender, EventArgs e)
        {
            if (ignoreTextChanges)
                return;

            double mana = Convert.ToDouble(txtMana.Text);
            double baseMana = Convert.ToDouble(activeClassLevelStats.BaseMana), multiplierPower = 0.0;

            multiplierPower = mana / baseMana;
            ignoreTextChanges = true;
            txtMultiplierMana.Text = multiplierPower.ToString();
            ignoreTextChanges = false;
        }
        private void txtMinDmg_TextChanged(object sender, EventArgs e)
        {
            if (ignoreTextChanges)
                return;

            double minDmg = Convert.ToDouble(txtMinDmg.Text);
            double baseDamage, attackPower, multiplierDamage, damageVariance, rate;
            baseDamage = attackPower = multiplierDamage = damageVariance = rate = 0.0;

            attackPower = Convert.ToDouble(activeClassLevelStats.BaseMeleeAttackPower);
            damageVariance = Convert.ToDouble(activeCreature.DamageVariance);
            rate = Convert.ToDouble(activeCreature.Rate) / 1000.0;

            switch (activeCreature.Expansion)
            {
                case "0":
                    baseDamage = Convert.ToDouble(activeClassLevelStats.BaseDamageExp0);
                    break;
                case "1":
                    baseDamage = Convert.ToDouble(activeClassLevelStats.BaseDamageExp1);
                    break;
            }

            multiplierDamage = ((baseDamage * damageVariance + attackPower / 14.0) * rate) / minDmg;
            ignoreTextChanges = true;
            txtMultiplierDamage.Text = multiplierDamage.ToString();
            ignoreTextChanges = false;
        }
        private void txtMaxDmg_TextChanged(object sender, EventArgs e)
        {
            if (ignoreTextChanges)
                return;

            double maxDmg = Convert.ToDouble(txtMaxDmg.Text);

            double baseDamage, attackPower, multiplierDamage, damageVariance, rate;
            baseDamage = attackPower = multiplierDamage = damageVariance = rate = 0.0;

            attackPower = Convert.ToDouble(activeClassLevelStats.BaseMeleeAttackPower);
            damageVariance = Convert.ToDouble(activeCreature.DamageVariance);
            rate = Convert.ToDouble(activeCreature.Rate) / 1000.0;

            switch (activeCreature.Expansion)
            {
                case "0":
                    baseDamage = Convert.ToDouble(activeClassLevelStats.BaseDamageExp0);
                    break;
                case "1":
                    baseDamage = Convert.ToDouble(activeClassLevelStats.BaseDamageExp1);
                    break;
            }

            multiplierDamage = ((baseDamage * damageVariance * 1.5 + attackPower / 14.0) * rate) / maxDmg;
            ignoreTextChanges = true;
            txtMultiplierDamage.Text = multiplierDamage.ToString();
            ignoreTextChanges = false;
        }
        private void txtArmor_TextChanged(object sender, EventArgs e)
        {
            if (ignoreTextChanges)
                return;

            double armor = Convert.ToDouble(txtArmor.Text);
            double baseArmor = Convert.ToDouble(activeClassLevelStats.BaseArmor), multiplierArmor = 0.0;

            multiplierArmor = armor / baseArmor;
            ignoreTextChanges = true;
            txtMultiplierArmor.Text = multiplierArmor.ToString();
            ignoreTextChanges = false;
        }

        private void txtEquipmentTemplateId_TextChanged(object sender, EventArgs e)
        {
            activeCreature.EquipmentTemplateId = txtEquipmentTemplateId.Text;
            updateEquipment();
        }

        private void txtUnitFlagsMask_TextChanged(object sender, EventArgs e)
        {
            try
            {
                activeCreature.UnitFlags = txtUnitFlagsMask.Text;
                populateFlagValues(typeof(UnitFlags), Convert.ToInt32(activeCreature.UnitFlags), clbUnitFlags, txtUnitFlagsMask);
            }
            catch { }
        }

        private void txtMechanicImmuneMask_TextChanged(object sender, EventArgs e)
        {
            try
            {
                activeCreature.MechanicImmuneMask = txtMechanicImmuneMask.Text;
                populateFlagValues(typeof(MechanicImmunities), Convert.ToInt32(activeCreature.MechanicImmuneMask), clbMechanicImmune, txtMechanicImmuneMask);
            }
            catch { }
        }

        private void txtExtraFlagsMask_TextChanged(object sender, EventArgs e)
        {
            try
            { 
                activeCreature.ExtraFlags = txtExtraFlagsMask.Text;
                populateFlagValues(typeof(CreatureExtraFlags), Convert.ToInt32(activeCreature.ExtraFlags), clbExtraFlags, txtExtraFlagsMask);
            }
            catch { }
        }

        private void txtNPCFlagsMask_TextChanged(object sender, EventArgs e)
        {
            try
            {
                activeCreature.NpcFlags = txtNPCFlagsMask.Text;
                populateFlagValues(typeof(NPCFlags), Convert.ToInt32(activeCreature.NpcFlags), clbNPCFlags, txtNPCFlagsMask);
            }
            catch { }
        }

        private void txtCreatureTypeFlagsMask_TextChanged(object sender, EventArgs e)
        {
            try
            {
                activeCreature.CreatureTypeFlags = txtCreatureTypeFlagsMask.Text;
                populateFlagValues(typeof(CreatureTypeFlags), Convert.ToInt32(activeCreature.CreatureTypeFlags), clbCreatureTypeFlags, txtCreatureTypeFlagsMask);
            }
            catch { }
        }

        private void txtDynamicFlagsMask_TextChanged(object sender, EventArgs e)
        {
            try
            {
                activeCreature.DynamicFlags = txtDynamicFlagsMask.Text;
                populateFlagValues(typeof(UnitDynFlags), Convert.ToInt32(activeCreature.DynamicFlags), clbDynamicFlags, txtDynamicFlagsMask);
            }
            catch { }
        }
    }
}
