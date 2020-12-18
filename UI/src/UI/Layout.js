import React, { Component } from 'react'
import { connect } from 'react-redux'
import Navmenu from './NavMenu'

class Layout extends Component {
    render() {
        return (
            <div className="wrapper">
                <Navmenu />
                <main>
                    { this.props.children }
                </main>
            </div>
        )
    }
}

export default connect()(Layout)