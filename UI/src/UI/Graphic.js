import React from 'react';
import {Line} from 'react-chartjs-2';


export default class Example extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            historyArr: props.historyArr
        };
    }

    getRandomColor() {
        var letters = '0123456789ABCDEF'.split('');
        var color = '#';
        for (var i = 0; i < 6; i++ ) {
            color += letters[Math.floor(Math.random() * 16)];
        }
        return color;
    }

    getDataset(index) {
        const color = this.getRandomColor();
        return {
            label: this.props.historyArr[index].name,
                fill: false,
            lineTension: 0.1,
            borderColor: color,
            borderCapStyle: 'butt',
            borderDash: [],
            borderDashOffset: 0.0,
            borderJoinStyle: 'miter',
            pointBorderColor: color,
            pointBackgroundColor: color,
            pointBorderWidth: 1,
            pointHoverRadius: 5,
            pointHoverBorderWidth: 2,
            pointRadius: 1,
            pointHitRadius: 10,
            data: this.props.historyArr[index].temps
        }
    }

    prepareData() {
        if (this.props.historyArr.length === 0)
            return {}

        let ds = []
        for (let i = 0; i < this.props.historyArr.length; i++) {
            ds.push(this.getDataset(i))
        }

        return {
            labels: this.props.historyArr[0].dates,
            datasets: ds
        };
    }

    render() {
        return (
            <div>
                <h3>Temperature graph</h3>
                <Line data={this.prepareData()} />
                <br />
                <div className="pt-2 pb-2 mb-2 border-top" />
                <br />
            </div>
        );
    }
}