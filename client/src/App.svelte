<script lang="ts">
  import Router, { push, location } from "svelte-spa-router";
  import Home from "./routes/Home.svelte";
  import Login from "./routes/Login.svelte";
  import Register from "./routes/Register.svelte";
  import { authStore } from "./stores/authStore";
  import Upload from "./routes/Upload.svelte";
  import Files from "./routes/Files.svelte";
  import File from "./routes/File.svelte";
  import NotFound from "./routes/NotFound.svelte";



  const routes = {
    "/": Home,
    "/register": Register,
    "/login": Login,
    "/upload": Upload,
    "/files": Files,
    "/files/:id": File,
    "*": NotFound
  };
  
  // Home       /
  // Register   /register
  // Login      /login
  // Upload     /upload
  // Files      /files      
  // File       /files/{id}

  $: {
    $location, $authStore
    if ($authStore?.authenticated === false && !["/login", "/register"].includes($location)) {
      push("#/login");
    } else if ($authStore?.authenticated === true && ["/login", "/register"].includes($location)) {
      push("#/");
    }
  }
</script>

<div class="wrapper">
  <Router {routes} />
</div>