import React, { Component } from 'react';
import PlacePicker from './PlacePicker';
import Climate from './Climate';
import ClimateRow from './ClimateRow';

class App extends Component {
  constructor() {
    super();

    this.state = { climates: [this.newClimate(), this.newClimate()] };
  }

  newClimate() {
    return new Climate(() => this.onClimateChanged());
  }

  onClimateChanged() {
    this.setState(this.state);
  }

  onPlaceSelected(atIndex, place) {
    this.state.climates[atIndex].setPlace(place);
  };

  render() {
    return (
      <div>
        <p>Compare climates of two places on Earth easily.</p>
        <p>Just type in their names in any language you like and see suggestions.</p>
        {
          this.state.climates.map((data, index) =>
            <div key = {index} className='climate-container'>
              <PlacePicker onPlaceSelected = { place => this.onPlaceSelected(index, place) } caption = {`Place ${index+1}`}/>
              { data.hasData() && <ClimateRow climate={data}/> }
            </div>
          )
        }
        <span>{this.state.climates[1].compareTo(this.state.climates[0])}</span>
      </div>
    );
  }
}

export default App;
