import React, { Component } from 'react';
import './ClimateRow.css';
import getComfortLevel from './comfortLevel';

const monthNames = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];

class ClimateRow extends Component {
    makeTemperatureCell(temperature, index) {
        const comfortLevel = getComfortLevel(temperature);
        return <td key={index} className={'comfort-' + comfortLevel}>{temperature.toFixed(1)}</td>;
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
                            <td>Average high</td>
                            {this.props.averageHighs.map(this.makeTemperatureCell)}
                        </tr>
                    </tbody>
                </table>
            </div>
        );
    }
}

export default ClimateRow;