﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using OOAD_G6_najjaci_tim.Data;
using OOAD_G6_najjaci_tim.Models;

namespace OOAD_G6_najjaci_tim.Controllers
{
    public class RezervacijaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

      
        public RezervacijaController(ApplicationDbContext context, IMemoryCache memoryCache)
        {
            _context = context; _cache = memoryCache;
        }

        // GET: Rezervacija
        public async Task<IActionResult> Index()
        {
            if (_cache.TryGetValue("KorisnikId", out int korisnikId))
                ViewData["IdKorisnikSaNalogom"] = korisnikId;
           var r= await _context.Rezervacija
         .Include(r => r.Film)
         .Include(r => r.KorisnikSaNalogom)
         .Where(r => r.IdKorisnikSaNalogom == korisnikId)
         .ToListAsync();
            return View(r);
        }
        public async Task<IActionResult> ForAdmin()
        {
            var applicationDbContext = _context.Rezervacija.Include(r => r.Film).Include(r => r.KorisnikSaNalogom);
            return View("ForAdmin",await applicationDbContext.ToListAsync());
        }

        // GET: Rezervacija/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rezervacija = await _context.Rezervacija
                .Include(r => r.Film)
                .Include(r => r.KorisnikSaNalogom)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rezervacija == null)
            {
                return NotFound();
            }

            return View(rezervacija);
        }
    

        // GET: Rezervacija/Create
        public IActionResult Create(string ime,int id)
        { 
            ViewData["Film"] = ime;
            ViewData["IdFilm"] = id;
            if (_cache.TryGetValue("KorisnikId", out int korisnikId))
                ViewData["IdKorisnikSaNalogom"] = korisnikId;
            
            return View();
        }

        // POST: Rezervacija/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]

        
        public async Task<IActionResult> Create(int idKorisnikSaNalogom, int idFilm)
        {
            if (ModelState.IsValid)
            {
                Rezervacija rezervacija = new Rezervacija
                {
                    IdKorisnikSaNalogom = idKorisnikSaNalogom,
                    IdFilm = idFilm
                };

              
                _context.Add(rezervacija);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Prikaz forme za kreiranje ako ModelState nije ispravan
            ViewData["Film"] = new SelectList(_context.Film, "Ime", "Ime");
            ViewData["IdFilm"] = new SelectList(_context.Film, "Id", "Id", idFilm);
            ViewData["IdKorisnikSaNalogom"] = new SelectList(_context.KorisnikSaNalogom, "Id", "Id", idKorisnikSaNalogom);
            return View();
        }









        // GET: Rezervacija/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rezervacija = await _context.Rezervacija.FindAsync(id);
            if (rezervacija == null)
            {
                return NotFound();
            }
            ViewData["IdFilm"] = new SelectList(_context.Film, "Id", "Id", rezervacija.IdFilm);
            ViewData["IdKorisnikSaNalogom"] = new SelectList(_context.KorisnikSaNalogom, "Id", "Id", rezervacija.IdKorisnikSaNalogom);
            return View(rezervacija);
        }

        // POST: Rezervacija/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,IdKorisnikSaNalogom,IdFilm")] Rezervacija rezervacija)
        {
            if (id != rezervacija.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rezervacija);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RezervacijaExists(rezervacija.Id))
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
            ViewData["IdFilm"] = new SelectList(_context.Film, "Id", "Id", rezervacija.IdFilm);
            ViewData["IdKorisnikSaNalogom"] = new SelectList(_context.KorisnikSaNalogom, "Id", "Id", rezervacija.IdKorisnikSaNalogom);
            return View(rezervacija);
        }

        // GET: Rezervacija/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rezervacija = await _context.Rezervacija
                .Include(r => r.Film)
                .Include(r => r.KorisnikSaNalogom)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rezervacija == null)
            {
                return NotFound();
            }

            return View(rezervacija);
        }

        // POST: Rezervacija/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rezervacija = await _context.Rezervacija.FindAsync(id);
            _context.Rezervacija.Remove(rezervacija);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ForAdmin));
        }

        private bool RezervacijaExists(int id)
        {
            return _context.Rezervacija.Any(e => e.Id == id);
        }
    }
}