import {create} from 'zustand'
import {devtools, persist} from "zustand/middleware";
import User from "../models/User"

export interface IUserStore {
    user: User | null,
    setUser: (user: User) => void,
    logout: () => void
}

export const useUserStore = create<IUserStore>()(devtools(
        persist(
            (set) => ({
                user: null,
                setUser: (user) => set(
                    {
                        user: user
                    }
                ),
                logout: () => set({
                    user: null
                })
            })
            ,
            {
                name: "user-store"
            }
        )
    )
)