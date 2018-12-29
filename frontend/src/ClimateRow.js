import React, { Component } from 'react';
import './ClimateRow.css';

import { getTemperatureComfortLevel, getPrecipitationComfortLevel } from './comfortLevel';

const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

class ClimateRow extends Component {
    makeTemperatureCell(temperature, index) {
        const comfortLevel = getTemperatureComfortLevel(temperature);
        return <td key={index} className={'comfort-' + comfortLevel}>{temperature.toFixed(1)}</td>;
    }

    makePrecipitationCell(precipitation, index) {
        const comfortLevel = getPrecipitationComfortLevel(precipitation);
        return <td key={index} className={'precipitation-' + comfortLevel}>{precipitation.toFixed(1)}</td>;
    }
    
    render() {
        return (
            <div>
                <table className='climate-table'>
                    <thead>
                        <tr>
                            <th></th>
                            {monthNames.map((name, index) => <th key={index}>{name}</th>)}
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td className="row-header">Average high temperature, °C</td>
                            {this.props.averageHighs.map(this.makeTemperatureCell)}
                        </tr>
                        <tr>
                            <td className="row-header">Precipitation, mm</td>
                            {this.props.precipitation.map(this.makePrecipitationCell)}
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}

export default ClimateRow;