<script lang="ts">
  import Header from "../components/Header.svelte";
  import { config } from "../config";

  let fileInput: HTMLInputElement | undefined;
  const submit = async () => {
    const file = fileInput?.files?.[0];
    if (!file) return;
    console.log(file); 

    await fetch(config.baseUrl + "/api/file/upload", {
      method: "POST",
      headers: {
        "Content-Type": file.type
      },
      body: file
    });
    console.log("finito")
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