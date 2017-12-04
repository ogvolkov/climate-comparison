import _ from 'lodash';
import React, { Component } from 'react';

import './PlacePicker.css';
import Autosuggest from 'react-autosuggest';
import PlaceService from './Api/PlaceService';

class PlacePicker extends Component {
    constructor() {
        super();

        this.state = {
            value: '',
            suggestions: []
        };
    }

    onChange = (event, { newValue }) => this.setState({ value: newValue });

    fetchSuggestions = (value) => {
        PlaceService.findPlaces(value)
            .then(response => {
                if (response.ok) {
                    response.json().then(
                        data => this.setState({ suggestions: data })
                    );
                }
            });
    };

    debouncedLoadSuggestions = _.debounce(this.fetchSuggestions, 200);

    onSuggestionsFetchRequested = ({ value }) => this.debouncedLoadSuggestions(value);

    onSuggestionsClearRequested = () => this.setState({ suggestions: [] });

    onSuggestionSelected = (event, data) => this.props.onPlaceSelected(data.suggestion);

    getSuggestionValue = suggestion => '';
    
    renderSuggestion = suggestion => (
        <div>
            <span className='place-name'>{suggestion.name}</span>
            <span className='country-name'>{suggestion.country}</span>
        </div>
    );    

    render() {
        const { value, suggestions } = this.state;

        const inputProps = {
            placeholder: 'place',
            value,
            onChange: this.onChange
        };

        return (
            <div className='place-picker-container'>
                <label>Select a place</label>
                <Autosuggest
                    suggestions = {suggestions}
                    onSuggestionsFetchRequested = {this.onSuggestionsFetchRequested}
                    onSuggestionsClearRequested = {this.onSuggestionsClearRequested}
                    onSuggestionSelected = {this.onSuggestionSelected}
                    getSuggestionValue = {this.getSuggestionValue}
                    renderSuggestion = {this.renderSuggestion}
                    inputProps = {inputProps}
                />
            </div>
        );
    }
}

export default PlacePicker;