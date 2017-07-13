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

        // Class Level Stats
        public string BaseHealthExp0 { get; set; }
        public string BaseHealthExp1 { get; set; }
        public string BaseMana { get; set; }
        public string BaseDamageExp0 { get; set; }
        public string BaseDamageExp1 { get; set; }
        public string BaseMeleeAttackPower { get; set; }
        public string BaseRangedAttackPower { get; set; }
        public string BaseArmor { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    class DBConnect
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string uid;
        private string password;

        public DBConnect()
        {
            Initialize();
        }

        private void Initialize()
        {
            server = "localhost";
            database = "voamangos";
            uid = "root";
            password = "password";

            string connectionString = "SERVER=" + server + ";" + "DATABASE=" + database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public List<Creature> GetCreatures()
        {
            string query = "SELECT * FROM creature_template A JOIN creature_template_classlevelstats B ON A.MinLevel = B.Level AND A.UnitClass = B.Class";
            
            List<Creature> creatures = new List<Creature>();

            if (OpenConnection())
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader dataReader = cmd.ExecuteReader();
                
                while (dataReader.Read())
                {
                    Creature c = new Creature()
                    {
                        // Creature Template
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

                        // Class Level Stats
                        BaseHealthExp0 = dataReader["BaseHealthExp0"].ToString(),
                        BaseHealthExp1 = dataReader["BaseHealthExp1"].ToString(),
                        BaseDamageExp0 = dataReader["BaseDamageExp0"].ToString(),
                        BaseDamageExp1 = dataReader["BaseDamageExp1"].ToString(),
                        BaseMana = dataReader["BaseMana"].ToString(),
                        BaseMeleeAttackPower = dataReader["BaseMeleeAttackPower"].ToString(),
                        BaseRangedAttackPower = dataReader["BaseRangedAttackPower"].ToString(),
                        BaseArmor = dataReader["BaseArmor"].ToString(),
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
