<script lang="ts">
  import Header from "../components/Header.svelte";
  import { config } from "../config";
  import type { IFile } from "../types/IFile";

  let fileInput: HTMLInputElement | undefined;
  let status: HTMLParagraphElement | undefined;

  const submit = async () => {
    const file = fileInput?.files?.[0];
    if (!file) return;
    if (!status) return;
    
    try {
      const request = await fetch(config.baseUrl + "/api/files/upload", {
        method: "POST",
        credentials: "include",
        headers: {
          "Content-Type": file.type,
          "X-Filename": encodeURIComponent(file.name)
        },
        body: file,
      });

      if (!request.ok) {
        const response = await request.text();
        status.innerHTML = `An error occured: ${response}`;
        return;
      } 

      const response: IFile = await request.json();
      status.innerHTML = `File uploaded succesfully <a href="#/files/${response.id}">View</a>`;
    } catch (error) {
      status.innerHTML = "An error occured or file is too large.";
    }

    // Obviously such try-catch is ambiguous; this ties closely to the unresolved issue with supposed 413 code
    // I don't know how to get around it, so that's pretty much here to stay

  }
</script>

<div class="route-wrapper">
  
  <Header />
 
  <main>
    <p class="upload-header">File upload</p>

    <div class="upload-container">
      <input type="file" bind:this={fileInput}>
      <button on:click={submit}>Submit</button>
    </div>

    <p class="upload-status" bind:this={status}></p>

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

  .upload-header {
    font-size: 2em;
    font-weight: 600;
  }

  .upload-container {
    width: 25em;
    display: flex;
    flex-direction: column;
    gap: 0.25em;
  }
</style>