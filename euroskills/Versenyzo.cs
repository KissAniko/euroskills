using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace euroskills
{
    internal class Versenyzo
    {
        // Itt kötelező a get, set feltöltés... mindkettő, különben nem fogja hozni az adatokat!

        string nev;
        string szakma;
        string orszag;
        int pontszam;

        public Versenyzo(string nev, string szakma, string orszag, int pontszam)
        {
            this.Nev = nev;
            this.Szakma = szakma;
            this.Orszag = orszag;
            this.Pontszam = pontszam;
        }

        public string Nev { get => nev; set => nev = value; }
        public string Szakma { get => szakma; set => szakma = value; }
        public string Orszag { get => orszag; set => orszag = value; }
        public int Pontszam { get => pontszam; set => pontszam = value; }
    }
}