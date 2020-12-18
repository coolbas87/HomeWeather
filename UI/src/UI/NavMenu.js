import React, {Component} from 'react';
import { Container, Navbar, NavbarBrand, NavItem, NavLink } from 'react-bootstrap';
import { Link } from 'react-router-dom';

export default class NavMenu extends Component {
    render() {
        return (
            <header>
                <Navbar className="main-header navbar navbar-expand navbar-dark" style={{marginLeft: 0}}>
                    <Container fluid={true}>
                        <NavbarBrand tag={Link} href="/"><span className="brand-text font-weight-light">HomeWeather</span></NavbarBrand>
                        <ul className="navbar-nav">
                            <NavItem>
                                <NavLink tag={Link} className="text-light" href="/">Home</NavLink>
                            </NavItem>
                            <NavItem>
                                <NavLink tag={Link} className="text-light" href="/TempHistory">Temperature history</NavLink>
                            </NavItem>
                        </ul>
                    </Container>
                </Navbar>
            </header>
        );
    }
}
