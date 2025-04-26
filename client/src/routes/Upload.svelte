<script lang="ts">
  import Header from "../components/Header.svelte";
  import { config } from "../config";

  let fileInput: HTMLInputElement | undefined;
  const submit = async () => {
    const file = fileInput?.files?.[0];
    if (!file) return;
    console.log(file); 
    
    try {
      const request = await fetch(config.baseUrl + "/api/files/upload", {
        method: "POST",
        credentials: "include",
        headers: {
          "Content-Type": file.type,
          "X-Filename": file.name
        },
        body: file,
      });
      console.log("Oki");
    } catch (error) {
      console.log("Error");      
    }
    console.log("Finito");
    // Obviously such try-catch is ambiguous; this ties closely to an issue with supposed 413 code
    
  }
</script>

<div class="route-wrapper">
  
  <Header />
 
  <main>
    Upload

    <div>
      <input type="file" bind:this={fileInput}>
      <button on:click={submit}>Submit</button>
    </div>

  </main>

</div>