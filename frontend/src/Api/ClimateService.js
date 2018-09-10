class ClimateService {
    static getClimate(placeId) {
        return fetch(`${process.env.REACT_APP_API_URL}/climate/${placeId}`, { headers: { accept: 'application/json' } });
    }
}

export default ClimateService;