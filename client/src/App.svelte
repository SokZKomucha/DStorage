<script lang="ts">
  import Router from "svelte-spa-router";
  import Home from "./routes/Home.svelte";
  import Login from "./routes/Login.svelte";
  import Register from "./routes/Register.svelte";
  import { authStore } from "./stores/authStore";

  const routes = {
    "/": Home,
    "/register": Register,
    "/login": Login
  };
  
  // Home       /
  // Register   /register
  // Login      /login
  // Upload     /upload
  // Files      /files      /files/{id}

  (async () => {

  })();

</script>

<div class="wrapper">
  <Router {routes} />
</div>