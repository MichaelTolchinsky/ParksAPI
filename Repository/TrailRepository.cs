using Microsoft.EntityFrameworkCore;
using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository
{
    public class TrailRepository : ITrailRepository
    {
        private readonly ApplicationDbContext _db;

        public TrailRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateTrail(Trail Trail)
        {
            _db.Trails.Add(Trail);
            return Save();
        }

        public bool DeleteTrail(Trail Trail)
        {
            _db.Trails.Remove(Trail);
            return Save();
        }

        public Trail GetTrail(int TrailkId)
        {
            return _db.Trails.Include(n => n.NationalPark).FirstOrDefault(n => n.Id == TrailkId);
        }

        public ICollection<Trail> GetTrails()
        {
            return _db.Trails.Include(n => n.NationalPark).OrderBy(n => n.Name).ToList();
        }

        public bool TrailExists(string Name)
        {
            return _db.Trails.Any(n => n.Name == Name.ToLower().Trim());
        }

        public bool TrailExists(int Id)
        {
            return _db.Trails.Any(n => n.Id == Id);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateTrail(Trail Trail)
        {
            _db.Trails.Update(Trail);
            return Save();
        }

        public ICollection<Trail> GetTrailsInNationalPark(int NpId)
        {
            return _db.Trails.Include(n => n.NationalPark).Where(n => n.NationalParkId == NpId).ToList();
        }
    }
}
