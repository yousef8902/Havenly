using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Repos.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Havenly.BLL.Services.Implementations
{
    public class FilteringSearchServices:IFilteringSearchServices
    {
        IPropertyRepository _RepoProperty;
        public FilteringSearchServices(IPropertyRepository Repo)
        {
            _RepoProperty = Repo;
        }
        public async Task<IQueryable<Property>> SearchByCity(string city)
        {
            try
            {
               var query = await _RepoProperty.Find(x => x.Address.City == city && x.Listing.IsValid == true);
                while (query == null) { }

                 return query.AsQueryable();
            } 
            catch {

                Console.WriteLine("error while filtering by city");
                return Enumerable.Empty<Property>().AsQueryable();

            }

        }
        public async Task<IQueryable<Property>> FilterByPriceRange(decimal low, decimal high)
        {
            try
            {
                Expression<Func<Property, bool>> condition = (x => x.Listing.Price > low &&
                                                 x.Listing.Price < high && x.Listing.IsValid == true);
                var query = await _RepoProperty.Find(condition);


                return query.AsQueryable();
            }
            catch
            {

                Console.WriteLine("error while filtering by price");
                return Enumerable.Empty<Property>().AsQueryable();

            }

        }
        public async Task<IQueryable<Property>> FilterByNumberOfQuests(int low, int high)
        {
            try
            {
                Expression<Func<Property, bool>> condition = (x => x.NumberOfGuests >= low &&
                                                 x.NumberOfGuests <= high &&x.Listing.IsValid==true);
                var query = await _RepoProperty.Find(condition);


                return query.AsQueryable();
            }
            catch
            {

                Console.WriteLine("error while filtering by guests");
                return Enumerable.Empty<Property>().AsQueryable();

            }


        }
        public async Task<IQueryable<Property>> FilterByRatings(double low, double high)
        {
            try
            {
                Expression<Func<Property, bool>> condition = (x => x.Rating >= low &&
                                                 x.Rating <= high && x.Listing.IsValid == true);
                var query = await _RepoProperty.Find(condition);


                return query.AsQueryable();
            }
            catch
            {

                Console.WriteLine("error while filtering by Ratings");
                return Enumerable.Empty<Property>().AsQueryable();

            }
            

        }
    }
}
