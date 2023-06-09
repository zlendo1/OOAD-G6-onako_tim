﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OOAD_G6_najjaci_tim.Data;
using OOAD_G6_najjaci_tim.Models;

namespace OOAD_G6_najjaci_tim.Controllers
{
    public class KartasController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;


        public KartasController(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context; _cache = memoryCache;
        }
        public IActionResult Create(int idr, int id)
        {
            ViewData["IdRezervacija"] = idr;
            ViewData["IdFilm"] = id;
            ViewData["IdSjedisteUTerminu"] = new SelectList(_context.SjedisteUTerminu, "Id", "Id");
            ViewData["IdTermin"] = new SelectList(_context.Termin, "Id", "Vrijeme");
            if (_cache.TryGetValue("KorisnikId", out int korisnikId))
                ViewData["IdKorisnikSaNalogom"] = korisnikId;

            return View();
        }
        public async Task<IActionResult> Filter(int? genre)
        {
            var query = _context.Karta.Include(k => k.KorisnikSaNalogom).Include(f=>f.Film).Include(l=>l.SjedisteUTerminu).Include(l=>l.Rezervacija).Include(k=>k.Termin);
            var karte = await query.ToListAsync();

            if (genre == null)
            {
                return View("ForAdmin",karte);
            }
            else
            {
                karte.RemoveAll(k => k.KorisnikSaNalogom.Id != genre);
                return View("ForAdmin",karte);
            }
        }


        // GET: Kartas
        public async Task<IActionResult> Index()
        {
            if (_cache.TryGetValue("KorisnikId", out int korisnikId))
            {
                var applicationDbContext = _context.Karta
                    .Include(k => k.Film)
                    .Include(k => k.KorisnikSaNalogom)
                    .Include(k => k.Rezervacija)
                    .Include(k => k.SjedisteUTerminu)
                    .Include(k => k.Termin)
                    .Where(k => k.IdKorisnikSaNalogom == korisnikId);

                return View(await applicationDbContext.ToListAsync());
            }

            return View();
        }

        public async Task<IActionResult> ForAdmin()
        {
            var applicationDbContext = _context.Karta.Include(k => k.Film).Include(k => k.KorisnikSaNalogom).Include(k => k.Rezervacija).Include(k => k.SjedisteUTerminu).Include(k => k.Termin);
            return View("ForAdmin",await applicationDbContext.ToListAsync());
        }

        // GET: Kartas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var karta = await _context.Karta
                .Include(k => k.Film)
                .Include(k => k.KorisnikSaNalogom)
                .Include(k => k.Rezervacija)
                .Include(k => k.SjedisteUTerminu)
                .Include(k => k.Termin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (karta == null)
            {
                return NotFound();
            }

            return View(karta);
        }

        // GET: Kartas/Create
        

        // POST: Kartas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Create(int idKorisnikSaNalogom, int idRezervacija, int idFilm, [Bind("IdSjedisteUTerminu,IdTermin")] Karta karta)
        {
            if (ModelState.IsValid)
            {
                if (_cache.TryGetValue("KorisnikId", out int genre)) ;
                    var racun = await _context.Racun.FirstOrDefaultAsync(r => r.IdKorisnikSaNalogom == idKorisnikSaNalogom);
                karta.IdKorisnikSaNalogom = idKorisnikSaNalogom;
                karta.IdFilm = idFilm;
                karta.IdRezervacija = idRezervacija;
                if (racun.StanjeRacuna > 5)
                {
                    _context.Add(karta);
                    await _context.SaveChangesAsync();

                    //  await _context.SaveChangesAsync();
                    return RedirectToAction("Index", new {genre=genre});
                }
                else
                {
                    TempData["ErrorMessage"] = "Nemate dovoljno novca na računu."; return View();
                }
             
            }

            ViewData["IdFilm"] = new SelectList(_context.Film, "Id", "Id", karta.IdFilm);
            ViewData["IdKorisnikSaNalogom"] = new SelectList(_context.KorisnikSaNalogom, "Id", "Id", karta.IdKorisnikSaNalogom);
            ViewData["IdRezervacija"] = new SelectList(_context.Rezervacija, "Id", "Id", karta.IdRezervacija);
            ViewData["IdSjedisteUTerminu"] = new SelectList(_context.SjedisteUTerminu, "Id", "Id", karta.IdSjedisteUTerminu);
            ViewData["IdTermin"] = new SelectList(_context.Termin, "Id", "Vrijeme", karta.IdTermin);
            ViewData["IdKorisnikSaNalogom"] = idKorisnikSaNalogom;
            ViewData["IdFilm"] = idFilm;
            ViewData["IdRezervacija"] = idRezervacija;

            return View(karta);
        }


        // GET: Kartas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var karta = await _context.Karta.FindAsync(id);
            if (karta == null)
            {
                return NotFound();
            }
            ViewData["IdFilm"] = new SelectList(_context.Film, "Id", "Id", karta.IdFilm);
            ViewData["IdKorisnikSaNalogom"] = new SelectList(_context.KorisnikSaNalogom, "Id", "Id", karta.IdKorisnikSaNalogom);
            ViewData["IdRezervacija"] = new SelectList(_context.Rezervacija, "Id", "Id", karta.IdRezervacija);
            ViewData["IdSjedisteUTerminu"] = new SelectList(_context.SjedisteUTerminu, "Id", "Id", karta.IdSjedisteUTerminu);
            ViewData["IdTermin"] = new SelectList(_context.Termin, "Id", "Vrijeme", karta.IdTermin);
            return View(karta);
        }

        // POST: Kartas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdSjedisteUTerminu,IdKorisnikSaNalogom,IdFilm,IdRezervacija,IdTermin")] Karta karta)
        {
            if (id != karta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(karta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!KartaExists(karta.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ForAdmin));
            }
            ViewData["IdFilm"] = new SelectList(_context.Film, "Id", "Id", karta.IdFilm);
            ViewData["IdKorisnikSaNalogom"] = new SelectList(_context.KorisnikSaNalogom, "Id", "Id", karta.IdKorisnikSaNalogom);
            ViewData["IdRezervacija"] = new SelectList(_context.Rezervacija, "Id", "Id", karta.IdRezervacija);
            ViewData["IdSjedisteUTerminu"] = new SelectList(_context.SjedisteUTerminu, "Id", "Id", karta.IdSjedisteUTerminu);
            ViewData["IdTermin"] = new SelectList(_context.Termin, "Id", "Id", karta.IdTermin);
            return View(karta);
        }

        // GET: Kartas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var karta = await _context.Karta
                .Include(k => k.Film)
                .Include(k => k.KorisnikSaNalogom)
                .Include(k => k.Rezervacija)
                .Include(k => k.SjedisteUTerminu)
                .Include(k => k.Termin)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (karta == null)
            {
                return NotFound();
            }

            return View(karta);
        }

        // POST: Kartas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var karta = await _context.Karta.FindAsync(id);
            _context.Karta.Remove(karta);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ForAdmin));
        }

        private bool KartaExists(int id)
        {
            return _context.Karta.Any(e => e.Id == id);
        }
    }
}
