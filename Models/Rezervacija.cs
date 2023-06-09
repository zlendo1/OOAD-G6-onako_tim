﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OOAD_G6_najjaci_tim.Models
{
    public class Rezervacija
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("KorisnikSaNalogom")]
        public int IdKorisnikSaNalogom { get; set; }
        public KorisnikSaNalogom KorisnikSaNalogom { get; set; }

        [ForeignKey("Film")]
        public int IdFilm { get; set; }
        public Film Film { get; set; }

        public Rezervacija() { }
    }
}
