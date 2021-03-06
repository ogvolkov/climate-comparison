import ClimateService from './Api/ClimateService';
import compareClimates from './compareClimates';

class Climate {
  constructor(onChange) {
    this.temperature = null;
    this.precipitation = null;
    this.onChange = onChange;
  }

  setPlace(place) {
    this.place = place;
    this.temperature = null;
    this.precipitation = null;

    this.load();
    this.fireChangeEvent();
  }

  load() {
    ClimateService.getTemperature(this.place.id)
      .then(response => {
          if (response.ok) {
              response.json().then(
                temperature => {
                  this.temperature = temperature;
                  this.fireChangeEvent();
                }
              );
          };
      });

    ClimateService.getPrecipitation(this.place.id)
      .then(response => {
          if (response.ok) {
              response.json().then(
                precipitation => {
                  this.precipitation = precipitation;
                  this.fireChangeEvent();
                }
              );
          };
      });
  }

  compareTo(another){
    if (this.temperature === null || another.temperature === null) {
      return null;
    }

    const compareString = compareClimates(this.temperature, another.temperature);
    return `Climate at ${this.place.name} is ${compareString} compared to ${another.place.name}`;
  }

  hasData() {
    return this.temperature !== null || this.precipitation !== null;
  }

  fireChangeEvent() {
    this.onChange();
  }
}

export default Climate;
