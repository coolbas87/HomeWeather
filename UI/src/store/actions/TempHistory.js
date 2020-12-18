import axios from '../../axiosBase';
import * as moment from 'moment';
import {
    FETCH_TEMP_HISTORY_SUCCESS,
    FETCH_TEMP_HISTORY_ERROR,
    SET_DATES_TEMP_HISTORY
} from '../actions/actionTypes';

export function fetchTempHistory(from, to) {
    return async dispatch => {
        try
        {
            const response = await axios.get(`api/TempHistory/${from}/${to}`)
            const tempHistory = response.data

            dispatch(fetchTempHistorySuccess(tempHistory))
        }
        catch(e)
        {
            dispatch(fetchTempHistoryError(e))
        }
    }
}

export function fetchTempHistoryById(snID, from, to) {
    return async dispatch => {
        try 
        {
            const response = await axios.get(`api/TempHistory/${snID}/${from}/${to}`)
            const tempHistory = response.data

            dispatch(fetchTempHistorySuccess(tempHistory))
        } catch (e) {
            dispatch(fetchTempHistoryError(e))
        }
    }
}

export function fetchTempHistorySuccess(tempHistory) {
    return {
        type: FETCH_TEMP_HISTORY_SUCCESS,
        tempHistory: tempHistory
    }
}

export function fetchTempHistoryError(e) {
    return {
        type: FETCH_TEMP_HISTORY_ERROR,
        error: e
    }    
}

export function setHistoryDates(from, to) {
    return {
        type: SET_DATES_TEMP_HISTORY,
        from: new moment(from).format('YYYY-MM-DD'),
        to: new moment(to).format('YYYY-MM-DD')
    }
}