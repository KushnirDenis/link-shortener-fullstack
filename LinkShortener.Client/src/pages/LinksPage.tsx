import React, {useEffect, useState} from 'react';
import {apiV1} from "../tools/axios";
import {useUserStore} from "../stores/UserStore";
import i18n from "../tools/i18n";
import {Link} from "../models/Link";
import {UserLinksDto} from "../models/UserLinksDto";
import {Button, Form, Modal, Table} from "react-bootstrap";
import Loader from "../components/Loader";
import {Translation} from "react-i18next";

const LinksPage = () => {
    const user = useUserStore(state => state.user);
    const [links, setLinks] = useState<Link[]>([])
    const [newLink, setNewLink] = useState("");
    const [loading, setLoading] = useState(false)
    const [show, setShow] = useState(false);
    let cursor = -1;
    const fetchLinks = async (moreLinks: boolean = false) => {
        try {
            if (moreLinks) {
                const {data} = await apiV1.get<UserLinksDto>(`users/${user?.id}/links?cursor=${cursor}`, {
                    headers: {
                        "Accept-Language": i18n.language,
                        "Authorization": `Bearer ${user?.jwtToken}`
                    }
                })
                cursor = data.cursor;
                setLinks([...links, ...data.links])
            } else {
                setLoading(true);
                const {data} = await apiV1.get<UserLinksDto>(`users/${user?.id}/links`, {
                    headers: {
                        "Accept-Language": i18n.language,
                        "Authorization": `Bearer ${user?.jwtToken}`
                    }
                })
                cursor = data.cursor;
                setLinks(data.links)
            }

            setLoading(false);
        } catch (err: any) {
            let responseMessage = err.response.data.message;
            responseMessage > 1 ? responseMessage.forEach((m: string) => alert(m)) : alert(responseMessage)
        }
    }

    const shortLink = async () => {
        if (newLink.length < 12) {
            alert("Неверная ссылка")
            return;
        }

        try {
            const {data} = await apiV1.post<Link>(`links/`, {
                    url: newLink
                },
                {
                    headers: {
                        "Accept-Language": i18n.language,
                        "Authorization": `Bearer ${user?.jwtToken}`
                    }

                })

            setLinks([data, ...links]);
            closeModal();
        } catch (err: any) {
            let responseMessage = err.response.data.message;
            responseMessage > 1 ? responseMessage.forEach((m: string) => alert(m)) : alert(responseMessage)
        }
    }

    const closeModal = () => {
        setNewLink("");
        setShow(false);
    }

    async function loadMore() {
        if (window.innerHeight + document.documentElement.scrollTop === document.scrollingElement?.scrollHeight) {
            await fetchLinks(true);
        }
    }

    useEffect(() => {
        fetchLinks()
        window.addEventListener('scroll', loadMore);
    }, [])

    return (
        <div>
            <Modal show={show} onHide={closeModal}>
                <Modal.Header closeButton>
                    <Modal.Title>
                        <Translation>
                            {(t) => t("shortenLink")}
                        </Translation>
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <Form onSubmit={(e) => e.preventDefault()}>
                        <Form.Group className="mb-3" controlId="exampleForm.ControlInput1">
                            <Form.Label>URL:</Form.Label>
                            <Form.Control
                                type="text"
                                placeholder="https://..."
                                onChange={(e) => setNewLink(e.target.value)}
                                onKeyDown={(e) => e.key === "Enter" ? shortLink() : ""}
                                value={newLink}
                                autoFocus
                            />
                        </Form.Group>
                    </Form>
                </Modal.Body>
                <Modal.Footer>
                    <Button type={"submit"} variant="primary" onClick={shortLink}>
                        <Translation>
                            {(t) => t("addNew")}
                        </Translation>
                    </Button>
                    <Button variant="secondary" onClick={closeModal}>
                        <Translation>
                            {(t) => t("close")}
                        </Translation>
                    </Button>
                </Modal.Footer>
            </Modal>

            <Button type={"button"}
                    className={"d-block ms-auto mb-4"}
                    onClick={() => {
                        setShow(true)
                    }
                    }
            >
                <Translation>
                    {(t) => t("addNew")}
                </Translation>
            </Button>
            {loading ? <Loader/>
                :
                <div>
                    <span>Total: {links.length}</span>
                    <Table>
                        <thead>
                        <tr>
                            <th>
                                <Translation>
                                    {(t) => t("initialLink")}
                                </Translation>
                            </th>
                            <th>
                                <Translation>
                                    {(t) => t("shortLink")}
                                </Translation>
                            </th>
                            <th className={"text-end"}>
                                <Translation>
                                    {(t) => t("controlButtons")}
                                </Translation>
                            </th>
                        </tr>
                        </thead>
                        <tbody>
                        {links.map(l => <tr key={l.id}>
                            <td>
                                <a href={l.initialLink}>
                                    {l.initialLink}
                                </a>
                            </td>
                            <td>{l.shortCode}</td>
                            <td className={"text-end"}><Button variant={"primary"}>View</Button></td>
                        </tr>)}
                        </tbody>
                    </Table>
                </div>
            }
        </div>
    );
};

export default LinksPage;