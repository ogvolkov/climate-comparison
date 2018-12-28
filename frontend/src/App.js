import React, { Component } from 'react';
import PlacePicker from './PlacePicker';
import ClimateRow from './ClimateRow';
import ClimateService from './Api/ClimateService';
import compareClimates from './compareClimates';

class App extends Component {
  constructor() {
    super();

    const noPlaceSelected = { place: null, climate: null};
    this.state = { climates: [noPlaceSelected, noPlaceSelected] };
  }

  replaceClimate(atIndex, climate) {
    let newClimates = this.state.climates.map((item, index) =>
      (index === atIndex) ? {place: item.place, climate: climate }: item
    );
    this.setState({ climates: newClimates });
  }

  loadClimate(atIndex, place) {
    ClimateService.getClimate(place.id)
      .then(response => {
          if (response.ok) {
              response.json().then(
                climate => this.replaceClimate(atIndex, climate)
              );
          };
      });
  }

  onPlaceSelected = (atIndex, place) => {
    let newClimates = this.state.climates.map((item, index) =>
      (index === atIndex) ? { place: place, climate: null }: item
    );
    this.setState({ climates: newClimates });

    this.loadClimate(atIndex, place);
  };

  render() {
    const climate1 = this.state.climates[0];
    const climate2 = this.state.climates[1];

    return (
      <div>
        {
          this.state.climates.map((data, index) =>
            <div key = {index} className='climate-container'>
              <PlacePicker onPlaceSelected = { place => this.onPlaceSelected(index, place) }/>

              {
                data.place &&
                <h2>{data.place.name + ', ' + data.place.country}</h2>
              }

              {
                data.climate &&
                <ClimateRow averageHighs={data.climate.averageHighs} precipitation={data.climate.precipitation}/>
              }
            </div>
          )
        }
        {
          climate1.place && climate1.climate && climate2.place && climate2.climate &&
          <span>Climate at {climate2.place.name} is {compareClimates(climate1.climate, climate2.climate)} compared to {climate1.place.name}</span>
        }
      </div>
    );
  }
}

export default App;
