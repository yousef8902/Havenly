using AutoMapper;
using Havenly.BLL.ModelVMs;
using Havenly.BLL.Services.Abstractions;
using Havenly.DAL.Entities;
using Havenly.DAL.Repos.Abstractions;

namespace Havenly.BLL.Services.Implementations
{
    public class FavoriteService : IFavoriteService
    {
        private readonly IFavoriteRepository _favoriteRepo;
        private readonly IListingRepository _listingRepo;
        private readonly IPropertyRepository _propertyRepo;
        private readonly IMapper _mapper;

        public FavoriteService(
            IFavoriteRepository favoriteRepo,
            IListingRepository listingRepo,
            IPropertyRepository propertyRepo,
            IMapper mapper)
        {
            _favoriteRepo = favoriteRepo;
            _listingRepo = listingRepo;
            _propertyRepo = propertyRepo;
            _mapper = mapper;
        }

        public async Task<bool> AddFavoriteAsync(string userId, long listingId)
        {
            if (string.IsNullOrEmpty(userId) || listingId <= 0)
                return false;

            var exists = (await _favoriteRepo.Find(f => f.UserID == userId && f.ListingID == listingId)).Any();
            if (exists)
                return true;

            var favorite = new Favorite();
            favorite.Create(userId, listingId);
            await _favoriteRepo.Add(favorite);
            return true;
        }

        public async Task<bool> RemoveFavoriteAsync(string userId, long listingId)
        {
            if (string.IsNullOrEmpty(userId) || listingId <= 0)
                return false;

            var matches = (await _favoriteRepo.Find(f => f.UserID == userId && f.ListingID == listingId)).ToList();
            if (!matches.Any())
                return false;

            foreach (var fav in matches)
            {
                _favoriteRepo.Delete(fav);
            }
            await _favoriteRepo.SaveChanges();

            return true;
        }

        public async Task<bool> ToggleFavoriteAsync(string userId, long listingId)
        {
            if (string.IsNullOrEmpty(userId) || listingId <= 0)
                return false;

            var isFav = await IsFavoriteAsync(userId, listingId);
            if (isFav)
            {
                await RemoveFavoriteAsync(userId, listingId);
                return false; // Now unfavorited
            }
            else
            {
                await AddFavoriteAsync(userId, listingId);
                return true; // Now favorited
            }
        }

        public async Task<bool> IsFavoriteAsync(string userId, long listingId)
        {
            if (string.IsNullOrEmpty(userId) || listingId <= 0)
                return false;

            var matches = await _favoriteRepo.Find(f => f.UserID == userId && f.ListingID == listingId);
            return matches.Any();
        }

        public async Task<FavoritesPageVM> GetUserFavoritesAsync(string userId)
        {
            var vm = new FavoritesPageVM();
            if (string.IsNullOrEmpty(userId))
                return vm;

            var favorites = (await _favoriteRepo.Find(f => f.UserID == userId)).ToList();
            var cards = new List<PropertyCardVM>();

            foreach (var fav in favorites)
            {
                var listing = await _listingRepo.GetById(fav.ListingID);
                if (listing == null) continue;

                var property = await _propertyRepo.GetDetailbyId(listing.PropertyID);
                if (property == null || property.IsDeleted) continue;

                var card = _mapper.Map<PropertyCardVM>(property);
                if (card != null)
                {
                    card.Id = listing.ListingID;
                    card.Price = listing.Price;
                    card.IsFavorite = true;
                    cards.Add(card);
                }
            }

            vm.Saved = cards;
            return vm;
        }
    }
}
