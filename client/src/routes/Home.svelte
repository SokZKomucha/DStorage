<script lang="ts">
  import { config } from "../config";
  import { authStore } from "../stores/authStore";


</script>

<div>
  <p>Home</p>
  <p>Hello, {$authStore?.data?.username}!</p>
  <button on:click={() => {
    fetch(config.baseUrl + "/api/auth/logout", { credentials: "include" })
      .then(() => authStore.set({ authenticated: false, data: null }));
  }}>Logout</button>
</div>