﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using moviesite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Xamarin.Forms;

namespace moviesite.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext c;
        private readonly UserManager<IdentityUser> _userManager;

        public ProfileController(AppDbContext c, UserManager<IdentityUser> userManager)
        {
            this.c = c;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Watchlist(string sortOrder, string currentFilter, string searchString, int searchYear, int? pageNumber, int pageSize = 25)
        {

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["YearSortParam"] = sortOrder == "year" ? "year_asc" : "year";
            ViewBag.LengthSortParam = sortOrder == "length" ? "length_asc" : "length";
            ViewBag.ScoreSortParam = sortOrder == "score" ? "score_asc" : "score";
            ViewBag.Score1SortParam = sortOrder == "score1" ? "score1_asc" : "score1";


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            var user = await _userManager.GetUserAsync(HttpContext.User);
            var table = c.WantToWatch.Where(x => x.UserId == user.Id);
            var query = c.TBLMOVIES.Join(c.WantToWatch, x => x.Id, y => y.MovieId, (x, y) => x);


            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.FilmName.Contains(searchString));
            }
            if (searchYear != 0)
            {
                query = query.Where(x => x.FilmYear == searchYear);
            }

            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(x => x.FilmName),
                "year_asc" => query.OrderBy(x => x.FilmYear),
                "year" => query.OrderByDescending(x => x.FilmYear),
                "length_asc" => query.OrderBy(x => x.FilmLength),
                "length" => query.OrderByDescending(x => x.FilmLength),
                "score_asc" => query.OrderBy(x => x.FilmScore),
                "score" => query.OrderByDescending(x => x.FilmScore),
                "score1_asc" => query.OrderBy(x => x.FilmScoreTwo),
                "score1" => query.OrderByDescending(x => x.FilmScoreTwo),
                _ => query.OrderBy(x => x.FilmName),
            };

            return View(await PaginatedList<Film>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Watched(string sortOrder, string currentFilter, string searchString, int searchYear, int? pageNumber, int pageSize = 25)
        {

            ViewData["CurrentFilter"] = searchString;
            ViewData["CurrentSort"] = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["YearSortParam"] = sortOrder == "year" ? "year_asc" : "year";
            ViewBag.LengthSortParam = sortOrder == "length" ? "length_asc" : "length";
            ViewBag.ScoreSortParam = sortOrder == "score" ? "score_asc" : "score";
            ViewBag.Score1SortParam = sortOrder == "score1" ? "score1_asc" : "score1";


            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }


            var user = await _userManager.GetUserAsync(HttpContext.User);
            var table = c.Watched.Where(x => x.UserId == user.Id);
            var query = c.TBLMOVIES.Join(c.Watched, x => x.Id, y => y.MovieId, (x, y) => x);


            if (!String.IsNullOrEmpty(searchString))
            {
                query = query.Where(b => b.FilmName.Contains(searchString));
            }
            if (searchYear != 0)
            {
                query = query.Where(x => x.FilmYear == searchYear);
            }

            query = sortOrder switch
            {
                "name_desc" => query.OrderByDescending(x => x.FilmName),
                "year_asc" => query.OrderBy(x => x.FilmYear),
                "year" => query.OrderByDescending(x => x.FilmYear),
                "length_asc" => query.OrderBy(x => x.FilmLength),
                "length" => query.OrderByDescending(x => x.FilmLength),
                "score_asc" => query.OrderBy(x => x.FilmScore),
                "score" => query.OrderByDescending(x => x.FilmScore),
                "score1_asc" => query.OrderBy(x => x.FilmScoreTwo),
                "score1" => query.OrderByDescending(x => x.FilmScoreTwo),
                _ => query.OrderBy(x => x.FilmName),
            };

            return View(await PaginatedList<Film>.CreateAsync(query.AsNoTracking(), pageNumber ?? 1, pageSize));

        }

        public async Task<IActionResult> AddToWatchlist(string id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            WantToWatch film = new()
            {
                UserId = user.Id,
                MovieId = id
            };

            var item = (from z in c.WantToWatch
                        where z.UserId == film.UserId && z.MovieId == film.MovieId
                        select z).ToList();

            foreach (var z in item)
            {
                return RedirectToAction("Watchlist");
            }

            c.WantToWatch.Add(film);
            c.SaveChanges();
            return RedirectToAction("Watchlist");

        }

        public async Task<IActionResult> AddToWatched(string id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            Watched film = new()
            {
                UserId = user.Id,
                MovieId = id
            };

            var item = (from z in c.Watched
                       where z.UserId == film.UserId && z.MovieId == film.MovieId
                       select z).ToList();

            foreach(var z in item)
            {
                return RedirectToAction("Watched");               
            }

            c.Watched.Add(film);
            c.SaveChanges();
            return RedirectToAction("Watched");
        }

        public async Task<IActionResult> RemoveFromWatchlist(string id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            WantToWatch film = new()
            {
                UserId = user.Id,
                MovieId = id
            };

            var item = (from z in c.WantToWatch
                        where z.UserId == film.UserId && z.MovieId == film.MovieId
                        select z).ToList();

            foreach (var z in item)
            {
                c.WantToWatch.Remove(z);
            }

            c.SaveChanges();
            return RedirectToAction("Watchlist");
        }

        public async Task<IActionResult> RemoveFromWatched(string id)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            Watched film = new()
            {
                UserId = user.Id,
                MovieId = id
            };

            var item = (from z in c.Watched
                        where z.UserId == film.UserId && z.MovieId == film.MovieId
                        select z).ToList();

            foreach(var  z in item)
            {
                c.Watched.Remove(z);
            }
                
            c.SaveChanges();
            return RedirectToAction("Watched");
        }

    }
}
