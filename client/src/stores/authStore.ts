import { writable } from "svelte/store";
import { config } from "../config";
import type { IAuthData } from "../types/IAuthData";
import type { IAuthResponse } from "../types/IAuthResponse";

export const authStore = writable<IAuthData | null>(null);

authStore.subscribe(v => {
  console.log("Updated", v, new Date());
});

(async () => {
  const request = await fetch(config.baseUrl + "/api/auth", {
    credentials: "include"
  });
  
  if (request.status !== 200) {
    authStore.set({ authenticated: false, data: null });
    return;
  }

  const response: IAuthResponse = await request.json();
  authStore.set({ authenticated: true, data: response });
})();

// https://lucia-auth.com/sessions/cookies/