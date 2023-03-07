import React, {useState} from 'react';
import {Translation, useTranslation} from "react-i18next";
import {Button, Form} from "react-bootstrap";
import {apiV1} from '../tools/axios';
import AuthDto from "../models/AuthDto";
import i18n from "../tools/i18n";
import {useUserStore} from "../stores/UserStore";
import User from "../models/User";
import {useNavigate} from "react-router-dom";

const RegisterPage = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const {t} = useTranslation();
    const setUser = useUserStore(store => store.setUser);
    const navigate = useNavigate();

    const register = async () => {
        if (password != confirmPassword) {
            alert(t("passwordsNotMatch"))
            return;
        }
        try {
            const {data} = await apiV1.post<AuthDto>("auth/register", {
                    email: email,
                    password: password,
                },
                {
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
                {(t) => <h1 className={"text-center my-4"}>{t('registerIn')} LinkShortener</h1>}
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
                <Form.Group className="mb-3" controlId="formConfirmPassword">
                    <Form.Label>
                        <Translation>
                            {(t) => <p>{t('repeatPassword')}</p>}
                        </Translation>
                    </Form.Label>
                    <Form.Control type="password"
                                  placeholder="******"
                                  onChange={(e) => setConfirmPassword(e.target.value)}
                                  value={confirmPassword}
                    />
                </Form.Group>
                <Button variant="primary"
                        type="submit"
                        onClick={register}
                >
                    <Translation>
                        {(t) => <p>{t('register')}</p>}
                    </Translation>
                </Button>
            </Form>
        </div>
    );
};

export default RegisterPage;