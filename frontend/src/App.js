import React, { Component } from 'react';
import PlacePicker from './PlacePicker';
import Climate from './Climate';
import ClimateRow from './ClimateRow';

class App extends Component {
  constructor() {
    super();
    
    const urlParams = new URLSearchParams(window.location.search);
    const p1 = urlParams.get('p1');
    const p2 = urlParams.get('p2');

    this.state = { climates: [this.newClimate(p1), this.newClimate(p2)] };
  }

  newClimate(placeId) {
    return new Climate(placeId, () => this.onClimateChanged());
  }

  onClimateChanged(paramId) {
    this.setState(this.state);
  }

  onPlaceSelected(atIndex, place) {
    this.state.climates[atIndex].setPlace(place);
    const urlParams = new URLSearchParams(window.location.search);
    const paramName = atIndex === 0 ? 'p1': 'p2';
    urlParams.set(paramName, place.id);
    const newUrl = window.location.origin + window.location.pathname + '?' + urlParams.toString();
    // TODO push/pop
    window.history.replaceState({path:newUrl},'',newUrl);
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

        <div className='attribution'>
          <p>Temperature data by <a href='http://berkeleyearth.org/'>Berkeley Earth</a>.</p>
          <p>Precipitation data by <a href='http://www.cru.uea.ac.uk/'>Climatic Research Unit, University of East Anglia</a>.</p>
          <p>Cities data by <a href='https://www.geonames.org/'>GeoNames</a>.</p>
          <p>Source code by <a href='https://github.com/ogvolkov/climate-comparison'>Oleg Volkov</a></p>
        </div>
      </div>
    );
  }
}

export default App;
