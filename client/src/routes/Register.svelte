<script lang="ts">
  import { config } from "../config";
  import { authStore } from "../stores/authStore";
  import type { IRegisterRespone } from "../types/IRegisterResponse";

  let username: string = "";
  let password: string = "";
  let status: string = "";

  async function onClick() {
    const request = await fetch(config.baseUrl + "/api/auth/register", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username, password }),
      credentials: "include",
    });

    if (request.status !== 201) {
      status = await request.text();
      authStore.set({ authenticated: false, data: null });
      return;
    }

    const response: IRegisterRespone = await request.json();
    authStore.set({ authenticated: true, data: response });
  }
</script>

<div>
  Register <br />
  <input type="text" bind:value={username} /> <br />
  <input type="password" bind:value={password} /> <br />
  <button on:click={onClick}>Register</button>
  <p>{status}</p>
</div>
