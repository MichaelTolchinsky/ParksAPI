using ParkyAPI.Data;
using ParkyAPI.Models;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ParkyAPI.Repository
{
    public class NationalParkRepository : INationalParkRepository
    {
        private readonly ApplicationDbContext _db;

        public NationalParkRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public bool CreateNationalPark(NationalPark NationalPark)
        {
            _db.NationalParks.Add(NationalPark);
            return Save();
        }

        public bool DeleteNationalPark(NationalPark NationalPark)
        {
            _db.NationalParks.Remove(NationalPark);
            return Save();
        }

        public NationalPark GetNationalPark(int NationalParkId)
        {
            return _db.NationalParks.FirstOrDefault(n => n.Id == NationalParkId);
        }

        public ICollection<NationalPark> GetNationalParks()
        {
            return _db.NationalParks.OrderBy(n => n.Name).ToList();
        }

        public bool NationalParkExists(string Name)
        {
            return _db.NationalParks.Any(n => n.Name == Name.ToLower().Trim());
        }

        public bool NationalParkExists(int Id)
        {
            return _db.NationalParks.Any(n => n.Id == Id);
        }

        public bool Save()
        {
            return _db.SaveChanges() >= 0 ? true : false;
        }

        public bool UpdateNationalPark(NationalPark NationalPark)
        {
            _db.NationalParks.Update(NationalPark);
            return Save();
        }
    }
}
