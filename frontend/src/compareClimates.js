
import _ from 'lodash';
import { getTemperatureComfortLevel } from './comfortLevel';

function compareClimates(climate1, climate2) {
    const averageHighs1 = _.orderBy(climate1.averageHighs);
    const averageHighs2 = _.orderBy(climate2.averageHighs);

    const differences = _.zipWith(averageHighs1, averageHighs2,
        (t1, t2) => getTemperatureComfortLevel(t2) - getTemperatureComfortLevel(t1)
    );

    const warmer = _.filter(differences, it => it > 0);
    const colder = _.filter(differences, it => it < 0);

    const warmerTotal = _.sum(warmer);
    const colderTotal = _.sum(colder);
    
    let warmerString;

    switch (true) {
        case warmerTotal > 10:
            warmerString = 'much warmer';
            break;
        case warmerTotal > 3:
            warmerString =  'warmer';
            break;
        case warmerTotal > 1:
            warmerString =  'slightly warmer';
            break;
        default:
            warmerString = null;
    }

    let colderString;
    switch (true){
        case colderTotal < -10:
            colderString = 'much colder';
            break;
        case colderTotal < -3:
            colderString = 'colder';
            break;
        case colderTotal < -1:
            colderString =  'slightly colder';
            break;
        default:
            colderString = null;
    }

    switch (true){
        case warmerString !== null && colderString !== null:
            return 'both ' + warmerString + ' and ' + colderString;
        case warmerString !== null && colderString === null:
            return warmerString;
        case colderString !== null && warmerString == null:
            return colderString;
        default:
            return 'almost the same';
    }
}

export default compareClimates;