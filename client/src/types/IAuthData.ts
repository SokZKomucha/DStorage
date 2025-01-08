import type { IAuthResponse } from "./IAuthResponse"

export interface IAuthData {
  authenticated: boolean
  data: null | IAuthResponse
}