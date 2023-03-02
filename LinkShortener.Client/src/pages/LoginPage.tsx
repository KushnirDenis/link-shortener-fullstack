import React, {useState} from 'react';
import {Button, Form} from 'react-bootstrap';
import {Translation} from "react-i18next";
import axios from "axios";
import i18n from "../tools/i18n";
import {IUserStore, useUserStore} from "../stores/UserStore";
import {useStore} from "zustand";


const login = async (email: string, password: string) => {
    // e.preventDefault();
    try {
        let response = await axios.post("http://localhost:4000/api/v1/auth/login",
            {
                email: email,
                password: password
            },
            {
                headers: {
                    "Accept-Language": i18n.language
                }
            })

        console.log(response.data)

    } catch (err: any) {
        let responseMessage = err.response.data.message;
        responseMessage > 1 ? responseMessage.forEach((m: string) => alert(m)) : alert(responseMessage)
    }


}

const LoginPage = () => {
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");

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
                        onClick={async (e) => {
                            await login(email, password)
                        }}
                >
                    <Translation>
                        {(t) => <p>{t('login')}</p>}
                    </Translation>
                </Button>
            </Form>
        </div>
    );
};

export default LoginPage;