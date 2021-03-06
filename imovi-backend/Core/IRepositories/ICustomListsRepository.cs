using imovi_backend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace imovi_backend.Core.IRepositories
{
    public interface ICustomListsRepository:IGenericRepository<CustomList>
    {
        Task<object> ListsWithMovies(Guid userId);
        Task<CustomListMovie> AddToList(CustomListMovie customListMovie);
        Task<CustomListMovie> RemoveFromList(CustomListMovie customListMovie);
    }
}
