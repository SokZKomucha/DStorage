<script lang="ts">
  import Header from "../components/Header.svelte";
  import { config } from "../config";
  import type { IPagedFiles } from "../types/IPagedFiles";

  let pageNumber = 0;
  let pagedFilesResponse: IPagedFiles | null = null;
  $: getPagedFiles(pageNumber).then(r => pagedFilesResponse = r);

  async function getPagedFiles(page: number) {
    const request = await fetch(config.baseUrl + `/api/files?pageNumber=${page}`, {
      credentials: "include"
    });
    
    if (!request.ok) return null;
    
    const response: IPagedFiles = await request.json();
    return response;
  }
</script>

<div class="route-wrapper">
  
  <Header />
  
  <main>
    
    <p class="pagination">
      <!-- <button disabled={pagedFilesResponse?.previousPage == null} on:click={() => pageNumber--}>Prev</button>
      <span>{pageNumber + 1}</span>
      <button disabled={pagedFilesResponse?.nextPage == null} on:click={() => pageNumber++}>Next</button> -->

      <span class="page-current">Page {pageNumber + 1}</span>
      <button class="page-prev" disabled={pagedFilesResponse?.previousPage == null} on:click={() => pageNumber--}>Prev</button>
      <button class="page-next" disabled={pagedFilesResponse?.nextPage == null} on:click={() => pageNumber++}>Next</button>
    </p>

    {#if pagedFilesResponse}
    <table>
      <tr>
        <th>#</th>
        <th>ID</th>
        <th>Filename</th>
        <th>Size (bytes)</th>
        <th>Uploaded at</th>
        <th></th>
      </tr>
      {#each (pagedFilesResponse.files) as file, i}
        <tr>
          <td>{pagedFilesResponse.filesPerPage * pageNumber + i + 1}</td>
          <td>{file.id}</td>
          <td>{file.filename}</td>
          <td>{file.fileSize}</td>
          <td>{new Date(file.uploadDate).toISOString().replace(/T|Z/g, " ")}</td>
          <td class="file-actions">
            <a href={`#/files/${file.id}`}>View</a>
            <a href={config.baseUrl + "/api/files/download/" + file.id}>Download</a>
          </td>
        </tr>
      {/each}
    </table>
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

  table {
    border-collapse: collapse;
    table-layout: fixed;
  }

  td, th {
    padding-left: 0.25em;
    padding-right: 1em;
    padding-block: 0.25em;
    text-align: left;
  }

  td:nth-child(1), th:nth-child(1) { width: 5%; }
  td:nth-child(2), th:nth-child(2) { width: 5%; }
  td:nth-child(3), th:nth-child(3) { width: 40%; }
  td:nth-child(4), th:nth-child(4) { width: 15%; }
  td:nth-child(5), th:nth-child(5) { width: 15%; }
  /* td:nth-child(6), th:nth-child(6) { width: 100%; } Not setting the last one makes it occupy all remaining space */

  th {
    font-weight: bold;
  }

  tr:not(:first-child) {
    border-top: 1px solid rgba(0, 0, 0, 0.5);
  }

  .file-actions {
    padding-inline: 0;
    display: flex;
    justify-content: center;
    gap: 0.75em;
  }

  .file-actions > a {
    /* color: black; */
    text-decoration: none;
    /* text-decoration-color: rgba(0, 0, 0, 0.5); */
  }

  .pagination {
    display: flex;
    gap: 0.5em;
  }

  .pagination > button {
    padding-inline: 0.5em;
    cursor: pointer;
  }

  .pagination > button[disabled] {
    cursor: default !important;
  }
</style>