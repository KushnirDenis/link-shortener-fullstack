import React from 'react';
import {Button, Container, Nav, Navbar} from "react-bootstrap";
import i18n from "../tools/i18n";
import {Link} from "react-router-dom";
import {Translation} from "react-i18next";

const Header = () => {
    return (
        <Navbar bg="primary" variant="dark">
            <Container>
                <Navbar.Brand>
                    <Link to={"/"} className={"text-light text-decoration-none"}>
                        LinkShortener</Link>
                </Navbar.Brand>
                <div className="ms-auto d-flex">
                    <Nav>
                        <Nav.Link>
                            <Link to={"/auth/login"} className={"text-light text-decoration-none"}>
                                <Translation>
                                    {(t) => t("login")}
                                </Translation>
                            </Link>
                        </Nav.Link>
                        <Nav.Link>
                            <Link to={"/auth/register"} className={"text-light text-decoration-none"}>
                                <Translation>
                                    {(t) => t("register")}
                                </Translation>
                            </Link>
                        </Nav.Link>
                    </Nav>
                    <div className="lang-buttons text-light text-decoration-none ms-3">
                        <Button
                            onClick={async () => {
                                await i18n.changeLanguage("ru")
                            }
                            }
                        >Ru</Button>
                        /
                        <Button
                            onClick={async () => {
                                await i18n.changeLanguage("en")
                            }}>
                            En</Button>
                    </div>
                </div>
            </Container>
        </Navbar>
    );
};

export default Header;