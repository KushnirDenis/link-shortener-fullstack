import React from 'react';
import {useUserStore} from "../stores/UserStore";

export type ProtectedRouteProps = {
    children: JSX.Element
};

const ProtectedRoute = (props: ProtectedRouteProps) => {
    const user = useUserStore(state => state.user)

    return (
        <div>
            {user?.jwtToken === undefined ?
            <p>need auth</p>
                :
                props.children
            }
        </div>
    );
};

export default ProtectedRoute;