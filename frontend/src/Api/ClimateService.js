class ClimateService {
    static getTemperature(placeId) {
        return fetch(`${process.env.REACT_APP_API_URL}/climate/temperature/${placeId}`, { headers: { accept: 'application/json' } });
    }

    static getPrecipitation(placeId) {
        return fetch(`${process.env.REACT_APP_API_URL}/climate/precipitation/${placeId}`, { headers: { accept: 'application/json' } });
    }
}

export default ClimateService;