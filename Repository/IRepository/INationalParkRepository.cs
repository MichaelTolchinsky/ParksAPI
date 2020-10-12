using ParkyAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Repository.IRepository
{
    public interface INationalParkRepository
    {
        ICollection<NationalPark> GetNationalParks();
        NationalPark GetNationalPark(int NationalParkId);
        bool NationalParkExists(string Name);
        bool NationalParkExists(int Id);
        bool CreateNationalPark(NationalPark NationalPark);
        bool UpdateNationalPark(NationalPark NationalPark);
        bool DeleteNationalPark(NationalPark NationalPark);
        bool Save();
    }
}
