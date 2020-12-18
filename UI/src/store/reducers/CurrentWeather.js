import {
    FETCH_CURRENT_TEMP_SUCCESS,
    FETCH_CURRENT_TEMP_ERROR
} from '../actions/actionTypes'

const initialState = {
    currentTempSensors: []
}

export default function currentWeatherReducer(state = initialState, action) {
    switch (action.type) {
        case FETCH_CURRENT_TEMP_SUCCESS:
            return {
                ...state, currentTempSensors: action.currentTempSensors
            }
        case FETCH_CURRENT_TEMP_ERROR:
            return {
                ...state, error: action.error
            }
        default:
            return state
    }
}