class ClimateService {
    static getClimate(placeId) {
        return fetch('/api/climate/' + placeId, { headers: { accept: 'application/json' } });
    }
}

export default ClimateService;