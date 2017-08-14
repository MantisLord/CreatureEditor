using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CreatureEditor
{
    class Creature : ICloneable
    {
        // Creature Template
        public string Entry { get; set; }
        public string Name { get; set; }
        public string Title { get; set; } // SubName
        public string CreatureType  { get; set; }
        public string Expansion { get; set; }
        public string Rank { get; set; }
        public string UnitClass { get; set; }
        public string MinLevel { get; set; }
        public string MaxLevel { get; set; }
        public string Family { get; set; }
        public string SpeedWalk { get; set; }
        public string SpeedRun { get; set; }
        public string MovementType { get; set; }
        public string Faction { get; set; }
        public string ModelId1 { get; set; }
        public string ModelId2 { get; set; }
        public string ModelId3 { get; set; }
        public string ModelId4 { get; set; }
        public string NpcFlags { get; set; }
        public string UnitFlags { get; set; }
        public string DynamicFlags { get; set; }
        public string ExtraFlags { get; set; }
        public string CreatureTypeFlags { get; set; }
        public string LootId { get; set; }
        public string PickpocketLootId { get; set; }
        public string SkinningLootId { get; set; }
        public string EquipmentTemplateId { get; set; }
        public string GossipMenuId { get; set; }
        public string AIName { get; set; }
        public string ScriptName { get; set; }
        public string MinLootGold { get; set; }
        public string MaxLootGold { get; set; }
        public string InhabitType { get; set; }
        public string RegenerateStats { get; set; }

        public string HealthMultiplier { get; set; }
        public string ManaMultiplier { get; set; } // PowerMultiplier
        public string DamageMultiplier { get; set; }
        public string ExperienceMultiplier { get; set; }
        public string ArmorMultiplier { get; set; }

        public string Rate { get; set; } // MeleeBaseAttackTime/RangedBaseAttackTime
        public string DamageVariance { get; set; }
        public string DamageSchool { get; set; }

        public string Leash { get; set; }
        public string MechanicImmuneMask { get; set; }
        public string SchoolImmuneMask { get; set; }

        public string ResistanceArcane { get; set; }
        public string ResistanceFire { get; set; }
        public string ResistanceNature { get; set; }
        public string ResistanceFrost { get; set; }
        public string ResistanceShadow { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    class EquipmentTemplate
    {
        public string Entry { get; set; }
        public string EquipEntry1 { get; set; }
        public string EquipEntry2 { get; set; }
        public string EquipEntry3 { get; set; }
    }

    class ClassLevelStatistic
    {
        public string Level { get; set; }
        public string Class { get; set; }
        public string BaseHealthExp0 { get; set; }
        public string BaseHealthExp1 { get; set; }
        public string BaseDamageExp0 { get; set; }
        public string BaseDamageExp1 { get; set; }
        public string BaseMana { get; set; }
        public string BaseArmor { get; set; }
        public string BaseRangedAttackPower { get; set; }
        public string BaseMeleeAttackPower { get; set; }
    }

    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;
        private string port;

        public DBConnect(string serv, string db, string u, string pw, string pt)
        {
            server = serv;
            database = db;
            uid = u;
            password = pw;
            port = pt;

            Initialize();
        }

        private void Initialize()
        {
            string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "PORT=" + port + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            connection.Open();
            return true;
        }

        private bool CloseConnection()
        {
            connection.Close();
            return true;
        }

        public List<EquipmentTemplate> GetEquipment()
        {
            string query = @"SELECT 
                                A.Entry, 
                                CONCAT(item1.name, ' (', A.EquipEntry1, ')') AS EquipEntry1,
                                CONCAT(item2.name, ' (', A.EquipEntry2, ')') AS EquipEntry2,
                                CONCAT(item3.name, ' (', A.EquipEntry3, ')') AS EquipEntry3
                            FROM creature_equip_template A
                            LEFT OUTER JOIN item_template item1 ON A.EquipEntry1 = item1.entry
                            LEFT OUTER JOIN item_template item2 ON A.EquipEntry2 = item2.entry
                            LEFT OUTER JOIN item_template item3 ON A.EquipEntry3 = item3.entry";
            List<EquipmentTemplate> equipment = new List<EquipmentTemplate>();

            if (OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    EquipmentTemplate e = new EquipmentTemplate()
                    {
                        Entry = dataReader["Entry"].ToString(),
                        EquipEntry1 = dataReader["EquipEntry1"].ToString(),
                        EquipEntry2 = dataReader["EquipEntry2"].ToString(),
                        EquipEntry3 = dataReader["EquipEntry3"].ToString(),
                    };
                    equipment.Add(e);
                }

                dataReader.Close();
                CloseConnection();
            }

            return equipment;
        }

        public List<ClassLevelStatistic> GetClassLevelStats()
        {
            string query = "SELECT * FROM creature_template_classlevelstats";

            List<ClassLevelStatistic> classlvlstats = new List<ClassLevelStatistic>();
            if (OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();

                while (dataReader.Read())
                {
                    ClassLevelStatistic cls = new ClassLevelStatistic()
                    {
                        Class = dataReader["Class"].ToString(),
                        Level = dataReader["Level"].ToString(), 
                        BaseHealthExp0 = dataReader["BaseHealthExp0"].ToString(),
                        BaseHealthExp1 = dataReader["BaseHealthExp1"].ToString(),
                        BaseDamageExp0 = dataReader["BaseDamageExp0"].ToString(),
                        BaseDamageExp1 = dataReader["BaseDamageExp1"].ToString(),
                        BaseMana = dataReader["BaseMana"].ToString(),
                        BaseMeleeAttackPower = dataReader["BaseMeleeAttackPower"].ToString(),
                        BaseRangedAttackPower = dataReader["BaseRangedAttackPower"].ToString(),
                        BaseArmor = dataReader["BaseArmor"].ToString(),
                    };
                    classlvlstats.Add(cls);
                }

                dataReader.Close();
                CloseConnection();
            }

            return classlvlstats;
        }

        public List<Creature> GetCreatures()
        {
            string query = "SELECT * FROM creature_template";
            
            List<Creature> creatures = new List<Creature>();

            if (OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                
                while (dataReader.Read())
                {
                    Creature c = new Creature()
                    {
                        Entry = dataReader["Entry"].ToString(),
                        Name = dataReader["Name"].ToString(),
                        Title = dataReader["SubName"].ToString(),
                        CreatureType = dataReader["CreatureType"].ToString(),
                        Expansion = dataReader["Expansion"].ToString(),
                        Rank = dataReader["Rank"].ToString(),
                        UnitClass = dataReader["UnitClass"].ToString(),
                        MinLevel = dataReader["MinLevel"].ToString(),
                        MaxLevel = dataReader["MaxLevel"].ToString(),
                        HealthMultiplier = dataReader["HealthMultiplier"].ToString(),
                        ManaMultiplier = dataReader["PowerMultiplier"].ToString(),
                        DamageMultiplier = dataReader["DamageMultiplier"].ToString(),
                        ArmorMultiplier = dataReader["ArmorMultiplier"].ToString(),
                        ExperienceMultiplier = dataReader["ExperienceMultiplier"].ToString(),
                        Rate = dataReader["MeleeBaseAttackTime"].ToString(),
                        DamageVariance = dataReader["DamageVariance"].ToString(),
                        DamageSchool = dataReader["DamageSchool"].ToString(),
                        ResistanceArcane = dataReader["ResistanceArcane"].ToString(),
                        ResistanceFire = dataReader["ResistanceFire"].ToString(),
                        ResistanceNature = dataReader["ResistanceNature"].ToString(),
                        ResistanceFrost = dataReader["ResistanceFrost"].ToString(),
                        ResistanceShadow = dataReader["ResistanceShadow"].ToString(),
                        MechanicImmuneMask = dataReader["MechanicImmuneMask"].ToString(),
                        SchoolImmuneMask = dataReader["SchoolImmuneMask"].ToString(),
                        Leash = dataReader["LeashRange"].ToString(),
                        Family = dataReader["Family"].ToString(),
                        SpeedWalk = dataReader["SpeedWalk"].ToString(),
                        SpeedRun = dataReader["SpeedRun"].ToString(),
                        MovementType = dataReader["MovementType"].ToString(),
                        Faction = dataReader["FactionAlliance"].ToString(),
                        ModelId1 = dataReader["ModelId1"].ToString(),
                        ModelId2 = dataReader["ModelId2"].ToString(),
                        ModelId3 = dataReader["ModelId3"].ToString(),
                        ModelId4 = dataReader["ModelId4"].ToString(),
                        AIName = dataReader["AIName"].ToString(),
                        ScriptName = dataReader["ScriptName"].ToString(),
                        CreatureTypeFlags = dataReader["CreatureTypeFlags"].ToString(),
                        DynamicFlags = dataReader["DynamicFlags"].ToString(),
                        EquipmentTemplateId = dataReader["EquipmentTemplateId"].ToString(),
                        ExtraFlags = dataReader["ExtraFlags"].ToString(),
                        GossipMenuId = dataReader["GossipMenuId"].ToString(),
                        LootId = dataReader["LootId"].ToString(),
                        NpcFlags = dataReader["NpcFlags"].ToString(),
                        PickpocketLootId = dataReader["PickpocketLootId"].ToString(),
                        SkinningLootId = dataReader["SkinningLootId"].ToString(),
                        UnitFlags = dataReader["UnitFlags"].ToString(),
                        MaxLootGold = dataReader["MaxLootGold"].ToString(),
                        MinLootGold = dataReader["MinLootGold"].ToString(),
                        InhabitType = dataReader["InhabitType"].ToString(),
                        RegenerateStats = dataReader["RegenerateStats"].ToString(),
                    };
                    creatures.Add(c);
                }
                
                dataReader.Close();
                CloseConnection();
            }

            return creatures;
        }
    }
}
