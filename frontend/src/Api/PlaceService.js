class PlaceService {
    static findPlaces(searchText) {
        return fetch(`${process.env.REACT_APP_API_URL}/places?search=${searchText}`, { headers: { accept: 'application/json' } });
    }

    static get(placeId) {
        return fetch(`${process.env.REACT_APP_API_URL}/places/${placeId}`, { headers: { accept: 'application/json' } });
    }
}

export default PlaceService;