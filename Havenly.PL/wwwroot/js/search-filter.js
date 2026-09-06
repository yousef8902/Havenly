// AJAX Search and Filter Functionality
(function () {
    'use strict';

    // Get all filter form elements
    const filterForm = document.getElementById('filterForm');
    const searchForm = document.querySelector('form[asp-controller="Property"][asp-action="Index"]');
    const resultsContainer = document.getElementById('propertiesGrid');

    if (!filterForm && !searchForm) {
        console.warn('Filter or search form not found');
        return;
    }

    // Collect all filter inputs
    const getFilterInputs = () => {
        const inputs = new FormData(filterForm || searchForm);
        return Object.fromEntries(inputs);
    };

    // Build query string from filter values
    const buildQueryString = (filters) => {
        const params = new URLSearchParams();

        Object.entries(filters).forEach(([key, value]) => {
            if (value && value !== '') {
                // Handle array values (like amenities)
                if (Array.isArray(value)) {
                    value.forEach(v => params.append(key, v));
                } else {
                    params.set(key, value);
                }
            }
        });

        return params.toString();
    };

    // Fetch filtered results via AJAX
    const fetchFilteredResults = async () => {
        try {
            const filters = getFilterInputs();
            const queryString = buildQueryString(filters);
            const url = `/Property/Index?${queryString}`;

            const response = await fetch(url, {
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                console.error('Filter request failed:', response.status);
                return;
            }

            const html = await response.text();

            // Parse the response and extract just the results grid
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, 'text/html');
            const newResults = doc.querySelector('[id="propertiesGrid"]');

            if (newResults && resultsContainer) {
                resultsContainer.innerHTML = newResults.innerHTML;

                // Re-attach event listeners to new favorite buttons
                attachFavoriteListeners();

                // Scroll to results
                resultsContainer.scrollIntoView({ behavior: 'smooth', block: 'start' });
            }
        } catch (error) {
            console.error('Error fetching filtered results:', error);
            window.havenlyToast?.('Failed to update results', true);
        }
    };

    // Handle search form submission (top search bar)
    if (searchForm) {
        searchForm.addEventListener('submit', (e) => {
            e.preventDefault();
            fetchFilteredResults();
        });
    }

    // Handle filter form changes (sidebar)
    if (filterForm) {
        // Category dropdown
        const categorySelect = document.getElementById('f-category');
        if (categorySelect) {
            categorySelect.addEventListener('change', fetchFilteredResults);
        }

        // Guests dropdown
        const guestsSelect = document.getElementById('f-guests');
        if (guestsSelect) {
            guestsSelect.addEventListener('change', fetchFilteredResults);
        }

        // Bedrooms dropdown
        const roomsSelect = document.getElementById('f-rooms');
        if (roomsSelect) {
            roomsSelect.addEventListener('change', fetchFilteredResults);
        }

        // Price range slider
        const priceSlider = document.getElementById('f-price');
        if (priceSlider) {
            priceSlider.addEventListener('change', fetchFilteredResults);
        }

        // Rating slider
        const ratingSlider = document.getElementById('f-rating');
        if (ratingSlider) {
            ratingSlider.addEventListener('change', fetchFilteredResults);
        }

        // Amenities checkboxes
        const amenityCheckboxes = filterForm.querySelectorAll('input[name="amenities"]');
        if (amenityCheckboxes.length > 0) {
            amenityCheckboxes.forEach(checkbox => {
                checkbox.addEventListener('change', fetchFilteredResults);
            });
        }
    }

    // Attach listeners to favorite buttons
    const attachFavoriteListeners = () => {
        const favoriteForms = document.querySelectorAll('form[asp-controller="Favorites"][asp-action="Toggle"]');
        favoriteForms.forEach(form => {
            form.addEventListener('submit', handleFavoriteToggle);
        });
    };

    // Handle favorite toggle via AJAX
    const handleFavoriteToggle = async (e) => {
        e.preventDefault();

        const form = e.target;
        const button = form.querySelector('button[type="submit"]');
        const svg = button?.querySelector('svg');
        const listingIdInput = form.querySelector('input[name="listingId"]');
        const listingId = listingIdInput?.value;

        if (!listingId) {
            console.error('Listing ID not found');
            return;
        }

        // Optimistic update - toggle the icon immediately
        const isFilled = svg?.classList.contains('fill-red-500');
        if (isFilled) {
            svg?.classList.remove('fill-red-500', 'text-red-500');
            svg?.setAttribute('fill', 'none');
        } else {
            svg?.classList.add('fill-red-500', 'text-red-500');
            svg?.setAttribute('fill', 'currentColor');
        }

        try {
            const formData = new FormData(form);
            const response = await fetch(form.action, {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (!response.ok) {
                console.error('Favorite toggle failed:', response.status);
                // Revert on error
                if (!isFilled) {
                    svg?.classList.remove('fill-red-500', 'text-red-500');
                    svg?.setAttribute('fill', 'none');
                } else {
                    svg?.classList.add('fill-red-500', 'text-red-500');
                    svg?.setAttribute('fill', 'currentColor');
                }
                window.havenlyToast?.('Failed to update favorite', true);
                return;
            }

            const data = await response.json();
            if (data.success) {
                // Favorite state is already toggled optimistically
                window.havenlyToast?.(data.isFavorite ? 'Added to favorites' : 'Removed from favorites', false);
            } else {
                // Revert if server says it failed
                if (!isFilled) {
                    svg?.classList.remove('fill-red-500', 'text-red-500');
                    svg?.setAttribute('fill', 'none');
                } else {
                    svg?.classList.add('fill-red-500', 'text-red-500');
                    svg?.setAttribute('fill', 'currentColor');
                }
                window.havenlyToast?.(data.message || 'Failed to update favorite', true);
            }
        } catch (error) {
            console.error('Error toggling favorite:', error);
            // Revert on error
            if (!isFilled) {
                svg?.classList.remove('fill-red-500', 'text-red-500');
                svg?.setAttribute('fill', 'none');
            } else {
                svg?.classList.add('fill-red-500', 'text-red-500');
                svg?.setAttribute('fill', 'currentColor');
            }
            window.havenlyToast?.('An error occurred', true);
        }
    };

    // Initial attachment of favorite listeners
    attachFavoriteListeners();

    // Prevent form submission and use AJAX instead
    if (filterForm) {
        filterForm.addEventListener('submit', (e) => {
            e.preventDefault();
            fetchFilteredResults();
        });
    }
})();
