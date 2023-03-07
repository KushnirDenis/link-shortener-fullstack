import {Link} from "./Link";

export interface UserLinksDto {
    links: Link[],
    cursor: number
}