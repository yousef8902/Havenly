using Havenly.BLL.ModelVMs;

namespace Havenly.BLL.Services.Abstractions
{
    public interface IFavoriteService
    {
        Task<bool> AddFavoriteAsync(string userId, long listingId);
        Task<bool> RemoveFavoriteAsync(string userId, long listingId);
        Task<bool> ToggleFavoriteAsync(string userId, long listingId);
        Task<bool> IsFavoriteAsync(string userId, long listingId);
        Task<FavoritesPageVM> GetUserFavoritesAsync(string userId);
    }
}
