<script lang="ts">
  import Header from "../components/Header.svelte";
    import { config } from "../config";
    import type { IFile } from "../types/IFile";

  export let params: Record<string, string | undefined> = {};
  let fileDetails: IFile | null = null;
  let status: HTMLParagraphElement | undefined;

  $: {
    params
    getFileDetails().then(r => fileDetails = r);
  }


  // Obviously not implemented yet, and won't be for a long time
  async function getFileDetails() {
    const request = await fetch(config.baseUrl + `/api/files/${params["id"]}`, {
      credentials: "include"
    });
    
    if (!request.ok) {
      if (!status) return null;
      const response = await request.text();
      status.style.display = "block";
      status.innerHTML = `An error occured; ${response}`;
      return null;
    }

    if (status) status.style.display = "none";
    const response: IFile = await request.json();
    return response;
  }
</script>

<div class="route-wrapper">

  <Header />

  <main>
    <p class="status" bind:this={status}></p>
    {#if fileDetails}
      <p class="file-header">File details of {fileDetails.filename}</p>
      <div class="file-details">
        <p class="file-property file-id">File ID:</p> <p class="value">{fileDetails.id}</p>
        <p class="file-property file-name">Filename:</p> <p class="value">{fileDetails.filename}</p>
        <p class="file-property file-size">File size (bytes):</p> <p class="value">{fileDetails.fileSize}</p>
        <p class="file-property file-upload-date">Upload date:</p> <p class="value">{new Date(fileDetails.uploadDate).toISOString().replace(/T|Z/g, " ")}</p>
        <p><a href={config.baseUrl + "/api/files/download/" + fileDetails.id}>Download link</a></p>
      </div>
      {/if}
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

  .file-header {
    font-size: 2em;
    font-weight: 600;
  }

  .file-details {
    display: grid;
    grid-template-columns: max-content auto;
  }

  .file-details > p {
    padding-right: 1em;
    padding-block: 0.25em;
  }
</style>