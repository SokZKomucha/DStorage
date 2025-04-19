<script lang="ts">
  import { config } from "../config";
  import { authStore } from "../stores/authStore";

  const logOut = async () => {
    await fetch(config.baseUrl + "/api/auth/logout", { credentials: "include" })
    authStore.set({ authenticated: false, data: null });
    // Don't really need to await this
  }
</script>

<header>
  <ul class="routes">
    <li><a href="#/">Home</a></li>
    <li><a href="#/files">Files</a></li>
    <li><a href="#/upload">Upload</a></li>
  </ul>
  <p>
    <span class="username-wrapper">Logged in as <span class="username">{$authStore?.data?.username}</span></span>
    <!-- svelte-ignore a11y-click-events-have-key-events -->
    <!-- svelte-ignore a11y-no-static-element-interactions -->
    <span class="logout" on:click={logOut}>Log out</span> 
  </p>
</header>

<style>
  header {
    padding-inline: 2em;
    padding-block: 1.5em;
    display: flex;
    justify-content: space-between;
    font-size: 1.15em;
  
    box-shadow: 0 0 0.5em 0 rgb(0, 0, 0, 0.25);
  }

  .routes {
    display: flex;
    gap: 2em;
    list-style-type: none;
  }

  .routes > li > a {
    color: black;
    text-decoration: none;
    font-weight: bold;
  }

  p {
    display: flex;
    gap: 1.5em;
  }

  .username {
    font-weight: bold;
  }

  .logout {
    background: none;
    border: none;
    cursor: pointer;
    font-weight: bold;
  }
</style>