import React from 'react';


export default class InfoBox extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            name: props.name,
            temperature: props.from,
            rom: props.to,
            loading: props.loading || false
        };
    }

    getClassForRow(temperature) {
        const cls = [
            'info-box bg-gradient-'
        ]

        if (temperature >= 30) {
            cls.push('danger')
        } else if (temperature >= 15) {
            cls.push('warning')
        } else if (temperature >= 0) {
            cls.push('info')
        } else if (temperature >= -15) {
            cls.push('primary')
        } else if (temperature >= -30) {
            cls.push('purple')
        } else {
            cls.push('light')
        }

        return cls.join('')
    }

    render() {
        return(
            <div className={this.getClassForRow(this.props.temperature)}>
                <span className="info-box-icon"><i className="fas fa-thermometer-half"></i></span>
                <div className="info-box-content">
                    <span className="info-box-text">{this.props.name}</span>
                    <span className="info-box-number">{this.props.temperature}</span>
                </div>
                {
                    this.props.loading
                    ?
                    <div className="overlay">
                        <i className="fas fa-2x fa-sync-alt fa-spin"></i>
                    </div>
                    :
                    null
                }
            </div>
        )
    }
}