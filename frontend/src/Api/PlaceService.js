class PlaceService {
    static findPlaces(searchText) {
        return fetch('/api/places?search=' + searchText, { headers: { accept: 'application/json' } });
    }
}

export default PlaceService;