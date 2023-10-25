using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows;
using MySqlConnector;

namespace euroskills
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    /* Beállítjuk a localhost (Xampp elindításával) és elkészítjük az adatbázist euroskills névvel.
      A Megjegyzések.txt-ben a táblák feltöltése megtalálható, az adatok pedig az adatok.sql fájlban. */


    // Az SQL használata miatt be kell állítani a következőket,  Különben nem fog működni a program.
    // Rámegyünk egérrel az Explorerben a "euroskills"-re, azután jobb klikk --->
    // ---> megnyitjuk a "Manage NuGet Packages "...---> ezután "Browse" és keresőbe beírjuk SQL vagy MySQL és installáljuk valamelyiket.
    // Én a delfinest tettem be ( MySqlConnector), de a MySql.Data is jó, csak akkor fent, a using beállításánál is figyelni kell erre.  

    public partial class MainWindow : Window
    {
        // A következő adatok betöltéséhez példákat lehet látni a "Megjegyzések.txt" fájlban. 


        // 1. SQL kapcsolat létrehozása:
        private readonly string connectionString = "datasource=127.0.0.1;port=3306;username=root;password=;database=euroskills;";

        /* datasource -- ez a linkben is látható, de ha véletlen nem, akkor a localhost felső sorában a 'kiszolgálónál'.
                         Általában a számsor ugyanaz.
           port -- két helyen is magtalálható. Egyik az Xampp használatakor, mikor elindítjuk az adatbázist.
                   A START-ok mellett van egy 4 jegyű számsor, illetve megtalálható a localhost-ban is a 'változók' menüben
                   a 'port'-nál. Egyébként a  'változók' menüben szinte minden megvan és módosítható is, aki  ért hozzá.                  
           username -- a localhost felső sorában a 'felhasználó fiókok' mezőben találjuk. Azt a sort kell nézni, amelyikben 
                       a kiszolgáló számsora is benne van.
           password -- ha van megadva az adatbázishoz jelszó, itt is meg kell adni. Ha nincs, akkor üres stringként kell beírni,
                       mert a  password sem maradhat ki.
           database -- az adatbázis neve    */

        //------------------------------------------------------------------------------------------------------------------------


        // 2. Kapcsolat objektuma:
        private MySqlConnection connection;                           // Ha nem működne a MySqlConnection,
                                                                      // akkor a using-ot is be kell állítani:
                                                                      // (using MySql.Data.MySqlClient;)

        // én ezt állítottam be, mert nekem a MySqlConnector van letöltve:
        // (using MySqlConnector;)

        // 4. Lista létrehozása:
        private List<Versenyzo> versenyzok;

        //-----------------------------------------------------------------------------------------------------------       
        public MainWindow()
        {
            InitializeComponent();
        }

        //------------------------------------------------------------------------------------------------------------
        private void Betoltes()
        {
            try
            {
                versenyzok = new List<Versenyzo>();
                connection = new MySqlConnection(connectionString);
                connection.Open();                                                   // megnyitjuk a kapcsolatot


                //.................

                // 5. Lekérdezés szövegének megírása.
                // Csinálunk egy táblákközti összekapcsolást és lekérdezést: 

                string lekerdezesSzoveg = " SELECT nev, szakmaNev, orszagNev, pont" +
                                          " FROM versenyzo " +
                                          " INNER JOIN orszag ON orszag.id = versenyzo.orszagId" +
                                          " INNER JOIN szakma ON szakma.id = versenyzo.szakmaId";

                //.................

                // 6. Lekérdezés utasításának objektuma:
                MySqlCommand lekerdez = new MySqlCommand(lekerdezesSzoveg, connection);

                //.................

                // 7. Olvasó létrehozása az eredményhez:
                MySqlDataReader olvaso = lekerdez.ExecuteReader();                   // lekérdezés. Olyan mint a StringReader,
                                                                                     // végigmegy a sorokon

                //.................

                // 8. OLvasó végigolvasása:
                while (olvaso.Read())
                {

                    string nev = olvaso.GetString(0);
                    string szakma = olvaso.GetString(1);
                    string orszag = olvaso.GetString(2);
                    int pontszam = olvaso.GetInt32(3);                               // arra figyeljünk, hogy a "lekerdezesSzoveg"-ben
                                                                                     // lévő megnevezések ugyanabban
                                                                                     // a sorrendben legyenek itt is. Ez fontos!

                    versenyzok.Add(new Versenyzo(nev, szakma, orszag, pontszam));    // Feltöltés


                }

                //9. OLvasó lezárása: 
                olvaso.Close();                                                     // Kötelező lezárni, különben nem fog működni semmi
                connection.Close();                                                  // lezárjuk a kapcsolatot
                                                                                     //................

                // 10.DataGrid - hez hozzákötés: (a result, a DataGrid neve)
                adatokTablazat.ItemsSource = versenyzok;                                // Megadtuk adatforrásnak
                                                                                        //.................

            }
            catch (Exception ex)                                                     // kivételkezelés
            {
                MessageBox.Show(ex.Message);
            }

        }
        //-----------------------------------------------------------------------------------------------------------------------


        private void Window_Loaded(object sender, RoutedEventArgs e)                 // xaml-ben hoztuk létre Loaded-re dupla katt 
        {
            Betoltes();                                                              // itt hívjuk meg a Betoltest
        }

        //------------------------------------------------------------------------------------------------------------------------

        private void button1_Click(object sender, RoutedEventArgs e)
        {

            // 1. feladat: A gomb megnyomásával írja ki  az összes magyar veresenyzőt.  
            //      - egyik megoldás (LINQ): Ezzel a megoldással jól lehet lekérdezéseket kezelni, vizsgára tökéletes,
            //                               viszont hátránya az, ha valami változás történik az adatbázisba, akkor a LINQ nem
            //                               követi a frissítéseket. Így hibás adatokat fog visszaadni, vagy adhat vissza.
            //                               Ha mindig pontos adatokat szeretnénk kapni, akkor a második megoldásra törekedjünk. 

            // LINQ megoldás: 

            var f1 = versenyzok.Where(x => x.Orszag == "Magyarország");
            adatokTablazat.ItemsSource = f1;

            //...............            

            //  Összes magyar veresenyző - másik megoldás: Ez a megoldás lefrissíti az adatokat az inkonzisztencia (adatösszeférhetetlenség)
            //                                             elkerülése végett.



            /*            List<Versenyzo> f1 = new List<Versenyzo>(); 
                        connection.Open();

                        string lekerdezesSzoveg = "SELECT nev, szakmaNev, orszagNev, pont " +
                                                  "FROM versenyzo " +
                                                  "INNER JOIN orszag ON orszag.id = versenyzo.orszagId " +
                                                  "INNER JOIN szakma ON szakma.id = versenyzo.szakmaId  " +
                                                  "WHERE orszagNev = 'Magyarország'";

                        MySqlCommand lekerdez = new MySqlCommand(lekerdezesSzoveg, connection);
                        MySqlDataReader olvaso = lekerdez.ExecuteReader();
                        while (olvaso.Read())
                        {

                            string nev = olvaso.GetString(0);
                            string szakma = olvaso.GetString(1);
                            string orszag = olvaso.GetString(2);
                            int pontszam = olvaso.GetInt32(3); 
                            f1.Add(new Versenyzo(nev, szakma, orszag, pontszam));
                        }   

                            olvaso.Close();
                        //  connection.Close();

            // A gomb megnyomásával kirjuk a magyar versenyzők számát is - egyik megoldás: 

                        //  adatokTablazat.ItemsSource = f1;
                        //  feladat1Lebel.Content = "Magyarországi versenyzők száma: " + f1.Count;

//.......................

            // Magyar versenyzők száma - másik megoldás:

                        adatokTablazat.ItemsSource = f1;
                        lekerdezesSzoveg = "SELECT COUNT(*) " +
                                           "FROM versenyzo " +
                                           "INNER JOIN orszag ON orszag.id = versenyzo.orszagId " +                                     
                                           "WHERE orszagNev = 'Magyarország'";
                        lekerdez = new MySqlCommand (lekerdezesSzoveg, connection);
                        olvaso = lekerdez.ExecuteReader() ;
                        olvaso.Read();
                        int db = olvaso.GetInt32(0);
                        feladat1Lebel.Content = "Magyarorszagi versenyzők száma: " + db;


                        connection.Close();                                                  

            */

        }
        //-------------------------------------------------------------------------------------------------------------------------

        // 2. feladat: Azokat a versenyzőket szeretnénk látni a listában, akiknek a minimális pontszáma az,
        //             ami a slider alatt látható.
        //             A lista legyen rendezve pontszám szerint csökkenő, azon belül név szerint növekső sorrendbe.

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderlabel != null)  // Enélkül nem fog megjelenni és működni a csúszka, mert hamarabb lefut, mint ahogy beállítjuk!
            {
                sliderlabel.Content = slider.Value.ToString();
            }

        }

        private void MinimalisPontszam_Click(object sender, RoutedEventArgs e)

        {
            int alsoHatar = Convert.ToInt32(slider.Value);
            string lekerdezesSzoveg = $"SELECT nev, szakmaNev, orszagNev, pont " +
                                      $"FROM versenyzo " +
                                      $"INNER JOIN orszag ON orszag.id = versenyzo.orszagId " +
                                      $"INNER JOIN szakma ON szakma.id = versenyzo.szakmaId " +
                                      $"WHERE pont>{alsoHatar} " +
                                      $"ORDER BY pont DESC, nev ";

            connection.Open();

            MySqlCommand lekerdez = new MySqlCommand(lekerdezesSzoveg, connection);
            MySqlDataReader olvaso = lekerdez.ExecuteReader();
            List<Versenyzo> f2 = new List<Versenyzo>();

            while (olvaso.Read())
            {
                string nev = olvaso.GetString(0);
                string szakma = olvaso.GetString(1);
                string orszag = olvaso.GetString(2);
                int pontszam = olvaso.GetInt32(3);
                f2.Add(new Versenyzo(nev, szakma, orszag, pontszam));
            }

            olvaso.Close();
            connection.Close();
            adatokTablazat.ItemsSource = f2;

            // LINQ-val most csak a nevet és pontszámot íratjuk ki az f2 feladat megoldásából. 

            var f3 = f2.Select(x => new
            {
                Név = x.Nev,
                Pontszám = x.Pontszam      // Név, pontszám --- ezek a címkék, amikbe akár ékezeteket is tudunk használni.
            });
            adatokTablazat.ItemsSource = f3;
        }
    }
}
    

