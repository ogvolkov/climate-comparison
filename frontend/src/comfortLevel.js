function getComfortLevel(temperature) {
    const comfortLevels = [-10, 0, 6, 12, 15, 19, 25, 30, 35];
    
    if (temperature < comfortLevels[0]) {
        return 0;
    }

    if (temperature >= comfortLevels[comfortLevels.length - 1]) {
        return comfortLevels.length;
    }
    
    for (let i = 0; i < comfortLevels.length - 1; i++) {
        if (comfortLevels[i] <= temperature && temperature < comfortLevels[i+1]) {
            return i + 1;
        }
    }
}

export default getComfortLevel;