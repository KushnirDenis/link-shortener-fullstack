import {create} from 'zustand'
import {devtools, persist} from "zustand/middleware";
import User from "../models/User"
import AuthDto from "../models/AuthDto";

export interface IUserStore {
    user: User | null,
    setAuth: (auth: AuthDto) => void
}

export const useUserStore = create<IUserStore>()
devtools(
    persist(
        (set, getState) => ({
            user: null,
            setAuth: (auth: AuthDto) => set((state: IUserStore) => {
                return {
                    user: auth
                }
            })

        }),
        {
            name: "user-store"
        }
    )
)