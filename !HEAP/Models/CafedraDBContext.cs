using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Diplom_v1._0._0.Models
{
    //public class People
    //{
    //    private CafedraDBContext context;
    //    public int Id { get; set; }
    //    public string Fam { get; set; }
    //    public string Name { get; set; }
    //    public string Otchestvo { get; set; }
    //}
    public class CafedraDBContext
    {
        public string ConnectionString { get; set; }
        public CafedraDBContext(string connectionString)
        {
            this.ConnectionString = connectionString;
        }
        private MySqlConnection GetSqlConnection()
        {
            return new MySqlConnection(ConnectionString);
        }
        public List<person> GetPeoples()
        {
            var ret = new List<person>();
            using(var conn = GetSqlConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM people", conn);
                using(var reader = cmd.ExecuteReader())
                {
                    //var tb = reader.FieldCount;
                    
                    while (reader.Read())
                    {
                        ret.Add(new person() {
                            id_man = reader.GetInt32("id_man"),
                            fam = reader.GetString("fam"),
                            name = reader.GetString("name"),
                            otchestvo = reader.GetString("otchestvo"),
                            INN = reader.GetString("INN"),
                            sv_vo_PFR = reader.GetString("sv_vo_PFR"),
                            serial_pasport = reader.GetString("serial_pasport"),
                            number_pasport = reader.GetString("number_pasport"),
                            data_vidachi_pasporta = reader.GetString("data_vidachi_pasporta"),
                            kem_vidan = reader.GetString("kem_vidan"),
                            data_rojdeniya = reader.GetString("data_rojdeniya"),
                            strana_1 = reader.GetString("strana_1"),
                            region_1 = reader.GetString("region_1"),
                            index_1 = reader.GetString("index_1"),
                            gorod_1 = reader.GetString("gorod_1"),
                            ulica_1 = reader.GetString("ulica_1"),
                            dom_1 = reader.GetString("dom_1"),
                            kv_1 = reader.GetString("kv_1"),
                            strana_2 = reader.GetString("strana_2"),
                            region_2 = reader.GetString("region_2"),
                            index_2 = reader.GetString("index_2"),
                            gorod_2 = reader.GetString("gorod_2"),
                            ulica_2 = reader.GetString("ulica_2"),
                            dom_2 = reader.GetString("dom_2"),
                            kv_2 = reader.GetString("kv_2"),
                            telefon_dom = reader.GetString("telefon_dom"),
                            telefon_sot = reader.GetString("telefon_sot"),
                            e_mail = reader.GetString("e_mail"),
                            mesto_raboti = reader.GetString("mesto_raboti"),
                            ceh_otdel = reader.GetString("ceh_otdel"),
                            doljnost = reader.GetString("doljnost"),
                            telefon_rabochii = reader.GetString("telefon_rabochii"),
                            semeinoe_polojenie = reader.GetString("semeinoe_polojenie"),
                            nomer_vb = reader.GetString("nomer_vb"),
                            data_vidachi_vb = reader.GetString("data_vidachi_vb"),
                            kategoruya_godnosti = reader.GetString("kategoruya_godnosti"),
                            komissariat = reader.GetString("komissariat"),
                            kategoriya_zapasa = reader.GetString("kategoriya_zapasa"),
                            voinskoe_zvanie = reader.GetString("voinskoe_zvanie"),
                            kod_VUS = reader.GetString("kod_VUS"),
                            sostav_profil = reader.GetString("sostav_profil"),
                            voinskii_uchet = reader.GetString("voinskii_uchet"),
                            chto_zakonchil = reader.GetString("chto_zakonchil"),
                            kogda_zakonchil = reader.GetString("kogda_zakonchil"),
                            nacionalnost = reader.GetString("nacionalnost"),
                            pol = reader.GetString("pol"),
                            data_change = reader.GetString("data_change"),
                            dokument_ob_obrazov = reader.GetString("dokument_ob_obrazov"),
                            serial_dokumenta = reader.GetString("serial_dokumenta"),
                            number_dokumenta = reader.GetString("number_dokumenta"),
                            data_reg_po_mj = reader.GetString("data_reg_po_mj"),
                            sem_polojenie_OKIN = reader.GetString("sem_polojenie_OKIN"),
                            kod_OKATO = reader.GetString("kod_OKATO"),
                            kod_OKIN_grajd = reader.GetString("kod_OKIN_grajd"),
                            kod_OKIN_inyaz = reader.GetString("kod_OKIN_inyaz"),
                            staj = reader.GetString("staj"),
                            kod1 = reader.GetString("kod1"),
                            data_zapoln_anketi = reader.GetString("data_zapoln_anketi")
                            //** = reader.GetString("*"),
                        });
                    }
                }
            }
            //conn.Close();
            return ret;
        }
        public List<student> GetStudents(){
            var ret = new List<student>();
            using(var conn = GetSqlConnection())
            {
                conn.Open();
                var cmd = new MySqlCommand("SELECT * FROM student", conn);
                using(var reader = cmd.ExecuteReader())
                {                    
                    while (reader.Read())
                    {
                        ret.Add(new student(){
                            id_man = reader.GetInt32("id_man"),
                            number_zach = reader.GetString("number_zach")
                        });
                    }
                }
            }
            return ret;
        }
    }
}
