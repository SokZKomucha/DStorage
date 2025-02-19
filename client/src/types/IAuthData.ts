import type { IAuthResponse } from "./IAuthResponse"

/** Represents an authentication object */
export interface IAuthData {
  authenticated: boolean
  data: IAuthResponse | null
}