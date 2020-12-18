import React, { Component } from 'react'
import { connect } from 'react-redux'
import { fetchTempHistory, fetchTempHistoryById, setHistoryDates } from '../store/actions/TempHistory'
import {Container, Table, Button, Col, Row} from 'react-bootstrap'
import * as moment from 'moment'
import DateTimePicker from '../UI/DateTimePicker'
import Graphic from '../UI/Graphic'

class TempHistory extends Component {
    componentDidMount() {
        this.fetchHistory()
    }

    fetchHistory() {
        if (!this.props.snID)
        {
            this.props.fetchTempHistory(this.props.from, this.props.to)
        }
        else
        {
            this.props.fetchTempHistoryById(this.props.snID, this.props.from, this.props.to)
        }
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

    getSensorsHistoryForGraphic() {
        let group = this.props.tempHistory.reduce((r, obj) => {
            const name = obj.snID + '|' + obj.sensors.name
            r[name] = [...r[name] || [], {date:obj.date, temp:obj.temperature}];
            return r;
        }, {});
        console.log('group', group)
    }

    renderHistory() {  
        return this.props.tempHistory.map((itemHist, index) => {
            return (
                <tr key={index} className={this.getClassForRow(itemHist.temperature)}>
                    <td>{Number(itemHist.temperature).toFixed(1)}</td>
                    <td>{moment(itemHist.date).format('DD.MM.YYYY HH:mm:ss')}</td>
                    <td>{itemHist.sensors.name}</td>
                    <td>{itemHist.sensors.rom}</td>
                </tr>
            )
        })
    }

    render() {
        return(
            <div className="content-wrapper" style={{marginLeft: 0}}>
                <Container fluid>
                    <section className="content-header">
                        <h4>Temperture History</h4>
                    </section>
                    <section className="content">
                        <Row>
                            <Col xs="auto">
                                <DateTimePicker 
                                    from={new Date(moment(this.props.from))}
                                    to={new Date(moment(this.props.to))}
                                    onDatesChanged={this.props.setHistoryDates} />
                            </Col>
                            <Col>
                                <Button onClick={ () => this.fetchHistory()} color="primary" size="sm">Show</Button>
                            </Col>
                        </Row>
                        <br />
                        <Graphic historyObj={this.getSensorsHistoryForGraphic()} />
                        <br />
                        <div className="pt-2 pb-2 mb-2 border-top" />
                        <br />
                        <Table striped bordered>
                            <thead>
                                <tr>
                                <th>Temperature</th>
                                <th>Date</th>
                                <th>Sensor name</th>
                                <th>ROM</th>
                                </tr>
                            </thead>
                            <tbody>
                                {this.renderHistory()}
                            </tbody>
                        </Table>
                    </section>
                </Container>
            </div>
        )
    }
}

function mapStateToProps(state) {
    return {
        tempHistory: state.tempHistory.tempHistory,
        from: state.tempHistory.from,
        to: state.tempHistory.to
    }
}

function mapDispatchToProps(dispatch) {
    return {
        fetchTempHistory: (from, to) => dispatch(fetchTempHistory(from, to)),
        fetchTempHistoryById: (snID, from, to) => dispatch(fetchTempHistoryById(snID, from, to)),
        setHistoryDates: (from, to) => dispatch(setHistoryDates(from, to))
    }
}
    
export default connect(mapStateToProps, mapDispatchToProps)(TempHistory);