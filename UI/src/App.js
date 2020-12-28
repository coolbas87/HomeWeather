import React, { Component } from 'react';
//import { Route } from 'react-router';
import Layout from './UI/Layout';
import CurrentWeather from './containers/CurrentWeather'
import TempHistory from './containers/TempHistory'
import {Route, Switch, Redirect, withRouter} from 'react-router-dom'
import {connect} from 'react-redux'
// import './App.css';

class App extends Component {
  render() {
    let routes = (
      <Switch>
        <Route exact path="/" component={CurrentWeather} />
        <Route path="/TempHistory/:snID/:from/:to" component={TempHistory} />
        <Route path="/TempHistory/:from/:to" component={TempHistory} />
        <Route path="/TempHistory/" component={TempHistory} />
        <Redirect to="/" />
      </Switch>
    )

    return (
      <Layout>
        {routes}
      </Layout>
    )
  }
}

function mapStateToProps(state) {
  return {
    ...state
  }
}

export default withRouter(connect(mapStateToProps)(App));