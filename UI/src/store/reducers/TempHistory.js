import * as moment from 'moment'
import {
    FETCH_TEMP_HISTORY_SUCCESS,
    FETCH_TEMP_HISTORY_ERROR,
    SET_DATES_TEMP_HISTORY
} from '../actions/actionTypes'

const initialState = {
    tempHistory: [],
    snID: null,
    from: new moment().subtract(7, 'days').format('YYYY-MM-DD'),
    to: new moment().format('YYYY-MM-DD')
}

export default function tempHistoryReducer(state = initialState, action) {
    switch (action.type) {
        case FETCH_TEMP_HISTORY_SUCCESS:
            return {
                ...state, tempHistory: action.tempHistory
            }
        case FETCH_TEMP_HISTORY_ERROR:
            return {
                ...state, error: action.error
            }
        case SET_DATES_TEMP_HISTORY:
            return {
                ...state, from: action.from, to: action.to
            }
        default:
            return state
    }
}