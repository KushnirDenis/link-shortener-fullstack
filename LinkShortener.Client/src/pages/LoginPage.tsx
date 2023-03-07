import React, {useState} from 'react';
import {Button, Form} from 'react-bootstrap';
import {Translation} from "react-i18next";
import i18n from "../tools/i18n";
import {useUserStore} from "../stores/UserStore";
import User from "../models/User";
import AuthDto from "../models/AuthDto";
import {apiV1} from "../tools/axios";
import {useNavigate} from "react-router-dom";

const LoginPage = () => {
        const [email, setEmail] = useState("");
        const [password, setPassword] = useState("");
        const navigate = useNavigate();
        const setUser = useUserStore(state => state.setUser);


        const login = async () => {
            try {
                let {data} = await apiV1.post<AuthDto>("auth/login",
                    {
                        email: email,
                        password: password
                    }, {
                        headers: {
                            "Accept-Language": i18n.language
                        }
                    })

                const user: User = {
                    id: data.id,
                    email: email,
                    jwtToken: data.jwtToken
                }

                setUser(user);
                navigate("/links");

            } catch (err: any) {
                let responseMessage = err.response.data.message;
                responseMessage > 1 ? responseMessage.forEach((m: string) => alert(m)) : alert(responseMessage)
            }


        }

        return (
            <div>
                <Translation>
                    {(t) => <h1 className={"text-center my-4"}>{t('signInTo')} LinkShortener</h1>}
                </Translation>

                <Form onSubmit={(e) => e.preventDefault()}>
                    <Form.Group className="mb-3" controlId="formBasicEmail">
                        <Form.Label>
                            <Translation>
                                {(t) => <p>{t('email')}</p>}
                            </Translation>
                        </Form.Label>
                        <Form.Control type="email"
                                      placeholder="admin@gmail.com"
                                      onChange={(e) => setEmail(e.target.value)}
                                      value={email}
                        />
                    </Form.Group>

                    <Form.Group className="mb-3" controlId="formBasicPassword">
                        <Form.Label>
                            <Translation>
                                {(t) => <p>{t('password')}</p>}
                            </Translation>
                        </Form.Label>
                        <Form.Control type="password"
                                      placeholder="******"
                                      onChange={(e) => setPassword(e.target.value)}
                                      value={password}
                        />
                    </Form.Group>
                    <Button variant="primary"
                            type="submit"
                            onClick={login}
                    >
                        <Translation>
                            {(t) => <p>{t('login')}</p>}
                        </Translation>
                    </Button>
                </Form>
            </div>
        );
    }
;

export default LoginPage;