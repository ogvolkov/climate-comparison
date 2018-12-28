const temperatureComfortLevels = [-10, 0, 6, 12, 15, 19, 25, 30, 35];

const precipitationComfortLevels = [10, 30, 43, 60, 77, 87, 110, 135, 184, 220];

function getTemperatureComfortLevel(temperature) {
    return getComfortLevel(temperature, temperatureComfortLevels);
}

function getPrecipitationComfortLevel(precipitation) {
    return getComfortLevel(precipitation, precipitationComfortLevels);
}

function getComfortLevel(value, levels) {
    if (value < levels[0]) {
        return 0;
    }

    if (value >= levels[levels.length - 1]) {
        return levels.length;
    }
    
    for (let i = 0; i < levels.length - 1; i++) {
        if (levels[i] <= value && value < levels[i+1]) {
            return i + 1;
        }
    }
}

export { getTemperatureComfortLevel, getPrecipitationComfortLevel };