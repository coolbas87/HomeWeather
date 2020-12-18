import { combineReducers } from 'redux'
import currentWeatherReducer from './CurrentWeather'
import tempHistoryReducer from './TempHistory'

export default combineReducers({
    currentWeather: currentWeatherReducer,
    tempHistory: tempHistoryReducer
})