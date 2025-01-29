<script lang="ts">
  import { config } from "../config";
  import { authStore } from "../stores/authStore";
  import type { ILoginResponse } from "../types/ILoginResponse";

  let username: string = "";
  let password: string = "";
  let status: string = "";

  async function onClick() {
    const request = await fetch(config.baseUrl + "/api/auth/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username, password }),
      credentials: "include"
    });
    
    if (request.status !== 200) {
      status = await request.text();
      authStore.set({ authenticated: false, data: null })
      return;
    }

    const response: ILoginResponse = await request.json();
    authStore.set({ authenticated: true, data: response })
  }

</script>

<div>
  Login
  <input type="text" bind:value={username} />
  <input type="password" bind:value={password} />
  <button on:click={onClick}>Login</button>
  <p>{status}</p>
</div>