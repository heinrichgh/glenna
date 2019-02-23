using System;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IProfessionRepository : IRepository<Profession>
    {
        Profession Save(Profession member);
    }
}