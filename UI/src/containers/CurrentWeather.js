import React, { Component } from 'react'
import { connect } from 'react-redux'
import { fetchCurrentTemp } from '../store/actions/CurrentWeather'
import {Container, Row, Col} from 'react-bootstrap'
import InfoBox from '../UI/InfoBox'


class CurrentWeather extends Component {

    componentDidMount() {
        this.props.fetchCurrentTemp()
    }

    getClassForRow(temperature) {
        const cls = [
            'table-'
        ]

        if (temperature >= 30) {
            cls.push('danger')
        } else if (temperature >= 20) {
            cls.push('warning')
        } else if (temperature >= 5) {
            cls.push('info')
        } else if (temperature >= -5) {
            cls.push('primary')
        } else {
            cls.push('light')
        }

        return cls.join('')
    }

    renderSensors() {
        const chunk = 4
        let resArray = []

        for (let i = 0; i < this.props.currentTempSensors.length; i += chunk) {
            var sensorsChunk = this.props.currentTempSensors.slice(i, i + chunk)
            let arr = sensorsChunk.map((sensor, index) => {
                return (
                    <Col key={index}>
                        <InfoBox
                            name={sensor.name}
                            temperature={Number(sensor.temperature).toFixed(1)}
                            rom={sensor.rom}
                        />
                    </Col>
                )
            })
            resArray.push(<Row key={i} sm={arr.length >= 4 ? 4 : arr.length}>{arr}</Row>)
        }
        return resArray
    }

    render() {
        return (
            <div className="content-wrapper" style={{marginLeft: 0}}>
                <Container fluid>
                    <section className="content-header">
                        <h4>Active sensors</h4>
                    </section>
                    <section className="content">
                        {this.renderSensors()}
                    </section>
                </Container>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        currentTempSensors: state.currentWeather.currentTempSensors
    }
}

function mapDispatchToProps(dispatch) {
    return {
        fetchCurrentTemp: () => dispatch(fetchCurrentTemp())
    }
}

export default connect(mapStateToProps, mapDispatchToProps)(CurrentWeather);