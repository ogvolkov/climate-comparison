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

        this.lastRequestId = 0;
    }

    onChange = (event, { newValue }) => this.setState({ value: newValue });

    fetchSuggestions = (value) => {
        if (value.length < 3) return;

        const requestId = ++this.lastRequestId;

        PlaceService.findPlaces(value)
            .then(response => {
                if (response.ok && this.lastRequestId === requestId) {
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

    getSuggestionValue = suggestion => `${suggestion.name}, ${suggestion.country}`;
    
    renderSuggestion = suggestion => (
        <div>
            <span className='place-name'>{suggestion.name}</span>
            <span className='country-name'>{suggestion.country}</span>
        </div>
    );    

    componentDidMount() {
        if (this.props.initialPlaceId) {
            PlaceService.get(this.props.initialPlaceId).then(response => {
                if (response.ok) {                    
                    response.json().then(place => {
                        const placeString = this.getSuggestionValue(place);
                        this.setState({value: placeString});
                    });
                }
            });
        }
    }

    render() {
        const { value, suggestions } = this.state;

        const inputProps = {
            placeholder: this.props.caption,
            value,
            onChange: this.onChange
        };

        return (
            <div className='place-picker-container'>
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