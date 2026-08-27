using Havenly.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IFilteringSearchServices
    {
        Task<IQueryable<Property>> SearchByCity(string city);
        Task<IQueryable<Property>> FilterByPriceRange(decimal low, decimal high);
        Task<IQueryable<Property>> FilterByNumberOfQuests(int low, int high);
        Task<IQueryable<Property>> FilterByRatings(double low, double high);

    }
}
