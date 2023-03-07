import React from 'react';
import {Button, Container, Nav, Navbar} from "react-bootstrap";
import i18n from "../tools/i18n";
import {Link} from "react-router-dom";
import {useUserStore} from "../stores/UserStore";
import {Translation} from "react-i18next";

const Header = () => {
    const {user, logout} = useUserStore();

    return (
        <Navbar bg="primary" variant="dark" className={"mb-5"}>
            <Container>
                <Navbar.Brand>
                    <Link to={"/"} className={"text-light text-decoration-none"}>
                        LinkShortener</Link>
                </Navbar.Brand>

                <div className="ms-auto d-flex text-light">
                    {user?.jwtToken === undefined ?
                        <Nav>
                            <Link to={"/auth/login"} className={"text-light text-decoration-none me-3"}>
                                <Translation>
                                    {(t) => t("login")}
                                </Translation>
                            </Link>
                            <Link to={"/auth/register"} className={"text-light text-decoration-none"}>
                                <Translation>
                                    {(t) => t("register")}
                                </Translation>
                            </Link>
                        </Nav>
                        :
                        <div>
                            <Link to={"/links"} className={"text-light text-decoration-none"}>
                                {user.email}
                            </Link>
                            <Button type="button"
                            onClick={logout}
                            >
                                <Translation>
                                    {(t) => t("logout")}
                                </Translation>
                            </Button>
                        </div>
                    }

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