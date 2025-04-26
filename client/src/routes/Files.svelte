<script lang="ts">
  import Header from "../components/Header.svelte";
  import { config } from "../config";
  import type { IPagedFiles } from "../types/IPagedFiles";

  let pageNumber = 0;
  
  let pagedFilesResponse: IPagedFiles | null = null;
  $: {
    getPagedFiles(pageNumber).then(r => pagedFilesResponse = r);
  }
  
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
    
    <p>
      <button disabled={pagedFilesResponse?.previousPage == null} on:click={() => pageNumber--}>Prev</button>
      <span>{pageNumber + 1}</span>
      <button disabled={pagedFilesResponse?.nextPage == null} on:click={() => pageNumber++}>Next</button>
    </p>

    {#if pagedFilesResponse}
    <table>
      {#each (pagedFilesResponse.files) as file}
        <tr>
          <td>{file.id}</td>
          <td>{file.userId}</td>
          <td>{file.filename}</td>
          <td>{file.fileSize}</td>
          <td>{file.uploadDate}</td>
        </tr>
      {/each}
    </table>
    {/if}
  </main>

</div>