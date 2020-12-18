import axios from '../../axiosBase';
import {
    FETCH_CURRENT_TEMP_SUCCESS,
    FETCH_CURRENT_TEMP_ERROR
} from '../actions/actionTypes';

export function fetchCurrentTemp() {
    return async dispatch => {
        try {
            const response = await axios.get('api/Temperature')
            const currentTempSensors = response.data            

            dispatch(fetchCurrentTempSuccess(currentTempSensors))
        } catch (e) {
            dispatch(fetchCurrentTempError(e))
        }
    }
}

export function fetchCurrentTempSuccess(currentTempSensors) {
    return {
        type: FETCH_CURRENT_TEMP_SUCCESS,
        currentTempSensors: currentTempSensors
    }
}

export function fetchCurrentTempError(e) {
    return {
        type: FETCH_CURRENT_TEMP_ERROR,
        error: e
    }
}