<script lang="ts">
  import "./../styles/base.css";
  import { config } from "../config";
  import { authStore } from "../stores/authStore";
  import Header from "../components/Header.svelte";


</script>

<div class="route-wrapper">

  <Header />

  <main>
    <p class="username">Hello {$authStore?.data?.username}!</p>
    <p>There's nothing here yet.</p>
    <p> Visit <a href="#/files">files</a> to view your uploaded files, or <a href="#/upload">upload</a> to upload them.</p>
  </main>

</div>

<style>
  main {
    flex: 1;
    padding-block: 4em;
    padding-inline: 8em;
    display: flex;
    flex-direction: column;
    gap: 1em;
  }

  .username {
    font-size: 2em;
    font-weight: 600;
  }
</style>